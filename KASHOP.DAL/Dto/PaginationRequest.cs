using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class PaginationRequest
    {
        [Range(1, 1_000_000, ErrorMessage = "PageNumber must be between 1 and 1,000,000.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 20;

        [StringLength(100, ErrorMessage = "Search cannot exceed 100 characters.")]
        public string? Search    { get; set; }

        public bool SortDescending { get; set; }
    }
}
