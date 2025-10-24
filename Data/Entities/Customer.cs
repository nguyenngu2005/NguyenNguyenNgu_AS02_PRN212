using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string? CustomerFullName { get; set; }
        public string? Telephone { get; set; }
        public string EmailAddress { get; set; } = null!;
        public DateOnly? CustomerBirthday { get; set; }
        public byte? CustomerStatus { get; set; } 
        public string? Password { get; set; }

        public ICollection<BookingReservation> BookingReservations { get; set; } = new List<BookingReservation>();
    }
}
