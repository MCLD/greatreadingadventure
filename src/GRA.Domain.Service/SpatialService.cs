using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Service.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class SpatialService : BaseUserService<SpatialService>
    {
        private readonly IDistributedCache _cache;
        private readonly SiteLookupService _siteLookupService;
        private readonly IBranchRepository _branchRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ISpatialDistanceRepository _spatialDistanceRepository;

        public SpatialService(ILogger<SpatialService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDistributedCache cache,
            SiteLookupService siteLookupService,
            IBranchRepository branchRepository,
            ILocationRepository locationRepository,
            ISpatialDistanceRepository spatialDistanceRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _locationRepository = locationRepository
                ?? throw new ArgumentNullException(nameof(locationRepository));
            _spatialDistanceRepository = spatialDistanceRepository
                ?? throw new ArgumentNullException(nameof(spatialDistanceRepository));
        }

        public async Task<ServiceResult<string>>
            GetGeocodedAddressAsync(string address)
        {
            var serviceResult = new ServiceResult<string>();

            var (geocodingEnabled, APIKey) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(),
                SiteSettingKey.Events.GoogleMapsAPIKey);

            if (!geocodingEnabled)
            {
                _logger.LogCritical("Geocoding called without geocoding enabled");
                throw new Exception("Geocoding is not enabled.");
            }

            var formattedAddress = address.Trim();

            var cacheKey = $"a{formattedAddress}.{CacheKey.AddressGeocoding}";
            var geolocation = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(geolocation))
            {

                serviceResult.Status = ServiceResultStatus.Success;
                serviceResult.Data = geolocation;
            }
            else
            {
                dynamic jsonResult;

                using (var client = new HttpClient())
                {
                    try
                    {
                        var encodedAddress = WebUtility.UrlEncode(formattedAddress);
                        var response = await client.GetAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={APIKey}");
                        response.EnsureSuccessStatusCode();

                        var stringResult = await response.Content.ReadAsStringAsync();
                        jsonResult = JsonConvert.DeserializeObject(stringResult);

                        if (jsonResult.status == "ZERO_RESULTS")
                        {
                            serviceResult.Status = ServiceResultStatus.Warning;
                            serviceResult.Message = "No results found for address.";
                        }
                        else if (jsonResult.status != "OK")
                        {
                            _logger.LogError($"Error getting geocoding results for address {address}: {jsonResult.status}");
                            serviceResult.Status = ServiceResultStatus.Error;
                            serviceResult.Message = "An error occured, please try again later.";
                        }
                        else
                        {
                            var result = jsonResult.results[0];
                            double latitude = result.geometry.location.lat;
                            double longitude = result.geometry.location.lng;

                            geolocation = $"{latitude},{longitude}";

                            await _cache.SetStringAsync(cacheKey, geolocation, ExpireIn(60));

                            serviceResult.Status = ServiceResultStatus.Success;
                            serviceResult.Data = geolocation;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogCritical(ex, $"Google API error: {ex.Message}");
                        serviceResult.Status = ServiceResultStatus.Error;
                        serviceResult.Message = "An error occured, please try again later.";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, ex.Message);
                        serviceResult.Status = ServiceResultStatus.Error;
                        serviceResult.Message = "An error occured, please try again later.";
                    }
                }
            }
            return serviceResult;
        }

        public async Task<int> GetSpatialDistanceIdForGeolocationAsync(string geolocation)
        {
            var siteId = GetCurrentSiteId();

            var headerId = await _spatialDistanceRepository
                .GetIdByGeolocationAsync(siteId, geolocation);

            if (headerId.HasValue)
            {
                return headerId.Value;
            }

            var spatialDistanceHeader = new SpatialDistanceHeader
            {
                CreatedAt = _dateTimeProvider.Now,
                CreatedBy = -1,
                Geolocation = geolocation,
                IsValid = true,
                SiteId = siteId
            };

            var geocordinates = geolocation.Split(',').Select(_ => double.Parse(_)).ToArray();
            var latitude = geocordinates[0];
            var longitude = geocordinates[1];

            var spatialDistanceDetailList = new List<SpatialDistanceDetail>();

            var branches = await _branchRepository.GetAllAsync(siteId, true);

            foreach (var branch in branches)
            {
                var branchGeolocation = branch.Geolocation.Split(',')
                    .Select(_ => double.Parse(_)).ToArray();
                var distance = GetHaversineDistance(latitude, longitude,
                    branchGeolocation[0], branchGeolocation[1]);

                var spatialDistanceDetail = new SpatialDistanceDetail
                {
                    BranchId = branch.Id,
                    Distance = distance
                };

                spatialDistanceDetailList.Add(spatialDistanceDetail);
            }

            var locations = await _locationRepository.GetAll(siteId, true);

            foreach (var location in locations)
            {
                var locationGeolocation = location.Geolocation.Split(',')
                    .Select(_ => double.Parse(_)).ToArray();
                var distance = GetHaversineDistance(latitude, longitude,
                    locationGeolocation[0], locationGeolocation[1]);

                var spatialDistanceDetail = new SpatialDistanceDetail
                {
                    LocationId = location.Id,
                    Distance = distance
                };

                spatialDistanceDetailList.Add(spatialDistanceDetail);
            }

            spatialDistanceHeader = await _spatialDistanceRepository.AddHeaderWithDetailsListAsync(
                spatialDistanceHeader, spatialDistanceDetailList);

            return spatialDistanceHeader.Id;
        }

        public double GetHaversineDistance(double latitude1, double longitude1,
            double latitude2, double longitude2)
        {
            const int R = 3959; // In miles
            var latitudeArc = ToRadians(latitude2 - latitude1);
            var longitudeArc = ToRadians(longitude2 - longitude1);
            var latitude1Radians = ToRadians(latitude1);
            var latitude2Radians = ToRadians(latitude2);

            var a = (Math.Sin(latitudeArc / 2) * Math.Sin(latitudeArc / 2))
                + (Math.Sin(longitudeArc / 2) * Math.Sin(longitudeArc / 2)
                * Math.Cos(latitude1Radians) * Math.Cos(latitude2Radians));
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
