using Microsoft.AspNetCore.Identity;
using TrainingManagement.Auth.Commons.Enums;

namespace TrainingManagement.Auth.Models;

public class AppUser : IdentityUser
{

    public static AppUser Create(string userName,string email, string userCode, string fullName, Guid trainingCenterId) 
        => new(userName, email, userCode, fullName, trainingCenterId);

    private AppUser(string userName,string email,string userCode, string fullName,Guid trainingCenterId) : base(userName)
    {
        Email = email;
        UserCode = userCode;
        TainingCenterId = trainingCenterId;
        FullName = fullName;

    }
    public AppUser()
    {

    }

    public UserType UserType { get; set; }
    public string UserCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public Guid TainingCenterId { get; set; }
}
