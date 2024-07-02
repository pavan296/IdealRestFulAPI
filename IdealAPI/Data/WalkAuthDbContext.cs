using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IdealAPI.Data
{
    public class WalkAuthDbContext:IdentityDbContext
    {
        public WalkAuthDbContext(DbContextOptions<WalkAuthDbContext> options):base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "6EF8A953-B0D3-4829-8F3C-4812CCF3E9B6";
            var writerRoleId = "B9771080-21E2-4306-A019-506446522799";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=readerRoleId,
                    ConcurrencyStamp=readerRoleId,
                    Name="Reader",
                    NormalizedName="Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id=writerRoleId,
                    ConcurrencyStamp =writerRoleId,
                    Name="Writer",
                    NormalizedName="Writer".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
