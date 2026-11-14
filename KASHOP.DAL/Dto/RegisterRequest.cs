using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class RegisterRequest
    {
        [Required, StringLength(150)] public string FullName { get; set; }
        [Required, EmailAddress, StringLength(256)] public string Email { get; set; }
        [Required, StringLength(100, MinimumLength = 8)] public string Password { get; set; }
        [Required, StringLength(100, MinimumLength = 3)] public string UserName { get; set; }
        [Required, Phone, StringLength(30)] public string PhoneNumber { get; set; }

    }   
}
