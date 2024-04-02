using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        public Task<BaseResponseDTO> CreatePackage(PackageRequestDTO packageRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> DeletePackage(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<List<Package>>> GetAllPackages()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> GetPackageById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> UpdatePackage(PackageRequestDTO packageRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
