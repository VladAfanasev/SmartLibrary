namespace SmartLibrary.Domain.Interfaces;

public interface IUser
{
    int Id { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    string PasswordHash { get; set; }
    bool IsActive { get; set; }
    DateTime RegistrationDate { get; set; }
    DateTime RenewalDate { get; set; }
    int MembershipTypeID { get; set; }
}