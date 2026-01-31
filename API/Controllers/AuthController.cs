using Auth.Manager.Models;
using Auth.Manager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // // [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenExchangeService _exchange;
        private readonly RefreshTokenService _refresh;

        public AuthController(
            TokenExchangeService exchange,
            RefreshTokenService refresh)
        {
            _exchange = exchange;
            _refresh = refresh;
        }

        [HttpPost("exchange")]
        public async Task<AuthResult> Exchange([FromBody] string token)
            => await _exchange.ExchangeAsync(token);

        [HttpPost("refresh")]
        public async Task<AuthResult> Refresh([FromBody] string refreshToken)
            => await _refresh.RefreshAsync(refreshToken);
    }
}
