using HealthNetDb.Entities;
using System.ComponentModel.DataAnnotations;
namespace HealthNet.DTOs
{
    public class UserRegisterRequestDto
    {
        // Details provided by User
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be ONLY digits")] //checking whether the Phone number has only digits
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage = "Role name is required")] 
        public string RoleName { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; } = null!;
        public Users ToUserEntity()
        {
            return new Users
            {
                Name = Name.Trim(),
                Email = Email.Trim().ToLower(),
                Phone = Phone.Trim(),
                Password = Password
            };
        }
    }
}