using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;

namespace GRA.Domain.Service
{
    public class ConfigurationService : Abstract.BaseService<ConfigurationService>
    {
        private readonly ISiteRepository siteRepository;
        private readonly ISystemRepository systemRepository;
        private readonly IBranchRepository branchRepository;
        private readonly IProgramRepository programRepository;
        private readonly IChallengeRepository challengeRepository;
        public ConfigurationService(ILogger<ConfigurationService> logger,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            IChallengeRepository challengeRepository) : base(logger)
        {
            if (siteRepository == null)
            {
                throw new ArgumentNullException(nameof(siteRepository));
            }
            this.siteRepository = siteRepository;

            if (systemRepository == null)
            {
                throw new ArgumentNullException(nameof(systemRepository));
            }
            this.systemRepository = systemRepository;

            if (branchRepository == null)
            {
                throw new ArgumentNullException(nameof(branchRepository));
            }
            this.branchRepository = branchRepository;

            if (programRepository == null)
            {
                throw new ArgumentNullException(nameof(programRepository));
            }
            this.programRepository = programRepository;

            if (challengeRepository == null)
            {
                throw new ArgumentNullException(nameof(challengeRepository));
            }
            this.challengeRepository = challengeRepository;
        }

        public bool NeedsInitialSetup()
        {
            return siteRepository.GetAll().Count() == 0;
        }

        public void InitialSetup(Model.User adminUser)
        {
            int creatorUserId = 0;
            var allSites = siteRepository.GetAll();

            if (allSites.Count() > 0)
            {
                throw new Exception($"Can't perform initial setup with existing sites: found {allSites.Count()} in the database.");
            }

            var site = new Model.Site
            {
                Name = "Default site",
                Path = "default"
            };
            // create default site
            site = siteRepository.AddSave(creatorUserId, site);

            if (site == null)
            {
                throw new Exception("Unable to add initial default site or multiple sites found.");
            }

            var system = new Model.System
            {
                SiteId = site.Id,
                Name = "Maricopa County Library District"
            };
            system = systemRepository.AddSave(creatorUserId, system);

            if (system == null)
            {
                throw new Exception("Unable to add initial default system or multiple systems found.");
            }

            var branch = new Model.Branch
            {
                SiteId = site.Id,
                SystemId = system.Id,
                Name = "Admin",
                Address = "2700 N. Central Ave Ste 700, 85004",
                Telephone = "602-652-3064",
                Url = "http://mcldaz.org/"
            };

            branch = branchRepository.AddSave(creatorUserId, branch);

            var program = new Model.Program
            {
                SiteId = site.Id,
                Achiever = 1000,
                Name = "Winter Reading Program"
            };

            program = programRepository.AddSave(creatorUserId, program);

            foreach (var value in Enum.GetValues(typeof(Model.ChallengeTaskType)))
            {
                challengeRepository.AddChallengeTaskType(creatorUserId, value.ToString());
            }
            challengeRepository.Save();

            // sample challenge

            var challenge = new Model.Challenge
            {
                SiteId = site.Id,
                RelatedSystemId = system.Id,
                Name = "Test challenge",
                Description = "This is a test challenge!",
                IsActive = false,
                IsDeleted = false,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = branch.Id
            };

            int positionCounter = 1;
            challenge.AddTask(creatorUserId, new Model.ChallengeTask
            {
                Title = "Be excellent to each other",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            challenge.AddTask(creatorUserId, new Model.ChallengeTask
            {
                Title = "Party on, dudes!",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            challenge.AddTask(creatorUserId, new Model.ChallengeTask
            {
                Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death",
                Author = "Kurt Vonnegut",
                Isbn = "978-0385333849",
                ChallengeTaskType = Model.ChallengeTaskType.Book,
                Position = positionCounter++
            });

            challengeRepository.AddSave(creatorUserId, challenge);
        }
    }
}
