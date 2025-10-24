using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class BookingReservation
    {
        public int BookingReservationID { get; set; } 
        public DateOnly? BookingDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public int CustomerID { get; set; }
        public byte? BookingStatus { get; set; }

        public Customer Customer { get; set; } = null!;
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
}
