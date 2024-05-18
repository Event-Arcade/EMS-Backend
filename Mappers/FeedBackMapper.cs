using EMS.BACKEND.API.DTOs.ResponseDTOs;
using EMS.BACKEND.API.Models;
namespace EMS.BACKEND.API.DTOs.Mappers
{
    public static class FeedBackMapper
    {
        public static FeedBackResponseDTO ToFeedBackResponseDTO(this FeedBack feedBack, ICollection<string> feedBackStaticResourcesUrls)
        {
            return new FeedBackResponseDTO
            {
                Id = feedBack.Id,
                Comment = feedBack.Comment,
                PostedOn = feedBack.PostedOn,
                Rating = feedBack.Rating,
                ServiceId = feedBack.ServiceId,
                ApplicationUserId = feedBack.ApplicationUserId,
                FeedBackStaticResourcesUrls = feedBackStaticResourcesUrls
            };
        }

        public static FeedBack ToFeedBack(this FeedBackRequestDTO feedBackRequestDTO, string applicationUserId)
        {
            return new FeedBack
            {
                Comment = feedBackRequestDTO.Comment,
                PostedOn = DateTime.Now,
                Rating = feedBackRequestDTO.Rating,
                ServiceId = feedBackRequestDTO.ServiceId,
                ApplicationUserId = applicationUserId
            };
        }
    }
}