using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class RoomType
    {
        public int RoomTypeID { get; set; } 
        public string RoomTypeName { get; set; } = null!;
        public string? TypeDescription { get; set; }
        public string? TypeNote { get; set; }

        public ICollection<RoomInformation> Rooms { get; set; } = new List<RoomInformation>();
    }
}
