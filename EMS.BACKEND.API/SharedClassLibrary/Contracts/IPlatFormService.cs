using SharedClassLibrary.DTOs;
using static SharedClassLibrary.DTOs.ServiceResponses;

namespace SharedClassLibrary.Contracts
{
    public interface IPlatFormService
    {
        Task<GeneralResponse> CreatePlatform(PlatformDTO platformDTO);
    }
}
