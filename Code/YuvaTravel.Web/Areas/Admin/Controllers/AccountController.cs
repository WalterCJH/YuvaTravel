using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Account;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Infrastructure.Model.Mail;
using YuvaTravel.Web.Extensions;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : BaseController
    {
        private readonly EmailOptions _emailOptions;

        public AccountController(IUnitOfWork unitOfWork, EmailOptions emailOptions) : base(unitOfWork)
        {
            _emailOptions = emailOptions;
        }

        public IActionResult Login(string? returnUrl)
        {
            var dto = new LoginDto
            {
                ReturnUrl = returnUrl,
                ErrorMsg = TempData["error-msg"]?.ToString()
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = uow.UserRepo.FindEmail(dto.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "帳號或密碼錯誤");
                    return View(dto);
                }

                var userData = new UserData
                {
                    UserGuid = user.UserGuid,
                    UserId = user.UserId,
                    UserGroupId = user.UserGroupId,
                    UserName = user.Name
                };
                if (user.UserGroup != null)
                    userData.AuthorizeLevel = user.UserGroup.AuthorizeLevel;
                HttpContext.Session.Set(StrSession.AdminUserData, userData);

                if (user.LoginResetPassword)
                    return RedirectToAction("LoginChangePassword", "Dashbroad", new { area = "Admin" });

                if (string.IsNullOrEmpty(dto.ReturnUrl))
                    return RedirectToAction("Index", "Dashbroad", new { area = "Admin" });

                if (!dto.ReturnUrl.StartsWith("/"))
                    return StatusCode(401);

                return Redirect(dto.ReturnUrl);
            }

            return View(dto);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(StrSession.AdminUserData);
            return RedirectToAction("Login", new { area = "Admin" });
        }

        public IActionResult ForgetPassword(string? e)
        {
            return View(new ForgetPasswordDto { Email = e });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = uow.UserRepo.FindEmail(dto.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "找不到此Email");
                    return View(dto);
                }

                var userForgetPassword = new UserForgetPassword
                {
                    ForgetId = Guid.NewGuid(),
                    UserGuid = user.UserGuid,
                    RequestTime = DateTime.Now
                };
                uow.UserForgetPasswordRepo.Add(userForgetPassword);
                await uow.CommitAsync();

                var sb = new StringBuilder();
                sb.AppendLine("請於24小時內至下方連結設定新密碼");
                sb.AppendLine($"{Request.Scheme}://{Request.Host}/Admin/Account/SetNewPassword/{userForgetPassword.ForgetId}");

                var info = new MailInfo();
                info.Receivers.Add(new MailAddressInfo(dto.Email));
                info.Subject = $"{StrText.AdminName} - 忘記密碼驗證信";
                info.Body = sb.ToString();
                info.IsBodyHtml = false;

                var emailHelper = new EmailHelper(SystemConfig, _emailOptions);
                await emailHelper.SendMailAsync(info);

                TempData["error-msg"] = $"請至{dto.Email}收驗證信";
                return View("Message");
            }

            return View(dto);
        }

        public async Task<IActionResult> SetNewPassword(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                var userForgetPassword = await uow.UserForgetPasswordRepo.FindAsync(guid);
                if (userForgetPassword != null && userForgetPassword.CheckTime == null && userForgetPassword.RequestTime.AddDays(1) > DateTime.Now)
                {
                    var dto = new SetNewPasswordDto { ForgetId = userForgetPassword.ForgetId };
                    return View(dto);
                }
            }

            TempData["error-msg"] = "系統異常,請重新取得忘記密碼驗證信";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNewPassword(SetNewPasswordDto dto)
        {
            if (ModelState.IsValid)
            {
                var userForgetPassword = await uow.UserForgetPasswordRepo.FindAsync(dto.ForgetId, isANT: false);
                if (userForgetPassword != null && userForgetPassword.CheckTime == null)
                {
                    userForgetPassword.CheckTime = DateTime.Now;
                    User user = await uow.UserRepo.FindGuidAsync(userForgetPassword.UserGuid, isANT: false);
                    user.Password = uow.UserRepo.HashPassword(dto.NewPassword);
                    await uow.CommitAsync();
                    TempData["error-msg"] = "請使用新密碼登入";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error-msg"] = "系統異常,請重新取得忘記密碼驗證信";
                    return RedirectToAction("Login");
                }
            }

            return View(dto);
        }

        public IActionResult Privacy() => View();
    }
}
