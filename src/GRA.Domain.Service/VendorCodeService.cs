using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Report;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Service.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using Stubble.Core.Builders;

namespace GRA.Domain.Service
{
    public class VendorCodeService : BaseUserService<VendorCodeService>
    {
        private const string BranchIdRowHeading = "Branch Id";
        private const string CouponRowHeading = "Free Book Code";
        private const string DetailsRowHeading = "Title";
        private const string EmailAddressRowHeading = "Email";
        private const string ErrorParseError = "Parse error on {Field}, row {SpreadsheetRow}: {ErrorMessage}";
        private const string ErrorUnableToParse = "Unable to parse {Field}, row {SpreadsheetRow}: {Value}";
        private const string OrderDateRowHeading = "Creation Date";
        private const string PackingSlipRowHeading = "Pickpack Number";
        private const string SentDateRowHeading = "Sent Date";
        private const string ShipDateRowHeading = "Ship Date";
        private const string TrackingNumberRowHeading = "UPS Tracking Number";
        private const string UserIdRowHeading = "User Id";

        private readonly ICodeGenerator _codeGenerator;
        private readonly EmailService _emailService;
        private readonly IJobRepository _jobRepository;
        private readonly LanguageService _languageService;
        private readonly MailService _mailService;
        private readonly MessageTemplateService _messageTemplateService;
        private readonly IPathResolver _pathResolver;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly SegmentService _segmentService;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly SiteLookupService _siteLookupService;
        private readonly SiteService _siteService;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodePackingSlipRepository _vendorCodePackingSlipRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly IVendorCodeTypeRepository _vendorCodeTypeRepository;

        public VendorCodeService(EmailService emailService,
            ICodeGenerator codeGenerator,
            IDateTimeProvider dateTimeProvider,
            IJobRepository jobRepository,
            ILogger<VendorCodeService> logger,
            IPathResolver pathResolver,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            IVendorCodePackingSlipRepository vendorCodePackingSlipRepository,
            IVendorCodeRepository vendorCodeRepository,
            IVendorCodeTypeRepository vendorCodeTypeRepository,
            LanguageService languageService,
            MailService mailService,
            MessageTemplateService messageTemplateService,
            PrizeWinnerService prizeWinnerService,
            SegmentService segmentService,
            SiteLookupService siteLookupService,
            SiteService siteService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageVendorCodes);

            ArgumentNullException.ThrowIfNull(codeGenerator);
            ArgumentNullException.ThrowIfNull(emailService);
            ArgumentNullException.ThrowIfNull(jobRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(mailService);
            ArgumentNullException.ThrowIfNull(messageTemplateService);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(prizeWinnerService);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(sharedLocalizer);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(userRepository);
            ArgumentNullException.ThrowIfNull(vendorCodePackingSlipRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeRepository);
            ArgumentNullException.ThrowIfNull(vendorCodeTypeRepository);

            _codeGenerator = codeGenerator;
            _emailService = emailService;
            _jobRepository = jobRepository;
            _languageService = languageService;
            _mailService = mailService;
            _messageTemplateService = messageTemplateService;
            _pathResolver = pathResolver;
            _prizeWinnerService = prizeWinnerService;
            _segmentService = segmentService;
            _sharedLocalizer = sharedLocalizer;
            _siteLookupService = siteLookupService;
            _siteService = siteService;
            _userRepository = userRepository;
            _vendorCodePackingSlipRepository = vendorCodePackingSlipRepository;
            _vendorCodeRepository = vendorCodeRepository;
            _vendorCodeTypeRepository = vendorCodeTypeRepository;
        }

        private Dictionary<string, VendorCodePackingSlip> PsCache { get; set; }

        public Task<VendorCodeType> AddTypeAsync(VendorCodeType vendorCodeType)
        {
            ArgumentNullException.ThrowIfNull(vendorCodeType);
            return AddTypeInternalAsync(vendorCodeType);
        }

        public async Task AssignSpareAsync(int vendorCodeTypeId,
            int userId,
            string reason,
            int activeUserId)
        {
            await _vendorCodeRepository
                .AssociateCodeAsync(vendorCodeTypeId, userId, reason, activeUserId);
        }

        public async Task AssociateAsync(int vendorCodeId, string reason)
        {
            var code = await _vendorCodeRepository.GetByIdAsync(vendorCodeId);
            code.AssociatedUserId = code.UserId;
            code.ReasonForReassignment = reason;
            code.ReassignedAt = _dateTimeProvider.Now;
            code.ReassignedByUserId = GetActiveUserId();
            code.UserId = null;
            await _vendorCodeRepository.UpdateSaveAsync(GetActiveUserId(), code);
        }

        public async Task<byte[]> ExportVendorCodesAsync(int vendorCodeTypeId)
        {
            VerifyManagementPermission();
            var codes = await _vendorCodeRepository.GetAllCodesAsync(vendorCodeTypeId);

            await using var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            foreach (var code in codes)
            {
                await writer.WriteLineAsync(code);
            }
            await writer.FlushAsync();
            await memoryStream.FlushAsync();

            return memoryStream.ToArray();
        }

