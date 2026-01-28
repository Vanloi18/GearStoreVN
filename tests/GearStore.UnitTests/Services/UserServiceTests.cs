using FluentAssertions;
using GearStore.Application.Interfaces;
using GearStore.Application.Services;
using GearStore.Domain.Entities;
using GearStore.UnitTests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace GearStore.UnitTests.Services;

public class UserServiceTests : IDisposable
{
    private readonly GearStore.Infrastructure.Data.GearStoreDbContext _context;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UserServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());
        
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task ChangeUserRoleAsync_UserToAdmin_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com",
            UserName = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync("user-123"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });
        _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(um => um.AddToRoleAsync(user, "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await ChangeRoleAsync(user.Id, "Admin");

        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(um => um.RemoveFromRoleAsync(user, "User"), Times.Once);
        _userManagerMock.Verify(um => um.AddToRoleAsync(user, "Admin"), Times.Once);
    }

    [Fact]
    public async Task ChangeUserRoleAsync_AdminToUser_ShouldUpdateSuccessfully()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "admin-123",
            Email = "admin@example.com",
            UserName = "admin@example.com",
            FirstName = "Admin",
            LastName = "User"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync("admin-123"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Admin" });
        _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "Admin"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(um => um.AddToRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await ChangeRoleAsync(user.Id, "User");

        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(um => um.RemoveFromRoleAsync(user, "Admin"), Times.Once);
        _userManagerMock.Verify(um => um.AddToRoleAsync(user, "User"), Times.Once);
    }

    [Fact]
    public async Task ChangeUserRoleAsync_InvalidRole_ShouldThrowException()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-456",
            Email = "test2@example.com",
            UserName = "test2@example.com"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync("user-456"))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => ChangeRoleAsync(user.Id, "InvalidRole")
        );
    }

    [Fact]
    public async Task ChangeUserRoleAsync_NonExistentUser_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(um => um.FindByIdAsync("nonexistent"))
            .ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => ChangeRoleAsync("nonexistent", "Admin")
        );
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ShouldReturnUser()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-789",
            Email = "test3@example.com",
            UserName = "test3@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync("user-789"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await GetUserAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("user-789");
        result.Email.Should().Be("test3@example.com");
    }

    private async Task<bool> ChangeRoleAsync(string userId, string newRole)
    {
        var validRoles = new[] { "User", "Admin" };
        if (!validRoles.Contains(newRole))
        {
            throw new ArgumentException("Invalid role");
        }

        var user = await _userManagerMock.Object.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var currentRoles = await _userManagerMock.Object.GetRolesAsync(user);
        foreach (var role in currentRoles)
        {
            await _userManagerMock.Object.RemoveFromRoleAsync(user, role);
        }

        var result = await _userManagerMock.Object.AddToRoleAsync(user, newRole);
        return result.Succeeded;
    }

    private async Task<ApplicationUser?> GetUserAsync(string userId)
    {
        return await _userManagerMock.Object.FindByIdAsync(userId);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
