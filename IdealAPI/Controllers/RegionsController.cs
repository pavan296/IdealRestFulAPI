using AutoMapper;
using IdealAPI.CustomActionFilter;
using IdealAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PracticeAPI.Data;
using PracticeAPI.Model.Domain;
using PracticeAPI.Model.DTO;
using PracticeAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace PracticeAPI.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    public class RegionsController : ControllerBase
    {
        private readonly WalkDbContext DbContext;
        private readonly IRegionRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;
        

        public RegionsController(WalkDbContext _dbcontext,IRegionRepository repository,IMapper mapper,ILogger<RegionsController> logger)
        {
            DbContext = _dbcontext;
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
          
        }
        [HttpGet]
        [Authorize(Roles ="Reader")]
        //[MapToApiVersion("1.0")]
        public async  Task<IActionResult> GetAllV1()
        {
            logger.LogInformation("Getting all Regions");
            
            var regions = await repository.GetAllAsync();
            logger.LogInformation($"{JsonConvert.SerializeObject(regions)}");
            return Ok(mapper.Map<List<Region>>(regions));
        }

        //[HttpGet]
        ////[Authorize(Roles ="Reader")]
        ////[MapToApiVersion("2.0")]
        //public async Task<IActionResult> GetAllV2()
        //{
        //    logger.LogInformation("Getting all Regions");

        //    var regions = await repository.GetAllAsync();
        //    logger.LogInformation($"{JsonConvert.SerializeObject(regions)}");
        //    return Ok(mapper.Map<List<Region>>(regions));
        //}

        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            //var region = await DbContext.Regions.FindAsync(id);
            var region=await repository.GetById(id);
            if (region != null) return Ok(mapper.Map<RegionDTO>(region));

            return NotFound();
        }

        [HttpPost]
        [ValidateModelAttribute]
        public async Task<IActionResult> Create([FromBody] AddRegionReqDto regionReqDto)
        {
                var regionDomainModel = mapper.Map<Region>(regionReqDto);
                var regions = await repository.CreateAsync(regionDomainModel);
                var regionDto = mapper.Map<RegionDTO>(regions);

                return CreatedAtAction(nameof(Get), new { id = regionDto.Id }, regionDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModelAttribute]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AddRegionReqDto regionDto)
        {
                var regionDomainModel = mapper.Map<Region>(regionDto);

                regionDomainModel = await repository.UpdateAsync(id, regionDomainModel);
                if (regionDomainModel == null) return null;

                return Ok(mapper.Map<RegionDTO>(regionDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel=await repository.DeleteAsync(id);

            return Ok(mapper.Map<RegionDTO>(regionDomainModel));
        }
    }
}
