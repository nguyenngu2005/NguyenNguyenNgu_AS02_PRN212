    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Business.Abstractions;
    using Business.Dtos;
    using Business.Enums;
    using Business.Validators;
    using Data.Repositories;
    using Data.Entities;
    using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public sealed class BookingService : IBookingService
    {
        private readonly IUnitOfWork _uow;
        private readonly ReservationDtoValidator _validator = new();

        public BookingService(IUnitOfWork uow) { _uow = uow; }

        public IQueryable<ReservationDto> Search(int? customerId, DateOnly? from, DateOnly? to, string? keyword)
        {
            var q = _uow.Reservations.Query()
                .Include(r => r.Customer)
                .Include(r => r.BookingDetails).ThenInclude(d => d.Room)
                .AsQueryable();

            if (customerId.HasValue) q = q.Where(r => r.CustomerID == customerId.Value);
            if (from.HasValue) q = q.Where(r => r.BookingDate >= from.Value);
            if (to.HasValue) q = q.Where(r => r.BookingDate <= to.Value);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                q = q.Where(r => (r.Customer.CustomerFullName ?? "").Contains(k)
                              || r.BookingReservationID.ToString().Contains(k));
            }

            return q.OrderByDescending(r => r.BookingDate)
                    .Select(r => new ReservationDto
                    {
                        BookingReservationID = r.BookingReservationID,
                        BookingDate = r.BookingDate,
                        CustomerID = r.CustomerID,
                        CustomerName = r.Customer.CustomerFullName,
                        BookingStatus = (BookingStatus?)(r.BookingStatus ?? (byte)BookingStatus.Pending),
                        TotalPrice = r.TotalPrice,
                        Details = r.BookingDetails.Select(d => new BookingDetailDto
                        {
                            RoomID = d.RoomID,
                            StartDate = d.StartDate,
                            EndDate = d.EndDate,
                            ActualPrice = d.ActualPrice,
                            RoomNumber = d.Room.RoomNumber
                        }).ToList()
                    });
        }

        public async Task<ReservationDto?> GetAsync(int id)
        {
            return await Search(null, null, null, id.ToString()).FirstOrDefaultAsync();
        }

        public async Task<(bool ok, string? error)> CreateAsync(ReservationDto dto)
        {
            var vr = _validator.Validate(dto);
            if (!vr.IsValid) return (false, vr.Errors.First().ErrorMessage);

            foreach (var d in dto.Details)
            {
                if (!await RoomAvailableAsync(d.RoomID, d.StartDate, d.EndDate, null))
                    return (false, $"Room {d.RoomID} is not available in selected dates.");
            }

            var total = dto.Details.Sum(d => d.ActualPrice ?? 0);

            var res = new BookingReservation
            {
                BookingReservationID = dto.BookingReservationID, 
                BookingDate = dto.BookingDate ?? DateOnly.FromDateTime(DateTime.Today),
                CustomerID = dto.CustomerID,
                BookingStatus = (byte?)dto.BookingStatus ?? (byte)BookingStatus.Pending,
                TotalPrice = total
            };

            await _uow.Reservations.AddAsync(res);

            foreach (var d in dto.Details)
            {
                await _uow.BookingDetails.AddAsync(new BookingDetail
                {
                    BookingReservationID = res.BookingReservationID,
                    RoomID = d.RoomID,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    ActualPrice = d.ActualPrice
                });
            }

            await _uow.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(ReservationDto dto)
        {
            var vr = _validator.Validate(dto);
            if (!vr.IsValid) return (false, vr.Errors.First().ErrorMessage);

            var res = await _uow.Reservations.Query()
                .Include(r => r.BookingDetails)
                .FirstOrDefaultAsync(r => r.BookingReservationID == dto.BookingReservationID);

            if (res == null) return (false, "Reservation not found.");

            foreach (var d in dto.Details)
            {
                if (!await RoomAvailableAsync(d.RoomID, d.StartDate, d.EndDate, dto.BookingReservationID))
                    return (false, $"Room {d.RoomID} is not available in selected dates.");
            }

            res.BookingDate = dto.BookingDate ?? res.BookingDate;
            res.CustomerID = dto.CustomerID;
            res.BookingStatus = (byte?)dto.BookingStatus ?? res.BookingStatus;
            _uow.BookingDetails.RemoveRange(res.BookingDetails);
            foreach (var d in dto.Details)
            {
                await _uow.BookingDetails.AddAsync(new BookingDetail
                {
                    BookingReservationID = dto.BookingReservationID,
                    RoomID = d.RoomID,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    ActualPrice = d.ActualPrice
                });
            }

            res.TotalPrice = dto.Details.Sum(d => d.ActualPrice ?? 0);
            await _uow.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int id)
        {
            var res = await _uow.Reservations.Query()
                .Include(r => r.BookingDetails)
                .FirstOrDefaultAsync(r => r.BookingReservationID == id);

            if (res == null) return (false, "Reservation not found.");

            _uow.BookingDetails.RemoveRange(res.BookingDetails);
            _uow.Reservations.Remove(res);
            await _uow.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> RoomAvailableAsync(int roomId, DateOnly start, DateOnly end, int? ignoreReservationId = null)
        {
            var q = _uow.BookingDetails.Query().Where(d => d.RoomID == roomId);
            if (ignoreReservationId.HasValue)
                q = q.Where(d => d.BookingReservationID != ignoreReservationId.Value);

            return !await q.AnyAsync(d => start <= d.EndDate && end >= d.StartDate);
        }
    }
}
