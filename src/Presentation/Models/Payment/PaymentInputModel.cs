using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Payment
{
    public class PaymentInputModel
    {
        [Required(ErrorMessage ="Amount is required")]
        [Range(5000, long.MaxValue, ErrorMessage = "Price should be bigger than 5000")]
        public long Amount { get; set; }
    }
}