        public async Task<JobStatus> GenerateVendorCodesAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ManageVendorCodes))
            {
                var stopwatch = Stopwatch.StartNew();

                string timeElapsed;
                string timeRemaining;
                string status;
                double msPerCode;
                double remainingMs;

                double lastUpdate = 0;
                int lastSave = 0;
                int lastLog = 0;
                int count = 1;

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails = JsonConvert
                    .DeserializeObject<JobDetailsGenerateVendorCodes>(job.SerializedParameters);

                token.Register(() =>
                {
                    _logger.LogWarning("Generating vendor codes for user {RequestingUser} was cancelled after {Elapsed} ms",
                        requestingUser,
                        stopwatch.ElapsedMilliseconds);
                });

                var codeType
                    = await _vendorCodeTypeRepository.GetByIdAsync(jobDetails.VendorCodeTypeId);

                if (codeType == null)
                {
                    return new JobStatus
                    {
                        PercentComplete = 0,
                        Complete = true,
                        Error = true,
                        Status = $"Invalid vendor code type id: {jobDetails.VendorCodeTypeId}"
                    };
                }
                else
                {
                    if (codeType.SiteId != GetCurrentSiteId())
                    {
                        return new JobStatus
                        {
                            Complete = true,
                            Error = true,
                            Status = $"Vendor code type id {jobDetails.VendorCodeTypeId} is not attached to site {GetCurrentSiteId()}"
                        };
                    }
                }

                await _jobRepository.UpdateStatusAsync(jobId,
                    $"Generating {jobDetails.NumberOfCodes} for Vendor Code Type Id {jobDetails.VendorCodeTypeId}");

                _logger.LogInformation("User {RequestingUser} requested {NumberOfCodes} codes for Vendor Code Type Id {VendorCodeTypeId}",
                    requestingUser,
                    jobDetails.NumberOfCodes,
                    jobDetails.VendorCodeTypeId);

                var vendorCode = new VendorCode
                {
                    IsUsed = false,
                    SiteId = codeType.SiteId,
                    VendorCodeTypeId = codeType.Id
                };

                for (; count <= jobDetails.NumberOfCodes; count++)
                {
                    if (token.IsCancellationRequested)
                    {
                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Cancelled after generating {count} codes for {requestingUser} in {stopwatch.ElapsedMilliseconds} ms.");
                        break;
                    }

                    vendorCode.Code = _codeGenerator.Generate(jobDetails.CodeLength);
                    await _vendorCodeRepository.AddAsync(requestingUser, vendorCode);

                    if (count - lastSave > 1000)
                    {
                        await _vendorCodeRepository.SaveAsync();

                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Generated {count}/{jobDetails.NumberOfCodes}");

                        lastSave = count;
                    }

                    if (stopwatch.ElapsedMilliseconds - lastUpdate > 5000
                        || count == 1)
                    {
                        if (stopwatch.ElapsedMilliseconds <= 1000 && count <= 1)
                        {
                            status = $"Generated {count}/{jobDetails.NumberOfCodes}";
                        }
                        else
                        {
                            timeElapsed = TimeSpan
                                .FromMilliseconds(stopwatch.ElapsedMilliseconds)
                                .ToString(@"mm\:ss",
                                    System.Globalization.DateTimeFormatInfo.InvariantInfo);
                            msPerCode = (double)stopwatch.ElapsedMilliseconds / count;
                            remainingMs = msPerCode * (jobDetails.NumberOfCodes - count);
                            timeRemaining = TimeSpan
                                .FromMilliseconds(remainingMs)
                                .ToString(@"mm\:ss",
                                    System.Globalization.DateTimeFormatInfo.InvariantInfo);

                            status = $"Generated {count}/{jobDetails.NumberOfCodes}, {timeElapsed} elasped, est. {timeRemaining} remaining";

                            if (count - lastLog > jobDetails.NumberOfCodes / 10)
                            {
                                _logger.LogDebug("Vendor codes: {Percent}% {Count}/{Total} @ {Each} ea. {Elapsed} elasped, est. {Remaining} remaining",
                                    GetPercent(count, jobDetails.NumberOfCodes),
                                    count,
                                    jobDetails.NumberOfCodes,
                                    msPerCode,
                                    timeElapsed,
                                    timeRemaining);
                                lastLog = count;
                            }
                        }

                        progress?.Report(new JobStatus
                        {
                            PercentComplete = GetPercent(count, jobDetails.NumberOfCodes),
                            Status = status,
                            Error = false
                        });

                        lastUpdate = stopwatch.ElapsedMilliseconds;
                    }
                }
                await _vendorCodeRepository.SaveAsync();

                count--;

                _logger.LogInformation("Inserted {Count} vendor codes in {Elapsed} ms.",
                    count,
                    stopwatch.ElapsedMilliseconds);

                timeElapsed = TimeSpan
                    .FromMilliseconds(stopwatch.ElapsedMilliseconds)
                    .ToString(@"mm\:ss", DateTimeFormatInfo.InvariantInfo);

                await _jobRepository.UpdateStatusAsync(jobId,
                    $"Inserted {count} vendor codes in {timeElapsed}.");

                return new JobStatus
                {
                    PercentComplete = token.IsCancellationRequested
                        ? GetPercent(count, jobDetails.NumberOfCodes)
                        : 100,
                    Complete = true,
                    Status = $"Inserted {count} vendor codes in {timeElapsed}."
                };
            }
            else
            {
                return new JobStatus
                {
                    Complete = true,
                    Error = true,
                    Status = "You do not have permission to insert vendor codes."
                };
            }
        }

        public async Task<IEnumerable<VendorCodeInfo>> GetAssociatedVendorCodeInfoAsync(int userId)
        {
            var authorized = false;
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId
                || userId == GetActiveUserId()
                || HasPermission(Permission.ViewParticipantDetails))
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
                var codes = await _vendorCodeRepository.GetAssociatedVendorCodes(userId);

                var vendorCodeInfos = new List<VendorCodeInfo>();

                foreach (var code in codes)
                {
                    vendorCodeInfos.Add(await GetVendorCodeInfoAsync(code));
                }

                return vendorCodeInfos;
            }
            else
            {
                _logger.LogError(
                    "User {AuthId} doesn't have permission to view details for {UserId}.",
                    authId,
                    userId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<PackingSlipSummary> GetHoldSlipsAsync(string packingSlipNumber)
        {
            var siteId = GetCurrentSiteId();
            var site = await _siteLookupService.GetByIdAsync(siteId);
            var codes = await _vendorCodeRepository.GetHoldSlipsAsync(packingSlipNumber);
            return new PackingSlipSummary
            {
                PackingSlipNumber = packingSlipNumber,
                ProgramInfo = site.Name,
                VendorCodes = codes.OrderBy(_ => _.Details)
                    .ThenBy(_ => _.LastName)
                    .ThenBy(_ => _.FirstName)
                    .ToList()
            };
        }

        public async Task<ICollection<VendorCodeItemStatus>>
            GetOrderedNotShippedAsync(int vendorCodeTypeId, int? systemId, int? branchId)
        {
            var itemStatuses = await _vendorCodeRepository
                .GetOrderedNotShipped(vendorCodeTypeId, systemId, branchId);

            return itemStatuses.OrderBy(_ => _.OrderDate)
                .ThenBy(_ => _.ShipDate)
                .ThenBy(_ => _.ArrivalDate)
                .ToList();
        }

        public async Task<VendorCodeStatus> GetStatusAsync(int vendorCodeTypeId)
        {
            VerifyManagementPermission();
            return await _vendorCodeRepository.GetStatusAsync(vendorCodeTypeId);
        }

        public async Task<ICollection<VendorCodeType>> GetTypeAllAsync()
        {
            return await _vendorCodeTypeRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<VendorCodeType> GetTypeById(int id)
        {
            return await _vendorCodeTypeRepository.GetByIdAsync(id);
        }

        public async Task<DataWithCount<ICollection<VendorCodeType>>>
            GetTypePaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            filter ??= new BaseFilter();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<VendorCodeType>>
            {
                Data = await _vendorCodeTypeRepository.PageAsync(filter),
                Count = await _vendorCodeTypeRepository.CountAsync(filter)
            };
        }

        public async Task<ICollection<VendorCodeEmailAward>> GetUnreportedEmailAwardCodes(
            int vendorCodeTypeId)
        {
            VerifyManagementPermission();

            return await _vendorCodeRepository.GetUnreportedEmailAwardCodes(GetCurrentSiteId(),
                vendorCodeTypeId);
        }

        public async Task<VendorCode> GetUserVendorCodeAsync(int userId)
        {
            var codes = await GetUserVendorCodesAsync(userId, false);
            return codes.FirstOrDefault();
        }

        public async Task<IEnumerable<VendorCodeInfo>> GetUserVendorCodesInfoAsync(int userId)
        {
            var vendorCodes = await GetUserVendorCodesAsync(userId, true);
            if (vendorCodes?.Count() > 0)
            {
                var vendorCodeInfos = new List<VendorCodeInfo>();
                foreach (var vendorCode in vendorCodes)
                {
                    vendorCodeInfos.Add(await GetVendorCodeInfoAsync(vendorCode));
                }
                return vendorCodeInfos;
            }
            return null;
        }

        public async Task<VendorCode> GetVendorCodeByCode(string code)
        {
            return await _vendorCodeRepository.GetByCode(code);
        }

        public async Task PopulateVendorCodeStatusAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var vendorCode = await GetUserVendorCodeAsync(user.Id);

            if (!string.IsNullOrEmpty(vendorCode?.Code))
            {
                var vendorCodeInfo = await GetVendorCodeInfoAsync(vendorCode);

                user.CanDonateVendorCode = vendorCodeInfo.CanDonate;
                user.CanEmailAwardVendorCode = vendorCodeInfo.CanEmailAward;
                user.Donated = vendorCodeInfo.IsDonated;
                user.EmailAwarded = vendorCodeInfo.IsEmailAwarded;
                user.EmailAwardInstructions = vendorCodeInfo.EmailAwardInstructions;
                user.IsDonationLocked = vendorCodeInfo.IsDonationLocked;
                user.NeedsToAnswerVendorCodeQuestion
                    = vendorCodeInfo.NeedsToAnswerVendorCodeQuestion;
                user.VendorCode = vendorCodeInfo.VendorCodeDisplay;
                user.VendorCodeMessage = vendorCodeInfo.VendorCodeMessage;
                user.VendorCodePackingSlip = vendorCodeInfo.VendorCodePackingSlip;
                user.VendorCodeUrl = vendorCodeInfo.VendorCodeUrl;
                user.VendorIsDamaged = vendorCodeInfo.IsDamaged;
                user.VendorIsMissing = vendorCodeInfo.IsMissing;
                user.VendorOrderStatus = vendorCodeInfo.OrderStatus;
            }
        }

        public async Task<JobStatus> ReceivePackingSlipJobAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ReceivePackingSlips))
            {
                _logger.LogError("User {UserId} doesn't have permission to receive packing slips.",
                    requestingUser);

                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
            else
            {
                var sw = Stopwatch.StartNew();

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsReceivePackingSlip>(job.SerializedParameters);

                string packingSlipNumber = jobDetails.PackingSlipNumber;

                token.Register(() =>
                {
                    _logger.LogWarning("Receipt of packing slip {PackingSlipNumber} for user {UserId} was cancelled after {Elapsed} ms",
                        packingSlipNumber,
                        requestingUser,
                        sw?.Elapsed.TotalMilliseconds);
                });

                var codes = await _vendorCodeRepository
                    .GetByPackingSlipAsync(packingSlipNumber);

                int recordCount = 0;
                int totalRecords = codes.Count;

                VendorCodeType vendorCodeType = null;

                var receivedAt = _dateTimeProvider.Now;

                int emailsSent = 0;
                int emailErrors = 0;
                var emailDetails = new Model.Utility.DirectEmailDetails(jobDetails.SiteName)
                {
                    SendingUserId = GetActiveUserId()
                };

                foreach (var code in codes)
                {
                    if (vendorCodeType == null || vendorCodeType.Id != code.VendorCodeTypeId)
                    {
                        vendorCodeType = await _vendorCodeTypeRepository
                            .GetByIdAsync(code.VendorCodeTypeId);
                    }

                    code.ArrivalDate = receivedAt;

                    code.IsDamaged = jobDetails.DamagedItems?.Contains(code.Id) == true;
                    if (jobDetails.DamagedItems?.Contains(code.Id) == true)
                    {
                        code.IsDamaged = true;
                        code.IsMissing = false;
                    }
                    else if (jobDetails.MissingItems?.Contains(code.Id) == true)
                    {
                        code.IsMissing = true;
                        code.IsDamaged = false;
                    }
                    else
                    {
                        code.IsDamaged = false;
                        code.IsMissing = false;
                    }

                    await _vendorCodeRepository.UpdateSaveNoAuditAsync(code);

                    if (vendorCodeType.AwardPrizeOnPackingSlip
                        && code.IsDamaged != true
                        && code.IsMissing != true)
                    {
                        bool? result = await AwardPrizeSendMailAsync(code,
                            vendorCodeType,
                            emailDetails);

                        if (result == true)
                        {
                            emailsSent++;
                        }
                        else if (result == false)
                        {
                            emailErrors++;
                        }
                    }

                    recordCount++;

                    if (recordCount % 10 == 0 || recordCount == 1)
                    {
                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Processing record {recordCount}/{totalRecords}...");

                        progress?.Report(new JobStatus
                        {
                            PercentComplete = recordCount * 100 / totalRecords,
                            Status = $"Processing record {recordCount}/{totalRecords}...",
                            Error = false
                        });
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (token.IsCancellationRequested)
                {
                    await _jobRepository.UpdateStatusAsync(jobId,
                        $"Import cancelled at record {recordCount}/{totalRecords}.");

                    return new JobStatus
                    {
                        Status = $"Operation cancelled at record {recordCount}."
                    };
                }
                else
                {
                    await _jobRepository.UpdateStatusAsync(jobId,
                        $"Inserting packing slip {packingSlipNumber}...");

                    progress?.Report(new JobStatus
                    {
                        PercentComplete = recordCount * 100 / totalRecords,
                        Status = $"Inserting packing slip {packingSlipNumber}...",
                        Error = false
                    });

                    await _vendorCodePackingSlipRepository.AddSaveAsync(requestingUser,
                        new VendorCodePackingSlip
                        {
                            IsReceived = true,
                            CreatedAt = receivedAt,
                            CreatedBy = requestingUser,
                            PackingSlip = packingSlipNumber,
                            SiteId = GetCurrentSiteId()
                        });
                }

                await _jobRepository.UpdateStatusAsync(jobId,
                    $"Updated {recordCount} records of {totalRecords}, inserted packing slip {packingSlipNumber}, sent {emailsSent} emails, {emailErrors} email errors.");

                _logger.LogInformation("Updated {RecordCount} records of {TotalRecords}, inserted packing slip {PackingSlipNumber} sent {EmailsSent} emails, {EmailErrors} email errors in {Elapsed} ms",
                    recordCount,
                    totalRecords,
                    packingSlipNumber,
                    emailsSent,
                    emailErrors,
                    sw?.ElapsedMilliseconds ?? 0);

                var sb = new StringBuilder("Packing slip <strong>")
                    .Append(packingSlipNumber)
                    .Append("</strong> received: ")
                    .Append(recordCount)
                    .Append(" records updated, ")
                    .Append(emailsSent)
                    .Append(" emails sent");
                if (emailErrors > 0)
                {
                    sb.Append(", ")
                    .Append(emailErrors)
                    .Append(" email errors");
                }
                sb.Append('.');

                return new JobStatus
                {
                    PercentComplete = 100,
                    Complete = true,
                    Status = sb.ToString(),
                };
            }
        }

        public async Task<int> RedeemHouseholdCodes(int headOfHouseholdId)
        {
            VerifyPermission(Permission.RedeemBulkVendorCodes);
            var authId = GetClaimId(ClaimType.UserId);

            var householdPendingCodes = await _vendorCodeRepository
                .GetPendingHouseholdCodes(headOfHouseholdId);

            foreach (var code in householdPendingCodes)
            {
                code.IsDonated = false;
                code.IsEmailAward = false;
                await _vendorCodeRepository.UpdateSaveAsync(authId, code);
            }

            return householdPendingCodes.Count;
        }

        public async Task<VendorCode> ResolveCodeStatusAsync(int userId,
            bool? donate,
            bool? emailAward,
            string emailAddrses = null)
        {
            var authorized = false;
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId
                || userId == GetActiveUserId()
                || HasPermission(Permission.ViewParticipantDetails))
            {
                authorized = true;
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (!authorized)
            {
                authorized = user.HouseholdHeadUserId == authId;
            }

            if (authorized)
            {
                var siteId = GetClaimId(ClaimType.SiteId);

                var vendorCode = await _vendorCodeRepository.GetUserVendorCode(userId);

                var vendorCodeType = await _vendorCodeTypeRepository.GetByIdAsync(
                    vendorCode.VendorCodeTypeId);

                if (_dateTimeProvider.Now >= vendorCodeType.ExpirationDate)
                {
                    _logger.LogError("Vendor code {VendorCodeTypeId} has expired.",
                        vendorCodeType.Id);
                    throw new GraException("The code you are trying to redeem has expired.");
                }

                if (donate == true && vendorCodeType.DonationMessageTemplateId.HasValue)
                {
                    vendorCode.IsDonated = true;
                    vendorCode.IsEmailAward = false;
                }
                else if (emailAward == true && !string.IsNullOrWhiteSpace(emailAddrses)
                    && vendorCodeType.EmailAwardMessageTemplateId.HasValue)
                {
                    vendorCode.EmailAwardAddress = emailAddrses;
                    vendorCode.IsDonated = false;
                    vendorCode.IsEmailAward = true;
                }
                else if (donate == false && emailAward == false)
                {
                    if (!vendorCode.IsDonationLocked)
                    {
                        vendorCode.IsDonated = false;
                    }
                    else
                    {
                        throw new GraException("Unable to remove donation commitment.");
                    }
                    vendorCode.IsEmailAward = false;
                }
                else if (donate == null && emailAward == null)
                {
                    vendorCode.EmailAwardAddress = null;
                    vendorCode.IsDonated = null;
                    vendorCode.IsEmailAward = null;
                }
                await _vendorCodeRepository.UpdateSaveAsync(userId, vendorCode);

                int languageId = string.IsNullOrEmpty(user.Culture)
                    ? await _languageService.GetDefaultLanguageIdAsync()
                    : await _languageService.GetLanguageIdAsync(user.Culture);

                if (vendorCode.IsDonated == true)
                {
                    await SendVendorDonationMailAsync(userId, languageId, siteId, vendorCodeType);
                }
                else if (vendorCode.IsEmailAward == true)
                {
                    await SendVendorEmailAwardMailAsync(userId, languageId, siteId, vendorCodeType);
                }
                else if (vendorCode.IsDonated == false && vendorCode.IsEmailAward == false)
                {
                    await SendVendorCodeMailAsync(userId,
                        languageId,
                        siteId,
                        vendorCodeType,
                        vendorCode.Code);
                }

                return vendorCode;
            }
            else
            {
                _logger.LogError("User {AuthenticatedUserId} doesn't have permission to update code donation status for {UserId}.",
                    authId,
                    userId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task SaveAsync()
        {
            await _vendorCodeRepository.SaveAsync();
        }

        public async Task<bool> SiteHasCodesAsync()
        {
            return await _vendorCodeTypeRepository.SiteHasCodesAsync(GetCurrentSiteId());
        }

        public async Task<bool?> UnmarkDamagedMissingAsync(int currentUserId, string vendorCode)
        {
            var code = await _vendorCodeRepository.GetByCode(vendorCode);
            code.IsDamaged = false;
            code.IsMissing = false;
            await _vendorCodeRepository.UpdateSaveAsync(currentUserId, code);
            var type = await _vendorCodeTypeRepository.GetByIdAsync(code.VendorCodeTypeId);

            var site = await _siteLookupService.GetByIdAsync(GetCurrentSiteId());
            var emailDetails = new Model.Utility.DirectEmailDetails(site.Name)
            {
                SendingUserId = GetActiveUserId()
            };

            return await AwardPrizeSendMailAsync(code, type, emailDetails);
        }

        public async Task<JobStatus> UpdateEmailAwardStatusFromExcelAsync(int jobId,
                    CancellationToken token,
                    IProgress<JobStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ManageVendorCodes))
            {
                var sw = Stopwatch.StartNew();

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsVendorCodeStatus>(job.SerializedParameters);

                string filename = jobDetails.Filename;

                token.Register(() =>
                {
                    string duration = "";
                    if (sw?.Elapsed != null)
                    {
                        duration = $" after {sw.Elapsed:c}";
                    }

                    _logger.LogWarning("Import of {Filename} for user {RequestingUser} was cancelled{Duration}.",
                        filename,
                        requestingUser,
                        duration);
                });

                string fullPath = _pathResolver.ResolvePrivateTempFilePath(filename);

                if (!File.Exists(fullPath))
                {
                    _logger.LogError("Could not find {FullPath}", fullPath);
                    return new JobStatus
                    {
                        PercentComplete = 0,
                        Status = "Could not find the import file.",
                        Error = true,
                        Complete = true
                    };
                }

                try
                {
                    await using var stream = new FileStream(fullPath, FileMode.Open);
                    int emailAddressColumnId = 0;
                    int sentDateColumnId = 0;
                    int userIdColumnId = 0;
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
                                progress?.Report(new JobStatus
                                {
                                    PercentComplete = row * 100 / totalRows,
                                    Status = $"Processing row {row}/{totalRows}...",
                                    Error = false
                                });
                            }
                            if (row == 1)
                            {
                                progress?.Report(new JobStatus
                                {
                                    PercentComplete = 1,
                                    Status = $"Processing row {row}/{totalRows}...",
                                    Error = false
                                });
                                for (int i = 0; i < excelReader.FieldCount; i++)
                                {
                                    switch (excelReader.GetString(i)?.Trim() ?? $"Column{i}")
                                    {
                                        case EmailAddressRowHeading:
                                            emailAddressColumnId = i;
                                            break;

                                        case SentDateRowHeading:
                                            sentDateColumnId = i;
                                            break;

                                        case UserIdRowHeading:
                                            userIdColumnId = i;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (excelReader.GetValue(emailAddressColumnId) != null
                                    && excelReader.GetValue(sentDateColumnId) != null
                                    && excelReader.GetValue(userIdColumnId) != null)
                                {
                                    string emailAddress = null;
                                    DateTime? sentDate = null;
                                    int? userId = null;

                                    try
                                    {
                                        emailAddress = GetExcelString(excelReader,
                                            row,
                                            emailAddressColumnId,
                                            "email address");
                                    }
                                    catch (GraException gex)
                                    {
                                        issues.Add(gex.Message);
                                    }

                                    try
                                    {
                                        sentDate = GetExcelDateTime(excelReader,
                                            row,
                                            sentDateColumnId,
                                            "sent date");
                                    }
                                    catch (GraException gex)
                                    {
                                        issues.Add(gex.Message);
                                    }

                                    try
                                    {
                                        userId = GetExcelInt(excelReader,
                                            row,
                                            userIdColumnId,
                                            "user id");
                                    }
                                    catch (GraException gex)
                                    {
                                        issues.Add(gex.Message);
                                    }

                                    if (!string.IsNullOrEmpty(emailAddress)
                                        && sentDate.HasValue
                                        && userId.HasValue)
                                    {
                                        var code = await _vendorCodeRepository
                                            .GetUserVendorCode(userId.Value);
                                        if (code == null)
                                        {
                                            _logger.LogError("File contained code for user {UserId} which was not found in the database", userId);
                                            issues.Add($"Uploaded file contained code for user <code>{userId}</code> which couldn't be found in the database.");
                                        }
                                        else
                                        {
                                            if (sentDate == code.EmailAwardSent)
                                            {
                                                alreadyCurrent++;
                                            }
                                            else
                                            {
                                                if (sentDate != null)
                                                {
                                                    code.EmailAwardSent = sentDate;
                                                }
                                                await _vendorCodeRepository
                                                    .UpdateSaveNoAuditAsync(code);
                                                updated++;
                                            }
                                        }
                                    }
                                }
                            }
                            if (token.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Cancelled importing at row {row}.");

                        return new JobStatus
                        {
                            Status = $"Operation cancelled at row {row}."
                        };
                    }

                    var sb = new StringBuilder("<strong>Import complete</strong>");
                    if (updated > 0)
                    {
                        sb.Append(": ").Append(updated).Append(" records were updated");
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
                        sb.Append(alreadyCurrent).Append(" records were already current");
                    }
                    sb.Append('.');

                    await _jobRepository.UpdateStatusAsync(jobId,
                         $"Import complete with {issues.Count} issues in {sw.ElapsedMilliseconds} ms");

                    if (issues.Count > 0)
                    {
                        _logger.LogInformation("Import complete with issues: {Issues}", sb);
                        sb.Append(" Issues detected:<ul>");
                        foreach (string issue in issues)
                        {
                            sb.Append("<li>").Append(issue).Append("</li>");
                        }
                        sb.Append("</ul>");

                        return new JobStatus
                        {
                            PercentComplete = 100,
                            Complete = true,
                            Status = sb.ToString(),
                            Error = true
                        };
                    }
                    else
                    {
                        _logger.LogInformation("Import success: {Details}", sb);
                        return new JobStatus
                        {
                            PercentComplete = 100,
                            Complete = true,
                            Status = sb.ToString(),
                        };
                    }
                }
                finally
                {
                    File.Delete(fullPath);
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to import email award code statuses.",
                    requestingUser);
                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
        }

        public async Task UpdateEmailNotReportedAsync(int currentUserId,
                    IEnumerable<VendorCodeEmailAward> emailAwards)
        {
            VerifyManagementPermission();

            if (emailAwards != null)
            {
                foreach (var emailAward in emailAwards)
                {
                    var vendorCode = await _vendorCodeRepository
                        .GetByIdAsync(emailAward.VendorCodeId);
                    vendorCode.EmailAwardReported = null;
                    await _vendorCodeRepository.UpdateAsync(currentUserId, vendorCode);
                }
                await _vendorCodeRepository.SaveAsync();
            }
        }

        public async Task UpdateEmailReportedAsync(int currentUserId,
                    DateTime when,
                    int vendorCodeId)
        {
            VerifyManagementPermission();

            var vendorCode = await _vendorCodeRepository.GetByIdAsync(vendorCodeId);
            vendorCode.EmailAwardReported = when;
            await _vendorCodeRepository.UpdateAsync(currentUserId, vendorCode);
        }

        public async Task<JobStatus> UpdateStatusFromExcelAsync(int jobId,
                    CancellationToken token,
                    IProgress<JobStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ManageVendorCodes))
            {
                var sw = Stopwatch.StartNew();

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsVendorCodeStatus>(job.SerializedParameters);

                string filename = jobDetails.Filename;

                using (LogContext.PushProperty("ImportFileName", jobDetails.OriginalFilename))
                {
                    token.Register(() =>
                    {
                        _logger.LogWarning("Import of {FilePath} for user {UserId} was cancelled after {Elapsed} ms",
                            filename,
                            requestingUser,
                            sw?.Elapsed.TotalMilliseconds);
                    });

                    string fullPath = _pathResolver.ResolvePrivateTempFilePath(filename);

                    if (!File.Exists(fullPath))
                    {
                        _logger.LogError("Could not find {FilePath}", fullPath);
                        return new JobStatus
                        {
                            PercentComplete = 0,
                            Status = "Could not find the import file.",
                            Error = true,
                            Complete = true
                        };
                    }

                    try
                    {
                        await using var stream = new FileStream(fullPath, FileMode.Open);
                        int branchColumnId = 0;
                        int couponColumnId = 0;
                        int detailsColumnId = 0;
                        int orderDateColumnId = 0;
                        int shipDateColumnId = 0;
                        int packingSlipColumnId = 0;
                        int trackingNumberColumnId = 0;
                        bool hasPackingSlipColumn = false;
                        bool hasTrackingNumberColumn = false;
                        var issues = new List<string>();
                        int row = 0;
                        int totalRows = 0;
                        int updated = 0;
                        int alreadyCurrent = 0;
                        int donationCount = 0;
                        int undonationCount = 0;
                        var vendorCodeType = await _vendorCodeTypeRepository
                            .GetByIdAsync(jobDetails.VendorCodeTypeId);

                        int emailsSent = 0;
                        int emailErrors = 0;
                        var emailDetails = new Model.Utility.DirectEmailDetails(jobDetails.SiteName)
                        {
                            SendingUserId = GetActiveUserId()
                        };

                        using (var excelReader = ExcelReaderFactory.CreateReader(stream))
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
                                using (LogContext.PushProperty("ImportFileRow", row))
                                {
                                    if (row % 10 == 0)
                                    {
                                        await _jobRepository.UpdateStatusAsync(jobId,
                                            $"Processing row {row}/{totalRows}...");

                                        progress?.Report(new JobStatus
                                        {
                                            PercentComplete = row * 100 / totalRows,
                                            Status = $"Processing row {row}/{totalRows}...",
                                            Error = false
                                        });
                                    }
                                    if (row == 1)
                                    {
                                        progress?.Report(new JobStatus
                                        {
                                            PercentComplete = 1,
                                            Status = $"Processing row {row}/{totalRows}...",
                                            Error = false
                                        });
                                        for (int i = 0; i < excelReader.FieldCount; i++)
                                        {
                                            switch (excelReader.GetString(i)?.Trim() ?? $"Column{i}")
                                            {
                                                case BranchIdRowHeading:
                                                    branchColumnId = i;
                                                    break;

                                                case CouponRowHeading:
                                                    couponColumnId = i;
                                                    break;

                                                case DetailsRowHeading:
                                                    detailsColumnId = i;
                                                    break;

                                                case OrderDateRowHeading:
                                                    orderDateColumnId = i;
                                                    break;

                                                case ShipDateRowHeading:
                                                    shipDateColumnId = i;
                                                    break;

                                                case PackingSlipRowHeading:
                                                    packingSlipColumnId = i;
                                                    hasPackingSlipColumn = true;
                                                    break;

                                                case TrackingNumberRowHeading:
                                                    trackingNumberColumnId = i;
                                                    hasTrackingNumberColumn = true;
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (excelReader.GetValue(couponColumnId) != null
                                            && excelReader.GetValue(branchColumnId) == null)
                                        {
                                            string coupon = null;
                                            try
                                            {
                                                coupon = GetExcelString(excelReader,
                                                    row,
                                                    couponColumnId,
                                                    "code");
                                                coupon = coupon?.Trim();
                                            }
                                            catch (GraException gex)
                                            {
                                                issues.Add(gex.Message);
                                            }
                                            if (!string.IsNullOrEmpty(coupon))
                                            {
                                                try
                                                {
                                                    _logger.LogInformation("In import of {FileName}, code {Code} has no branch id, marking as donated",
                                                        filename,
                                                        coupon);
                                                    await MarkCodeDonatedAsync(coupon);
                                                    updated++;
                                                    donationCount++;
                                                }
                                                catch (GraException gex)
                                                {
                                                    issues.Add(gex.Message);
                                                }
                                            }
                                        }
                                        else if (excelReader.GetValue(couponColumnId) != null
                                            && (excelReader.GetValue(orderDateColumnId) != null
                                                || excelReader.GetValue(shipDateColumnId) != null
                                                || excelReader.GetValue(detailsColumnId) != null
                                                || excelReader.GetValue(branchColumnId) != null
                                                || excelReader.GetValue(packingSlipColumnId) != null
                                                || excelReader.GetValue(trackingNumberColumnId) != null))
                                        {
                                            string coupon = null;
                                            DateTime? orderDate = null;
                                            DateTime? shipDate = null;
                                            string packingSlip = null;
                                            string trackingNumber = null;
                                            string details = null;
                                            int? branchId = null;

                                            try
                                            {
                                                coupon = GetExcelString(excelReader,
                                                    row,
                                                    couponColumnId,
                                                    "code");
                                                coupon = coupon?.Trim();
                                            }
                                            catch (InvalidCastException icex)
                                            {
                                                issues.Add($"The value in row {row} for {CouponRowHeading} is invalid: {icex.Message}");
                                            }
                                            catch (GraException gex)
                                            {
                                                issues.Add(gex.Message);
                                            }

                                            try
                                            {
                                                orderDate = GetExcelDateTime(excelReader,
                                                    row,
                                                    orderDateColumnId,
                                                    "order date");
                                            }
                                            catch (InvalidCastException icex)
                                            {
                                                issues.Add($"The value in row {row} for {OrderDateRowHeading} is invalid: {icex.Message}");
                                            }
                                            catch (GraException gex)
                                            {
                                                issues.Add(gex.Message);
                                            }

                                            try
                                            {
                                                shipDate = GetExcelDateTime(excelReader,
                                                    row,
                                                    shipDateColumnId,
                                                    "ship date");
                                            }
                                            catch (InvalidCastException icex)
                                            {
                                                issues.Add($"The value in row {row} for {ShipDateRowHeading} is invalid: {icex.Message}");
                                            }
                                            catch (GraException gex)
                                            {
                                                issues.Add(gex.Message);
                                            }

                                            try
                                            {
                                                details = GetExcelString(excelReader,
                                                    row,
                                                    detailsColumnId,
                                                    "details");
                                            }
                                            catch (InvalidCastException icex)
                                            {
                                                issues.Add($"The value in row {row} for {DetailsRowHeading} is invalid: {icex.Message}");
                                            }
                                            catch (GraException gex)
                                            {
                                                issues.Add(gex.Message);
                                            }

                                            try
                                            {
                                                branchId = GetExcelInt(excelReader,
                                                    row,
                                                    branchColumnId,
                                                    "branch id");
                                            }
                                            catch (InvalidCastException icex)
                                            {
                                                issues.Add($"The value in row {row} for {BranchIdRowHeading} is invalid: {icex.Message}");
                                            }
                                            catch (GraException gex)
                                            {
                                                issues.Add(gex.Message);
                                            }

                                            if (hasPackingSlipColumn)
                                            {
                                                try
                                                {
                                                    packingSlip = GetExcelString(excelReader,
                                                        row,
                                                        packingSlipColumnId,
                                                        "packing slip");
                                                }
                                                catch (InvalidCastException icex)
                                                {
                                                    issues.Add($"The value in row {row} for {PackingSlipRowHeading} is invalid: {icex.Message}");
                                                }
                                                catch (GraException gex)
                                                {
                                                    issues.Add(gex.Message);
                                                }
                                            }

                                            if (hasTrackingNumberColumn)
                                            {
                                                try
                                                {
                                                    trackingNumber = GetExcelString(excelReader,
                                                        row,
                                                        trackingNumberColumnId,
                                                        "tracking number");
                                                    if (trackingNumber?.Length > 512)
                                                    {
                                                        issues.Add($"Tracking number on row {row} is too long, truncating at 512 characters!");
                                                        trackingNumber = trackingNumber[..512];
                                                    }
                                                }
                                                catch (InvalidCastException icex)
                                                {
                                                    issues.Add($"The value in row {row} for {TrackingNumberRowHeading} is invalid: {icex.Message}");
                                                }
                                                catch (GraException gex)
                                                {
                                                    issues.Add(gex.Message);
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(coupon)
                                                && (orderDate != null
                                                    || shipDate != null
                                                    || !string.IsNullOrEmpty(details)
                                                    || branchId != null
                                                    || packingSlip != default
                                                    || trackingNumber != null))
                                            {
                                                var code = await _vendorCodeRepository
                                                    .GetByCode(coupon);
                                                if (code == null)
                                                {
                                                    _logger.LogError("File contained code {Code} which was not found in the database",
                                                        coupon);
                                                    issues.Add($"Uploaded file contained code <code>{coupon}</code> which couldn't be found in the database.");
                                                }
                                                else
                                                {
                                                    bool recordIsCurrent = (code.ArrivalDate.HasValue
                                                            && code.IsDamaged != true
                                                            && code.IsMissing != true)
                                                        || (orderDate == code.OrderDate
                                                            && shipDate == code.ShipDate
                                                            && details == code.Details
                                                            && branchId == code.BranchId
                                                            && trackingNumber == code.TrackingNumber
                                                            && (packingSlip == default
                                                                || code.PackingSlip == packingSlip));

                                                    if (recordIsCurrent)
                                                    {
                                                        // looks current but let's verify that if there is
                                                        // a branchId that it is NOT set as donated
                                                        if (branchId.HasValue && code.IsDonated == true)
                                                        {
                                                            _logger.LogInformation("Import: {FileName}, code {Code} has a branch id but is marked donated, un-donating",
                                                                filename,
                                                                code.Code);
                                                            recordIsCurrent = false;
                                                            undonationCount++;
                                                        }
                                                    }

                                                    if (recordIsCurrent)
                                                    {
                                                        alreadyCurrent++;
                                                    }
                                                    else
                                                    {
                                                        DateTime? arrivalDate = null;

                                                        if (packingSlip != default
                                                            && code.ArrivalDate == null)
                                                        {
                                                            arrivalDate = (await GetPs(packingSlip))?
                                                                .CreatedAt;
                                                        }

                                                        code = await UpdateVendorCodeAsync(code,
                                                            orderDate,
                                                            shipDate,
                                                            details,
                                                            branchId,
                                                            packingSlip,
                                                            trackingNumber,
                                                            arrivalDate);

                                                        bool prizeAwarded = false;

                                                        if (shipDate != null
                                                            && branchId != null
                                                            && vendorCodeType.AwardPrizeOnShipDate)
                                                        {
                                                            await AwardPrizeAsync(code);
                                                            prizeAwarded = true;
                                                        }

                                                        if (packingSlip != default
                                                            && vendorCodeType.AwardPrizeOnPackingSlip
                                                            && await GetPs(packingSlip) != null)
                                                        {
                                                            await AwardPrizeAsync(code);
                                                            prizeAwarded = true;
                                                        }

                                                        if (prizeAwarded
                                                            && vendorCodeType
                                                                .ReadyForPickupEmailTemplateId
                                                                .HasValue
                                                            && code.UserId.HasValue)
                                                        {
                                                            var result = await SendPickupEmailAsync(
                                                                vendorCodeType
                                                                    .ReadyForPickupEmailTemplateId
                                                                    .Value,
                                                                code.UserId.Value,
                                                                emailDetails,
                                                                code);

                                                            if (result == true)
                                                            {
                                                                emailsSent++;
                                                            }
                                                            else if (result == false)
                                                            {
                                                                emailErrors++;
                                                            }
                                                        }

                                                        updated++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }
                            }
                        }

                        if (token.IsCancellationRequested)
                        {
                            await _jobRepository.UpdateStatusAsync(jobId,
                                $"Import cancelled at row {row}/{totalRows}.");

                            return new JobStatus
                            {
                                Status = $"Operation cancelled at row {row}."
                            };
                        }

                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Updated {updated} records, {donationCount} donations, {undonationCount} un-donations, {alreadyCurrent} already current, {issues?.Count ?? 0} issues.");

                        _logger.LogInformation("Import of {FileName} completed: {UpdatedRecords} updates, {DonationCount} donations, {UndonationCount} un-donations, {CurrentRecords} already current, {IssueCount} issues, {EmailsSent} sent emails, {ErrorEmails} errors emails in {Elapsed} ms",
                            filename,
                            updated,
                            donationCount,
                            undonationCount,
                            alreadyCurrent,
                            issues?.Count ?? 0,
                            emailsSent,
                            emailErrors,
                            sw?.ElapsedMilliseconds ?? 0);

                        var sb = new StringBuilder("<strong>Import complete</strong>");
                        if (updated > 0)
                        {
                            sb.Append(": ").Append(updated).Append(" records were updated");
                        }
                        if (donationCount > 0)
                        {
                            sb.Append(", ").Append(donationCount).Append(" donations");
                        }
                        if (undonationCount > 0)
                        {
                            sb.Append(", ").Append(undonationCount).Append(" un-donations");
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
                            sb.Append(alreadyCurrent).Append(" records were already current");
                        }
                        if (emailsSent + emailErrors > 0)
                        {
                            sb.Append(", sent ")
                                .Append(emailsSent)
                                .Append(" emails");
                            if (emailErrors > 0)
                            {
                                sb.Append(" with ")
                                .Append(emailErrors)
                                .Append(" email errors");
                            }
                        }
                        sb.Append('.');

                        if (issues.Count > 0)
                        {
                            sb.Append(" Issues detected:<ul>");
                            foreach (string issue in issues)
                            {
                                sb.Append("<li>").Append(issue).Append("</li>");
                            }
                            sb.Append("</ul>");
                            return new JobStatus
                            {
                                PercentComplete = 100,
                                Complete = true,
                                Status = sb.ToString(),
                                Error = true
                            };
                        }
                        else
                        {
                            return new JobStatus
                            {
                                PercentComplete = 100,
                                Complete = true,
                                Status = sb.ToString(),
                            };
                        }
                    }
                    finally
                    {
                        File.Delete(fullPath);
                    }
                }
            }
            else
            {
                _logger.LogError(
                    "User {UserId} doesn't have permission to import vendor code statuses.",
                    requestingUser);
                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
        }

        public Task<VendorCodeType> UpdateTypeAsync(VendorCodeType vendorCodeType)
        {
            ArgumentNullException.ThrowIfNull(vendorCodeType);
            return UpdateTypeInternalAsync(vendorCodeType);
        }

        public async Task<PackingSlipSummary> VerifyPackingSlipAsync(string packingSlipNumber)
        {
            var codes = await _vendorCodeRepository.GetByPackingSlipAsync(packingSlipNumber);
            var packingSlipSummary = new PackingSlipSummary
            {
                PackingSlipNumber = packingSlipNumber,
                VendorCodes = codes.OrderBy(_ => _.Details)
                    .ThenBy(_ => _.LastName)
                    .ThenBy(_ => _.FirstName)
                    .ToList(),
                VendorCodePackingSlip = await _vendorCodePackingSlipRepository
                    .GetByPackingSlipNumberAsync(packingSlipNumber)
            };

            _logger.LogInformation("Verifying packing slip {PackingSlipNumber} has {VendorCodeCount} vendor codes associated {Status}",
                packingSlipSummary.PackingSlipNumber,
                packingSlipSummary.VendorCodes?.Count ?? 0,
                packingSlipSummary.VendorCodePackingSlip?.CreatedAt != null
                    ? $"already inserted at {packingSlipSummary.VendorCodePackingSlip?.CreatedAt}"
                    : "not inserted yet");

            return packingSlipSummary;
        }

        public async Task<IDictionary<string, DateTime>> ViewedPackingSlipsAsync(int userId)
        {
            return await _userRepository.GetViewedPackingSlipsAsync(userId);
        }

        private static ILookup<string, string> ValidateVendorCodeType(VendorCodeType vendorCodeType)
        {
            var fieldErrors = new FieldErrorList();

            if (vendorCodeType.AwardPrizeOnPackingSlip && vendorCodeType.AwardPrizeOnShipDate)
            {
                fieldErrors.Add(nameof(vendorCodeType.AwardPrizeOnPackingSlip),
                    "Please only award prize based on packing slip or ship date.");
                fieldErrors.Add(nameof(vendorCodeType.AwardPrizeOnShipDate),
                    "Please only award prize based on packing slip or ship date.");
            }

            return fieldErrors.AsILookup();
        }

        private async Task<VendorCodeType> AddTypeInternalAsync(VendorCodeType vendorCodeType)
        {
            VerifyManagementPermission();
            vendorCodeType.SiteId = GetCurrentSiteId();

            var fieldErrors = ValidateVendorCodeType(vendorCodeType);

            if (fieldErrors.Count > 0)
            {
                throw new GraFieldValidationException(fieldErrors);
            }

            return await _vendorCodeTypeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeType);
        }

        private async Task AwardPrizeAsync(VendorCode code)
        {
            var userId = code.UserId.Value;
            var vcPrizes = await _prizeWinnerService.GetVendorCodePrizes(userId);
            if (vcPrizes.SingleOrDefault(_ => _.VendorCodeId == code.Id) == null)
            {
                await _prizeWinnerService.AddPrizeWinnerAsync(new PrizeWinner
                {
                    CreatedBy = userId,
                    UserId = userId,
                    VendorCodeId = code.Id,
                });
            }
        }

        /// <summary>
        /// Award the prize and send the email if there's one to be sent
        /// </summary>
        /// <param name="vendorCode">The vendor code</param>
        /// <param name="vendorCodeType">The vendor code type</param>
        /// <param name="emailDetails">An emailDetails if there's a need to send emails</param>
        /// <returns></returns>
        private async Task<bool?> AwardPrizeSendMailAsync(VendorCode vendorCode,
            VendorCodeType vendorCodeType,
            Model.Utility.DirectEmailDetails emailDetails)
        {
            bool? result = null;
            await AwardPrizeAsync(vendorCode);

            if (vendorCodeType.ReadyForPickupEmailTemplateId.HasValue
                && vendorCode.UserId.HasValue)
            {
                try
                {
                    result = await SendPickupEmailAsync(
                        vendorCodeType.ReadyForPickupEmailTemplateId.Value,
                        vendorCode.UserId.Value,
                        emailDetails,
                        vendorCode);
                }
                catch (GraException)
                {
                    result = false;
                }
            }

            return result;
        }

        private async Task<VendorCodePackingSlip> GetPs(string packingSlip)
        {
            VendorCodePackingSlip ps;

            PsCache ??= new Dictionary<string, VendorCodePackingSlip>();

            if (PsCache.TryGetValue(packingSlip, out VendorCodePackingSlip value))
            {
                return value;
            }
            else
            {
                ps = await _vendorCodePackingSlipRepository
                    .GetByPackingSlipNumberAsync(packingSlip);
                PsCache.Add(packingSlip, ps);
                _logger.LogInformation(
                    "Found records for packing slip {PackingSlip} which {Status}",
                    packingSlip,
                    ps != null
                    ? $"was received on {ps.CreatedAt:d}"
                    : "has not been received");
                return ps;
            }
        }

        private async Task<IEnumerable<VendorCode>> GetUserVendorCodesAsync(int userId,
            bool multiple)
        {
            var authorized = false;
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId
                || userId == GetActiveUserId()
                || HasPermission(Permission.ViewParticipantDetails))
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
                var vendorCodes = await _vendorCodeRepository.GetUserVendorCodes(userId);
                foreach (var vendorCode in vendorCodes.Where(_ => _ != null))
                {
                    var codeType = await _vendorCodeTypeRepository
                        .GetByIdAsync(vendorCode.VendorCodeTypeId);
                    vendorCode.CanBeDonated = codeType.DonationMessageTemplateId.HasValue;
                    vendorCode.CanBeEmailAward = codeType.EmailAwardMessageTemplateId.HasValue;
                    vendorCode.ExpirationDate = codeType.ExpirationDate;
                    if (!string.IsNullOrEmpty(codeType.Url))
                    {
                        var user = await _userRepository.GetByIdAsync(userId);

                        vendorCode.Url = await new StubbleBuilder().Build()
                            .RenderAsync(codeType.Url, new Dictionary<string, string>
                            {
                                { TemplateToken.VendorBranchToken, user.BranchId
                                    .ToString(CultureInfo.InvariantCulture) },
                                { TemplateToken.VendorCodeToken, vendorCode.Code }
                            });
                    }
                    if (!multiple)
                    {
                        break;
                    }
                }
                return vendorCodes;
            }
            else
            {
                _logger.LogError(
                    "User {AuthId} doesn't have permission to view details for {UserId}.",
                    authId,
                    userId);
                throw new GraException("Permission denied.");
            }
        }

        private async Task<VendorCodeInfo> GetVendorCodeInfoAsync(string code)
        {
            return await GetVendorCodeInfoAsync(await GetVendorCodeByCode(code));
        }

        private async Task<VendorCodeInfo> GetVendorCodeInfoAsync(VendorCode vendorCode)
        {
            var vendorCodeInfo = new VendorCodeInfo
            {
                VendorCode = vendorCode
            };

            if (vendorCode != null)
            {
                var userId = vendorCode.UserId ?? vendorCode.AssociatedUserId;

                var culture = userId.HasValue
                    ? await _userRepository.GetCultureAsync(userId.Value)
                    : _userContextProvider.GetCurrentCulture().Name;

                var languageId = await _languageService.GetLanguageIdAsync(culture);

                var vendorCodeType = await _vendorCodeTypeRepository
                    .GetByIdAsync(vendorCode.VendorCodeTypeId);

                vendorCodeInfo.IsDonated = vendorCode.IsDonated;
                if (vendorCodeInfo.IsDonated == true)
                {
                    vendorCodeInfo.IsDonationLocked = vendorCode.IsDonationLocked;
                }
                vendorCodeInfo.IsEmailAwarded = vendorCode.IsEmailAward;

                vendorCodeInfo.ReshipmentDetectedDate = vendorCode.ReshipmentDetectedDate;
                vendorCodeInfo.ReshipmentPriorPackingSlip = vendorCode.ReshipmentPriorPackingSlip;

                if ((vendorCode.CanBeDonated || vendorCode.CanBeEmailAward)
                    && vendorCode.IsDonated == null && vendorCode.IsEmailAward == null)
                {
                    if (!vendorCode.ExpirationDate.HasValue
                        || vendorCode.ExpirationDate.Value > _dateTimeProvider.Now)
                    {
                        vendorCodeInfo.CanDonate = vendorCode.CanBeDonated;
                        vendorCodeInfo.CanEmailAward = vendorCode.CanBeEmailAward;
                        vendorCodeInfo.NeedsToAnswerVendorCodeQuestion = true;

                        if (vendorCode.CanBeEmailAward)
                        {
                            var templateText = await _messageTemplateService.GetMessageTextAsync(
                                    vendorCodeType.EmailAwardMessageTemplateId.Value,
                                    languageId);

                            vendorCodeInfo.EmailAwardInstructions = templateText.Body;

                            if (string.IsNullOrWhiteSpace(vendorCodeInfo.EmailAwardInstructions))
                            {
                                _logger.LogError("Email award instructions are not set for code type {codeTypeId}",
                                    vendorCode.VendorCodeTypeId);
                            }
                        }
                    }
                }
                else if (vendorCode.CanBeDonated && vendorCode.IsDonated == true)
                {
                    if (vendorCodeType.DonationSegmentId.HasValue)
                    {
                        vendorCodeInfo.VendorCodeDisplay = await _segmentService
                            .GetDbTextAsync(vendorCodeType.DonationSegmentId.Value, languageId);
                    }
                    else
                    {
                        vendorCodeInfo.VendorCodeDisplay = string.Empty;
                    }
                }
                else if (vendorCode.CanBeEmailAward && vendorCode.IsEmailAward == true)
                {
                    if (vendorCodeType.EmailAwardSegmentId.HasValue)
                    {
                        vendorCodeInfo.VendorCodeDisplay = await _segmentService
                            .GetDbTextAsync(vendorCodeType.EmailAwardSegmentId.Value, languageId);
                    }
                    else
                    {
                        vendorCodeInfo.VendorCodeDisplay = string.Empty;
                    }
                }
                else
                {
                    vendorCodeInfo.VendorCodeDisplay = vendorCode.Code;
                    vendorCodeInfo.VendorCodeUrl = vendorCode.Url;

                    var vendorPrize = await _prizeWinnerService
                        .GetPrizeForVendorCodeAsync(vendorCode.Id);

                    var itemName = "Item";

                    if (!string.IsNullOrWhiteSpace(vendorCode.Details))
                    {
                        itemName = vendorCode.Details;
                    }

                    if (vendorPrize?.RedeemedAt.HasValue == true)
                    {
                        vendorCodeInfo.OrderStatus = VendorOrderStatus.Received;
                        vendorCodeInfo.PickupDate = vendorPrize.RedeemedAt.Value;
                        vendorCodeInfo.VendorCodeMessage
                            = _sharedLocalizer[Annotations.Info.VendorItemPickedup,
                                itemName,
                                vendorPrize.RedeemedAt.Value
                                    .ToString("d", CultureInfo.InvariantCulture)];
                    }
                    else
                    {
                        vendorCodeInfo.IsMissing = vendorCode.IsMissing == true;
                        vendorCodeInfo.IsDamaged = vendorCode.IsDamaged == true;

                        if (vendorCode.ArrivalDate.HasValue
                            && vendorCode.IsDamaged != true
                            && vendorCode.IsMissing != true)
                        {
                            vendorCodeInfo.VendorCodeMessage
                                = _sharedLocalizer[Annotations.Info.VendorItemArrived,
                                    itemName,
                                    vendorCode.ArrivalDate.Value
                                        .ToString("d", CultureInfo.InvariantCulture)];

                            vendorCodeInfo.OrderStatus = VendorOrderStatus.Arrived;
                        }
                        else if (vendorCode.ShipDate.HasValue)
                        {
                            vendorCodeInfo.VendorCodeMessage
                                = _sharedLocalizer[Annotations.Info.VendorItemShipped,
                                    itemName,
                                    vendorCode.ShipDate.Value
                                        .ToString("d", CultureInfo.InvariantCulture)];

                            vendorCodeInfo.OrderStatus = VendorOrderStatus.Shipped;
                        }
                        else if (vendorCode.OrderDate.HasValue)
                        {
                            vendorCodeInfo.VendorCodeMessage
                                = _sharedLocalizer[Annotations.Info.VendorItemOrdered,
                                    itemName,
                                    vendorCode.OrderDate.Value
                                        .ToString("d", CultureInfo.InvariantCulture)];

                            vendorCodeInfo.OrderStatus = VendorOrderStatus.Ordered;
                        }
                        else
                        {
                            vendorCodeInfo.OrderStatus = VendorOrderStatus.Pending;
                        }
                    }

                    vendorCodeInfo.VendorCodePackingSlip = vendorCode.PackingSlip;
                }
            }

            return vendorCodeInfo;
        }

        private async Task MarkCodeDonatedAsync(string coupon)
        {
            var vendorCode = await _vendorCodeRepository.GetByCode(coupon)
                ?? throw new GraException("Unable to find vendor code {coupon}");

            vendorCode.IsDonated = true;
            await _vendorCodeRepository.UpdateSaveAsync(GetActiveUserId(), vendorCode);
        }

        private async Task<bool?> SendPickupEmailAsync(int templateId,
            int userId,
            Model.Utility.DirectEmailDetails emailDetails,
            VendorCode code)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                emailDetails.ClearTags();
                emailDetails.DirectEmailTemplateId = templateId;
                emailDetails.LanguageId = string.IsNullOrEmpty(user.Culture)
                    ? await _languageService.GetDefaultLanguageIdAsync()
                    : await _languageService.GetLanguageIdAsync(user.Culture);
                emailDetails.ToAddress = user.Email;
                emailDetails.ToName = user.FullName;
                emailDetails.ToUserId = user.Id;
                emailDetails.Tags.Add("Details", code.Details);

                if (code.BranchId.HasValue)
                {
                    var branch = await _siteService.GetBranchByIdAsync(code.BranchId.Value);
                    if (branch != null)
                    {
                        emailDetails.Tags.Add("LocationAddress", branch.Address);
                        emailDetails.Tags.Add("LocationLink", branch.Url);
                        emailDetails.Tags.Add("LocationName", branch.Name);
                        emailDetails.Tags.Add("LocationTelephone", branch.Telephone);
                    }
                }

                var result = await _emailService.SendDirectAsync(emailDetails);
                return result.Successful;
            }

            return null;
        }

        private async Task SendVendorCodeMailAsync(int userId,
            int languageId,
            int? siteId,
            VendorCodeType codeType,
            string assignedCode)
        {
            var message = await _messageTemplateService
                .GetMessageTextAsync(codeType.MessageTemplateId, languageId);

            var user = await _userRepository.GetByIdAsync(userId);

            var markedUpUrl = await new StubbleBuilder().Build()
                .RenderAsync(codeType.Url, new Dictionary<string, string>
                {
                    {TemplateToken.VendorBranchToken, user.BranchId
                        .ToString(CultureInfo.InvariantCulture)},
                    {TemplateToken.VendorCodeToken, assignedCode }
                });

            await _mailService.SendSystemMailAsync(new Mail
            {
                Body = message.Body,
                CanParticipantDelete = false,
                Subject = message.Subject,
                TemplateDictionary = new Dictionary<string, string>
                {
                    { TemplateToken.VendorCodeToken, assignedCode },
                    { TemplateToken.VendorLinkToken, markedUpUrl }
                },
                ToUserId = userId
            }, siteId);
        }

        private async Task SendVendorDonationMailAsync(int userId,
            int languageId,
            int? siteId,
            VendorCodeType codeType)
        {
            if (!codeType.DonationMessageTemplateId.HasValue)
            {
                throw new GraException("Cannot send donation mail - it is not configured.");
            }

            var message = await _messageTemplateService
                .GetMessageTextAsync(codeType.DonationMessageTemplateId.Value, languageId);

            await _mailService.SendSystemMailAsync(new Mail
            {
                Body = message.Body,
                CanParticipantDelete = false,
                Subject = message.Subject,
                ToUserId = userId
            }, siteId);
        }

        private async Task SendVendorEmailAwardMailAsync(int userId,
            int langaugeId,
            int? siteId,
            VendorCodeType codeType)
        {
            if (!codeType.EmailAwardMessageTemplateId.HasValue)
            {
                throw new GraException("Cannot send email award mail - it is not configured");
            }

            var message = await _messageTemplateService
                .GetMessageTextAsync(codeType.EmailAwardMessageTemplateId.Value, langaugeId);

            await _mailService.SendSystemMailAsync(new Mail
            {
                Body = message.Body,
                CanParticipantDelete = false,
                Subject = message.Subject,
                ToUserId = userId
            }, siteId);
        }

        private async Task<VendorCodeType> UpdateTypeInternalAsync(VendorCodeType vendorCodeType)
        {
            VerifyManagementPermission();
            vendorCodeType.SiteId = GetCurrentSiteId();

            var fieldErrors = ValidateVendorCodeType(vendorCodeType);

            if (fieldErrors.Count > 0)
            {
                throw new GraFieldValidationException(fieldErrors);
            }

            return await _vendorCodeTypeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                vendorCodeType);
        }

        private async Task<VendorCode> UpdateVendorCodeAsync(VendorCode code,
            DateTime? orderDate,
            DateTime? shipDate,
            string details,
            int? branchId,
            string packingSlip,
            string trackingNumber,
            DateTime? arrivalDate)
        {
            code.IsUsed = true;

            if (orderDate != null)
            {
                code.OrderDate = orderDate;
            }

            if (orderDate.HasValue
                && code.ShipDate.HasValue
                && code.OrderDate > code.ShipDate)
            {
                // we are considering this item to be a re-order under the same vendor code
                _logger.LogInformation("Vendor update {VendorCode} order {OrderDate:d} is after ship {ShipDate:d} - clearing: {Damaged} damaged and {Missing} missing flags, ship date, marking reshipped.",
                    code.Code,
                    orderDate,
                    code.ShipDate,
                    code.IsDamaged,
                    code.IsMissing);
                var priorPackingSlip = code.PackingSlip?.Trim();
                code.ArrivalDate = null;
                code.IsDamaged = false;
                code.IsMissing = false;
                code.PackingSlip = packingSlip?.Trim();
                code.ReshipmentDetectedDate = _dateTimeProvider.Now;
                code.ReshipmentPriorPackingSlip = priorPackingSlip;
                if (!string.IsNullOrWhiteSpace(trackingNumber))
                {
                    code.TrackingNumber = trackingNumber;
                }
            }

            if (shipDate != null)
            {
                code.ShipDate = shipDate;
            }

            if (!string.IsNullOrWhiteSpace(details))
            {
                code.Details = details.Length > 255
                    ? details[..255]
                    : details;
            }

            if (branchId != null)
            {
                code.BranchId = branchId;
                code.IsDonated = false;
            }

            if (code.IsDamaged != true && code.IsMissing != true)
            {
                if (arrivalDate != null && code.ArrivalDate == null)
                {
                    code.ArrivalDate = arrivalDate;
                }

                if (!string.IsNullOrWhiteSpace(packingSlip))
                {
                    code.PackingSlip = packingSlip?.Trim();
                }

                if (!string.IsNullOrWhiteSpace(trackingNumber))
                {
                    code.TrackingNumber = trackingNumber;
                }
            }

            return await _vendorCodeRepository.UpdateSaveNoAuditAsync(code);
        }

        #region Excel helper methods

        private DateTime? GetExcelDateTime(IExcelDataReader excelReader,
            int row,
            int columnId,
            string columnName)
        {
            ArgumentNullException.ThrowIfNull(excelReader);
            try
            {
                try
                {
                    return excelReader.GetDateTime(columnId);
                }
                catch (NullReferenceException)
                { }
                catch (InvalidCastException)
                {
                    string dateString = excelReader.GetString(columnId);
                    if (string.IsNullOrEmpty(dateString))
                    {
                        return null;
                    }

                    if (DateTime.TryParse(dateString, out var dateConversion))
                    {
                        return dateConversion;
                    }
                    else
                    {
                        _logger.LogError(ErrorUnableToParse, columnName, row, dateString);
                        throw new GraException($"Issue reading {columnName} on row {row}: {dateString}");
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogError(ErrorParseError, columnName, row, ex.Message);
                throw new GraException($"Issue reading {columnName} on row {row}: {ex.Message}");
            }
            return null;
        }

        private int GetExcelInt(IExcelDataReader excelReader,
            int row,
            int columnId,
            string columnName)
        {
            ArgumentNullException.ThrowIfNull(excelReader);
            var value = excelReader.GetValue(columnId);
            if (value != null)
            {
                try
                {
                    string stringValue = excelReader.GetString(columnId);
                    if (int.TryParse(stringValue, out int intValue))
                    {
                        return intValue;
                    }
                    else
                    {
                        _logger.LogWarning(ErrorParseError,
                            columnName,
                            row,
                            "Couldn't convert to a number");
                        throw new GraException($"Issue reading {columnName} on row {row}: Couldn't convert to a number");
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _logger.LogWarning(ErrorParseError, columnName, row, ex.Message);
                    throw new GraException($"Issue reading {columnName} on row {row}: {ex.Message}");
                }
                catch (InvalidCastException ex)
                {
                    _logger.LogWarning(ErrorParseError, columnName, row, ex.Message);
                    throw new GraException($"Issue reading {columnName} on row {row}: {ex.Message}");
                }
            }
            return default;
        }

        private string GetExcelString(IExcelDataReader excelReader,
            int row,
            int columnId,
            string columnName)
        {
            ArgumentNullException.ThrowIfNull(excelReader);
            try
            {
                return excelReader.GetString(columnId);
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogError(ErrorParseError, columnName, row, ex.Message);
                throw new GraException($"Issue reading {columnName} on line {row}: {ex.Message}");
            }
        }

        #endregion Excel helper methods
    }
}
