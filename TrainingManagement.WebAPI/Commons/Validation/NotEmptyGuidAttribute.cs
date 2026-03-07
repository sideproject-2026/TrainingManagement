using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.WebAPI.Commons.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute() : base("The GUID value cannot be empty.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            // Let [Required] handle null checks.
            return true;
        }

        if (value is Guid guid)
        {
            return guid != Guid.Empty;
        }

        return false;
    }
}
