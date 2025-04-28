using Ecommerce.Application.DTOs;
using Ecommerce.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/customers")]
// [Authorize(Roles = "Admin")]
public class AdminCustomerController : ControllerBase
{
  private readonly HttpClient _httpClient;
  private readonly string _authServerBaseUrl;

  public AdminCustomerController(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _authServerBaseUrl = configuration["AuthorizationServer:BaseUrl"] ?? "https://localhost:5000";
  }

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
  {
    if (pageIndex < 1 || pageSize <= 0)
      return BadRequest(ApiResponse<string>.Fail("Invalid paging parameters."));

    var response = await _httpClient.GetAsync($"{_authServerBaseUrl}/api/users?pageIndex={pageIndex}&pageSize={pageSize}");
    if (!response.IsSuccessStatusCode)
      return BadRequest(ApiResponse<string>.Fail("Failed to retrieve customers."));

    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<CustomerDto>>>();
    if (apiResponse?.Status != true)
      return BadRequest(ApiResponse<string>.Fail(apiResponse.Message));

    return Ok(ApiResponse<PagedResult<CustomerDto>>.Success(apiResponse.Data, apiResponse.Message));
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
  {
    if (string.IsNullOrEmpty(id))
      return BadRequest(ApiResponse<string>.Fail("Customer ID is required."));

    var response = await _httpClient.GetAsync($"{_authServerBaseUrl}/api/users/{id}");
    if (!response.IsSuccessStatusCode)
      return NotFound(ApiResponse<string>.Fail("Customer not found."));

    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
    if (apiResponse?.Status != true)
      return NotFound(ApiResponse<string>.Fail(apiResponse.Message));

    return Ok(ApiResponse<CustomerDto>.Success(apiResponse.Data, apiResponse.Message));
  }
}
