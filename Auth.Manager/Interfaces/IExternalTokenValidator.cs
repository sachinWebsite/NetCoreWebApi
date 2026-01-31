using System;
using Auth.Manager.Models;

namespace Auth.Manager.Interfaces;

public interface IExternalTokenValidator
{
  Task<UserIdentity> ValidateAsync(string externalToken);
}
