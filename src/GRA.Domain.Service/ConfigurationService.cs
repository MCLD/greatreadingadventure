using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class ConfigurationService : Abstract.BaseService<ConfigurationService>
    {
        private readonly IBranchRepository branchRepository;
        private readonly IChallengeRepository challengeRepository;
        private readonly IChallengeTaskRepository challengeTaskRepository;
        private readonly IProgramRepository programRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ISiteRepository siteRepository;
        private readonly ISystemRepository systemRepository;
        private readonly IUserRepository userRepository;
        public ConfigurationService(ILogger<ConfigurationService> logger,
            IBranchRepository branchRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IProgramRepository programRepository,
            IRoleRepository roleRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository) : base(logger)
        {
            if (branchRepository == null)
            {
                throw new ArgumentNullException(nameof(branchRepository));
            }
            this.branchRepository = branchRepository;

            if (challengeRepository == null)
            {
                throw new ArgumentNullException(nameof(challengeRepository));
            }
            this.challengeRepository = challengeRepository;

            if (challengeTaskRepository == null)
            {
                throw new ArgumentNullException(nameof(challengeTaskRepository));
            }
            this.challengeTaskRepository = challengeTaskRepository;

            if (programRepository == null)
            {
                throw new ArgumentNullException(nameof(programRepository));
            }
            this.programRepository = programRepository;
            if (roleRepository == null)
            {
                throw new ArgumentNullException(nameof(roleRepository));
            }
            this.roleRepository = roleRepository;
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
            if (userRepository == null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }
            this.userRepository = userRepository;
        }

        public bool NeedsInitialSetup()
        {
            return siteRepository.PageAll(0, 1).Count() == 0;
        }

        public Model.User InitialSetup(Model.User adminUser, string password)
        {
            var topSites = siteRepository.PageAll(0, 1);

            if (topSites.Count() > 0)
            {
                throw new Exception("Can't perform initial setup with existing sites: found existing sites in the database.");
            }

            var site = new Model.Site
            {
                Name = "Default site",
                Path = "default"
            };
            // create default site
            site = siteRepository.AddSave(-1, site);

            if (site == null)
            {
                throw new Exception("Unable to add initial default site or multiple sites found.");
            }

            var system = new Model.System
            {
                SiteId = site.Id,
                Name = "Maricopa County Library District"
            };
            system = systemRepository.AddSave(-1, system);

            var branch = new Model.Branch
            {
                SiteId = site.Id,
                SystemId = system.Id,
                Name = "Admin",
                Address = "2700 N. Central Ave Ste 700, 85004",
                Telephone = "602-652-3064",
                Url = "http://mcldaz.org/"
            };
            branch = branchRepository.AddSave(-1, branch);

            var program = new Model.Program
            {
                SiteId = site.Id,
                Achiever = 1000,
                Name = "Winter Reading Program"
            };
            program = programRepository.AddSave(-1, program);

            adminUser.BranchId = branch.Id;
            adminUser.ProgramId = program.Id;
            adminUser.SiteId = site.Id;
            var user = userRepository.AddSave(0, adminUser);
            user.CreatedBy = user.Id;
            user = userRepository.UpdateSave(user.Id, user);
            userRepository.SetUserPassword(user.Id, password);

            int creatorUserId = user.Id;

            site.CreatedBy = creatorUserId;
            site = siteRepository.UpdateSave(creatorUserId, site);

            system.CreatedBy = creatorUserId;
            system = systemRepository.UpdateSave(creatorUserId, system);

            branch.CreatedBy = creatorUserId;
            branch = branchRepository.UpdateSave(creatorUserId, branch);

            program.CreatedBy = creatorUserId;
            program = programRepository.UpdateSave(creatorUserId, program);

            var adminRole = roleRepository.AddSave(creatorUserId, new Model.Role
            {
                Name = "System Administrator"
            });

            userRepository.AddRole(creatorUserId, user.Id, adminRole.Id);

            foreach (var value in Enum.GetValues(typeof(Model.Permission)))
            {
                roleRepository.AddPermission(creatorUserId, value.ToString());
            }
            roleRepository.Save();

            foreach(var value in Enum.GetValues(typeof(Model.Permission)))
            {
                roleRepository.AddPermissionToRole(creatorUserId, adminRole.Id, value.ToString());
            }
            roleRepository.Save();

            foreach (var value in Enum.GetValues(typeof(Model.ChallengeTaskType)))
            {
                challengeTaskRepository.AddChallengeTaskType(creatorUserId, value.ToString());
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

            challenge = challengeRepository.AddSave(creatorUserId, challenge);

            int positionCounter = 1;
            challengeTaskRepository.AddSave(creatorUserId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Be excellent to each other",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            challengeTaskRepository.AddSave(creatorUserId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Party on, dudes!",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            challengeTaskRepository.AddSave(creatorUserId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death",
                Author = "Kurt Vonnegut",
                Isbn = "978-0385333849",
                ChallengeTaskType = Model.ChallengeTaskType.Book,
                Position = positionCounter++
            });

            return user;
        }
    }
}
