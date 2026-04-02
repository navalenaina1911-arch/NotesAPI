namespace Notes.Validators
{
    using FluentValidation;
    using Notes.DTO.ResponseDto;

    public class UpdateNoteDtoValidator : AbstractValidator<UpdateNoteDto>
    {
        public UpdateNoteDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(50).WithMessage("Title must not exceed 50 characters.");

            RuleFor(x => x.Content)
                .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage("UpdatedBy is required.")
                .MaximumLength(50).WithMessage("UpdatedBy must not exceed 50 characters.");
        }
    }

}
