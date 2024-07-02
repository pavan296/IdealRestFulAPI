using AutoMapper;
using IdealAPI.CustomActionFilter;
using IdealAPI.Model.DTO;
using IdealAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.Model.Domain;
using PracticeAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdealAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        [HttpPost]
        [ValidateModelAttribute]
        public async Task<IActionResult> Create([FromBody] AddWalkReqDto walkReqDto)
        {
                var mappedWalk = mapper.Map<Walk>(walkReqDto);
                await walkRepository.CreateAsync(mappedWalk);
                return Ok(mapper.Map<WalkDTO>(mappedWalk));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery
            ,[FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var walksDomainModel = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
                return Ok(mapper.Map<List<WalkDTO>>(walksDomainModel));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromQuery]Guid id)
        {
            return Ok(mapper.Map<WalkDTO>(await  walkRepository.GetByIdAsync(id)));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModelAttribute]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AddWalkReqDto regionDto)
        {
                var walkDomainModel = mapper.Map<Walk>(regionDto);

                walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);
                if (walkDomainModel == null) return null;

                return Ok(mapper.Map<WalkDTO>(walkDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.DeleteAsync(id);

            return Ok(mapper.Map<WalkDTO>(walkDomainModel));
        }
    }
}
