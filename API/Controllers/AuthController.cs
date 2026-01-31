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
        private readonly OneTimeTokenService _oneTime;

        public AuthController(
             TokenExchangeService exchange,
             RefreshTokenService refresh,
             OneTimeTokenService oneTime)
        {
            _exchange = exchange;
            _refresh = refresh;
            _oneTime = oneTime;
        }

        /// <summary>
        /// Issues a temporary one-time token intended only for local testing and development.
        /// This endpoint simulates the behavior of an external identity provider
        /// (such as Azure AD B2C) that would normally issue an access token
        /// after successful user authentication in a real-world scenario.
        /// </summary>
        /// <param name="emailId">
        /// The email address of the user for whom the temporary token is generated.
        /// </param>
        /// <returns>
        /// An <see cref="AuthResult"/> containing the issued temporary token
        /// and related authentication metadata.
        /// </returns>
        [HttpPost("IssueTestToken")]
        public async Task<AuthResult> IssueTestToken([FromBody] string emailId)
            => await _oneTime.IssueTemporaryTokenAsync(emailId);


        /// <summary>
        /// Exchanges an external or temporary token for application-specific
        /// authentication credentials.
        /// This operation validates the provided token and, if valid,
        /// issues a new access token and refresh token for use within the application.
        /// </summary>
        /// <param name="token">
        /// The token to be exchanged. This may be an external identity provider token
        /// or a temporary test token. The value must not be null or empty.
        /// </param>
        /// <returns>
        /// An <see cref="AuthResult"/> containing the newly issued access token,
        /// refresh token, and token expiration details.
        /// </returns>
        [HttpPost("exchange")]
        public async Task<AuthResult> Exchange([FromBody] string token)
            => await _exchange.ExchangeAsync(token);

        public record RefreshRequest(string RefreshToken);
        /// <summary>
        /// Refreshes an expired or soon-to-expire access token using a valid refresh token.
        /// This endpoint issues a new access token while preserving the user's
        /// authenticated session without requiring re-authentication.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token previously issued during authentication.
        /// </param>
        /// <returns>
        /// An <see cref="AuthResult"/> containing a new access token,
        /// refresh token, and updated expiration information.
        /// </returns>
        [HttpPost("refresh")]
        public async Task<AuthResult> Refresh([FromBody] RefreshRequest refreshToken)
            => await _refresh.RefreshAsync(refreshToken.RefreshToken);

    }
}
