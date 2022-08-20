using System;
using System.Security.Claims;
using MeuBolsoDigital.Core.Interfaces.Identity;
using Microsoft.AspNetCore.Http;

namespace MBD.BankAccounts.API.Identity
{
    public class WebAppUser : ILoggedUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebAppUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            IsAuthenticated
            ? Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
            : Guid.Empty;

        public string Email =>
            IsAuthenticated
            ? _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value
            : string.Empty;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
    }
}