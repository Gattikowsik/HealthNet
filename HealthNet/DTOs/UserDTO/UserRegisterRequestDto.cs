using HealthNetDb.Entities;

namespace HealthNet.DTOs
{
    public class UserRegisterRequestDto
    {
        // Details provided by User
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string Password { get; set; } = null!;
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