using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class PrizeWinnerService : Abstract.BaseUserService<PrizeWinnerService>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IPrizeWinnerRepository _prizeWinnerRepository;
        private readonly ITriggerRepository _triggerRepository;
        public PrizeWinnerService(ILogger<PrizeWinnerService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDrawingRepository drawingRepository,
            IPrizeWinnerRepository prizeWinnerRepository,
            ITriggerRepository triggerRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _drawingRepository = Require.IsNotNull(drawingRepository, nameof(drawingRepository));
            _prizeWinnerRepository = Require.IsNotNull(prizeWinnerRepository,
                nameof(prizeWinnerRepository));
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
            SetManagementPermission(Permission.ViewUserPrizes);
        }

        public async Task<PrizeWinner> AddPrizeWinnerAsync(PrizeWinner prizeWinner,
            bool userIdIsCurrentUser = false)
        {
            if (prizeWinner.DrawingId == null && prizeWinner.TriggerId == null)
            {
                throw new Exception("Prizes must be awarded through a drawing or a trigger.");
            }
            prizeWinner.SiteId = GetCurrentSiteId();
            prizeWinner.CreatedAt = _dateTimeProvider.Now;
            prizeWinner.CreatedBy = prizeWinner.UserId;
            int currentUserId = userIdIsCurrentUser ? prizeWinner.UserId : GetClaimId(ClaimType.UserId);
            return await _prizeWinnerRepository.AddSaveAsync(currentUserId, prizeWinner);
        }

        public async Task RedeemPrizeAsync(int prizeWinnerId)
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
                    prize.RedeemedAt = _dateTimeProvider.Now;
                    prize.RedeemedBy = authUserId;
                    await _prizeWinnerRepository.UpdateSaveAsync(authUserId, prize);
                }
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to redeem prize {prizeWinnerId}.");
                throw new GraException("Permission denied.");
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

        public async Task<DataWithCount<ICollection<PrizeWinner>>>
            PageUserPrizes(int userId, BaseFilter filter = default(BaseFilter))
        {
            VerifyManagementPermission();

            int siteId = GetCurrentSiteId();
            if (filter == null)
            {
                filter = new BaseFilter();
            }

            var prizes = await _prizeWinnerRepository
                    .PageByWinnerAsync(siteId, userId, (int)filter.Skip, (int)filter.Take);

            foreach (var prize in prizes)
            {
                if (prize.DrawingId != null)
                {
                    var drawing = await _drawingRepository.GetByIdAsync((int)prize.DrawingId);
                    prize.PrizeName = drawing.Name;
                    prize.PrizeRedemptionInstructions = drawing.RedemptionInstructions;
                }
                else if (prize.TriggerId != null)
                {
                    var trigger = await _triggerRepository.GetByIdAsync((int)prize.TriggerId);
                    prize.PrizeName = trigger.AwardPrizeName;
                    prize.PrizeRedemptionInstructions = trigger.AwardPrizeRedemptionInstructions;
                }
            }

            return new DataWithCount<ICollection<PrizeWinner>>
            {
                Data = prizes,
                Count = await _prizeWinnerRepository.CountByWinningUserId(siteId, userId)
            };
        }

        public async Task<int> GetUserWinCount(int userId, bool? redeemed = null)
        {
            VerifyManagementPermission();
            return await _prizeWinnerRepository.CountByWinningUserId(GetCurrentSiteId(), userId,
                redeemed);
        }

        public async Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId)
        {
            return await _prizeWinnerRepository.GetUserTriggerPrizeAsync(userId, triggerId);
        }
    }
}
