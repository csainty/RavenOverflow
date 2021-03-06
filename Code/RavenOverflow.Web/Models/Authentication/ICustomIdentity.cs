﻿using System.Security.Principal;

namespace RavenOverflow.Web.Models.Authentication
{
    public interface ICustomIdentity : IIdentity
    {
        string UserId { get; }
    }
}