using LoadBalancer.DAL.Validation.Utils;
using System.ComponentModel.DataAnnotations;

namespace LoadBalancer.DAL.Entities
{
    public class TrigonometryCalculation
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required(ErrorMessage = "Accuracy is required")]
        [Range(Constants.ACCURACY_MIN_VALUE, Constants.ACCURACY_MAX_VALUE, ErrorMessage = "Accuracy must be [1000; 100000]")]
        public int Accuracy { get; set; }

        [Range(Constants.X_MIN_VALUE, Constants.X_MAX_VALUE, ErrorMessage = "XForSin must be [-10000; 10000]")]
        public double? XForSin { get; set; }

        [Range(Constants.X_MIN_VALUE, Constants.X_MAX_VALUE, ErrorMessage = "XForCos must be [-10000; 10000]")]
        public double? XForCos { get; set; }

        public double? SinResult { get; set; }

        public double? CosResult { get; set; }

        public DateTime CalculationDate { get; set; } = DateTime.UtcNow;
    }
}
