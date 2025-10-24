using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class BookingDetail
    {
        public int BookingReservationID { get; set; }
        public int RoomID { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal? ActualPrice { get; set; }

        public BookingReservation BookingReservation { get; set; } = null!;
        public RoomInformation Room { get; set; } = null!;
    }
}
