using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public class PackageRepository(IServiceScopeFactory scopeFactory, IUserAccountRepository accountRepository) : IPackageRepository
    {
        public Task<BaseResponseDTO> CreateAsync(PackageRequestDTO entity)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<IEnumerable<Package>>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<Package>> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> UpdateAsync(PackageRequestDTO entity)
        {
            throw new NotImplementedException();
        }
    }
}
