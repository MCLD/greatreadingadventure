using System;
using Bogus;
using GRA.Abstract;
using GRA.Domain.Model;

namespace GRA.CommandLine.DataGenerator
{
    class DateTime
    {
        private Faker _faker;
        private SettableDateTimeProvider _dateTimeProvider;
        public DateTime(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException(nameof(dateTimeProvider));
            }
            var settableDateTimeProvider = dateTimeProvider as SettableDateTimeProvider;
            if (settableDateTimeProvider == null)
            {
                throw new ArgumentException("IDateTimeProvider is not settable.");
            }
            _dateTimeProvider = settableDateTimeProvider;
            _faker = new Faker();
        }

        public System.DateTime SetRandom(Site site)
        {
            System.DateTime generated;
            if (site.RegistrationOpens != null && site.ProgramEnds != null)
            {
                generated = _faker
                    .Date
                    .Between((System.DateTime)site.RegistrationOpens,
                        (System.DateTime)site.ProgramEnds);

            }
            else
            {
                generated = _faker.Date.Recent();
            }
            _dateTimeProvider.SetDateTime(generated);
            return generated;
        }

        public System.DateTime SetRandom(Site site, Domain.Model.User user)
        {
            System.DateTime generated;
            if (site.ProgramStarts != null && site.ProgramEnds != null)
            {
                System.DateTime startDate = site.ProgramStarts > user.CreatedAt
                    ? (System.DateTime)site.ProgramStarts
                    : user.CreatedAt;
                generated = _faker
                    .Date
                    .Between(startDate, (System.DateTime)site.ProgramEnds);
            }
            else
            {
                generated = _faker
                    .Date
                    .Between(user.CreatedAt, user.CreatedAt.AddDays(60));
            }
            _dateTimeProvider.SetDateTime(generated);
            return generated;
        }

    }
}