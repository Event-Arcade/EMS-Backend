using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.Models;
using EMS.BACKEND.API.DTOs.ResponseDTOs;

namespace Contracts
{
    public interface IFeedbackRepository : IBaseRepository<FeedBack, FeedBack>
    {
        Task<BaseResponseDTO<IEnumerable<FeedBack>>> GetFeedBacksByShopId(string shopId);
        Task<BaseResponseDTO<IEnumerable<FeedBack>>> GetFeedBacksByServiceId(string serviceId);
    }
}