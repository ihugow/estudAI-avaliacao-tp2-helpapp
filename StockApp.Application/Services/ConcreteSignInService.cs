using Microsoft.AspNetCore.Identity;
using StockApp.Application.Interfaces;
using StockApp.Infra.Data.Identity;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class ConcreteSignInService : ISignInService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ConcreteSignInService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }
    }
}