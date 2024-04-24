﻿using EMS.BACKEND.API.DTOs.RequestDTOs;
using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface ICategoryRepository : IBaseRepository<Category, CategoryRequestDTO>
    {

    }
}
