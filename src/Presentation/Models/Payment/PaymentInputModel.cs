using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Payment
{
    public class PaymentInputModel
    {
        [Required(ErrorMessage ="Amount is required")]
        [Range(0, long.MaxValue, ErrorMessage = "Price cannot be negative")]
        public long Amount { get; set; }
    }
}
