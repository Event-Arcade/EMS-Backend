using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IServiceRepository
    {
        Task<ServiceListResponse> GetAllServices();
        Task<ServiceListResponse> GetServicesByShopId(string shopId);
        Task<ServiceResponse> GetServiceById(string id);
        Task<ServiceResponse> Create(ServiceRequestDTO serviceRequestDTO);
        Task<ServiceResponse> Update(ServiceRequestDTO serviceRequestDTO);
        Task<ServiceResponse> Delete(string id);
    }
}
