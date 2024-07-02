using PracticeAPI.Model.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace IdealAPI.Model.DTO
{
    public class AddWalkReqDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        [Range(0,50)]
        public double LengthInKM { get; set; }
        public string? WalkImageUrl { get; set; }
        [Required]
        public Guid DifficultyID { get; set; }
        [Required]
        public Guid RegionId { get; set; }
    }
}
