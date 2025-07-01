using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using StockApp.Infra.Data.Identity;

namespace StockApp.Application.Interfaces
{
    public interface ISignInService
    {
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutAsync();
        Task SignInAsync(ApplicationUser user, bool isPersistent);
    }
}