using FluentValidation;

namespace Project.DTO_s.Account
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public class LoginDtoValidator : AbstractValidator<LoginDto>
        {
            public LoginDtoValidator()
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()
                    .WithMessage("User name required !");

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Password required !");
            }
        }
    }
}
