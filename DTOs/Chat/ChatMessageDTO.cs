namespace EMS.BACKEND.API.DTOs.Chat
{
    public class ChatMessageDTO
    {
        public int? Id { get; set; }
        public string? SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsRead { get; set; }
    }
}