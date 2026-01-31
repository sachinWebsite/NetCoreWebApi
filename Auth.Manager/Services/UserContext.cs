using System;
using Auth.Manager.Interfaces;
using Auth.Manager.Models;

namespace Auth.Manager.Services;

public class UserContext: IUserContext
{
    public Task<UserIdentity> GetAsync(string userId)
    {
        // TEMP / DEMO IMPLEMENTATION
        // Later this will come from DB / Identity Service

        var user = new UserIdentity(
            UserId: userId,
            Email: "demo@company.com",
            Provider: "Internal",
            Roles: new[] { "User" },
            Claims: new Dictionary<string, string>
            {
                ["tenantId"] = "default"
            }
        );

        return Task.FromResult(user);
    }
}