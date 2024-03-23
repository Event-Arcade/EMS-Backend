using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
        //records are used to create immutable objects 
        public record UserSession(string? Id, string? Email, string? Role);
        public record GeneralResponse(bool Flag, string Message);
        public record LoginResponse(bool Flag, string Token, string Message);
        public record StaticDataResponse(bool Flag, Stream bitStream, string contentType);
        public record UserResponse(bool Flag, string Message, UserResponseDTO userResponseDTO);
        public record ShopResponse(bool Flag, string Message, ShopResponseDTO shopResponseDTO);
        public record ShopListResponse(bool Flag, string Message, List<ShopResponseDTO> shopResponseDTOs);
        public record ServiceResponse(bool Flag, string Message, ServiceResponseDTO serviceResponseDTO);
        public record ServiceListResponse(bool Flag, string Message, List<ServiceResponseDTO> serviceResponseDTOs);
        public record CategoryResponse(bool Flag, string Message, CategoryResponseDTO categoryResponseDTO);
        public record CategoryListResponse(bool Flag, string Message, List<CategoryResponseDTO> categoryResponseDTOs);
        public record PackageResponse(bool Flag, string Message, PackageResponseDTO packageResponseDTO);
        public record PackageListResponse(bool Flag, string Message, List<PackageResponseDTO> packageResponseDTOs);
}
