using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input data"
                });
            }

            var response = await _registerRepository.Register(register);

            if (!response.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(new
            {
                success = true,
                message = response.Message
            });
        }
    }
}