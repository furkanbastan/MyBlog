using FluentValidation;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Service.FluentValidations;

public class AppUserValidator : AbstractValidator<AppUser>
{
    public AppUserValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3).WithMessage("İsim boyutu 3'ten küçük olamaz..")
            .MaximumLength(100).WithMessage("İsim boyutu yanlış!")
            .WithName("İsim");
    }
}
