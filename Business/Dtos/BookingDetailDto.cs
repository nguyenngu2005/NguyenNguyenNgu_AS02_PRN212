using System;

namespace Business.Dtos
{
    public class BookingDetailDto
    {
        public int RoomID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal? ActualPrice { get; set; }
        public string? RoomNumber { get; set; } 
    }
}
