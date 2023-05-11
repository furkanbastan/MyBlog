using FluentValidation;
using MyBlog.Entity.Entities;

namespace MyBlog.Service.FluentValidations;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3).WithMessage("İsim boyutu 3'ten küçük olamaz..")
            .MaximumLength(100).WithMessage("İsim boyutu yanlış!")
            .WithName("İsim");
    }
}
