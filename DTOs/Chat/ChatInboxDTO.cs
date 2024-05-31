namespace EMS.BACKEND.API.DTOs.Chat
{
    public class ChatInboxDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? ProfilePictureURL { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public bool? IsRead { get; set; }
        public int? UnreadMessages { get; set; }
        public bool? IsActive { get; set; }
    }
}