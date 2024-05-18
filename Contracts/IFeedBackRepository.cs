using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Models;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs;

namespace Contracts
{
    public interface IFeedbackRepository : IBaseRepository<FeedBackResponseDTO, FeedBackRequestDTO>
    {
        // TODO: Try to do implement otherwise delete these two methds
        Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> GetFeedBacksByShopId(string shopId);
        Task<BaseResponseDTO<IEnumerable<FeedBackResponseDTO>>> GetFeedBacksByServiceId(string serviceId);
    }
}