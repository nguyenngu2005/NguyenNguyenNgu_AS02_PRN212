using System;
using System.Collections.Generic;
using Business.Dtos;

namespace Business.Abstractions
{
    public interface IReportService
    {
        IEnumerable<ReservationDto> GetReservations(DateOnly from, DateOnly to, bool sortByBookingDateDesc = true);
    }
}
