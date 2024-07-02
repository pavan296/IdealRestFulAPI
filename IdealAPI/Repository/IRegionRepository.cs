using Microsoft.AspNetCore.Mvc;
using PracticeAPI.Model.Domain;
using PracticeAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticeAPI.Repository
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetAllAsync();
        Task<Region> GetById(Guid id);
        Task<Region> CreateAsync(Region region);
        Task<Region> UpdateAsync(Guid id,Region region);

        Task<Region> DeleteAsync(Guid id);


    }
}
