using GRA.Domain.Service;
using Xunit;

namespace GRA.Test
{
    public class EmailValidationTest
    {
        [Fact]
        public void ValidateInvalidEmails()
        {
            Assert.False(EmailService.ValidateAddress("plainaddress"));
            Assert.False(EmailService.ValidateAddress("#@%^%#$@#$@#.com"));
            Assert.False(EmailService.ValidateAddress("@example.com"));
            Assert.False(EmailService.ValidateAddress("email.example.com"));
            Assert.False(EmailService.ValidateAddress("email@example@example.com"));
            Assert.False(EmailService.ValidateAddress(".email@example.com"));
            Assert.False(EmailService.ValidateAddress("email.@example.com"));
            Assert.False(EmailService.ValidateAddress("email..email@example.com"));
            Assert.False(EmailService.ValidateAddress("email@example..com"));
            Assert.False(EmailService.ValidateAddress("Abc..123@example.com"));
        }

        [Fact]
        public void ValidateValidEmails()
        {
            Assert.True(EmailService.ValidateAddress("email@example.com"));
            Assert.True(EmailService.ValidateAddress("firstname.lastname@example.com"));
            Assert.True(EmailService.ValidateAddress("email@subdomain.example.com"));
            Assert.True(EmailService.ValidateAddress("firstname+lastname@example.com"));
            Assert.True(EmailService.ValidateAddress("email@123.123.123.123"));
            Assert.True(EmailService.ValidateAddress("email@[123.123.123.123]"));
            Assert.True(EmailService.ValidateAddress("“email”@example.com"));
            Assert.True(EmailService.ValidateAddress("1234567890@example.com"));
            Assert.True(EmailService.ValidateAddress("email@example-one.com"));
            Assert.True(EmailService.ValidateAddress("_______@example.com"));
            Assert.True(EmailService.ValidateAddress("email@example.name"));
            Assert.True(EmailService.ValidateAddress("email@example.museum"));
            Assert.True(EmailService.ValidateAddress("email@example.co.jp"));
            Assert.True(EmailService.ValidateAddress("firstname-lastname@example.com"));
        }
    }
}
