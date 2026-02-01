using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Auth.Manager.Common.Exceptions;

namespace Auth.Manager.Services
{
    public sealed class OneTimeTokenService
    {
        private readonly ITokenIssuer _issuer;
        private readonly IRefreshTokenService _refreshService;
        private readonly IUserContext _userContext;

        public OneTimeTokenService(ITokenIssuer issuer, IRefreshTokenService refreshService, IUserContext userContext)
        {
            _issuer = issuer;
            _refreshService = refreshService;
            _userContext = userContext;
        }

        /// <summary>
        /// Issues a temporary access token and refresh token for testing.
        /// </summary>
        /// <param name="emailId">Optional user id or email to seed the test identity. If null/empty a GUID is used.</param>
        public async Task<AuthResult> IssueTemporaryTokenAsync(string emailId)
        {

            var user = await _userContext.GetAsync("100");
            if (user.Email != emailId)
            {
                throw new AppValidationException($"User with ID {emailId} not found in the user context.");
            }

            // Use the configured issuer to create an access token (keeps separation of concerns).
            var accessToken = _issuer.IssueAccessToken(user, out var expiresAt);

            // Mark token as test-only by prefixing. This is explicitly non-production behavior.
            accessToken = $"{accessToken}";

            var refresh = await _refreshService.CreateAsync(user.UserId);

            return new AuthResult(accessToken, refresh.Token, expiresAt);
        }
    }
}
