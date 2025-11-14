using System.ComponentModel.DataAnnotations;

namespace Alis.Progressive.SandboxApp.Models;

public class PriceValidationViewModel
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name must not exceed 100 characters")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
    [Display(Name = "Price")]
    public decimal? Price { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = "Discount must be between $0.01 and $999,999.99")]
    [Display(Name = "Discount Amount")]
    public decimal? Discount { get; set; }
}

