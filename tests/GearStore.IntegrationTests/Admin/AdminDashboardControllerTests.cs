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

public class AdminDashboardControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public AdminDashboardControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboardStats_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDashboardStats_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/admin/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDashboardStats_WithUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDashboardStats_ResponseStructure_ShouldBeValid()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/dashboard");
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsDto>>();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.TotalUsers.Should().BeGreaterOrEqualTo(0);
        result.Data.TotalOrders.Should().BeGreaterOrEqualTo(0);
        result.Data.TotalProducts.Should().BeGreaterOrEqualTo(0);
        result.Data.TotalRevenue.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetDashboardStats_TotalRevenue_ShouldBeDecimal()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/dashboard");
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsDto>>();

        // Assert
        result!.Data!.TotalRevenue.Should().BeOfType<decimal>();
    }

    [Fact]
    public async Task GetDashboardStats_MultipleRequests_ShouldReturnConsistentData()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response1 = await _client.GetAsync("/api/admin/dashboard");
        var result1 = await response1.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsDto>>();

        var response2 = await _client.GetAsync("/api/admin/dashboard");
        var result2 = await response2.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsDto>>();

        // Assert
        result1!.Data!.TotalUsers.Should().Be(result2!.Data!.TotalUsers);
        result1.Data.TotalProducts.Should().Be(result2.Data.TotalProducts);
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
