using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
  private readonly HttpClient _httpClient;
  private readonly string _authServerBaseUrl;

  public UserController(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _authServerBaseUrl = configuration["AuthorizationServer:BaseUrl"] ?? "https://localhost:5000";
  }

  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile()
  {
    // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userId = "5c798509-6862-4972-b85c-dd4d691e01e8";

    if (string.IsNullOrEmpty(userId))
      return Unauthorized(ApiResponse<string>.Fail("User ID not found in token."));

    var response = await _httpClient.GetAsync($"{_authServerBaseUrl}/api/users/{userId}");
    if (!response.IsSuccessStatusCode)
      return NotFound(ApiResponse<string>.Fail("Profile not found."));

    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    if (apiResponse?.Status != true)
      return NotFound(ApiResponse<string>.Fail(apiResponse.Message));

    return Ok(ApiResponse<CustomerDto>.Success(apiResponse.Data, apiResponse.Message));
  }

  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerDto updateDto)
  {
    var userId = "5c798509-6862-4972-b85c-dd4d691e01e8";
    // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
      return Unauthorized(ApiResponse<string>.Fail("User ID not found in token."));

    var response = await _httpClient.PutAsJsonAsync($"{_authServerBaseUrl}/api/users/{userId}", updateDto);
    Console.WriteLine($"Response: {response}");
    if (!response.IsSuccessStatusCode)
      return BadRequest(ApiResponse<string>.Fail("Failed to update profile."));

    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    if (apiResponse?.Status != true)
      return BadRequest(ApiResponse<string>.Fail(apiResponse.Message));

    return Ok(ApiResponse<CustomerDto>.Success(apiResponse.Data, apiResponse.Message));
  }

  [HttpDelete("profile")]
  public async Task<IActionResult> DeleteProfile()
  {
    // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userId = "5c798509-6862-4972-b85c-dd4d691e01e8";
    if (string.IsNullOrEmpty(userId))
      return Unauthorized(ApiResponse<string>.Fail("User ID not found in token."));

    var response = await _httpClient.DeleteAsync($"{_authServerBaseUrl}/api/users/{userId}");
    if (!response.IsSuccessStatusCode)
      return BadRequest(ApiResponse<string>.Fail("Failed to delete profile."));

    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    if (apiResponse?.Status != true)
      return BadRequest(ApiResponse<string>.Fail(apiResponse.Message));

    return Ok(ApiResponse<bool>.Success(apiResponse.Data, apiResponse.Message));
  }
}
