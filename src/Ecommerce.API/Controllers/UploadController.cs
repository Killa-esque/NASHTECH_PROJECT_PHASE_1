using Ecommerce.Shared.Common;
using Ecommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/images")]
public class UploadController : ControllerBase
{
  private readonly ISupabaseStorageService _imageService;

  public UploadController(ISupabaseStorageService imageService)
  {
    _imageService = imageService;
  }

  [HttpPost("upload-product/{productId}")]
  public async Task<IActionResult> UploadProductImages(Guid productId, List<IFormFile> files)
  {
    if (files == null || files.Count == 0)
      return BadRequest(ApiResponse<string>.Fail("No files uploaded."));

    var imageUrls = new List<string>();

    foreach (var file in files)
    {
      using var stream = file.OpenReadStream();
      var imageUrl = await _imageService.UploadProductImageAsync(stream, file.FileName, file.ContentType, productId);
      imageUrls.Add(imageUrl);
    }
    var imageResult = new { images = imageUrls };
    return Ok(ApiResponse<object>.Success(imageResult, "Product Images uploaded."));
  }

  [HttpPost("upload-avatar/{userId}")]
  public async Task<IActionResult> UploadAvatar(string userId, IFormFile file)
  {
    if (file == null || file.Length == 0)
      return BadRequest(ApiResponse<string>.Fail("No file uploaded."));

    using var stream = file.OpenReadStream();
    var avatarUrl = await _imageService.UploadAvatarImageAsync(stream, file.FileName, file.ContentType, userId);

    // Optional: Save avatarUrl to user profile in DB here

    var avatarResult = new { avatarUrl = avatarUrl };
    return Ok(ApiResponse<object>.Success(avatarResult, "Avatar uploaded successfully."));
  }
}
