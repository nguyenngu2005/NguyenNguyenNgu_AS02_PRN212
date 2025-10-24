using System.Linq;
using System.Threading.Tasks;
using Business.Dtos;

namespace Business.Abstractions
{
    public interface IRoomService
    {
        IQueryable<RoomDto> Search(string? keyword);
        Task<RoomDto?> GetAsync(int id);
        Task<(bool ok, string? error)> CreateAsync(RoomDto dto);
        Task<(bool ok, string? error)> UpdateAsync(RoomDto dto);
        Task<(bool ok, string? error)> DeleteAsync(int id); 
    }
}
