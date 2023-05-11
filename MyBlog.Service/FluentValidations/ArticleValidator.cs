using FluentValidation;
using MyBlog.Entity.Entities;

namespace MyBlog.Service.FluentValidations;

public class ArticleValidator : AbstractValidator<Article>
{
    public ArticleValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3).WithMessage("Başlık boyutu 3'ten küçük olamaz..")
            .MaximumLength(100).WithMessage("Başlık boyutu yanlış!")
            .WithName("Başlık");
    }
}
