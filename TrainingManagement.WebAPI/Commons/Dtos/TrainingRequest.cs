using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.WebAPI.Commons.Dtos;

public class TrainingCenterRequest
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Code is required.")]
    public string Code { get; set; } = default!;

    [Required(ErrorMessage = "Email is required.")]
     [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = default!;

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; } = default!;

    [Required(ErrorMessage = "Municipality is required.")]
    public string Municipality { get; set; } = default!;

    [Required(ErrorMessage = "Contact number is required.")]
    public string ContactNo { get; set; } = default!;
}

