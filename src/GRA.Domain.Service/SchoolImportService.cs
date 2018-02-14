using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SchoolImportService : BaseUserService<SchoolImportService>
    {
        private readonly SchoolService _schoolService;
        public SchoolImportService(ILogger<SchoolImportService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            SchoolService schoolService) : base(logger, dateTimeProvider, userContextProvider)
        {
            _schoolService = schoolService
                ?? throw new ArgumentNullException(nameof(schoolService));
        }

        public async Task<(ImportStatus, string)> FromCsvAsync(StreamReader csvStream)
        {
            var notes = new List<string>();
            using (var csv = new CsvHelper.CsvReader(csvStream))
            {
                try
                {
                    int recordCount = 0;
                    int issues = 0;
                    int typesAdded = 0;
                    int districtsAdded = 0;
                    int schoolsAdded = 0;
                    var types = await _schoolService.GetTypesAsync();
                    var districts = await _schoolService.GetDistrictsAsync();
                    var schools = await _schoolService.GetSchoolsAsync();

                    var typeIndex = types.ToDictionary(_ => _.Name, _ => _.Id);
                    var districtIndex = types.ToDictionary(_ => _.Name, _ => _.Id);

                    foreach (var record in csv.GetRecords<SchoolImportExport>())
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(record.Type))
                            {
                                throw new Exception($"School type is blank on record {recordCount + 2}");
                            }
                            if (string.IsNullOrEmpty(record.District))
                            {
                                throw new Exception($"School district is blank on record {recordCount + 2}");
                            }
                            if (string.IsNullOrEmpty(record.Name))
                            {
                                throw new Exception($"School name is blank on record {recordCount + 2}");
                            }

                            if (record.Type.Length > 255)
                            {
                                record.Type = record.Type.Substring(0, 255);
                                string warning = $"<li>Type too long, truncated: <strong>{record.Type}</strong>.</li>";
                                if (!notes.Contains(warning))
                                {
                                    notes.Add(warning);
                                }
                            }

                            if (record.District.Length > 255)
                            {
                                record.Type = record.District.Substring(0, 255);
                                string warning = $"<li>District too long, truncated: <strong>{record.District}</strong>.</li>";
                                if (!notes.Contains(warning))
                                {
                                    notes.Add(warning);
                                }
                            }

                            if (record.Name.Length > 255)
                            {
                                record.Name = record.Name.Substring(0, 255);
                                string warning = $"<li>Type too long, truncated: <strong>{record.Name}</strong>.</li>";
                                if (!notes.Contains(warning))
                                {
                                    notes.Add(warning);
                                }
                            }

                            int typeId;
                            if (typeIndex.Keys.Contains(record.Type.Trim()))
                            {
                                typeId = typeIndex[record.Type.Trim()];
                            }
                            else
                            {
                                _logger.LogDebug($"Adding school type: {record.Type.Trim()}");
                                var type = await _schoolService.AddSchoolType(record.Type.Trim());
                                typeIndex.Add(record.Type.Trim(), type.Id);
                                typeId = type.Id;
                                typesAdded++;
                            }

                            int districtId;
                            if (districtIndex.Keys.Contains(record.District.Trim()))
                            {
                                districtId = districtIndex[record.District.Trim()];
                            }
                            else
                            {
                                _logger.LogDebug($"Adding school district: {record.District.Trim()}");
                                var district = await _schoolService.AddDistrict(new SchoolDistrict()
                                {
                                    Name = record.District.Trim()
                                });
                                districtIndex.Add(record.District.Trim(), district.Id);
                                districtId = district.Id;
                                districtsAdded++;
                            }

                            var schoolExists = schools.Where(_ => _.SchoolDistrictId == districtId
                                && _.Name == record.Name.Trim()).Any();

                            if (!schoolExists)
                            {
                                _logger.LogDebug($"Adding school: {record.Name.Trim()}");
                                await _schoolService
                                    .AddSchool(record.Name.Trim(), districtId, typeId);
                                schoolsAdded++;
                            }

                            recordCount++;
                        }
                        catch (Exception rex)
                        {
                            issues++;
                            if (rex.InnerException != null)
                            {
                                _logger.LogError($"School import error: {rex.InnerException.Message}");
                                notes.Add($"<li>Problem inserting record {recordCount + 2}: {rex.InnerException.Message}</li>");
                            }
                            else
                            {
                                _logger.LogError($"School import error: {rex.Message}");
                                notes.Add($"<li>Problem inserting record {recordCount + 2}: {rex.Message}</li>");
                            }
                        }
                    }
                    var returnMessage = new StringBuilder($"<p><strong>Imported {recordCount} records ({typesAdded} types, {districtsAdded} districts, {schoolsAdded} schools) and skipped {issues} rows due to issues.</strong></p>");
                    foreach (var note in notes)
                    {
                        returnMessage.AppendLine(note);
                    }
                    if (issues > 0)
                    {
                        return (ImportStatus.Warning, returnMessage.ToString());
                    }
                    if (notes.Count > 0)
                    {
                        return (ImportStatus.Info, returnMessage.ToString());
                    }
                    return (ImportStatus.Success, returnMessage.ToString());
                }
                catch (Exception ex)
                {
                    string error = $"CSV parsing error: {ex.Message}";
                    _logger.LogError(error);
                    return (ImportStatus.Danger, error);
                }
            }
        }
    }
}
