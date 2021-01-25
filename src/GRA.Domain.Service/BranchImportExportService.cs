using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class BranchImportExportService : BaseUserService<BranchImportExportService>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;
        private readonly SiteService _siteService;
        private readonly SpatialService _spatialService;

        public BranchImportExportService(ILogger<BranchImportExportService> logger,
            IDateTimeProvider dateTimeProvider,
            IPathResolver pathResolver,
            IUserContextProvider userContextProvider,
            IJobRepository jobRepository,
            SiteLookupService siteLookupService,
            SiteService siteService,
            SpatialService spatialService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _spatialService = spatialService
                ?? throw new ArgumentNullException(nameof(spatialService));
        }

        public async Task<byte[]> ExportAsync()
        {
            var branches = await _siteService.GetAllBranches();

            using var memoryStream = new System.IO.MemoryStream();
            using var writer = new System.IO.StreamWriter(memoryStream);
            using var csv = new CsvHelper.CsvWriter(writer,
                new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.Context.RegisterClassMap<Maps.BranchMap>();

            await csv.WriteRecordsAsync(branches.OrderBy(_ => _.SystemName).ThenBy(_ => _.Name));

            await csv.FlushAsync();
            await writer.FlushAsync();
            await memoryStream.FlushAsync();

            return memoryStream.ToArray();
        }

        public async Task<JobStatus> RunImportJobAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var job = await _jobRepository.GetByIdAsync(jobId);
            var jobDetails
                = JsonConvert.DeserializeObject<JobBranchImport>(job.SerializedParameters);

            _logger.LogInformation("Job {JobId}: {JobType} to import {Filename}",
                job.Id,
                job.JobType,
                jobDetails.Filename);

            ICollection<Branch> importBranches = null;
            ICollection<string> importSystems = null;

            token.Register(() =>
            {
                _logger.LogWarning("Job {JobId}: {ImportType} of {Filename} was cancelled after {Elapsed} ms.",
                    job.Id,
                    jobDetails.DoImport ? "Import" : "Test run",
                    jobDetails.Filename,
                    sw?.Elapsed.TotalMilliseconds);
            });

            string fullPath = _pathResolver.ResolvePrivateTempFilePath(jobDetails.Filename);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogError("Could not find {Filename}", fullPath);
                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Could not find the import file.",
                    Error = true,
                    Complete = true
                };
            }

            int importedCount = 0;
            int processedCount = 0;

            int systemAdd = 0;
            int systemEdit = 0;
            int branchAdd = 0;
            int branchEdit = 0;

            int geocodingIssues = 0;

            var createdAt = _dateTimeProvider.Now;

            try
            {
                // loop, watch for token.IsCancellationRequested
                // open csv file
                // look at current systems/branches
                // replace currents
                // import news
                // return result
                _logger.LogInformation("Job {JobId}: {ImportType} reading in CSV file: {Filename}",
                    job.Id,
                    jobDetails.DoImport ? "Import" : "Test run",
                    jobDetails.Filename);

                progress?.Report(new JobStatus
                {
                    Status = "Reading in CSV file..."
                });

                try
                {
                    importBranches = await ReadBranchesAsync(fullPath);
                }
                catch (System.IO.IOException ioex)
                {
                    _logger.LogError(ioex,
                        "Job {JobId}: Error reading CSV file {Filename}: {ErrorMessage}",
                        job.Id,
                        jobDetails.Filename,
                        ioex.Message);
                    return new JobStatus
                    {
                        Error = true,
                        Complete = true,
                        Status = $"Error reading records from the CSV file: {ioex.Message}"
                    };
                }

                if (importBranches?.Count == 0)
                {
                    return new JobStatus
                    {
                        Error = true,
                        Complete = true,
                        Status = "No records found in the CSV file."
                    };
                }

                importSystems = importBranches.Select(_ => _.SystemName)
                    .OrderBy(_ => _)
                    .Distinct()
                    .ToList();

                _logger.LogInformation("Job {JobId}: {ImportType} found {SystemCount} systems, {BranchCount} branches",
                    job.Id,
                    jobDetails.DoImport ? "Import" : "Test run",
                    importSystems?.Count,
                    importBranches?.Count);

                await _jobRepository.UpdateStatusAsync(jobId,
                    $"Found {importSystems?.Count} systems and {importBranches?.Count} branches in the CSV file");

                progress?.Report(new JobStatus
                {
                    Status = $"Found {importSystems?.Count} systems and {importBranches?.Count} branches in the uploaded file..."
                });
                var lastUpdate = sw.Elapsed.TotalSeconds;

                var totalRecords = importSystems?.Count + importBranches?.Count;

                var softwareSystems = await _siteService.GetSystemList();
                var systemIndex = softwareSystems.ToDictionary(k => k.Id, v => v.Name);

                if (importSystems?.Count > 0)
                {
                    foreach (var importSystem in importSystems)
                    {
                        processedCount++;
                        var alreadyPresent = softwareSystems
                            .FirstOrDefault(_ => _.Name == importSystem);

                        if (alreadyPresent != null)
                        {
                            _logger.LogInformation("Job {JobId}: Not creating system {SystemName}, already present as id {SystemId}",
                                job.Id,
                                importSystem,
                                alreadyPresent.Id);
                        }
                        else
                        {
                            Model.System existingSystem = null;
                            if (systemAdd + systemEdit == 0)
                            {
                                existingSystem = softwareSystems.SingleOrDefault();
                            }
                            if (existingSystem == null)
                            {
                                // add
                                systemAdd++;
                                if (jobDetails.DoImport)
                                {
                                    var addedSystem
                                        = await _siteService.AddSystemAsync(new Model.System
                                        {
                                            CreatedAt = createdAt,
                                            CreatedBy = jobDetails.UserId,
                                            Name = importSystem,
                                            SiteId = job.SiteId
                                        });
                                    systemIndex.Add(addedSystem.Id, importSystem);
                                    importedCount++;
                                }
                                else
                                {
                                    var fauxKey = systemAdd;
                                    while (systemIndex.ContainsKey(fauxKey))
                                    {
                                        fauxKey++;
                                    }
                                    systemIndex.Add(fauxKey, importSystem);
                                }
                            }
                            else
                            {
                                // edit
                                systemEdit++;
                                systemIndex[existingSystem.Id] = importSystem;
                                if (jobDetails.DoImport)
                                {
                                    _logger.LogInformation("Job {JobId}: Repurposing system {OldSystemName} as {NewSystemName}",
                                        job.Id,
                                        existingSystem.Name,
                                        importSystem);
                                    existingSystem.CreatedAt = createdAt;
                                    existingSystem.CreatedBy = jobDetails.UserId;
                                    existingSystem.Name = importSystem;
                                    await _siteService.UpdateSystemAsync(existingSystem);
                                    importedCount++;
                                }
                            }
                        }

                        if (sw.Elapsed.TotalSeconds > lastUpdate + 5)
                        {
                            progress?.Report(new JobStatus
                            {
                                PercentComplete = processedCount * 100 / totalRecords,
                                Status = $"Processed {processedCount}, imported {importedCount} records; on system: {importSystem}"
                            });
                            lastUpdate = sw.Elapsed.TotalSeconds;
                        }
                    }
                }

                var status = new JobStatus
                {
                    Status = $"Processed {processedCount}, imported {importedCount} records; now importing branches..."
                };

                if (totalRecords > 0)
                {
                    status.PercentComplete = processedCount * 100 / totalRecords;
                }

                progress?.Report(status);
                lastUpdate = sw.Elapsed.TotalSeconds;

                if (importBranches?.Count > 0)
                {
                    var softwareBranches = await _siteService.GetAllBranches();

                    foreach (var branch in importBranches)
                    {
                        processedCount++;
                        var alreadyPresent = softwareBranches
                            .FirstOrDefault(_ => _.Name == branch.Name
                                && _.SystemName == branch.SystemName);

                        if (alreadyPresent != null)
                        {
                            _logger.LogInformation("Job {JobId}: Not creating branch {BranchName}, already present as id {BranchId}",
                                job.Id,
                                branch.Name,
                                alreadyPresent.Id);
                        }
                        else
                        {
                            Model.Branch existingBranch = null;
                            int? systemId = null;

                            if (systemIndex.ContainsValue(branch.SystemName))
                            {
                                systemId = systemIndex
                                    .Single(_ => _.Value == branch.SystemName)
                                    .Key;
                            }

                            if (systemId == null)
                            {
                                _logger.LogInformation("Job {JobId}: Couldn't find system named {SystemName} for branch {BranchName} in the database.",
                                    job.Id,
                                    branch.SystemName,
                                    branch.Name);
                                return new JobStatus
                                {
                                    Error = true,
                                    Complete = true,
                                    Status = $"Could not find system {branch.SystemName} for branch {branch.Name}."
                                };
                            }
                            if (branchAdd + branchEdit == 0)
                            {
                                existingBranch = softwareBranches.SingleOrDefault();
                            }

                            //geolocation
                            string geocode = null;
                            if (!string.IsNullOrEmpty(branch.Address))
                            {
                                geocode = await GeocodeAsync(job.SiteId, branch.Address);
                                if (string.IsNullOrEmpty(geocode))
                                {
                                    geocodingIssues++;
                                }
                            }

                            if (existingBranch == null)
                            {
                                // add
                                branchAdd++;
                                if (jobDetails.DoImport)
                                {
                                    await _siteService.AddBranchAsync(new Model.Branch
                                    {
                                        Address = branch.Address,
                                        CreatedAt = createdAt,
                                        CreatedBy = jobDetails.UserId,
                                        Geolocation = geocode,
                                        Name = branch.Name,
                                        SystemId = (int)systemId,
                                        Telephone = branch.Telephone,
                                        Url = branch.Url
                                    });
                                    importedCount++;
                                }
                            }
                            else
                            {
                                // edit
                                branchEdit++;
                                if (jobDetails.DoImport)
                                {
                                    _logger.LogInformation("Job {JobId}: Repurposing branch {OldBranchName} as {NewBranchName}",
                                        job.Id,
                                        existingBranch.Name,
                                        branch.Name);
                                    existingBranch.Address = branch.Address;
                                    existingBranch.CreatedAt = createdAt;
                                    existingBranch.CreatedBy = jobDetails.UserId;
                                    existingBranch.Geolocation = geocode;
                                    existingBranch.Name = branch.Name;
                                    existingBranch.SystemId = (int)systemId;
                                    existingBranch.Telephone = branch.Telephone;
                                    existingBranch.Url = branch.Url;
                                    await _siteService.UpdateBranchAsync(existingBranch);
                                    importedCount++;
                                }
                            }
                        }

                        if (sw.Elapsed.TotalSeconds > lastUpdate + 5)
                        {
                            progress?.Report(new JobStatus
                            {
                                PercentComplete = processedCount * 100 / totalRecords,
                                Status = $"Processed {processedCount}, imported {importedCount} records; on branch: {branch.Name}"
                            });
                            lastUpdate = sw.Elapsed.TotalSeconds;
                        }
                    }
                }
            }
            finally
            {
                System.IO.File.Delete(fullPath);
            }
            sw.Stop();

            // get result, return new JobStatus (percent = 100, complete = tru, status = whatever, error = true/false
            if (token.IsCancellationRequested)
            {
                _logger.LogWarning("Job {JobId}: Cancelled after importing {ImportedCount} records and {Elapsed} ms",
                    job.Id,
                    importedCount,
                    sw.ElapsedMilliseconds);

                return new JobStatus
                {
                    Complete = true,
                    Status = $"Import cancelled after {importedCount} records and {sw.Elapsed:c} seconds."
                };
            }

            await _jobRepository.UpdateStatusAsync(jobId,
                $"Imported {systemAdd + systemEdit} systems and {branchAdd + branchEdit} branches.");

            _logger.LogInformation("Job {JobId}: {ImportType} added {SystemsAdded}, edited {SystemsEdited} systems; added {BranchesAdded}, edited {BranchesEdited} branches; {GeocodingIssues} geocoding issues",
                job.Id,
                jobDetails.DoImport ? "Import" : "Test run",
                systemAdd,
                systemEdit,
                branchAdd,
                branchEdit,
                geocodingIssues);

            var importType = jobDetails.DoImport ? "Imported" : "Test run imported";

            _logger.LogInformation("Job {JobId}: {ImportType} {importedCount} records in {Elapsed} ms",
                job.Id,
                importType,
                importedCount,
                sw.ElapsedMilliseconds);

            return new JobStatus
            {
                PercentComplete = 100,
                Complete = true,
                Status = $"{importType} {importedCount} records (systems: {systemAdd} added, {systemEdit} updated; branches: {branchAdd} added, {branchEdit} updated) with {geocodingIssues} geocoding issues in {sw.Elapsed:c} seconds."
            };
        }

        private static async Task<ICollection<Branch>> ReadBranchesAsync(string filename)
        {
            var branches = new List<Branch>();
            using var reader = new System.IO.StreamReader(filename);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null
            };
            using var csv = new CsvHelper.CsvReader(reader, config);
            csv.Context.RegisterClassMap<Maps.BranchMap>();

            await foreach (var branch in csv.GetRecordsAsync<Branch>())
            {
                branches.Add(branch);
            }
            return branches;
        }

        private async Task<string> GeocodeAsync(int siteId, string address)
        {
            if (await _siteLookupService.IsSiteSettingSetAsync(siteId,
                SiteSettingKey.Events.GoogleMapsAPIKey))
            {
                var result = await _spatialService
                    .GetGeocodedAddressAsync(address);
                await Task.Delay(500); // wait to avoid geocoding flood

                switch (result.Status)
                {
                    case Models.ServiceResultStatus.Success:
                        return result.Data;
                    case Models.ServiceResultStatus.Warning:
                        _logger.LogWarning("Unable to get geolocation for {Address}: {ErrorMessage}",
                            address,
                            result.Message);
                        break;
                    case Models.ServiceResultStatus.Error:
                        _logger.LogError("Unable to get geolocation for {Address}: {ErrorMessage}",
                            address,
                            result.Message);
                        break;
                    default:
                        _logger.LogError("Unexpected response {ServiceResultStatus} from geolocation of {Address} (data: {Response}): {ErrorMessage}",
                            result.Status,
                            address,
                            result.Data,
                            result.Message);
                        break;
                }
            }
            return null;
        }
    }
}
