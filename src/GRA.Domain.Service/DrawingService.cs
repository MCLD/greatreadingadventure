using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class DrawingService : Abstract.BaseUserService<DrawingService>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IDrawingCriterionRepository _drawingCriterionRepository;
        private readonly IMailRepository _mailRepository;
        public DrawingService(ILogger<DrawingService> logger,
            IUserContextProvider userContextProvider,
            IDrawingRepository drawingRepository,
            IDrawingCriterionRepository drawingCriterionRepository,
            IMailRepository mailRepository) : base(logger, userContextProvider)
        {
            _drawingRepository = Require.IsNotNull(drawingRepository, nameof(drawingRepository));
            _drawingCriterionRepository = Require.IsNotNull(drawingCriterionRepository,
                nameof(drawingCriterionRepository));
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
        }

        public async Task<DataWithCount<IEnumerable<Drawing>>> 
            GetPaginatedDrawingListAsync(int skip, int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                int siteId = GetCurrentSiteId();
                return new DataWithCount<IEnumerable<Drawing>>
                {
                    Data = await _drawingRepository.PageAllAsync(siteId, skip, take),
                    Count = await _drawingRepository.GetCountAsync(siteId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view all drawings.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<Drawing>> GetDetailsAsync(int id, int skip, int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                return new DataWithCount<Drawing>
                {
                    Data = await _drawingRepository.GetByIdAsync(id, skip, take),
                    Count = await _drawingRepository.GetWinnerCountAsync(id)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view drawing {id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<DrawingCriterion>>>
            GetPaginatedCriterionListAsync(int skip, int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                int siteId = GetCurrentSiteId();
                return new DataWithCount<IEnumerable<DrawingCriterion>>
                {
                    Data = await _drawingCriterionRepository.PageAllAsync(siteId, skip, take),
                    Count = await _drawingCriterionRepository.GetCountAsync(siteId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view all criteria.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> GetCriterionDetailsAsync(int id)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                return await _drawingCriterionRepository.GetByIdAsync(id);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view criterion {id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> AddCriterionAsync(DrawingCriterion criterion)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                criterion.SiteId = GetCurrentSiteId();
                return await _drawingCriterionRepository.AddSaveAsync(authUserId, criterion);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to add a criterion.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> EditCriterionAsync(DrawingCriterion criterion)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                var currentCriterion = await _drawingCriterionRepository.GetByIdAsync(criterion.Id);
                criterion.SiteId = currentCriterion.SiteId;
                return await _drawingCriterionRepository.UpdateSaveAsync(authUserId, criterion);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to edit criterion {criterion.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<int> GetEligibleCountAsync(int id)
        {
            // todo validate site
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                return await _drawingCriterionRepository.GetEligibleUserCountAsync(id);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to get eligible count.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<Drawing> PerformDrawingAsync(Drawing drawing)
        {
            // todo validate site
            int siteId = GetCurrentSiteId();
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                // insert drawing
                drawing.DrawingCriterion = default(DrawingCriterion);
                drawing.Id = default(int);
                drawing = await _drawingRepository.AddSaveAsync(authUserId, drawing);

                // pull list of eligible users
                var eligibleUserIds
                    = await _drawingCriterionRepository.GetEligibleUserIdsAsync(drawing.DrawingCriterionId);

                // ensure there are enough eligible users to do the drawing
                if (drawing.WinnerCount > eligibleUserIds.Count())
                {
                    throw new GraException($"Cannot draw {drawing.WinnerCount} from an eligible pool of {eligibleUserIds.Count()} participants.");
                }

                // prepare and perform the drawing
                var remainingUsers = new List<int>(eligibleUserIds);
                var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                var randomBytes = new byte[sizeof(int)];

                for (int count = 1; count <= drawing.WinnerCount; count++)
                {
                    rng.GetBytes(randomBytes);
                    int random = System.Math.Abs(System.BitConverter.ToInt32(randomBytes, 0));
                    int randomUserId = remainingUsers.ElementAt(random % remainingUsers.Count());

                    var winner = new DrawingWinner
                    {
                        DrawingId = drawing.Id,
                        UserId = randomUserId
                    };

                    // if there's an associated notification, send it now
                    if (!string.IsNullOrEmpty(drawing.NotificationSubject)
                        && !string.IsNullOrEmpty(drawing.NotificationMessage))
                    {
                        var mail = new Mail
                        {
                            SiteId = siteId,
                            Subject = drawing.NotificationSubject,
                            Body = drawing.NotificationMessage,
                            ToUserId = winner.UserId,
                            DrawingId = drawing.Id
                        };
                        mail = await _mailRepository.AddSaveAsync(authUserId, mail);
                        winner.MailId = mail.Id;
                    }

                    // add the winner - does not perform a save
                    await _drawingRepository.AddWinnerAsync(winner);

                    // remove them so they aren't drawn twice
                    remainingUsers.Remove(randomUserId);
                }
                await _drawingRepository.SaveAsync();

                // return the fully-populated drawing
                return await _drawingRepository.GetByIdAsync(drawing.Id);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to perform drawings.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RedeemWinnerAsync(int drawingId, int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing)
                || HasPermission(Permission.ViewUserDrawings))
            {
                await _drawingRepository.RedeemWinnerAsync(drawingId, userId);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to redeem user {userId} from drawing {drawingId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task UndoRedemptionAsnyc(int drawingId, int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing)
                || HasPermission(Permission.ViewUserDrawings))
            {
                await _drawingRepository.UndoRedemptionAsync(drawingId, userId);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to undo redemption user {userId} from drawing {drawingId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RemoveWinnerAsync(int drawingId, int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                await _drawingRepository.RemoveWinnerAsync(drawingId, userId);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to remove user {userId} from drawing {drawingId}.");
                throw new GraException("Permission denied.");
            }
        }
    }
}
