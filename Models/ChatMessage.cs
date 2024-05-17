using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.Models
{
    public class ChatMessage
    {
        [Key]
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public bool IsRead { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }

    }
}