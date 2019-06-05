using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using GRA.Controllers;
using GRA.Domain.Repository;
using GRA.Domain.Model;

namespace GRA.Test
{
    public class UnitTest1
    {
        public User adult;
        public User teen;
        public User kid;
        public User prere;

        public User GetUser(string type)
        {
            var user = new User();
            //Adult
            if(type == "adult")
            {
                user = new User
                {
                    SiteId = 1,
                    BranchId = 1,
                    SystemId = 1,
                    ProgramId = 4,
                    FirstName = "First",
                    LastName = "Last",
                    Username = "testuser1"
                };
            }
            //Teen
            if (type == "teen")
            {
                user = new User
                {
                    SiteId = 1,
                    BranchId = 1,
                    SystemId = 1,
                    ProgramId = 3,
                    FirstName = "First",
                    LastName = "Last",
                    Username = "testuser2"
                };
            }
            //Kid
            if (type == "kid")
            {
                user = new User
                {
                    SiteId = 1,
                    BranchId = 1,
                    SystemId = 1,
                    ProgramId = 2,
                    FirstName = "First",
                    LastName = "Last",
                    Username = "testuser3"
                };
            }
            //PrereaderS
            if (type == "prere")
            {
                user = new User
                {
                    SiteId = 1,
                    BranchId = 1,
                    SystemId = 1,
                    ProgramId = 1,
                    FirstName = "First",
                    LastName = "Last",
                    Username = "testuser4"
                };
            }
            return user;
        }
        //Challenge Creation
        [Fact]
        public void TestChallengeController()
        {
            adult = GetUser("adult");
            teen = GetUser("teen");
            kid = GetUser("kid");
            prere = GetUser("prere");

            var challenge = new Challenge
            {
                SiteId = 1,
                RelatedSystemId = 1,
                BadgeId = null,
                Name = "Get Along",
                Description = "This is a challenge encourging you to get along with others!",
                IsActive = false,
                IsDeleted = false,
                IsValid = true,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = 1
            };
        }
    }
}
