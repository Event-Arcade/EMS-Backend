namespace EMS.BACKEND.API.DTOs.ResponseDTOs
{
    public record ResponseDTO
    {
        public bool Flag { get; init; }
        public string Message { get; init; }
    }
    public record ResponseDTO<T> : ResponseDTO
    {
        public T Data { get; init; }
    }
    public record ResponseDTO<T, U> : ResponseDTO
    {
        public T Data1 { get; init; }
        public U Data2 { get; init; }
    }
    public record ResponseDTO<T, U, V> : ResponseDTO
    {
        public T Data1 { get; init; }
        public U Data2 { get; init; }
        public V Data3 { get; init; }
    }
}