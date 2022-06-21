﻿using CS.Authentication.Contracts.Requests;
using CS.Authentication.Contracts.Responses;
using CS.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace CS.Authentication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IIdentityService _identityService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IIdentityService identityService)
        {
            _logger = logger;
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthenticationResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))
                });
            }
            var result = await _identityService.LoginAsync(request);

            return BadRequest("Invalid login or password");
        }

        [HttpPost("refreshToken")]
        [Produces(typeof(string))]
        public IActionResult RefreshToken(string token, string refreshToken)
        {

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthenticationResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RegisterAsync(userModel);
            if(!authResponse.Success)
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }
    }
}
