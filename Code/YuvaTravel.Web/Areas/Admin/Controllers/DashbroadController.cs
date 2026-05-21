using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.Dashbroad;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class DashbroadController : BaseController
    {
        public DashbroadController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IActionResult Index() => View();

        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public IActionResult Error()
        {
            Response.StatusCode = 500;
            return View();
        }

        public IActionResult NoAuthorization() => View();

        public IActionResult LoginChangePassword()
        {
            var user = uow.UserRepo.Find(UserId);
            if (!user.LoginResetPassword)
                return RedirectToAction("Index");

            var dto = new LoginChangePassword { UserName = user.Name };
            return View(dto);
        }

        [HttpPost]
        public IActionResult LoginChangePassword(LoginChangePassword dto)
        {
            if (ModelState.IsValid)
            {
                // Find 預設 tracking
                User user = uow.UserRepo.Find(UserId);
                if (user == null) return NotFound();
                user.Password = uow.UserRepo.HashPassword(dto.Password);
                user.LoginResetPassword = false;
                uow.Commit();

                TempData["success-msg"] = $"{StrText.ChangePassword}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }
    }
}
