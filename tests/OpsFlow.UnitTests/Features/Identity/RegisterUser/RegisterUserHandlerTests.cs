using FluentAssertions;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Security;
using OpsFlow.Application.Features.Identity.RegisterUser;
using OpsFlow.Domain.Identity;
using OpsFlow.Application.Common.Exceptions;
namespace OpsFlow.UnitTests.Features.Identity.RegisterUser;

public sealed class RegisterUserHandlerTests
{
    [Fact]
    public async Task HandleAsyncShouldCreateUserAndCredential()
    {
        var userRepository = new FakeUserRepository();
        var credentialRepository = new FakeUserCredentialRepository();
        var passwordHasher = new FakePasswordHasher();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RegisterUserHandler(
            userRepository,
            credentialRepository,
            passwordHasher,
            unitOfWork);

        var command = new RegisterUserCommand(
            "fuad@example.com",
            "Fuad",
            "Hossain",
            "StrongPassword123!");

        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        result.Email.Should().Be("fuad@example.com");
        result.FirstName.Should().Be("Fuad");
        result.LastName.Should().Be("Hossain");

        userRepository.AddedUser.Should().NotBeNull();
        credentialRepository.AddedCredential.Should().NotBeNull();

        credentialRepository.AddedCredential!
            .PasswordHash
            .Should()
            .Be("HASHED_PASSWORD");

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsyncShouldRejectDuplicateEmail()
    {
        var existingUser = User.Create(
            "fuad@example.com",
            "Fuad",
            "Hossain");

        var userRepository = new FakeUserRepository
        {
            ExistingUser = existingUser
        };

        var credentialRepository = new FakeUserCredentialRepository();
        var passwordHasher = new FakePasswordHasher();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RegisterUserHandler(
            userRepository,
            credentialRepository,
            passwordHasher,
            unitOfWork);

        var command = new RegisterUserCommand(
            "FUAD@example.com",
            "Another",
            "User",
            "StrongPassword123!");

        var exception = await Assert.ThrowsAsync<ConflictException>(
        () => handler.HandleAsync(
        command,
        CancellationToken.None));

        exception.Message.Should()
            .Be("A user with this email already exists.");

        userRepository.AddedUser.Should().BeNull();
        credentialRepository.AddedCredential.Should().BeNull();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsyncShouldHashPasswordBeforeCreatingCredential()
    {
        var userRepository = new FakeUserRepository();
        var credentialRepository = new FakeUserCredentialRepository();
        var passwordHasher = new FakePasswordHasher();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RegisterUserHandler(
            userRepository,
            credentialRepository,
            passwordHasher,
            unitOfWork);

        var command = new RegisterUserCommand(
            "fuad@example.com",
            "Fuad",
            "Hossain",
            "MyPlainTextPassword");

        await handler.HandleAsync(
            command,
            CancellationToken.None);

        passwordHasher.PasswordPassedToHash
            .Should()
            .Be("MyPlainTextPassword");

        credentialRepository.AddedCredential!
            .PasswordHash
            .Should()
            .Be("HASHED_PASSWORD");
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? ExistingUser { get; init; }

        public User? AddedUser { get; private set; }

        public Task<User?> GetByNormalizedEmailAsync(
            string normalizedEmail,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(ExistingUser);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken)
        {
            AddedUser = user;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUserCredentialRepository
        : IUserCredentialRepository
    {
        private readonly List<UserCredential> _credentials = [];

        public UserCredential? AddedCredential =>
            _credentials.SingleOrDefault();

        public Task AddAsync(
            UserCredential credential,
            CancellationToken cancellationToken)
        {
            _credentials.Add(credential);
            return Task.CompletedTask;
        }

        public Task<UserCredential?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var credential = _credentials
                .SingleOrDefault(item => item.UserId == userId);

            return Task.FromResult(credential);
        }
    }
    private sealed class FakePasswordHasher : IPasswordHasher
    {
        private readonly string _expectedHash = "HASHED_PASSWORD";

        public string? PasswordPassedToHash { get; private set; }

        public string Hash(string password)
        {
            PasswordPassedToHash = password;

            return _expectedHash;
        }

        public bool Verify(
            string password,
            string passwordHash)
        {
            return passwordHash == _expectedHash;
        }
    }
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;
            return Task.FromResult(2);
        }
    }
}
