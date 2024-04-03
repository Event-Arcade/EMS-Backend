namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public record BaseResponseDTO
    {
        public bool Flag { get; init; }
        public string Message { get; init; }
    }
    public record BaseResponseDTO<T> : BaseResponseDTO
    {
        public T Data { get; init; }
    }
    public record BaseResponseDTO<T, U> : BaseResponseDTO
    {
        public T Data1 { get; init; }
        public U Data2 { get; init; }
    }
    public record BaseResponseDTO<T, U, V> : BaseResponseDTO
    {
        public T Data1 { get; init; }
        public U Data2 { get; init; }
        public V Data3 { get; init; }
    }
}