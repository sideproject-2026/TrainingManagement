using System.ComponentModel.DataAnnotations;
using TrainingManagement.WebAPI.Commons.Validation;

namespace TrainingManagement.WebAPI.Commons.Dtos;

public record UserRequest(
    [property: Required(ErrorMessage = "Username is required.")]
    string UserName,
    [property: Required(ErrorMessage = "User code is required.")]
    string UserCode,
    [property: Required(ErrorMessage = "Email is required.")]
    string Email,
    [property: Required(ErrorMessage = "Full name is required.")]
    string FullName,
    [property: Required(ErrorMessage = "Training center ID is required.")]
    [property: NotEmptyGuid(ErrorMessage = "Training center ID cannot be empty.")]
    Guid TrainingCenterId,
    [property: Required(ErrorMessage = "Password is required.")]
    string Password,
    [property: Required(ErrorMessage = "Roles are required.")]
    List<string> Roles);
