using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GearStore.Application.Common;
using GearStore.Application.DTOs.Admin;
using GearStore.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GearStore.IntegrationTests.Admin;

public class AdminUsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public AdminUsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AdminUserDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUsers_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_WithUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserRole_ValidRole_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var usersResponse = await _client.GetAsync("/api/admin/users");
        var usersResult = await usersResponse.Content.ReadFromJsonAsync<ApiResponse<List<AdminUserDto>>>();
        
        if (usersResult?.Data == null || !usersResult.Data.Any())
        {
            return;
        }

        var regularUser = usersResult.Data.FirstOrDefault(u => 
            u.Roles != null && u.Roles.Contains("User") && !u.Roles.Contains("Admin"));
        
        if (regularUser == null) return;

        // Act
        var response = await _client.PutAsync(
            $"/api/admin/users/{regularUser.Id}/role?role=Admin", 
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserRole_InvalidRole_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var usersResponse = await _client.GetAsync("/api/admin/users");
        var usersResult = await usersResponse.Content.ReadFromJsonAsync<ApiResponse<List<AdminUserDto>>>();
        
        if (usersResult?.Data == null || !usersResult.Data.Any())
        {
            return;
        }

        var user = usersResult.Data.First();

        // Act
        var response = await _client.PutAsync(
            $"/api/admin/users/{user.Id}/role?role=InvalidRole", 
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUsers_ResponseStructure_ShouldBeValid()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/users");
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AdminUserDto>>>();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.Should().AllSatisfy(user =>
        {
            user.Id.Should().NotBeNullOrEmpty();
            user.Email.Should().NotBeNullOrEmpty();
            user.Roles.Should().NotBeNull();
        });
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var loginDto = new LoginDto
        {
            Email = "admin@gearstore.com",
            Password = "Admin@123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        return result!.Data!.Token;
    }

    private async Task<string> GetUserTokenAsync()
    {
        var registerDto = new RegisterDto
        {
            Email = $"user{Guid.NewGuid()}@test.com",
            Password = "User@123",
            FirstName = "Test",
            LastName = "User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        return result!.Data!.Token;
    }
}
