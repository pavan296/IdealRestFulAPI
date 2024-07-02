using IdealAPI.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PracticeAPI.Data;
using PracticeAPI.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdealAPI.Repository
{
    public class SqlWalkRepository : IWalkRepository
    {
        private readonly WalkDbContext context;

        public SqlWalkRepository(WalkDbContext context)
        {
            this.context = context;
        }
        public async Task<Walk> CreateAsync(Walk walk)
        {
            await context.Walks.AddAsync(walk);
            await context.SaveChangesAsync();

            return walk;
        }

        public async Task<Walk> DeleteAsync(Guid id)
        {
            var existingwalk = await context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingwalk == null) return null;

            context.Remove(existingwalk);
            await context.SaveChangesAsync();
            return existingwalk;
        }

        public async Task<List<Walk>> GetAllAsync(string filterOn = null, string filterQuery = null, string sortBy = null, bool? isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var walks = context.Walks.Include("Difficulty").Include("Region").AsQueryable();

            //Filter
            if (string.IsNullOrWhiteSpace(filterOn) == false || string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    walks = walks.Where(x => x.Name.Equals(filterQuery));
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    walks = isAscending.Value ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);

                if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                    walks = isAscending.Value ? walks.OrderBy(x => x.LengthInKM) : walks.OrderByDescending(x => x.LengthInKM);
            }


            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk> GetByIdAsync(Guid id)
        {
            ResiliencyPolly resiliencyPolly = new ResiliencyPolly();
            var apiResponse = await resiliencyPolly.ConnectApi();

            if (apiResponse.IsSuccessful)
            {
                Console.Out.WriteLine(apiResponse.Content);
            }
            else
            {
                Console.WriteLine(apiResponse.ErrorMessage);
                throw new Exception(apiResponse.ErrorMessage);
            }
            var walk = await context.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            if (walk == null) return null;
            return walk;
        }

        public async Task<Walk> UpdateAsync(Guid id, Walk walk)
        {
            var existingwalk = await context.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingwalk == null) return null;

            existingwalk.Name = walk.Name;
            existingwalk.LengthInKM = walk.LengthInKM;
            existingwalk.Description = walk.Description;
            existingwalk.Description = walk.Description;
            existingwalk.WalkImageUrl = walk.WalkImageUrl;
            existingwalk.DifficultyID = walk.DifficultyID;
            existingwalk.RegionId = walk.RegionId;



            await context.SaveChangesAsync();
            return existingwalk;
        }
    }
}
