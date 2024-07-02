using IdealAPI.Repository;
using Microsoft.EntityFrameworkCore;
using PracticeAPI.Data;
using PracticeAPI.Model.Domain;
using PracticeAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticeAPI.Repository
{
    public class SqlRegionRepository : IRegionRepository
    {
        private readonly WalkDbContext context;
        private readonly ICachingInterface cacheService;

        public SqlRegionRepository(WalkDbContext context,ICachingInterface cacheService)
        {
            this.context = context;
            this.cacheService = cacheService;
        }

        public async Task<Region> CreateAsync(Region region)
        {
            
            var addedObj=await context.Regions.AddAsync(region);
            cacheService.SetData<Region>($"Regions{region.Id}", addedObj.Entity, TimeSpan.FromSeconds(30));
            await context.SaveChangesAsync();
            return region;
        }

        public async Task<Region> DeleteAsync(Guid id)
        {
            var existingRegion = await context.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (existingRegion == null) return null;

            context.Remove(existingRegion);
            cacheService.RemoveData($"Regions{id}");
            await context.SaveChangesAsync();
            return existingRegion;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            var cacheData = cacheService.GetData<List<Region>>("Regions");

            if(cacheData!=null && cacheData.Count > 0)
              return cacheData;

            cacheData= await context.Regions.ToListAsync();

            cacheService.SetData<List<Region>>("Regions", cacheData, TimeSpan.FromSeconds(30));

            return cacheData;
        }

        public async Task<Region> GetById(Guid id)
        {
            return await context.Regions.FirstOrDefaultAsync(context => context.Id == id);
        }

        public async Task<Region> UpdateAsync(Guid id,Region region)
        {
            var existingRegion = await context.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRegion == null) return null;

            existingRegion.Code= region.Code;
            existingRegion.Name= region.Name;
            existingRegion.RegionImageUrl=region.RegionImageUrl;


            await context.SaveChangesAsync();
            return existingRegion;
        }
    }
}
