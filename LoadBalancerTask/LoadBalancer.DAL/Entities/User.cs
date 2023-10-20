using LoadBalancer.DAL.Validation.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LoadBalancer.DAL.Entities
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(Constants.USERNAME_MAX_LENGTH, ErrorMessage = "Too long UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(Constants.EMAIL_MAX_LENGTH, ErrorMessage = "Too long email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [JsonIgnore]
        [MaxLength(Constants.PASSWORD_MAX_LENGTH, ErrorMessage = "Too long password")]
        public string Password { get; set; }
    }
}
