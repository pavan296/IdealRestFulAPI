using AutoMapper;
using IdealAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PracticeAPI.Controllers;
using PracticeAPI.Data;
using PracticeAPI.Model.Domain;
using PracticeAPI.Repository;

namespace IdealApi.Test
{
    public class RegionControllerTests
    {
        private readonly Mock<IRegionRepository> mockRepository;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILogger<RegionsController>> mockLogger;
        private readonly RegionsController controller;

        public RegionControllerTests()
        {
            mockRepository = new Mock<IRegionRepository>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILogger<RegionsController>>();
            controller = new RegionsController(null, mockRepository.Object, mockMapper.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetAllV1_ReturnsOkResult_WithListOfRegions()
        {
            //Arrange
            var mockRegions = new List<Region>
            {
                new Region { Id = Guid.NewGuid(), Name = "Region1" },
                new Region { Id = Guid.NewGuid(), Name = "Region2" }
            };
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockRegions);
            mockMapper.Setup(m => m.Map<List<Region>>(It.IsAny<List<Region>>())).Returns(mockRegions);

            //Act
            var result = await controller.GetAllV1();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnRegions = Assert.IsType<List<Region>>(okResult.Value);
            Assert.Equal(2, returnRegions.Count);
            mockLogger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.AtLeastOnce);

        }
    }
}