using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PrizeWinnerService : Abstract.BaseUserService<PrizeWinnerService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IDrawingCriterionRepository _drawingCriterionRepository;
        private readonly IDrawingRepository _drawingRepository;
        private readonly IPrizeWinnerRepository _prizeWinnerRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly ITriggerRepository _triggerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly IVendorCodeTypeRepository _vendorCodeTypeRepository;

        public PrizeWinnerService(ILogger<PrizeWinnerService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IDrawingCriterionRepository drawingCriterionRepository,
            IDrawingRepository drawingRepository,
            IPrizeWinnerRepository prizeWinnerRepository,
            ISystemRepository systemRepository,
            ITriggerRepository triggerRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository,
            IVendorCodeTypeRepository vendorCodeTypeRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _drawingCriterionRepository = drawingCriterionRepository
                ?? throw new ArgumentNullException(nameof(drawingCriterionRepository));
            _drawingRepository = drawingRepository
                ?? throw new ArgumentNullException(nameof(drawingRepository));
            _prizeWinnerRepository = prizeWinnerRepository
                ?? throw new ArgumentNullException(nameof(prizeWinnerRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _triggerRepository = triggerRepository
                ?? throw new ArgumentNullException(nameof(triggerRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _vendorCodeRepository = vendorCodeRepository
                ?? throw new ArgumentNullException(nameof(vendorCodeRepository));
            _vendorCodeTypeRepository = vendorCodeTypeRepository
                ?? throw new ArgumentNullException(nameof(vendorCodeTypeRepository));
            SetManagementPermission(Permission.ViewUserPrizes);
        }

        public async Task<PrizeWinner> AddPrizeWinnerAsync(PrizeWinner prizeWinner,
            bool userIdIsCurrentUser = false)
        {
            if (prizeWinner.DrawingId == null
                && prizeWinner.TriggerId == null
                && prizeWinner.VendorCodeId == null)
            {
                throw new GraException("Prizes must be awarded through a drawing, a trigger, or a vendor code.");
            }
            prizeWinner.SiteId = GetCurrentSiteId();
            prizeWinner.CreatedAt = _dateTimeProvider.Now;
            int currentUserId = userIdIsCurrentUser ? prizeWinner.UserId : GetClaimId(ClaimType.UserId);
            return await _prizeWinnerRepository.AddSaveAsync(currentUserId, prizeWinner);
        }

        public async Task<List<PrizeCount>> GetHouseholdUnredeemedPrizesAsync(int headId)
        {
            VerifyManagementPermission();

            return await _prizeWinnerRepository.GetHouseholdUnredeemedPrizesAsync(headId);
        }

        public async Task<PrizeWinner> GetPrizeForVendorCodeAsync(int vendorCodeId)
        {
            return await _prizeWinnerRepository.GetPrizeForVendorCodeAsync(vendorCodeId);
        }

        public async Task<PrizeWinner> GetUserDrawingPrizeAsync(int userId, int drawingId)
        {
            return await _prizeWinnerRepository.GetUserDrawingPrizeAsync(userId, drawingId);
        }

        public async Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId)
        {
            return await _prizeWinnerRepository.GetUserTriggerPrizeAsync(userId, triggerId);
        }

        public async Task<int> GetUserWinCount(int userId, bool? redeemed = null)
        {
            VerifyManagementPermission();
            return await _prizeWinnerRepository.CountByWinningUserId(GetCurrentSiteId(), userId,
                redeemed);
        }

        public async Task<ICollection<PrizeWinner>> GetVendorCodePrizes(int userId)
        {
            return await _prizeWinnerRepository.GetVendorCodePrizesAsync(userId);
        }

        public async Task<DataWithCount<ICollection<PrizeWinner>>>
            PageUserPrizes(ICollection<int> userIds, BaseFilter filter = default(BaseFilter))
        {
            VerifyManagementPermission();

            int siteId = GetCurrentSiteId();
            if (filter == null)
            {
                filter = new BaseFilter();
            }

            var prizes = await _prizeWinnerRepository
                    .PageByWinnerAsync(siteId, userIds, (int)filter.Skip, (int)filter.Take);

            foreach (var prize in prizes)
            {
                if (prize.DrawingId != null)
                {
                    var drawing = await _drawingRepository.GetByIdAsync((int)prize.DrawingId);
                    prize.PrizeName = drawing.Name;
                    prize.PrizeRedemptionInstructions = drawing.RedemptionInstructions;

                    var drawingCritera = await _drawingCriterionRepository
                        .GetByIdAsync(drawing.DrawingCriterionId);
                    prize.AvailableAtBranch = drawingCritera.BranchName;
                    prize.AvailableAtSystem = drawingCritera.SystemName;
                }
                else if (prize.TriggerId != null)
                {
                    var trigger = await _triggerRepository.GetByIdAsync((int)prize.TriggerId);
                    prize.AvailableAtBranch = trigger.LimitToBranchName;
                    prize.AvailableAtSystem = trigger.LimitToSystemName;
                    prize.PrizeName = trigger.AwardPrizeName;
                    prize.PrizeRedemptionInstructions = trigger.AwardPrizeRedemptionInstructions;
                }
                else if (prize.VendorCodeId != null)
                {
                    var vendorCode = await _vendorCodeRepository
                        .GetByIdAsync((int)prize.VendorCodeId);
                    var vendorCodeType = await _vendorCodeTypeRepository
                        .GetByIdAsync(vendorCode.VendorCodeTypeId);
                    var branch = await _branchRepository.GetByIdAsync((int)vendorCode.BranchId);
                    var system = await _systemRepository.GetByIdAsync(branch.SystemId);

                    prize.PrizeName = $"{vendorCodeType.Description}: {vendorCode.Details}";
                    prize.PrizeRedemptionInstructions = $"Give customer {vendorCode.Details} and click the green redemption button.";
                    prize.AvailableAtBranch = branch.Name;
                    prize.AvailableAtSystem = system.Name;
                }
            }

            return new DataWithCount<ICollection<PrizeWinner>>
            {
                Data = prizes,
                Count = await _prizeWinnerRepository.CountByWinningUserId(siteId, userIds)
            };
        }

        public async Task<DataWithCount<ICollection<PrizeWinner>>> PageUserPrizes(int userId, BaseFilter filter = default)
        {
            return await PageUserPrizes(new[] { userId }, filter);
        }

        public async Task RedeemPrizeAsync(int prizeWinnerId, string staffNotes)
        {
            int authUserId = GetClaimId(ClaimType.UserId);

            var prize = await _prizeWinnerRepository.GetByIdAsync(prizeWinnerId);

            if (HasPermission(Permission.ViewUserPrizes)
                || prize.UserId == authUserId
                || prize.UserId == GetActiveUserId())
            {
                if (prize.RedeemedAt.HasValue)
                {
                    _logger.LogError($"Double redeem attempt for prize {prizeWinnerId} by user {authUserId}");
                    throw new GraException($"This prize was already redeemed on {prize.RedeemedAt}");
                }
                else
                {
                    var authUser = await _userRepository.GetByIdAsync(authUserId);
                    prize.RedeemedAt = _dateTimeProvider.Now;
                    prize.RedeemedBy = authUserId;
                    prize.RedeemedByBranch = authUser.BranchId;
                    prize.RedeemedBySystem = authUser.SystemId;
                    prize.StaffNotes = staffNotes;

                    await _prizeWinnerRepository.UpdateSaveAsync(authUserId, prize);
                }
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to redeem prize {prizeWinnerId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RemovePrizeAsync(int prizeWinnerId)
        {
            VerifyManagementPermission();

            int authUserId = GetClaimId(ClaimType.UserId);
            var prize = await _prizeWinnerRepository.GetByIdAsync(prizeWinnerId);
            if (!prize.RedeemedAt.HasValue)
            {
                await _prizeWinnerRepository.RemoveSaveAsync(authUserId, prizeWinnerId);

                if (prize.DrawingId.HasValue)
                {
                    var winnerCount = await _drawingRepository.GetWinnerCountAsync(
                        prize.DrawingId.Value);
                    if (winnerCount == 0)
                    {
                        await _drawingRepository.SetArchivedAsync(authUserId, prize.DrawingId.Value,
                            true);
                    }
                }
            }
            else
            {
                _logger.LogError($"User {authUserId} cannot remove claimed prize {prize.Id}.");
                throw new GraException("Prizes that have been claimed cannot be removed.");
            }
        }

        public async Task UndoRedemptionAsync(int prizeWinnerId)
        {
            VerifyManagementPermission();

            int authUserId = GetClaimId(ClaimType.UserId);
            var prize = await _prizeWinnerRepository.GetByIdAsync(prizeWinnerId);

            if (!prize.RedeemedAt.HasValue)
            {
                _logger.LogError($"Prize not redeemed - undo attempt for {prizeWinnerId} by user {authUserId}");
                throw new GraException("This prize has not been redeemed!");
            }
            else
            {
                prize.RedeemedAt = null;
                prize.RedeemedBy = null;
                await _prizeWinnerRepository.UpdateSaveAsync(authUserId, prize);
                _logger.LogInformation($"User {authUserId} just undid redemption of prize id {prizeWinnerId} awarded to user {prize.UserId}");
            }
        }
    }
}
