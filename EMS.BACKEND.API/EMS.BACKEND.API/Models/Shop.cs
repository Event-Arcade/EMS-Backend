﻿namespace EMS.BACKEND.API.Models
{
    public class Shop
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public ApplicationUser Owner { get; set; }
        public List<Service> Services { get; set; }
    }
}