using Microsoft.AspNetCore.Identity;

namespace TrainingManagement.Auth.Models;

public class AppUser : IdentityUser
{

    private static AppUser Create(string userName, string employeeCode, string firstName, string lastName, string middleName, string company) 
        => new(userName, employeeCode, firstName, lastName, middleName, company);

    private AppUser(string userName,string employeeCode, string firstName,string lastName,string middleName,string company) : base(userName)
    {
        EmployeeCode = employeeCode;
        Company = company;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;

    }
    public AppUser()
    {

    }


    public string EmployeeCode { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string Company { get; set; } = default!;
}
