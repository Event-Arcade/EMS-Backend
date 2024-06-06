using EMS.BACKEND.API.Contracts;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.DTOs;

namespace Contracts
{
    public interface IFeedbackRepository : IBaseRepository<FeedBackResponseDTO, FeedBackRequestDTO>
    {

    }
}