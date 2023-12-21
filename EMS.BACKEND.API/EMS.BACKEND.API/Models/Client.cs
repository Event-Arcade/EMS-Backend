﻿using Microsoft.AspNetCore.Identity;

namespace EMS.BACKEND.API.Models
{
    public class Client : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street {  get; set; }
        public string City { get; set; }

        public string PostalCode { get; set; }
        public string Province { get; set; }
        public Boolean IsVendor { get; set; }
        public Double Rating { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
    }
}
