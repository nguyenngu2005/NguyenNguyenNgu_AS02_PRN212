using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class RoomInformation
    {
        public int RoomID { get; set; } 
        public string RoomNumber { get; set; } = null!;
        public string? RoomDetailDescription { get; set; }
        public int? RoomMaxCapacity { get; set; }
        public int RoomTypeID { get; set; }
        public byte? RoomStatus { get; set; } 
        public decimal? RoomPricePerDay { get; set; } 

        public RoomType RoomType { get; set; } = null!;
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
}
