using System;
using System.Collections.Generic;
using Business.Enums;

namespace Business.Dtos
{
    public class ReservationDto
    {
        public int BookingReservationID { get; set; } 
        public DateOnly? BookingDate { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public BookingStatus? BookingStatus { get; set; } = Enums.BookingStatus.Pending;
        public decimal? TotalPrice { get; set; }
        public List<BookingDetailDto> Details { get; set; } = new();
    }
}
