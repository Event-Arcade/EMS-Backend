using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface IServiceRepository
    {
        Task<BaseResponseDTO<List<Service>>> GetAllServices();
        Task<BaseResponseDTO<List<Service>>> GetServicesByShopId(string shopId);
        Task<BaseResponseDTO<Service>> GetServiceById(string id);
        Task<BaseResponseDTO> Create(ServiceRequestDTO serviceRequestDTO);
        Task<BaseResponseDTO> Update(ServiceRequestDTO serviceRequestDTO);
        Task<BaseResponseDTO> Delete(string id);
    }
}
