using Business.Enums;
using System;

namespace Business.Dtos
{
    public class CustomerDto
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; } = "";
        public string Telephone { get; set; } = "";
        public string Email { get; set; } = "";
        public DateOnly? Birthday { get; set; }
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;
        public string Password { get; set; } = "";
    }
}
