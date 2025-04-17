namespace SmartLibrary.Application.DTO.Users
{
    public class RegisterUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int MembershipTypeId { get; set; } = 1; // Default value
    }
}