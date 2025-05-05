using Ecommerce.Shared.Common;
using Ecommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Infrastructure.Data;
using System.Security.Claims;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/images")]
public class UploadController : ControllerBase
{
  private readonly ISupabaseStorageService _imageService;
  private readonly AppDbContext _dbContext;

  public UploadController(ISupabaseStorageService imageService, AppDbContext dbContext)
  {
    _imageService = imageService;
    _dbContext = dbContext;
  }

  [HttpPost("upload-product/{productId}")]
  // [Authorize(Roles = "Admin")] // Chỉ admin được upload hình sản phẩm
  public async Task<IActionResult> UploadProductImages(Guid productId, List<IFormFile> files)
  {
    if (files == null || files.Count == 0)
      return BadRequest(ApiResponse<string>.Fail("No files uploaded."));

    // Kiểm tra sản phẩm tồn tại
    var product = await _dbContext.Products.FindAsync(productId);
    if (product == null)
      return NotFound(ApiResponse<string>.Fail("Product not found."));

    // Upload hình ảnh lên Supabase
    var fileInputs = files.Select(f => (f.OpenReadStream(), f.FileName, f.ContentType)).ToList();
    var imageUrls = await _imageService.UploadProductImagesAsync(fileInputs, productId);

    // Lưu vào bảng ProductImage
    var productImages = imageUrls.Select((url, index) => new Ecommerce.Domain.Entities.ProductImage
    {
      Id = Guid.NewGuid(),
      ProductId = productId,
      ImageUrl = url,
      IsPrimary = index == 0, // Ảnh đầu tiên là ảnh chính
      CreatedDate = DateTime.UtcNow
    }).ToList();

    await _dbContext.ProductImages.AddRangeAsync(productImages);
    await _dbContext.SaveChangesAsync();

    var imageResult = new { images = imageUrls };
    return Ok(ApiResponse<object>.Success(imageResult, "Product images uploaded successfully."));
  }

  [HttpPost("upload-avatar")]
  public async Task<IActionResult> UploadAvatar(IFormFile file)
  {
    if (file == null || file.Length == 0)
      return BadRequest(ApiResponse<string>.Fail("No file uploaded."));

    // Lấy userId từ token
    // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userId = GetUserId();
    if (string.IsNullOrEmpty(userId))
      return Unauthorized(ApiResponse<string>.Fail("User not authenticated."));

    // Upload avatar lên Supabase
    using var stream = file.OpenReadStream();
    var avatarUrl = await _imageService.UploadAvatarImageAsync(stream, file.FileName, file.ContentType, userId);

    // Cập nhật AvatarUrl trong ApplicationUser
    var user = await _dbContext.Users.FindAsync(userId);
    if (user == null)
      return NotFound(ApiResponse<string>.Fail("User not found."));

    user.AvatarUrl = avatarUrl;
    _dbContext.Users.Update(user);
    await _dbContext.SaveChangesAsync();

    var avatarResult = new { avatarUrl = avatarUrl };
    return Ok(ApiResponse<object>.Success(avatarResult, "Avatar uploaded successfully."));
  }

  private string GetUserId()
  {
    // return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    return "a2ad953e-1482-4cf2-9b94-41c88aeb90e3";
  }
}
