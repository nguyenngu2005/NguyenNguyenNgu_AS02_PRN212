using System.Linq;
using System.Threading.Tasks;
using Business.Abstractions;
using Business.Dtos;
using Business.Validators;
using Data.Repositories;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _uow;
        private readonly CustomerDtoValidator _validator = new();

        public CustomerService(IUnitOfWork uow) => _uow = uow;

        public IQueryable<CustomerDto> Search(string? keyword)
        {
            var q = _uow.Customers.Query();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                q = q.Where(c => (c.CustomerFullName ?? "").Contains(keyword)
                              || c.EmailAddress.Contains(keyword)
                              || (c.Telephone ?? "").Contains(keyword));
            }
            return q
                .OrderBy(c => c.CustomerFullName)
                .Select(c => new CustomerDto
                {
                    CustomerID = c.CustomerID,
                    FullName = c.CustomerFullName ?? "",
                    Telephone = c.Telephone ?? "",
                    Email = c.EmailAddress,
                    Birthday = c.CustomerBirthday,
                    Status = (Business.Enums.CustomerStatus)(c.CustomerStatus ?? 1),
                    Password = c.Password ?? ""
                });
        }

        public async Task<CustomerDto?> GetAsync(int id)
        {
            var c = await _uow.Customers.GetAsync(id);
            if (c == null) return null;
            return new CustomerDto
            {
                CustomerID = c.CustomerID,
                FullName = c.CustomerFullName ?? "",
                Telephone = c.Telephone ?? "",
                Email = c.EmailAddress,
                Birthday = c.CustomerBirthday,
                Status = (Business.Enums.CustomerStatus)(c.CustomerStatus ?? 1),
                Password = c.Password ?? ""
            };
        }

        public async Task<bool> EmailExistsAsync(string email, int? ignoreId = null)
        {
            return await _uow.Customers.Query()
                .AnyAsync(c => c.EmailAddress == email && (!ignoreId.HasValue || c.CustomerID != ignoreId.Value));
        }

        public async Task<(bool ok, string? error)> CreateAsync(CustomerDto dto)
        {
            var vr = _validator.Validate(dto);
            if (!vr.IsValid) return (false, vr.Errors.First().ErrorMessage);

            if (await EmailExistsAsync(dto.Email))
                return (false, "Email already exists.");

            var entity = new Customer
            {
                CustomerFullName = dto.FullName,
                Telephone = dto.Telephone,
                EmailAddress = dto.Email,
                CustomerBirthday = dto.Birthday,
                CustomerStatus = (byte)dto.Status,
                Password = dto.Password
            };
            await _uow.Customers.AddAsync(entity);
            await _uow.SaveChangesAsync();
            dto.CustomerID = entity.CustomerID;
            return (true, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(CustomerDto dto)
        {
            var vr = _validator.Validate(dto);
            if (!vr.IsValid) return (false, vr.Errors.First().ErrorMessage);

            var c = await _uow.Customers.GetAsync(dto.CustomerID);
            if (c == null) return (false, "Customer not found.");

            if (await EmailExistsAsync(dto.Email, dto.CustomerID))
                return (false, "Email already exists.");

            c.CustomerFullName = dto.FullName;
            c.Telephone = dto.Telephone;
            c.EmailAddress = dto.Email;
            c.CustomerBirthday = dto.Birthday;
            c.CustomerStatus = (byte)dto.Status;
            c.Password = dto.Password;

            _uow.Customers.Update(c);
            await _uow.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int id)
        {
            var c = await _uow.Customers.GetAsync(id);
            if (c == null) return (false, "Customer not found.");

            _uow.Customers.Remove(c); 
            await _uow.SaveChangesAsync();
            return (true, null);
        }
    }
}
