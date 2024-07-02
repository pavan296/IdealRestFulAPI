using System.ComponentModel.DataAnnotations;

namespace PracticeAPI.Model.DTO
{
    public class AddRegionReqDto
    {
        [Required]
        [MinLength(3,ErrorMessage ="Code has to be minimum of 3 characters")]
        [MaxLength(3,ErrorMessage ="Code has to be maximum of 3 characters")]
        public string Code { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Maximum length for Name is 100 characters")]
        public string Name { get; set; }
        public string RegionImageUrl { get; set; }
    }
}
