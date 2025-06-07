using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class DrawingService : Abstract.BaseUserService<DrawingService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IDrawingCriterionRepository _drawingCriterionRepository;
        private readonly IDrawingRepository _drawingRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IPrizeWinnerRepository _prizeWinnerRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISystemRepository _systemRepository;

        public DrawingService(ILogger<DrawingService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IDrawingRepository drawingRepository,
            IDrawingCriterionRepository drawingCriterionRepository,
            IMailRepository mailRepository,
            IPrizeWinnerRepository prizeWinnerRepository,
            IProgramRepository programRepository,
            ISystemRepository systemRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(drawingCriterionRepository);
            ArgumentNullException.ThrowIfNull(drawingRepository);
            ArgumentNullException.ThrowIfNull(mailRepository);
            ArgumentNullException.ThrowIfNull(prizeWinnerRepository);
            ArgumentNullException.ThrowIfNull(programRepository);
            ArgumentNullException.ThrowIfNull(systemRepository);

            _branchRepository = branchRepository;
            _drawingCriterionRepository = drawingCriterionRepository;
            _drawingRepository = drawingRepository;
            _mailRepository = mailRepository;
            _prizeWinnerRepository = prizeWinnerRepository;
            _programRepository = programRepository;
            _systemRepository = systemRepository;
        }

        public async Task<DrawingCriterion> AddCriterionAsync(DrawingCriterion criterion)
        {
            ArgumentNullException.ThrowIfNull(criterion);
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                criterion.SiteId = GetCurrentSiteId();
                criterion.RelatedBranchId = GetClaimId(ClaimType.BranchId);
                criterion.RelatedSystemId = GetClaimId(ClaimType.SystemId);

                if (criterion.ProgramIds != null)
                {
                    var programs = await _programRepository.GetAllAsync(GetCurrentSiteId());
                    if (!programs.Select(_ => _.Id).Except(criterion.ProgramIds).Any())
                    {
                        criterion.ProgramIds = null;
                    }
                }

                return await _drawingCriterionRepository.AddSaveAsync(authUserId, criterion);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to add a criterion.",
                    authUserId);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> EditCriterionAsync(DrawingCriterion criterion)
        {
            ArgumentNullException.ThrowIfNull(criterion);

            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                var currentCriterion = await _drawingCriterionRepository.GetByIdAsync(criterion.Id);
                criterion.SiteId = currentCriterion.SiteId;
                criterion.RelatedBranchId = currentCriterion.RelatedBranchId;
                criterion.RelatedSystemId = currentCriterion.RelatedSystemId;

                if (criterion.ProgramIds != null)
                {
                    var programs = await _programRepository.GetAllAsync(GetCurrentSiteId());
                    if (!programs.Select(_ => _.Id).Except(criterion.ProgramIds).Any())
                    {
                        criterion.ProgramIds = new List<int>();
                    }
                }
                else
                {
                    criterion.ProgramIds = new List<int>();
                }

                return await _drawingCriterionRepository.UpdateSaveAsync(authUserId, criterion);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to edit criterion {CriterionId}.",
                    authUserId,
                    criterion.Id);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> GetCriterionDetailsAsync(int id)
        {
            if (HasPermission(Permission.PerformDrawing))
            {
                try
                {
                    return await _drawingCriterionRepository.GetByIdAsync(id);
                }
                catch (Exception)
                {
                    throw new GraException("The requested criteria could not be accessed or does not exist.");
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view criterion {CriterionId}.",
                    GetClaimId(ClaimType.UserId),
                    id);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Drawing> GetDetailsAsync(int id)
        {
            if (HasPermission(Permission.PerformDrawing))
            {
                try
                {
                    return await _drawingRepository.GetDetailsWinners(id);
                }
                catch (Exception)
                {
                    throw new GraException("The requested drawing could not be accessed or does not exist.");
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view drawing {Id}.",
                    GetClaimId(ClaimType.UserId),
                    id);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<Drawing>> GetDetailsAsync(int id, int skip, int take)
        {
            if (HasPermission(Permission.PerformDrawing))
            {
                try
                {
                    return new DataWithCount<Drawing>
                    {
                        Data = await _drawingRepository.GetDetailsWinners(id, skip, take),
                        Count = await _drawingRepository.GetWinnerCountAsync(id)
                    };
                }
                catch (Exception)
                {
                    throw new GraException("The requested drawing could not be accessed or does not exist.");
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view drawing {Id}.",
                    GetClaimId(ClaimType.UserId),
                    id);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<string> GetDrawingNameAsync(int id)
        {
            VerifyPermission(Permission.ViewUserPrizes);

            var drawing = await _drawingRepository.GetByIdAsync(id);

            return drawing?.Name;
        }

        public async Task<int> GetEligibleCountAsync(int id)
        {
            // todo validate site
            if (HasPermission(Permission.PerformDrawing))
            {
                return await _drawingCriterionRepository.GetEligibleUserCountAsync(id);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to get eligible count.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<DrawingCriterion>>>
            GetPaginatedCriterionListAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            if (HasPermission(Permission.PerformDrawing))
            {
                filter.SiteId = GetCurrentSiteId();
                return new DataWithCount<IEnumerable<DrawingCriterion>>
                {
                    Data = await _drawingCriterionRepository.PageAllAsync(filter),
                    Count = await _drawingCriterionRepository.GetCountAsync(filter)
                };
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view all criteria.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Drawing>>>
            GetPaginatedDrawingListAsync(DrawingFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            if (HasPermission(Permission.PerformDrawing))
            {
                filter.SiteId = GetCurrentSiteId();
                return new DataWithCount<IEnumerable<Drawing>>
                {
                    Data = await _drawingRepository.PageAllAsync(filter),
                    Count = await _drawingRepository.GetCountAsync(filter)
                };
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view all drawings.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Drawing> PerformDrawingAsync(Drawing drawing)
        {
            ArgumentNullException.ThrowIfNull(drawing);

            // todo validate site
            int siteId = GetCurrentSiteId();
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                // insert drawing
                drawing.DrawingCriterion = default;
                drawing.Id = default;
                drawing.RelatedBranchId = GetClaimId(ClaimType.BranchId);
                drawing.RelatedSystemId = GetClaimId(ClaimType.SystemId);
                drawing = await _drawingRepository.AddSaveAsync(authUserId, drawing);

                // pull list of eligible users
                var eligibleUserIds
                    = await _drawingCriterionRepository.GetEligibleUserIdsAsync(drawing.DrawingCriterionId);

                // ensure there are enough eligible users to do the drawing
                if (drawing.WinnerCount > eligibleUserIds.Count)
                {
                    throw new GraException($"Cannot draw {drawing.WinnerCount} from an eligible pool of {eligibleUserIds.Count} participants.");
                }

                // prepare and perform the drawing
                var remainingUsers = new List<int>(eligibleUserIds);
                var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                var randomBytes = new byte[sizeof(int)];

                for (int count = 1; count <= drawing.WinnerCount; count++)
                {
                    rng.GetBytes(randomBytes);
                    int random = System.Math.Abs(System.BitConverter.ToInt32(randomBytes, 0));
                    int randomUserId = remainingUsers[random % remainingUsers.Count];

                    var winner = new PrizeWinner
                    {
                        SiteId = siteId,
                        DrawingId = drawing.Id,
                        UserId = randomUserId
                    };

                    // if there's an associated notification, send it now
                    if (HasPermission(Permission.MailParticipants)
                        && !string.IsNullOrEmpty(drawing.NotificationSubject)
                        && !string.IsNullOrEmpty(drawing.NotificationMessage))
                    {
                        var mail = new Mail
                        {
                            SiteId = siteId,
                            Subject = drawing.NotificationSubject,
                            Body = drawing.NotificationMessage,
                            ToUserId = winner.UserId,
                            DrawingId = drawing.Id,
                            IsNew = true
                        };
                        mail = await _mailRepository.AddSaveAsync(authUserId, mail);
                        winner.MailId = mail.Id;
                    }

                    // add the winner - does not perform a save
                    await _prizeWinnerRepository.AddAsync(authUserId, winner);

                    // remove them so they aren't drawn twice
                    remainingUsers.Remove(randomUserId);
                }
                await _drawingRepository.SaveAsync();

                // return the fully-populated drawing
                return await _drawingRepository.GetByIdAsync(drawing.Id);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to perform drawings.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
        }

        private async Task ValidateCriterionAsync(DrawingCriterion criterion)
        {
            if (criterion.SystemId.HasValue)
            {
                if (!(await _systemRepository.ValidateAsync(
                    criterion.SystemId.Value, criterion.SiteId)))
                {
                    throw new GraException("Invalid System selection.");
                }
                if (criterion.BranchId.HasValue && !(await _branchRepository.ValidateAsync(
                        criterion.BranchId.Value, criterion.SystemId.Value)))
                {
                    throw new GraException("Invalid Branch selection.");
                }
            }
            else if (criterion.BranchId.HasValue && !(await _branchRepository.ValidateBySiteAsync(
                    criterion.BranchId.Value, criterion.SiteId)))
            {
                throw new GraException("Invalid Branch selection.");
            }

            if (criterion.ProgramId.HasValue && !(await _programRepository.ValidateAsync(
                    criterion.ProgramId.Value, criterion.SiteId)))
            {
                throw new GraException("Invalid Program selection.");
            }
        }
    }
}
