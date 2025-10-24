using System.Linq;
using System.Threading.Tasks;
using Business.Dtos;

namespace Business.Abstractions
{
    public interface ICustomerService
    {
        IQueryable<CustomerDto> Search(string? keyword);
        Task<CustomerDto?> GetAsync(int id);
        Task<(bool ok, string? error)> CreateAsync(CustomerDto dto);
        Task<(bool ok, string? error)> UpdateAsync(CustomerDto dto);
        Task<(bool ok, string? error)> DeleteAsync(int id); // cascade OK
        Task<bool> EmailExistsAsync(string email, int? ignoreId = null);
    }
}
