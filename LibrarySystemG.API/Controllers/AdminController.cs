using LibrarySystemG.API.IRepository;  
using LibrarySystemG.API.Model;        
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibrarySystemG.API.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        
        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminModel admin)
        {
            // Validate input
            if (admin == null || string.IsNullOrEmpty(admin.Username) || string.IsNullOrEmpty(admin.Password))
                return BadRequest(new { Message = "Username and password are required" });

            // Call repository
            var result = await _adminRepository.AdminLogin(admin);

            // Return result
            return Ok(result);
        }
    }
}