using LibrarySystemG.API.IRepository;  
using LibrarySystemG.API.Model.Request;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibrarySystemG.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;

        public LoginController(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(LoginModel login)
        {
            var response = await _loginRepository.Login(login);
            return Ok(response);
        }
    }
}