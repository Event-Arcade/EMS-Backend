using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.BACKEND.API.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description {get;set;}
        public string CategoryImagePath {get; set;}
        public string UserId{get; set;}
        public virtual ApplicationUser User{get;set;}

    }
}
