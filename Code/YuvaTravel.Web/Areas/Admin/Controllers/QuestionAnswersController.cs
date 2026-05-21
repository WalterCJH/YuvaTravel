using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.QuestionAnswers;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class QuestionAnswersController : BaseController
    {
        IUnitOfWork uow;

        public QuestionAnswersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            uow = unitOfWork;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.QuestionAnswer)]
        [ViewBagAuthor]
        public IActionResult Index(QuestionAnswerFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.QuestionAnswerRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<QuestionAnswer>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.QuestionAnswer)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            QuestionAnswer questionAnswer = await uow.QuestionAnswerRepo.FindAsync(id);   // 顯示用,ANT
            if (questionAnswer == null) return NotFound();
            return View(questionAnswer);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.QuestionAnswer)]
        public IActionResult Create()
        {
            var dto = new QuestionAnswerCreateOrEdit { DisplaySeq = uow.QuestionAnswerRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.QuestionAnswer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionAnswerCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var questionAnswer = new QuestionAnswer { QuestionAnswerId = Guid.NewGuid() };
                questionAnswer.InjectFrom(dto);
                questionAnswer.UserLog.CreateTime = DateTime.Now;
                questionAnswer.UserLog.CreateUserId = UserId;

                uow.QuestionAnswerRepo.Add(questionAnswer);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.QuestionAnswers}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.QuestionAnswer)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            QuestionAnswer questionAnswer = await uow.QuestionAnswerRepo.FindAsync(id);   // GET 顯示用,ANT
            if (questionAnswer == null) return NotFound();
            var dto = new QuestionAnswerCreateOrEdit();
            dto.InjectFrom(questionAnswer);
            dto.QuestionAnswerId = questionAnswer.QuestionAnswerId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.QuestionAnswer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionAnswerCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                QuestionAnswer questionAnswer = await uow.QuestionAnswerRepo.FindAsync(dto.QuestionAnswerId, isANT: false);
                if (questionAnswer == null) return NotFound();

                questionAnswer.InjectFrom(dto);
                questionAnswer.UserLog.UpdateTime = DateTime.Now;
                questionAnswer.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.QuestionAnswers}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.QuestionAnswer)]
        public async Task<IActionResult> Delete(Guid id)
        {
            QuestionAnswer questionAnswer = await uow.QuestionAnswerRepo.FindAsync(id);
            if (questionAnswer == null) return NotFound();
            return View(questionAnswer);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.QuestionAnswer)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid QuestionAnswerId)
        {
            // Delete 用 tracking 較安全
            QuestionAnswer questionAnswer = await uow.QuestionAnswerRepo.FindAsync(QuestionAnswerId, isANT: false);
            if (questionAnswer == null) return NotFound();
            uow.QuestionAnswerRepo.Delete(questionAnswer);
            TempData["success-msg"] = $"{StrText.QuestionAnswers}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
