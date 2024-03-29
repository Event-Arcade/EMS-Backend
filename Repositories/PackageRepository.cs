using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        public Task<PackageResponse> CreatePackage(PackageRequestDTO packageRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeletePackage(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PackageListResponse> GetAllPackages()
        {
            throw new NotImplementedException();
        }

        public Task<PackageResponse> GetPackageById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PackageResponse> UpdatePackage(PackageRequestDTO packageRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
