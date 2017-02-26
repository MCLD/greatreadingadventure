using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class DynamicAvatarService : BaseUserService<DynamicAvatarService>
    {
        private readonly IDynamicAvatarElementRepository _dynamicAvatarElementRepository;
        private readonly IDynamicAvatarLayerRepository _dynamicAvatarLayerRepository;
        public DynamicAvatarService(ILogger<DynamicAvatarService> logger,
            IUserContextProvider userContextProvider,
            IDynamicAvatarElementRepository dynamicAvatarElementRepository,
            IDynamicAvatarLayerRepository dynamicAvatarLayerRepository)
            : base(logger, userContextProvider)
        {
            _dynamicAvatarElementRepository = Require.IsNotNull(dynamicAvatarElementRepository,
                nameof(dynamicAvatarElementRepository));
            _dynamicAvatarLayerRepository = Require.IsNotNull(dynamicAvatarLayerRepository,
                nameof(dynamicAvatarLayerRepository));
            SetManagementPermission(Permission.ManageAvatars);
        }
        public async Task<Dictionary<int, int>> GetDefaultAvatarAsync()
        {
            var avatarParts = new Dictionary<int, int>();
            var layerIds = await _dynamicAvatarLayerRepository.GetLayerIdsAsync();
            foreach (int layerId in layerIds)
            {
                var elementId = await _dynamicAvatarElementRepository.GetIdByLayerIdAsync(layerId);
                avatarParts.Add(layerId, elementId);
            }
            return avatarParts;
        }

        public async Task<Dictionary<int, int>> ReturnValidated(IEnumerable<int> elementIds)
        {
            var avatarParts = new Dictionary<int, int>();
            var layerIds = await _dynamicAvatarLayerRepository.GetLayerIdsAsync();

            if (layerIds.Count() != elementIds.Count())
            {
                return null;
            }

            var layerElements = layerIds
                .Zip(elementIds, (LayerId, ElementId) => new { LayerId, ElementId })
                .ToDictionary(_ => _.LayerId, _ => _.ElementId);

            foreach (var layerId in layerElements.Keys)
            {
                if (!await _dynamicAvatarElementRepository.ExistsAsync(layerId, layerElements[layerId]))
                {
                    return null;
                }
            }
            return layerElements;
        }

        public async Task<int> GetNextElement(int layerNumber, int elementId)
        {
            var layerIds = await _dynamicAvatarLayerRepository.GetLayerIdsAsync();
            int layerId = layerIds.ElementAt(layerNumber - 1);
            var nextId
                = await _dynamicAvatarElementRepository.GetNextElement(layerId, elementId);

            if (nextId == null)
            {
                nextId = await _dynamicAvatarElementRepository.GetFirstElement(layerId);
            }

            return (int)nextId;
        }

        public async Task<int> GetPreviousElement(int layerNumber, int elementId)
        {
            var layerIds = await _dynamicAvatarLayerRepository.GetLayerIdsAsync();
            int layerId = layerIds.ElementAt(layerNumber - 1);
            var prevId
                = await _dynamicAvatarElementRepository.GetPreviousElement(layerId, elementId);

            if (prevId == null)
            {
                prevId = await _dynamicAvatarElementRepository.GetLastElement(layerId);
            }

            return (int)prevId;
        }

        public async Task<DynamicAvatarLayer> AddLayerAsync(DynamicAvatarLayer dynamicAvatarLayer)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarLayerRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                dynamicAvatarLayer);
        }

        public async Task<DynamicAvatarElement> AddElementAsync(DynamicAvatarElement dynamicAvatarElement)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarElementRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                dynamicAvatarElement);
        }
    }
}
