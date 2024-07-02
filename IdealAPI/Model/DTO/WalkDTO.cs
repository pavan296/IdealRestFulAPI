using PracticeAPI.Model.Domain;
using PracticeAPI.Model.DTO;
using System;

namespace IdealAPI.Model.DTO
{
    public class WalkDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKM { get; set; }
        public string? WalkImageUrl { get; set; }
        public Guid DifficultyID { get; set; }
        public Guid RegionId { get; set; }

        public DifficultyDTO Difficulty { get; set; }
        public RegionDTO Region { get; set; }
    }
}
