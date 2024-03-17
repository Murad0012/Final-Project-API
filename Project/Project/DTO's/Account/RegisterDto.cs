using FluentValidation;

namespace Project.DTO_s.Account
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public class RegisterDtoValidator : AbstractValidator<RegisterDto>
        {
            public RegisterDtoValidator()
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()
                    .WithMessage("User name required !");

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Password required !");

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("User name required !");

                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Password required !");
            }
        }
    }
}
