using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Omu.ValueInjecter;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Profiles;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class ProfilesController : BaseController
    {
        public ProfilesController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var userLoginProviders = uow.UserLoginProviderRepo.All().Where(p => p.UserGuid == UserGuid).ToList();

            if (userLoginProviders.Count > 0)
            {
                var userLoginProviderGoogle = userLoginProviders.FirstOrDefault(p => p.LoginProvider == ExternalLoginProvider.Google.ToString());
                if (userLoginProviderGoogle != null)
                {
                    ViewBag.GoogleProviderKey = userLoginProviderGoogle.ProviderKey;
                }
            }
        }

        [ViewBagSalutation]
        public async Task<IActionResult> Information()
        {
            User user = await uow.UserRepo.FindGuidAsync(UserGuid);
            if (user == null) return NotFound();
            var dto = new InformationDto();
            dto.InjectFrom(user);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagSalutation]
        public async Task<IActionResult> Information(InformationDto dto)
        {
            if (ModelState.IsValid)
            {
                User user = await uow.UserRepo.FindGuidAsync(dto.UserGuid, isANT: false);
                if (user == null) return NotFound();
                user.LastName = dto.LastName;
                user.FirstName = dto.FirstName;
                user.Salutation = dto.Salutation;
                user.Email = dto.Email;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Information}{StrText.EditSuccess}";
                return RedirectToAction("Information");
            }
            return View(dto);
        }

        public IActionResult ChangePassword()
        {
            var dto = new ChangePwdDto
            {
                UserGuid = UserGuid,
                IsNewUser = uow.UserRepo.IsNoPassword(UserGuid)
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePwdDto dto)
        {
            if (ModelState.IsValid)
            {
                User user = await uow.UserRepo.FindGuidAsync(dto.UserGuid, isANT: false);
                if (user == null) return NotFound();
                user.Password = uow.UserRepo.HashPassword(dto.NewPassword);

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ChangePassword}{StrText.Success}";
                return RedirectToAction("Information");
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            var redirectUrl = Url.Action("LinkLoginCallback", "Profiles");
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                Items = { { StrText.XsrfKey, UserGuid.ToString() } }
            };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> LinkLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                TempData["error-msg"] = $"{StrText.LinkExternalLogin}{StrText.Fail}";
                return RedirectToAction("Information");
            }

            var xsrfToken = result.Properties?.Items[StrText.XsrfKey];
            if (xsrfToken != UserGuid.ToString())
            {
                TempData["error-msg"] = $"{StrText.LinkExternalLogin}{StrText.Fail}";
                return RedirectToAction("Information");
            }

            var loginProvider = result.Principal.Identity?.AuthenticationType ?? "";
            var providerKey = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            var existingProvider = uow.UserLoginProviderRepo.All().FirstOrDefault(p => p.ProviderKey == providerKey);
            if (existingProvider != null)
            {
                TempData["error-msg"] = $"{loginProvider}{StrText.LinkExternalLogin}{StrText.Fail}";
            }
            else
            {
                var userLoginProvider = new UserLoginProvider
                {
                    UserGuid = UserGuid,
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey,
                    CreateTime = DateTime.Now
                };
                uow.UserLoginProviderRepo.Add(userLoginProvider);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{loginProvider}{StrText.LinkExternalLogin}{StrText.Success}";
            }

            return RedirectToAction("Information");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(string LoginProvider, string ProviderKey)
        {
            // Delete 用 tracking
            var userLoginProvider = uow.UserLoginProviderRepo.All(isANT: false).FirstOrDefault(p => p.LoginProvider == LoginProvider && p.ProviderKey == ProviderKey);
            if (userLoginProvider == null)
            {
                TempData["error-msg"] = $"{LoginProvider}{StrText.RemoveExternalLogin}{StrText.Fail}";
            }
            else
            {
                uow.UserLoginProviderRepo.Delete(userLoginProvider);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{LoginProvider}{StrText.RemoveExternalLogin}{StrText.Success}";
            }

            return RedirectToAction("Information");
        }
    }
}
