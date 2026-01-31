using System;
using Auth.Manager.Models;

namespace Auth.Manager.Interfaces;

public interface ITokenIssuer
{
    string IssueAccessToken(UserIdentity user, out DateTime expiresAt);
}
