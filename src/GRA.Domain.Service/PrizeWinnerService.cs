using GRA.Domain.Model;
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
            IUserContextProvider userContextProvider,
            IDrawingRepository drawingRepository,
            IPrizeWinnerRepository prizeWinnerRepository,
            ITriggerRepository triggerRepository) : base(logger, userContextProvider)
        {
            _drawingRepository = Require.IsNotNull(drawingRepository, nameof(drawingRepository));
            _prizeWinnerRepository = Require.IsNotNull(prizeWinnerRepository,
                nameof(prizeWinnerRepository));
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
            SetManagementPermission(Permission.ViewUserPrizes);
        }

        public async Task<PrizeWinner> AddPrizeWinnerAsync(PrizeWinner prizeWinner)
        {
            if (prizeWinner.DrawingId == null && prizeWinner.TriggerId == null)
            {
                throw new Exception("Prizes must be awarded through a drawing or a trigger.");
            }
            prizeWinner.SiteId = GetCurrentSiteId();
            prizeWinner.CreatedAt = DateTime.Now;
            prizeWinner.CreatedBy = prizeWinner.UserId;
            int authUserId = GetClaimId(ClaimType.UserId);
            return await _prizeWinnerRepository.AddSaveAsync(authUserId, prizeWinner);
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
                    prize.RedeemedAt = DateTime.Now;
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

        public async Task RemovePrize(int prizeWinnerId)
        {
            VerifyManagementPermission();

            int authUserId = GetClaimId(ClaimType.UserId);
            var prize = await _prizeWinnerRepository.GetByIdAsync(prizeWinnerId);

            await _prizeWinnerRepository.RemoveSaveAsync(authUserId, prizeWinnerId);
        }

        public async Task<DataWithCount<ICollection<PrizeWinner>>>
            PageUserPrizes(int userId, Filter filter = default(Filter))
        {
            VerifyManagementPermission();

            int siteId = GetCurrentSiteId();
            if (filter == null)
            {
                filter = new Filter();
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
    }
}
