﻿using System;
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
    public class EventImportService : Abstract.BaseUserService<EventImportService>
    {
        private readonly BadgeService _badgeService;
        private readonly EventService _eventService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        public EventImportService(ILogger<EventImportService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            BadgeService badgeService,
            EventService eventService,
            SiteService siteService,
            TriggerService triggerService) : base(logger, dateTimeProvider, userContextProvider)
        {
            _badgeService = badgeService ?? throw new ArgumentException(nameof(badgeService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
        }

        public async Task<(ImportStatus, string)> FromCsvAsync(StreamReader csvStream)
        {
            var notes = new List<string>();
            using (var csv = new CsvHelper.CsvReader(csvStream))
            {
                try
                {
                    int count = 0;
                    int issues = 0;
                    var branches = await _siteService.GetAllBranches();
                    var systems = await _siteService.GetSystemList();
                    foreach (var record in csv.GetRecords<EventImportExport>())
                    {
                        try
                        {
                            if (!File.Exists(record.BadgeFilePath))
                            {
                                throw new GraException($"Unable to find badge file at {record.BadgeFilePath}");
                            }
                            var system = systems
                                .Where(_ => _.Name.Trim().Equals(record.SystemName.Trim(),
                                    StringComparison.OrdinalIgnoreCase))
                                .SingleOrDefault();
                            if (system == null)
                            {
                                throw new GraException($"Unable to find system named {record.SystemName} in system list.");
                            }

                            var branch = branches
                                .Where(_ => _.SystemId == system.Id
                                    && _.Name.Trim().Equals(record.BranchName.Trim(),
                                        StringComparison.OrdinalIgnoreCase))
                                .SingleOrDefault();
                            if (branch == null)
                            {
                                throw new GraException($"Unable to find branch named {record.BranchName} for system {record.SystemName} in branch list.");
                            }

                            var badge = new Badge
                            {
                                Filename = "badge.png"
                            };
                            var addedBadge = await _badgeService.AddBadgeAsync(badge,
                                File.ReadAllBytes(record.BadgeFilePath));

                            if (record.Name.Length > 255)
                            {
                                record.Name = record.Name.Substring(0, 255);
                                string warning = $"<li>Name too long, truncated: <strong>{record.Name}</strong>.</li>";
                                if (!notes.Contains(warning))
                                {
                                    notes.Add(warning);
                                }
                            }

                            if (record.Description.Length > 1500)
                            {
                                record.Description = record.Description.Substring(0, 1500);
                                string warning = $"<li>Description too long, truncated: <strong>{record.Name}</strong>.</li>";
                                if (!notes.Contains(warning))
                                {
                                    notes.Add(warning);
                                }
                            }

                            var addedTrigger = await _triggerService.AddAsync(new Trigger
                            {
                                Name = $"Event '{record.Name}' at {record.BranchName} code",
                                SecretCode = record.SecretCode,
                                AwardMessage = $"Thanks for attending: {record.Name}!",
                                AwardPoints = record.Points,
                                AwardBadgeId = addedBadge.Id,
                                RelatedBranchId = branch.Id,
                                RelatedSystemId = branch.SystemId
                            });

                            var addedEvent = await _eventService.Add(new Event
                            {
                                AtBranchId = branch.Id,
                                Description = record.Description,
                                Name = record.Name,
                                RelatedBranchId = branch.Id,
                                RelatedSystemId = branch.SystemId,
                                RelatedTriggerId = addedTrigger.Id,
                                StartDate = record.Start,
                                IsActive = true,
                                IsValid = true
                            });

                            count++;
                        }
                        catch (Exception rex)
                        {
                            issues++;
                            if (rex.InnerException != null)
                            {
                                _logger.LogError($"Event insertion error: {rex.InnerException.Message}");
                                notes.Add($"<li>Problem inserting {record.Name}: {rex.InnerException.Message}</li>");
                            }
                            else
                            {
                                _logger.LogError($"Event insertion error: {rex.Message}");
                                notes.Add($"<li>Problem inserting {record.Name}: {rex.Message}</li>");
                            }
                        }
                    }
                    var returnMessage = new StringBuilder($"<p><strong>Imported {count} events and skipped {issues} due to issues.</strong></p>");
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
