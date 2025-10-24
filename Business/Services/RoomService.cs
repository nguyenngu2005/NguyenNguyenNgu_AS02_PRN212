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
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _uow;
        private readonly RoomDtoValidator _validator = new();

        public RoomService(IUnitOfWork uow) => _uow = uow;

        public IQueryable<RoomDto> Search(string? keyword)
        {
            // tách bước include để giữ type IQueryable
            var baseQuery = _uow.Rooms.Query().AsQueryable();
            baseQuery = baseQuery.Include(r => r.RoomType);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                baseQuery = baseQuery.Where(r =>
                    r.RoomNumber.Contains(k) ||
                    (r.RoomDetailDescription ?? "").Contains(k) ||
                    r.RoomType.RoomTypeName.Contains(k));
            }

            return baseQuery.OrderBy(r => r.RoomNumber)
                            .Select(r => new RoomDto
                            {
                                RoomID = r.RoomID,
                                RoomNumber = r.RoomNumber,
                                RoomDetailDescription = r.RoomDetailDescription,
                                RoomMaxCapacity = r.RoomMaxCapacity,
                                RoomTypeID = r.RoomTypeID,
                                RoomStatus = (Business.Enums.RoomStatus?)(r.RoomStatus ?? 1),
                                RoomPricePerDay = r.RoomPricePerDay,
                                RoomTypeName = r.RoomType.RoomTypeName
                            });
        }
        public async Task<RoomDto?> GetAsync(int id)
        {
            var r = await _uow.Rooms.Query().Include(x => x.RoomType).FirstOrDefaultAsync(x => x.RoomID == id);
            if (r == null) return null;
            return new RoomDto
            {
                RoomID = r.RoomID,
                RoomNumber = r.RoomNumber,
                RoomDetailDescription = r.RoomDetailDescription,
                RoomMaxCapacity = r.RoomMaxCapacity,
                RoomTypeID = r.RoomTypeID,
                RoomStatus = (RoomStatus?)(r.RoomStatus ?? (byte)RoomStatus.Available),
                RoomPricePerDay = r.RoomPricePerDay,
                RoomTypeName = r.RoomType.RoomTypeName
            };
        }

        public async Task<(bool ok, string? error)> CreateAsync(RoomDto dto)
        {
            var vr = _validator.Validate(dto);
            if (!vr.IsValid) return (false, vr.Errors.First().ErrorMessage);

            var entity = new RoomInformation
            {
                RoomNumber = dto.RoomNumber,
                RoomDetailDescription = dto.RoomDetailDescription,
                RoomMaxCapacity = dto.RoomMaxCapacity,
                RoomTypeID = dto.RoomTypeID,
                RoomStatus = (byte?)dto.RoomStatus ?? (byte)RoomStatus.Available,
                RoomPricePerDay = dto.RoomPricePerDay
            };
            await _uow.Rooms.AddAsync(entity);
            await _uow.SaveChangesAsync();
            dto.RoomID = entity.RoomID;
            return (true, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(RoomDto dto)
        {
            var vr = _validator.Validate(dto);
            if (!vr.IsValid) return (false, vr.Errors.First().ErrorMessage);

            var r = await _uow.Rooms.GetAsync(dto.RoomID);
            if (r == null) return (false, "Room not found.");

            r.RoomNumber = dto.RoomNumber;
            r.RoomDetailDescription = dto.RoomDetailDescription;
            r.RoomMaxCapacity = dto.RoomMaxCapacity;
            r.RoomTypeID = dto.RoomTypeID;
            r.RoomStatus = (byte?)dto.RoomStatus ?? (byte)RoomStatus.Available;
            r.RoomPricePerDay = dto.RoomPricePerDay;

            _uow.Rooms.Update(r);
            await _uow.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int id)
        {
            var r = await _uow.Rooms.GetAsync(id);
            if (r == null) return (false, "Room not found.");

            var hasTransactions = await _uow.BookingDetails.Query().AnyAsync(bd => bd.RoomID == id);

            if (hasTransactions)
            {
                r.RoomStatus = (byte)RoomStatus.Inactive;
                _uow.Rooms.Update(r);
            }
            else
            {
                _uow.Rooms.Remove(r);
            }

            await _uow.SaveChangesAsync();
            return (true, null);
        }
    }
}
