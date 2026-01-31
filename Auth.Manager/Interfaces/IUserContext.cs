using System;
using Auth.Manager.Models;

namespace Auth.Manager.Interfaces;

public interface IUserContext
{
    Task<UserIdentity> GetAsync(string userId);
}
