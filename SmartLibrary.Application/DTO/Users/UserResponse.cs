namespace SmartLibrary.Application.DTO.Users
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public int MembershipTypeId { get; set; }
    }
}