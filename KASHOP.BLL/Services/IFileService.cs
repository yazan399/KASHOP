using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface IFileService
    {
        public Task<string?> UploadAsync(IFormFile file);
    }
}
