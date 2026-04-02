namespace Notes.Validators
{
    using FluentValidation;
    using Notes.DTO.ResponseDto;

    public class CreateNoteDtoValidator : AbstractValidator<CreateNoteDto>
    {
        public CreateNoteDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(50).WithMessage("Title must not exceed 50 characters.");

            RuleFor(x => x.Content)
                .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.")
                .MaximumLength(50).WithMessage("CreatedBy must not exceed 50 characters.");
        }
    }

}
