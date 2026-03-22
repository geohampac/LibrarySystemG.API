using LibrarySystemG.API.IRepository;   
using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemG.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterRepository _registerRepository;

        public RegisterController(IRegisterRepository registerRepository)
        {
            _registerRepository = registerRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister([FromBody] RegisterModel register)
        {
           
            var response = await _registerRepository.Register(register);

            if (!response.Success)
            {
               
                return BadRequest(response);
            }

           
            return Ok(response);
        }
    }
}