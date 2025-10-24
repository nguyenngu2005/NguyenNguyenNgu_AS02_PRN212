using System;
using System.Collections.Generic;
using System.Linq;
using Business.Abstractions;
using Business.Dtos;
using Business.Enums;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;
        public ReportService(IUnitOfWork uow) => _uow = uow;

        public IEnumerable<ReservationDto> GetReservations(DateOnly from, DateOnly to, bool sortByBookingDateDesc = true)
        {
            // KHÔNG chain Include rồi gán vào biến kiểu "IQueryable<...>"
            // Làm tách bước để giữ type IQueryable
            var q = _uow.Reservations.Query().AsQueryable();
            q = q.Include(r => r.Customer);
            q = q.Include(r => r.BookingDetails);

            q = q.Where(r => r.BookingDate >= from && r.BookingDate <= to);
            q = sortByBookingDateDesc ? q.OrderByDescending(r => r.BookingDate)
                                      : q.OrderBy(r => r.BookingDate);

            return q.Select(r => new ReservationDto
            {
                BookingReservationID = r.BookingReservationID,
                BookingDate = r.BookingDate,
                CustomerID = r.CustomerID,
                CustomerName = r.Customer.CustomerFullName,
                BookingStatus = (BookingStatus?)(r.BookingStatus ?? 0),
                TotalPrice = r.TotalPrice,
                Details = r.BookingDetails.Select(d => new BookingDetailDto
                {
                    RoomID = d.RoomID,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    ActualPrice = d.ActualPrice
                }).ToList()
            }).ToList();
        }
    }
}
