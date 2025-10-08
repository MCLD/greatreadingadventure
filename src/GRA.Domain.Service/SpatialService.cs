using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Service.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class SpatialService : BaseUserService<SpatialService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IGraCache _cache;
        private readonly ILocationRepository _locationRepository;
        private readonly SiteLookupService _siteLookupService;
        private readonly ISpatialDistanceRepository _spatialDistanceRepository;

        public SpatialService(ILogger<SpatialService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IGraCache cache,
            ILocationRepository locationRepository,
            ISpatialDistanceRepository spatialDistanceRepository,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(locationRepository);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(spatialDistanceRepository);

            _branchRepository = branchRepository;
            _cache = cache;
            _locationRepository = locationRepository;
            _siteLookupService = siteLookupService;
            _spatialDistanceRepository = spatialDistanceRepository;
        }

        public static double GetHaversineDistance(double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            const int R = 3959; // In miles
            var latitudeArc = ToRadians(latitude2 - latitude1);
            var longitudeArc = ToRadians(longitude2 - longitude1);
            var latitude1Radians = ToRadians(latitude1);
            var latitude2Radians = ToRadians(latitude2);

            var a = (Math.Sin(latitudeArc / 2) * Math.Sin(latitudeArc / 2))
                + (Math.Sin(longitudeArc / 2) * Math.Sin(longitudeArc / 2)
                * Math.Cos(latitude1Radians) * Math.Cos(latitude2Radians));

            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        public async Task<ServiceResult<string>> GetGeocodedAddressAsync(string address)
        {
            ArgumentNullException.ThrowIfNull(address);

            var serviceResult = new ServiceResult<string>();

            var (geocodingEnabled, APIKey) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(),
                SiteSettingKey.Events.GoogleMapsAPIKey);

            if (!geocodingEnabled)
            {
                _logger.LogCritical("Geocoding called without geocoding enabled");

                serviceResult.Message = "Geocoding is not enabled.";
                serviceResult.Status = ServiceResultStatus.Error;

                return serviceResult;
            }

            var formattedAddress = address.Trim();

            var cacheKey = $"a{formattedAddress}.{CacheKey.AddressGeocoding}";
            var geolocation = await _cache.GetStringFromCache(cacheKey);

            if (!string.IsNullOrWhiteSpace(geolocation))
            {
                serviceResult.Status = ServiceResultStatus.Success;
                serviceResult.Data = geolocation;
            }
            else
            {
                dynamic jsonResult;

                using var client = new HttpClient();
                try
                {
                    var encodedAddress = WebUtility.UrlEncode(formattedAddress);
                    var uri = new Uri($"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={APIKey}");
                    var response = await client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    jsonResult = JsonConvert.DeserializeObject(stringResult);

                    string status = jsonResult?.status?.ToString();

                    if (status == "ZERO_RESULTS")
                    {
                        serviceResult.Status = ServiceResultStatus.Warning;
                        serviceResult.Message = "No results found for address.";
                    }
                    else if (status != "OK")
                    {
                        _logger.LogError("Error getting geocoding results for address {Address}: {Status}",
                            formattedAddress,
                            status);

                        serviceResult.Status = ServiceResultStatus.Error;
                        serviceResult.Message = "An error occured, please try again later.";
                    }
                    else
                    {
                        var result = jsonResult.results[0];
                        double latitude = result.geometry.location.lat;
                        double longitude = result.geometry.location.lng;

                        geolocation = $"{latitude},{longitude}";

                        await _cache.SaveToCacheAsync(cacheKey,
                            geolocation,
                            ExpireInTimeSpan(60));

                        serviceResult.Status = ServiceResultStatus.Success;
                        serviceResult.Data = geolocation;
                    }
                }
                catch (HttpRequestException hrex)
                {
                    _logger.LogCritical(hrex, "Google API error: {ErrorMessage}", hrex.Message);
                    serviceResult.Status = ServiceResultStatus.Error;
                    serviceResult.Message = "An error occured, please try again later.";
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Error: {ErrorMessage}", ex.Message);
                    serviceResult.Status = ServiceResultStatus.Error;
                    serviceResult.Message = "An error occured, please try again later.";
                }
            }
            return serviceResult;
        }

        public async Task<int> GetSpatialDistanceIdForGeolocationAsync(string geolocation)
        {
            ArgumentNullException.ThrowIfNull(geolocation);
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
                CreatedBy = await _siteLookupService.GetSystemUserId(),
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

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
