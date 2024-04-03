using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using SharedClassLibrary.Contracts;

namespace EMS.BACKEND.API.Repositories
{
    public async class PackageRepository(IServiceScopeFactory scopeFactory, IUserAccountRepository accountRepository) : IPackageRepository
    {
        public async Task<BaseResponseDTO> CreateAsync(PackageRequestDTO entity)
        {
            //Check entity is null
            if (entity == null)
            {
                return new BaseResponseDTO
                {
                    Message = "Package is null",
                    Flag = false
                };
            }

            //Check if the package already exists
            var package = await FindByIdAsync(entity.Id);
            if (package.Data != null)
            {
                return new BaseResponseDTO
                {
                    Message = "Package already exists",
                    Flag = false
                };
            }

            //


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
