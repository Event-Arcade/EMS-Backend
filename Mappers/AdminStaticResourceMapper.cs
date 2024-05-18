using EMS.BACKEND.API.DTOs.AdminStaticResource;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Mappers
{
    public static class AdminStaticResourceMapper
    {
        public static AdminStaticResourceResponseDTO ToAdminStaticResourceResponseDTO(this AdminStaticResource adminStaticResource, string url)
        {
            return new AdminStaticResourceResponseDTO
            {
                Id = adminStaticResource.Id,
                Name = adminStaticResource.Name,
                Description = adminStaticResource.Description,
                ResourceUrl = url,
                AdminId = adminStaticResource.AdminId
            };
        }

        public static AdminStaticResource ToAdminStaticResource(this AdminStaticResourceRequestDTO adminStaticResourceRequestDTO, string adminId)
        {
            return new AdminStaticResource
            {
                Name = adminStaticResourceRequestDTO.Name,
                Description = adminStaticResourceRequestDTO.Description,
                AdminId = adminId,
                ResourceUrl = string.Empty
            };
        }
    }
}