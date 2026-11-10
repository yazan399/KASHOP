using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface IFileService
    {
       public Task<Result<bool>> Delete(string publicId);
        public Task<Result<FileUploadResult>> UploadAsync(IFormFile file);

    }
}
