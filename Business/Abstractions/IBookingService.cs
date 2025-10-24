using System.Linq;
using System.Threading.Tasks;
using Business.Dtos;
using System;

namespace Business.Abstractions
{
    public interface IBookingService
    {
        IQueryable<ReservationDto> Search(int? customerId, DateOnly? from, DateOnly? to, string? keyword);
        Task<ReservationDto?> GetAsync(int id);
        Task<(bool ok, string? error)> CreateAsync(ReservationDto dto);
        Task<(bool ok, string? error)> UpdateAsync(ReservationDto dto);
        Task<(bool ok, string? error)> DeleteAsync(int id);
        Task<bool> RoomAvailableAsync(int roomId, DateOnly start, DateOnly end, int? ignoreReservationId = null);
    }
}
