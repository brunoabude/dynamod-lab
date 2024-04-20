using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;
using DynaMod.SampleApplication.Domain.Repository;
using DynaMod.SampleApplication.Domain.Services;
using Xunit;

namespace DynaMod.Tests.SampleApplication
{
    public class UserTests : SampleApplicationTestCase
    {
        protected readonly UserRepository _user_repository;

        public UserTests() : base()
        {
            _user_repository = new UserRepository(_session);
        }

        [Fact]
        public async Task UserTest()
        {
            _user_repository.CreateUser("john_doe", "John Doe", "johndoe@org.com", "johndoe123");
            await _session.CommitAsync();

            User john_doe = await _user_repository.GetUser("john_doe");

            Assert.NotNull(john_doe);
            Assert.Equal("john_doe", john_doe.Id);
            Assert.Equal("John Doe", john_doe.DisplayName);

            Assert.True(SecurityService.CheckPassword(john_doe, "johndoe123"));
            Assert.False(SecurityService.CheckPassword(john_doe, "johndoe124"));
            await _session.CommitAsync();

            john_doe = await _user_repository.GetUser("john_doe");
            john_doe.ChangeDisplayName("Mr. John Doe");
            
            await _session.CommitAsync();

            User mr_john_doe = await _user_repository.GetUser("john_doe");
            User mr_john_doe_second_reference = await _user_repository.GetUser("john_doe");

            Assert.Same(mr_john_doe, mr_john_doe_second_reference);

            Assert.Equal("Mr. John Doe", mr_john_doe.DisplayName);

            mr_john_doe.ChangeDisplayName("Mr. John Doe, The Master Of Nothing");
            Assert.Equal("Mr. John Doe, The Master Of Nothing", mr_john_doe_second_reference.DisplayName);
            await _session.CommitAsync();

            User mr_john_doe_master_of_nothing = await _user_repository.GetUser("john_doe");
            Assert.Equal("Mr. John Doe, The Master Of Nothing", mr_john_doe_master_of_nothing.DisplayName);
        }
    }
}
