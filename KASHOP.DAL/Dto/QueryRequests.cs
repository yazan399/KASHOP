using System.ComponentModel.DataAnnotations;
using KASHOP.DAL.Enums;

namespace KASHOP.DAL.Dto;

public enum ProductSortBy { Id, Name, Price, Discount, Quantity, Rate, CreatedOn }
public enum CategorySortBy { Id, Name, CreatedOn }
public enum OrderSortBy { Id, OrderDate, TotalAmount, Status, PaymentStatus }
public enum CartSortBy { ProductName, Price, Count, Total }
public enum UserSortBy { Id, UserName, Email, FullName }

public sealed class ProductQueryRequest : PaginationRequest, IValidatableObject
{
    public int? CategoryId { get; set; }
    [Range(0, double.MaxValue)] public decimal? MinPrice { get; set; }
    [Range(0, double.MaxValue)] public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Id;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice > MaxPrice)
            yield return new ValidationResult("MinPrice cannot be greater than MaxPrice.", [nameof(MinPrice), nameof(MaxPrice)]);
    }
}

public sealed class CategoryQueryRequest : PaginationRequest
{
    public CategorySortBy SortBy { get; set; } = CategorySortBy.Id;
}

public sealed class OrderQueryRequest : PaginationRequest, IValidatableObject
{
    public OrderStatusEnum? Status { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public PaymentMethodEnum? PaymentMethod { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public OrderSortBy SortBy { get; set; } = OrderSortBy.OrderDate;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromDate > ToDate)
            yield return new ValidationResult("FromDate cannot be later than ToDate.", [nameof(FromDate), nameof(ToDate)]);
    }
}

public sealed class CartQueryRequest : PaginationRequest
{
    public CartSortBy SortBy { get; set; } = CartSortBy.ProductName;
}

public sealed class UserQueryRequest : PaginationRequest
{
    public bool? EmailConfirmed { get; set; }
    public UserSortBy SortBy { get; set; } = UserSortBy.UserName;
}
