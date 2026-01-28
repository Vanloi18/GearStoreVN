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

public class AdminOrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public AdminOrdersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrders_WithAdminRole_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AdminOrderDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrders_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/admin/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOrders_WithUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/admin/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateOrderStatus_ValidTransition_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var ordersResponse = await _client.GetAsync("/api/admin/orders");
        var ordersResult = await ordersResponse.Content.ReadFromJsonAsync<ApiResponse<List<AdminOrderDto>>>();
        
        if (ordersResult?.Data == null || !ordersResult.Data.Any())
        {
            return;
        }

        var pendingOrder = ordersResult.Data.FirstOrDefault(o => o.Status == 1);
        if (pendingOrder == null) return;

        // Act
        var response = await _client.PutAsync(
            $"/api/admin/orders/{pendingOrder.Id}/status?status=2", 
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStatus_InvalidTransition_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var ordersResponse = await _client.GetAsync("/api/admin/orders");
        var ordersResult = await ordersResponse.Content.ReadFromJsonAsync<ApiResponse<List<AdminOrderDto>>>();
        
        if (ordersResult?.Data == null || !ordersResult.Data.Any())
        {
            return;
        }

        var completedOrder = ordersResult.Data.FirstOrDefault(o => o.Status == 4);
        if (completedOrder == null) return;

        // Act
        var response = await _client.PutAsync(
            $"/api/admin/orders/{completedOrder.Id}/status?status=1", 
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
