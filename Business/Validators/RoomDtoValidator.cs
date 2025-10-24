using Business.Dtos;
using FluentValidation;

namespace Business.Validators
{
    public class RoomDtoValidator : AbstractValidator<RoomDto>
    {
        public RoomDtoValidator()
        {
            RuleFor(x => x.RoomNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.RoomTypeID).GreaterThan(0);
            RuleFor(x => x.RoomMaxCapacity).GreaterThanOrEqualTo(1).When(x => x.RoomMaxCapacity.HasValue);
            RuleFor(x => x.RoomPricePerDay).GreaterThanOrEqualTo(0).When(x => x.RoomPricePerDay.HasValue);
        }
    }
}
