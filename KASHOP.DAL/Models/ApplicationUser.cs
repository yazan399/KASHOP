using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.DAL.Data;
using Microsoft.AspNetCore.Identity;

namespace KASHOP.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
           public string FullName { get; set; }
        public string? City { get; set; } // ? : nullable
        public string? Street { get; set; }
    }
}
