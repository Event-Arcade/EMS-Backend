using Microsoft.AspNetCore.Mvc;

namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public class Responses
    {
        //records are used to create immutable objects 
        public record UserSession(string? Id, string? Email, string? Role);
        public record GeneralResponse(bool Flag, string Message);
        public record LoginResponse(bool Flag, string Token, string Message);
        public record UserResponse(bool Flag, string Message, UserResponseDTO UserRequest);
    }
}
