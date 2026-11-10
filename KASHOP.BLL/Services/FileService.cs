using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http; 
using System.Threading.Tasks;
using KASHOP.BLL.Services;
using KASHOP.DAL.Dto;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CloudinaryDotNet.Actions;

namespace KASHOP.BLL.Services
{
    public class FileService : IFileService

    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<FileService> _logger;
        const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            _logger = logger;

            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException(
                    "CloudinarySettings are missing or incomplete. Configure CloudName, ApiKey, and ApiSecret.");
            }

            var account = new Account(
                cloudName,
                apiKey,
                apiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<Result<FileUploadResult>> UploadAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return Result<FileUploadResult>.Fail("No file was provided.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return Result<FileUploadResult>.Fail("File type not allowed.");
            }

            if (file.Length > MaxFileSize)
            {
                return Result<FileUploadResult>.Fail("File size exceeds the limit.");
            }




            using (var stream = file.OpenReadStream())
            {
              var uploadParams = new ImageUploadParams
              {
                  File = new FileDescription(file.FileName, stream),
                  Folder = "KASHOP14"
              };



                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                _logger.LogInformation(
                    "Cloudinary upload response: StatusCode={StatusCode}, PublicId={PublicId}, SecureUrl={SecureUrl}, Url={Url}, Error={Error}",
                    uploadResult.StatusCode,
                    uploadResult.PublicId,
                    uploadResult.SecureUrl,
                    uploadResult.Url,
                    uploadResult.Error?.Message);

                if(uploadResult.Error is not null)
                {
                    return Result<FileUploadResult>.Fail($"File upload failed: {uploadResult.Error.Message}");
                }

                if (uploadResult.SecureUrl is null)
                {
                    _logger.LogError(
                        "Cloudinary upload returned no secure URL for file {FileName} ({FileLength} bytes). Public ID: {PublicId}",
                        file.FileName,
                        file.Length,
                        uploadResult.PublicId);
                    return Result<FileUploadResult>.Fail("File upload did not return a secure URL.");
                }

                return Result<FileUploadResult>.Ok(data: new FileUploadResult
                {
                    Url = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId
                });

            }

           
        }

        public async Task<Result<bool>> Delete(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = _cloudinary.DestroyAsync(deletionParams);
            if (result.Result.Error is not null)
            {
                _logger.LogError(
                    "Cloudinary deletion failed for PublicId={PublicId}. Error: {Error}",
                    publicId,
                    result.Result.Error.Message);

                return Result<bool>.Fail($"File deletion failed: {result.Result.Error.Message}");
            }
            return Result<bool>.Ok(true);
        }
    }
}
