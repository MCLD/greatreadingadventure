using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class UserImportService : BaseUserService<UserImportService>
    {
        private const int MaxLength = 255;

        private readonly SiteService _siteService;

        public UserImportService(ILogger<UserImportService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            SiteService siteService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public async Task<UserImportResult> GetFromCsvAsync(StreamReader csvStream,
            int programId)
        {
            var users = new List<UserImportExport>();
            var errors = new List<string>();

            var program = await _siteService.GetProgramByIdAsync(programId);

            using (var csv = new CsvHelper.CsvReader(csvStream))
            {
                try
                {
                    int recordCount = 0;

                    foreach (var record in csv.GetRecords<UserImportExport>())
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(record.FirstName))
                            {
                                errors.Add($"First name is blank on record {recordCount + 2}");
                            }
                            else if (record.FirstName.Length > MaxLength)
                            {
                                errors.Add($"First name is longer than {MaxLength} characters on record {recordCount + 2}");
                            }

                            if (record.LastName?.Length > MaxLength)
                            {
                                errors.Add($"First name is longer than {MaxLength} characters on record {recordCount + 2}");
                            }

                            if (program.AgeRequired && !record.Age.HasValue)
                            {
                                errors.Add($"Age is blank on record {recordCount + 2}");
                            }

                            if (errors.Count == 0)
                            {
                                users.Add(new UserImportExport
                                {
                                    FirstName = record.FirstName,
                                    LastName = record.LastName,
                                    Age = program.AskAge ? record.Age : null
                                });
                            }

                            recordCount++;
                        }
                        catch (Exception rex)
                        {
                            if (rex.InnerException != null)
                            {
                                _logger.LogError($"Error reading household user import: {rex.InnerException.Message}");
                                errors.Add($"<li>Problem reading record {recordCount + 2}: {rex.InnerException.Message}</li>");
                            }
                            else
                            {
                                _logger.LogError($"Error reading household user import: {rex.Message}");
                                errors.Add($"<li>Problem reading record {recordCount + 2}: {rex.Message}</li>");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string error = $"CSV parsing error: {ex.Message}";
                    _logger.LogError(error);
                    errors.Add(error);
                }
            }

            var result = new UserImportResult();

            if (errors.Count > 0)
            {
                result.Errors = errors;
            }
            else
            {
                result.Users = users;
            }

            return result;
        }
    }
}
