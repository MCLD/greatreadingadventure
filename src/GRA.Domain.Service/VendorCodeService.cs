using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExcelDataReader;
using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace GRA.Domain.Service
{
    public class VendorCodeService : BaseUserService<VendorCodeService>
    {
        private readonly ICodeGenerator _codeGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly IVendorCodeTypeRepository _vendorCodeTypeRepository;
        private readonly MailService _mailService;
        public VendorCodeService(ILogger<VendorCodeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            ICodeGenerator codeGenerator,
            IVendorCodeRepository vendorCodeRepository,
            IVendorCodeTypeRepository vendorCodeTypeRepository,
            MailService mailService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageVendorCodes);
            _codeGenerator = Require.IsNotNull(codeGenerator, nameof(codeGenerator));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _vendorCodeRepository = Require.IsNotNull(vendorCodeRepository, nameof(vendorCodeRepository));
            _vendorCodeTypeRepository = Require.IsNotNull(vendorCodeTypeRepository, nameof(vendorCodeTypeRepository));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        public async Task<VendorCodeType> GetTypeById(int id)
        {
            return await _vendorCodeTypeRepository.GetByIdAsync(id);
        }

        public async Task<ICollection<VendorCodeType>> GetTypeAllAsync()
        {
            VerifyManagementPermission();
            return await _vendorCodeTypeRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<VendorCodeType>>> GetTypePaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<VendorCodeType>>
            {
                Data = await _vendorCodeTypeRepository.PageAsync(filter),
                Count = await _vendorCodeTypeRepository.CountAsync(filter)
            };
        }

        public async Task<VendorCodeType> AddTypeAsync(VendorCodeType vendorCodeType)
        {
            VerifyManagementPermission();
            vendorCodeType.SiteId = GetCurrentSiteId();
            return await _vendorCodeTypeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeType);
        }

        public async Task<VendorCodeType> UpdateTypeAsync(VendorCodeType vendorCodeType)
        {
            VerifyManagementPermission();
            vendorCodeType.SiteId = GetCurrentSiteId();
            return await _vendorCodeTypeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeType);
        }

        public async Task RemoveTypeAsync(int vendorCodeTypeId)
        {
            VerifyManagementPermission();
            await _vendorCodeTypeRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeTypeId);
        }

        public async Task<int> GenerateVendorCodesAsync(int vendorCodeTypeId, int numberOfCodes)
        {
            VerifyManagementPermission();
            var codeType = await _vendorCodeTypeRepository.GetByIdAsync(vendorCodeTypeId);
            if (codeType == null)
            {
                throw new GraException("Unable to find vendor code type.");
            }
            if (codeType.SiteId != GetCurrentSiteId())
            {
                throw new GraException("Code type provided does not match current site.");
            }

            int count = 1;
            var vendorCode = new VendorCode
            {
                IsUsed = false,
                SiteId = codeType.SiteId,
                VendorCodeTypeId = codeType.Id,
            };
            for (; count <= System.Math.Min(numberOfCodes, 5000); count++)
            {
                vendorCode.Code = _codeGenerator.Generate(15);
                await _vendorCodeRepository.AddAsync(GetClaimId(ClaimType.UserId), vendorCode);
                if (count % 1000 == 0)
                {
                    await _vendorCodeRepository.SaveAsync();
                }
            }
            await _vendorCodeRepository.SaveAsync();

            return --count;
        }

        public async Task<VendorCode> GetUserVendorCodeAsync(int userId)
        {
            var authorized = false;
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId || userId == GetActiveUserId() || HasPermission(Permission.ViewParticipantDetails))
            {
                authorized = true;
            }

            if (!authorized)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                authorized = user.HouseholdHeadUserId == authId;
            }

            if (authorized)
            {
                var vendorCode = await _vendorCodeRepository.GetUserVendorCode(userId);
                if (vendorCode != null)
                {
                    var codeType = await _vendorCodeTypeRepository
                        .GetByIdAsync(vendorCode.VendorCodeTypeId);
                    vendorCode.CanBeDonated = !string.IsNullOrEmpty(codeType.DonationMessage);
                    if (!string.IsNullOrEmpty(codeType.Url))
                    {
                        vendorCode.Url = codeType.Url.Contains(TemplateToken.VendorCodeToken)
                            ? codeType.Url.Replace(TemplateToken.VendorCodeToken, vendorCode.Code)
                            : codeType.Url;
                    }
                    return vendorCode;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        private const string CouponRowHeading = "Coupon";
        private const string OrderDateRowHeading = "Order Date";
        private const string ShipDateRowHeading = "Ship Date";


        public async Task<OperationStatus> UpdateStatusFromExcel(string filename,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ManageVendorCodes))
            {
                var sw = new Stopwatch();
                sw.Start();
                token.Register(() =>
                {
                    string duration = "";
                    if (sw != null && sw.Elapsed != null)
                    {
                        duration = $" after {((TimeSpan)sw.Elapsed).TotalSeconds.ToString("N2")} seconds";
                    }
                    _logger.LogWarning($"Import of {filename} for user {requestingUser} was cancelled{duration}.");
                });

                string fullPath = Path.Combine(Path.GetTempPath(), filename);

                if (!File.Exists(fullPath))
                {
                    _logger.LogError($"Could not find {fullPath}");
                    return new OperationStatus
                    {
                        PercentComplete = 0,
                        Status = "Could not find the import file.",
                        Error = true,
                        Complete = true
                    };
                }

                try
                {
                    using (var stream = new FileStream(fullPath, FileMode.Open))
                    {
                        int couponColumnId = 0;
                        int orderDateColumnId = 0;
                        int shipDateColumnId = 0;
                        var issues = new List<string>();
                        int row = 0;
                        int totalRows = 0;
                        int updated = 0;
                        int alreadyCurrent = 0;
                        using (var excelReader = ExcelReaderFactory.CreateBinaryReader(stream))
                        {
                            while (excelReader.Read())
                            {
                                row++;
                            }
                            totalRows = row;
                            row = 0;

                            excelReader.Reset();
                            while (excelReader.Read())
                            {
                                row++;
                                if (row % 10 == 0)
                                {
                                    progress.Report(new OperationStatus
                                    {
                                        PercentComplete = row * 100 / totalRows,
                                        Status = $"Processing row {row}/{totalRows}...",
                                        Error = false
                                    });
                                }
                                if (row == 1)
                                {
                                    progress.Report(new OperationStatus
                                    {
                                        PercentComplete = 1,
                                        Status = $"Processing row {row}/{totalRows}...",
                                        Error = false
                                    });
                                    for (int i = 0; i < excelReader.FieldCount; i++)
                                    {
                                        switch (excelReader.GetString(i).Trim() ?? $"Column{i}")
                                        {
                                            case CouponRowHeading:
                                                couponColumnId = i;
                                                break;
                                            case OrderDateRowHeading:
                                                orderDateColumnId = i;
                                                break;
                                            case ShipDateRowHeading:
                                                shipDateColumnId = i;
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    string coupon = null;
                                    DateTime? orderDate = null;
                                    DateTime? shipDate = null;
                                    try
                                    {
                                        coupon = excelReader.GetString(couponColumnId);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Parse error on code, row {row}: {ex.Message}");
                                        issues.Add($"Issue reading code on line {row}: {ex.Message}");
                                    }
                                    try
                                    {
                                        orderDate = excelReader.GetDateTime(orderDateColumnId);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Parse error on order date, row {row}: {ex.Message}");
                                        issues.Add($"Issue reading order date on row {row}: {ex.Message}");
                                    }
                                    try
                                    {
                                        shipDate = excelReader.GetDateTime(shipDateColumnId);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Parse error on ship date, row {row}: {ex.Message}");
                                        issues.Add($"Issue reading ship date on row {row}: {ex.Message}");
                                    }
                                    if (!string.IsNullOrEmpty(coupon)
                                        && (orderDate != null && shipDate != null))
                                    {
                                        var code = await _vendorCodeRepository.GetByCode(coupon);
                                        if (code == null)
                                        {
                                            _logger.LogError($"File contained code {coupon} which was not found in the database");
                                            issues.Add($"Uploaded file contained code <code>{coupon}</code> which couldn't be found in the database.");
                                        }
                                        else
                                        {
                                            if (orderDate == code.OrderDate && shipDate == code.ShipDate)
                                            {
                                                alreadyCurrent++;
                                            }
                                            else
                                            {
                                                code.IsUsed = true;
                                                if (orderDate != null)
                                                {
                                                    code.OrderDate = orderDate;
                                                }
                                                if (shipDate != null)
                                                {
                                                    code.ShipDate = shipDate;
                                                }
                                                await _vendorCodeRepository.UpdateSaveNoAuditAsync(code);
                                                updated++;
                                            }
                                        }
                                    }
                                }
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }
                            }

                            if (!excelReader.IsClosed)
                            {
                                excelReader.Close();
                            }
                        }

                        if (token.IsCancellationRequested)
                        {
                            return new OperationStatus
                            {
                                PercentComplete = 100,
                                Status = $"Operation cancelled at row {row}."
                            };
                        }

                        var sb = new StringBuilder("<strong>Import complete</strong>");
                        if (updated > 0)
                        {
                            sb.Append($": {updated} records were updated");
                        }
                        if (alreadyCurrent > 0)
                        {
                            if (updated > 0)
                            {
                                sb.Append(", ");
                            }
                            else
                            {
                                sb.Append(": ");
                            }
                            sb.Append($"{alreadyCurrent} records were already current");
                        }
                        sb.Append(".");

                        if (issues.Count > 0)
                        {
                            _logger.LogInformation($"Import complete with issues: {sb.ToString()}");
                            sb.Append(" Issues detected:<ul>");
                            foreach (string issue in issues)
                            {
                                sb.Append($"<li>{issue}</li>");
                            }
                            sb.Append("</ul>");
                            return new OperationStatus
                            {
                                PercentComplete = 100,
                                Status = sb.ToString(),
                                Error = true
                            };
                        }
                        else
                        {
                            _logger.LogInformation(sb.ToString());
                            return new OperationStatus
                            {
                                PercentComplete = 100,
                                Status = sb.ToString(),
                            };
                        }
                    }
                }
                finally
                {
                    File.Delete(fullPath);
                }
            }
            else
            {
                _logger.LogError($"User {requestingUser} doesn't have permission to view all reporting.");
                return new OperationStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
        }

        public async Task<bool> SiteHasCodesAsync()
        {
            return await _vendorCodeTypeRepository.SiteHasCodesAsync(GetCurrentSiteId());
        }

        public async Task<VendorCode> ResolveDonationStatusAsync(int userId, bool? donate)
        {
            var authorized = false;
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId || userId == GetActiveUserId() || HasPermission(Permission.ViewParticipantDetails))
            {
                authorized = true;
            }

            if (!authorized)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                authorized = user.HouseholdHeadUserId == authId;
            }

            if (authorized)
            {
                var siteId = GetClaimId(ClaimType.SiteId);

                var vendorCode = await _vendorCodeRepository.GetUserVendorCode(userId);
                vendorCode.IsDonated = donate;
                await _vendorCodeRepository.UpdateSaveAsync(userId, vendorCode);

                var vendorCodeType = await _vendorCodeTypeRepository.GetByIdAsync(vendorCode.VendorCodeTypeId);

                if (donate == null || donate == false)
                {
                    await SendVendorCodeMailAsync(userId, siteId, vendorCodeType, vendorCode.Code);
                }
                else if (donate == true
                  && !string.IsNullOrEmpty(vendorCodeType.DonationSubject)
                  && !string.IsNullOrEmpty(vendorCodeType.DonationMail))
                {
                    await SendVendorDonationMailAsync(userId, siteId, vendorCodeType);
                }

                return vendorCode;
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to update code donation status for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task PopulateVendorCodeStatusAsync(User user)
        {
            var vendorCode = await GetUserVendorCodeAsync(user.Id);
            if (vendorCode != null)
            {
                user.Donated = vendorCode.IsDonated;
                if (vendorCode.CanBeDonated && vendorCode.IsDonated == null)
                {
                    user.NeedsToAnswerDonationQuestion = true;
                }
                else if (vendorCode.CanBeDonated && vendorCode.IsDonated == true)
                {
                    var vendorCodeType
                        = await _vendorCodeTypeRepository.GetByIdAsync(vendorCode.VendorCodeTypeId);
                    user.VendorCode = vendorCodeType.DonationMessage;
                }
                else
                {
                    user.VendorCode = vendorCode.Code;
                    user.VendorCodeUrl = vendorCode.Url;
                    if (vendorCode.ShipDate.HasValue)
                    {
                        user.VendorCodeMessage = $"Shipped: {vendorCode.ShipDate.Value.ToString("d")}";
                    }
                    else if (vendorCode.OrderDate.HasValue)
                    {
                        user.VendorCodeMessage = $"Ordered: {vendorCode.OrderDate.Value.ToString("d")}";
                    }
                }
            }
        }

        private async Task SendVendorCodeMailAsync(int userId,
            int? siteId,
            VendorCodeType codeType,
            string assignedCode)
        {
            string body = null;
            if (!codeType.Mail.Contains(TemplateToken.VendorCodeToken))
            {
                // the token isn't in the message, just append the code to the end
                body = $"{codeType.Mail} {assignedCode}";
            }
            else
            {
                if (string.IsNullOrEmpty(codeType.Url))
                {
                    // we have a token but no url, replace the token with the code
                    body = codeType.Mail.Replace(TemplateToken.VendorCodeToken, assignedCode);
                }
                else
                {
                    string url = null;
                    // see if the url has the token in it, if so swap in the code
                    if(!codeType.Url.Contains(TemplateToken.VendorCodeToken))
                    {
                        url = codeType.Url;
                    }
                    else
                    {
                        url = codeType.Url.Replace(TemplateToken.VendorCodeToken, assignedCode);
                    }
                    // token and url - make token clickable to go to url
                    body = codeType.Mail.Replace(TemplateToken.VendorCodeToken,
                        $"<a href=\"{url}\" _target=\"blank\">{assignedCode}</a>");
                }
            }

            await _mailService.SendSystemMailAsync(new Mail
            {
                ToUserId = userId,
                CanParticipantDelete = false,
                Subject = codeType.MailSubject,
                Body = body
            }, siteId);
        }

        private async Task SendVendorDonationMailAsync(int userId,
            int? siteId,
            VendorCodeType codeType)
        {
            await _mailService.SendSystemMailAsync(new Mail
            {
                ToUserId = userId,
                CanParticipantDelete = false,
                Subject = codeType.DonationSubject,
                Body = codeType.DonationMail
            }, siteId);
        }
    }
}