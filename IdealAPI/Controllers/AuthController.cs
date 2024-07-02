using IdealAPI.Model.DTO;
using IdealAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IdealAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager,ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        //Post register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Password != null || registerRequestDto.Password.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                    if (identityResult.Succeeded)
                    {
                        return Ok("User registered successfully!");
                    }
                }
            }
            return BadRequest("Something Went Wrong");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user= await userManager.FindByEmailAsync(loginRequestDto.UserName);

            if (user != null)
            {
                var checkPassword=await userManager.CheckPasswordAsync(user,loginRequestDto.Password);
                if (checkPassword)
                {
                    //Create Token
                    var roles=await userManager.GetRolesAsync(user);
                    if (roles.Any())
                    {
                       var jwtToken= tokenRepository.CreateJWTToken(user, roles.ToArray());

                        var response = new LoginResponseDto
                        {
                            JwtToken=jwtToken
                        };

                        return Ok(jwtToken);
                    }
                }
            }

            return BadRequest("username or password incorrect");
        }
    }
}
