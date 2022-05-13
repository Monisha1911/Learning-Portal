using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RegisterAndLogin.Authentication;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAndLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager ,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user=await userManager.FindByNameAsync(model.UserName);
            if(user!=null&&await userManager.CheckPasswordAsync(user,model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        User = user.UserName
                    });

            }
            return Unauthorized();



        }





        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult>Register([FromBody] RegisterModel model)
        {
            var userExist = await userManager.FindByEmailAsync(model.Email);
            if (userExist != null)
                return StatusCode(StatusCodes.Status406NotAcceptable, new Response { Status = "Error" ,Message="Email Address already exist"});
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Password Should Contain One Uppercase,NUmbers And Special Symbol" });
            }
            return Ok(new Response { Status = "Success", Message = "User Created Successfully" });
        }


        [HttpPost]
        [Route("RegisterAdmin")]

        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExist = await userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "UserName Not Found" });
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Password Should Contain One Uppercase,NUmbers And Special Symbol" });
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if(await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User Created Successfully" });
        }




        public List<RegisterModel> users = RegisterModel.GetUsers();

        [HttpGet]
        public ActionResult GetUsers()
        {
            return Ok(users);
        }








        public static List<Course> courses = Course.GetAllCourses();



        [HttpGet]
        [Route("GetCourses")]
        public ActionResult GetAllCourses()
        {
            return Ok(courses);
        }

        [HttpGet]
        [Route("GetByID")]

        public ActionResult GetCoursesById(int CourseId)
        {
            Course CD = courses.Where(x => x.CourseId == CourseId).SingleOrDefault();
            return Ok(CD);

        }



        [HttpPost]
        [Route("Add Courses")]

        public ActionResult InsertCourses(Course C)
        {
            courses.Add(C);
            return Ok();
        }


    }

}
