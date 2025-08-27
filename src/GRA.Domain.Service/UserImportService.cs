using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
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

            using (var csv = new CsvHelper.CsvReader(csvStream, CultureInfo.InvariantCulture))
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
                                    FirstName = record.FirstName?.Trim(),
                                    LastName = record.LastName?.Trim(),
                                    Age = program.AskAge ? record.Age : null
                                });
                            }

                            recordCount++;
                        }
                        catch (Exception rex)
                        {
                            if (rex.InnerException != null)
                            {
                                _logger.LogError("Error reading household user import: {Message}",
                                    rex.InnerException.Message);
                                errors.Add($"<li>Problem reading record {recordCount + 2}: {rex.InnerException.Message}</li>");
                            }
                            else
                            {
                                _logger.LogError("Error reading household user import: {Message}",
                                    rex.Message);
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

        public async Task<UserImportResult> GetFromExcelAsync(string filepath, int programId)
        {
            var users = new List<UserImportExport>();
            var errors = new List<string>();

            var program = await _siteService.GetProgramByIdAsync(programId);

            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                using (var excelReader = ExcelReaderFactory.CreateReader(stream))
                {
                    int currentSheet = 1;
                    while (currentSheet < excelReader.ResultsCount)
                    {
                        excelReader.NextResult();
                        currentSheet++;
                    }

                    const string FirstNameRowHeading = "FirstName";
                    const string LastNameRowHeading = "LastName";
                    const string AgeRowHeading = "Age";

                    int firstNameColumnId = 0;
                    int lastNameColumnId = 0;
                    int ageColumnId = 0;
                    int row = 0;
                    while (excelReader.Read())
                    {
                        row++;
                        if (row == 1)
                        {
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                var columnName = excelReader.GetString(i).Trim() ?? $"Column{i}";
                                switch (columnName)
                                {
                                    case FirstNameRowHeading:
                                        firstNameColumnId = i;
                                        break;
                                    case LastNameRowHeading:
                                        lastNameColumnId = i;
                                        break;
                                    case AgeRowHeading:
                                        ageColumnId = i;
                                        break;
                                    default:
                                        _logger.LogInformation("Unrecognized column {Column} in household import.",
                                            columnName);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (excelReader.GetValue(firstNameColumnId) != null
                                    || excelReader.GetValue(lastNameColumnId) != null
                                    || excelReader.GetValue(ageColumnId) != null)
                            {
                                string firstName = default(string);
                                string lastName = default(string);
                                int? age = default(int?);

                                try
                                {
                                    firstName = excelReader.GetString(firstNameColumnId);

                                    if (string.IsNullOrEmpty(firstName))
                                    {
                                        throw new GraException("First name is empty.");
                                    }
                                    else if (firstName.Length > MaxLength)
                                    {
                                        throw new GraException($"First name is longer than {MaxLength} characters.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Invalid value for first name, row {Row}: {Message}",
                                        row, ex.Message);
                                    errors.Add($"Invalid value for first name on line {row}: {ex.Message}");
                                }

                                try
                                {
                                    lastName = excelReader.GetString(lastNameColumnId);

                                    if (string.IsNullOrEmpty(lastName))
                                    {
                                        throw new GraException("Last name is empty.");
                                    }
                                    else if (lastName.Length > MaxLength)
                                    {
                                        throw new GraException($"Last name is longer than {MaxLength} characters.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Invalid value for last name, row {Row}: {Message}",
                                        row, ex.Message);
                                    errors.Add($"Invalid value for last name on line {row}: {ex.Message}");
                                }

                                if (program.AskAge)
                                {
                                    try
                                    {
                                        var ageString = excelReader
                                            .GetValue(ageColumnId)?
                                            .ToString();

                                        if (string.IsNullOrWhiteSpace(ageString))
                                        {
                                            throw new GraException("Age is empty.");
                                        }

                                        var ageValueString = new string(ageString.Trim()
                                            .SkipWhile(_ => !char.IsDigit(_))
                                            .TakeWhile(char.IsDigit)
                                            .ToArray());

                                        if (string.IsNullOrWhiteSpace(ageValueString))
                                        {
                                            throw new GraException("Unable to get a number value from age.");
                                        }

                                        age = int.Parse(ageValueString);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError("Invalid value for age, row {Row}: {Message}",
                                            row, ex.Message);
                                        errors.Add($"Invalid value for age on line {row}: {ex.Message}");
                                    }
                                }

                                if (errors.Count == 0)
                                {
                                    users.Add(new UserImportExport
                                    {
                                        FirstName = firstName?.Trim(),
                                        LastName = lastName?.Trim(),
                                        Age = program.AskAge ? age : null
                                    });
                                }
                            }
                        }
                    }
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
