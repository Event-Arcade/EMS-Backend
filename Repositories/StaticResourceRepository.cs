using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Controllers
{
    public class StaticResourceRepository(IServiceProvider serviceProvider, ICloudProviderRepository cloudProvider, IConfiguration configuration) : IStaticResourceRepository
    {
        public Task<BaseResponseDTO> CreateAsync(AdminStaticResource entity)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BaseResponseDTO<AdminStaticResource>>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO<AdminStaticResource>> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseDTO> UpdateAsync(AdminStaticResource entity)
        {
            throw new NotImplementedException();
        }
    }
}