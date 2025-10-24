using Business.Enums;

namespace Business.Dtos
{
    public class RoomDto
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = "";
        public string? RoomDetailDescription { get; set; }
        public int? RoomMaxCapacity { get; set; }
        public int RoomTypeID { get; set; }
        public RoomStatus? RoomStatus { get; set; } = Enums.RoomStatus.Available;
        public decimal? RoomPricePerDay { get; set; }
        public string? RoomTypeName { get; set; } 
    }
}
