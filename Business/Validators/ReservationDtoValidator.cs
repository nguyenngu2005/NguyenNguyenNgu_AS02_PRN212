using Business.Dtos;
using FluentValidation;
using System.Linq;

namespace Business.Validators
{
    public class ReservationDtoValidator : AbstractValidator<ReservationDto>
    {
        public ReservationDtoValidator()
        {
            RuleFor(x => x.BookingReservationID).GreaterThan(0);
            RuleFor(x => x.CustomerID).GreaterThan(0);
            RuleForEach(x => x.Details).ChildRules(d =>
            {
                d.RuleFor(m => m.RoomID).GreaterThan(0);
                d.RuleFor(m => m.EndDate).GreaterThanOrEqualTo(m => m.StartDate);
                d.RuleFor(m => m.ActualPrice).GreaterThanOrEqualTo(0).When(m => m.ActualPrice.HasValue);
            });

            RuleFor(x => x.Details)
                .Must(list => list.GroupBy(i => i.RoomID).All(g => g.Count() == 1))
                .WithMessage("A room cannot appear twice in a single reservation.");
        }
    }
}
