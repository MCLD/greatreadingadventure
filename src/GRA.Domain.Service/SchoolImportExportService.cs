using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SchoolImportExportService : BaseUserService<SchoolImportExportService>
    {
        private readonly SchoolService _schoolService;

        public SchoolImportExportService(ILogger<SchoolImportExportService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            SchoolService schoolService) : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(schoolService);

            _schoolService = schoolService;
        }

        public async Task<(ImportStatus, string)> FromCsvAsync(StreamReader csvStream)
        {
            var notes = new List<string>();
            using var csv = new CsvHelper.CsvReader(csvStream, CultureInfo.InvariantCulture);
            try
            {
                int recordCount = 0;
                int issues = 0;
                int districtsAdded = 0;
                int schoolsAdded = 0;
                var districts = await _schoolService.GetDistrictsAsync();
                var schools = await _schoolService.GetForExportAsync();

                var districtIndex = districts.ToDictionary(_ => _.Name, _ => _.Id);

                foreach (var record in csv.GetRecords<SchoolImportExport>())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(record.District))
                        {
                            throw new GraException($"School district is blank on record {recordCount + 2}");
                        }
                        if (string.IsNullOrEmpty(record.Name))
                        {
                            throw new GraException($"School name is blank on record {recordCount + 2}");
                        }

                        if (record.District.Length > 255)
                        {
                            record.District = record.District[..255];
                            string warning = $"<li>District too long, truncated: <strong>{record.District}</strong>.</li>";
                            if (!notes.Contains(warning))
                            {
                                notes.Add(warning);
                            }
                        }

                        if (record.Name.Length > 255)
                        {
                            record.Name = record.Name[..255];
                            string warning = $"<li>Type too long, truncated: <strong>{record.Name}</strong>.</li>";
                            if (!notes.Contains(warning))
                            {
                                notes.Add(warning);
                            }
                        }

                        int districtId;
                        if (districtIndex.ContainsKey(record.District.Trim()))
                        {
                            districtId = districtIndex[record.District.Trim()];
                        }
                        else
                        {
                            var district = await _schoolService.AddDistrict(new SchoolDistrict
                            {
                                Name = record.District.Trim()
                            });
                            districtIndex.Add(record.District.Trim(), district.Id);
                            districtId = district.Id;
                            districtsAdded++;
                        }

                        var schoolExists = schools.Any(_ => _.District == record.District.Trim()
                            && _.Name == record.Name.Trim());

                        if (!schoolExists)
                        {
                            await _schoolService
                                .AddSchool(record.Name.Trim(), districtId);
                            schoolsAdded++;
                        }

                        recordCount++;
                    }
                    catch (Exception rex)
                    {
                        issues++;
                        if (rex.InnerException != null)
                        {
                            _logger.LogError(rex,
                                "School import error: {ErrorMessage}",
                                rex.InnerException.Message);
                            notes.Add($"<li>Problem inserting record {recordCount + 2}: {rex.InnerException.Message}</li>");
                        }
                        else
                        {
                            _logger.LogError(rex,
                                "School import error: {ErrorMessage}",
                                rex.Message);
                            notes.Add($"<li>Problem inserting record {recordCount + 2}: {rex.Message}</li>");
                        }
                    }
                }
                var returnMessage = new StringBuilder($"<p><strong>Read {recordCount} records (added {districtsAdded} districts, {schoolsAdded} schools) and skipped {issues} rows due to issues.</strong></p>");

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
                _logger.LogError(ex, "CSV parse error: {ErrorMessage}", ex.Message);
                return (ImportStatus.Danger, $"CSV parsing error: {ex.Message}");
            }
        }

        public async Task<byte[]> ToCsvAsync()
        {
            await using var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            await using var csv = new CsvHelper.CsvWriter(writer,
                new CsvConfiguration(CultureInfo.InvariantCulture));

            await csv.WriteRecordsAsync(await _schoolService.GetForExportAsync());

            await csv.FlushAsync();
            await writer.FlushAsync();
            await memoryStream.FlushAsync();

            return memoryStream.ToArray();
        }
    }
}
