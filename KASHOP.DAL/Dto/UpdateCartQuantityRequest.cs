using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto;

public sealed class UpdateCartQuantityRequest
{
    [Range(1, int.MaxValue)]
    public int Count { get; set; }
}
