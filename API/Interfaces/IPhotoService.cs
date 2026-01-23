using CloudinaryDotNet.Actions;

namespace API.Interfaces;
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }

    // IFormFile represents a file sent with Http request.