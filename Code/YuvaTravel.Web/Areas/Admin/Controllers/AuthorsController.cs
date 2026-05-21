using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Authors;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Infrastructure.Storage;
// 用 alias 避開新舊 IUnitOfWork 撞名
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class AuthorsController : BaseController
    {
        IUnitOfWork uow;
        IImageUploadService _imgUpload;

        public AuthorsController(IUnitOfWork unitOfWork, IImageUploadService imageUploadService) : base(unitOfWork)
        {
            uow = unitOfWork;
            _imgUpload = imageUploadService;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Author)]
        public async Task<IActionResult> Index(AuthorFilter filter)
        {
            ViewBag.GuideSelectList = await GetGuideSelectList(filter.GuideId);

            if (ModelState.IsValid)
                ViewBag.ListData = uow.AuthorRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<Author>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.Author)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.AuthorRepo.FindAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Author)]
        public async Task<IActionResult> Create()
        {
            var dto = new AuthorCreateOrEdit { DisplaySeq = uow.AuthorRepo.GetMaxDisplaySeq() };
            ViewBag.GuideSelectList = await GetGuideSelectList(null);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Author)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorCreateOrEdit dto)
        {
            await ApplyGuideNameIfLinked(dto);

            if (ModelState.IsValid)
            {
                var entity = new Author { AuthorId = Guid.NewGuid() };
                entity.InjectFrom(dto);
                entity.UserLog.CreateTime = DateTime.Now;
                entity.UserLog.CreateUserId = UserId;

                if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
                {
                    var folder = new[] { Folder.Authors };
                    var baseName = Path.GetFileNameWithoutExtension(dto.AvatarFile.FileName ?? entity.AuthorId.ToString());
                    entity.AvatarUrl = await _imgUpload.UploadAsync(dto.AvatarFile, ImageSize.AuthorAvatarWidth, folder, baseName);
                }

                uow.AuthorRepo.Add(entity);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Authors}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Author)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.AuthorRepo.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new AuthorCreateOrEdit();
            dto.InjectFrom(entity);
            dto.AuthorId = entity.AuthorId;
            dto.AvatarUrl_ = entity.AvatarUrl;
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Author)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorCreateOrEdit dto)
        {
            await ApplyGuideNameIfLinked(dto);

            if (ModelState.IsValid)
            {
                // ⚠️ FindAsync 預設 isANT=true 不 track,InjectFrom 的變更不會被 SaveChanges 寫回。
                //    Edit POST 必須 tracking。
                var entity = await uow.AuthorRepo.FindAsync(dto.AuthorId, isANT: false);
                if (entity == null) return NotFound();
                entity.InjectFrom(dto);
                entity.UserLog.UpdateTime = DateTime.Now;
                entity.UserLog.UpdateUserId = UserId;

                if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
                {
                    await _imgUpload.DeleteAsync(entity.AvatarUrl);
                    var folder = new[] { Folder.Authors };
                    var baseName = Path.GetFileNameWithoutExtension(dto.AvatarFile.FileName ?? entity.AuthorId.ToString());
                    entity.AvatarUrl = await _imgUpload.UploadAsync(dto.AvatarFile, ImageSize.AuthorAvatarWidth, folder, baseName);
                }

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Authors}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        /// <summary>選了嚮導 → 名稱/英文名強制從 Guide 帶,並清掉表單上殘留</summary>
        private async Task ApplyGuideNameIfLinked(AuthorCreateOrEdit dto)
        {
            if (!dto.GuideId.HasValue) return;
            var guide = await uow.GuideRepo.FindAsync(dto.GuideId);
            if (guide == null) return;
            dto.Name = guide.Name;
            dto.NameEn = guide.NameEn;
            // 清掉因 [Required] / IValidatableObject 殘留的 ModelState 錯誤
            ModelState.Remove(nameof(dto.Name));
            ModelState.Remove(nameof(dto.NameEn));
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Author)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await uow.AuthorRepo.FindAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Author)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Delete 用 tracking 才安全 (untracked Remove 雖然 EF 會自動 attach,但顯式較不易出錯)
            var entity = await uow.AuthorRepo.FindAsync(id, isANT: false);
            if (entity == null) return NotFound();
            await _imgUpload.DeleteAsync(entity.AvatarUrl);
            uow.AuthorRepo.Delete(entity);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.Authors}{StrText.DeleteSuccess}";
            return RedirectToAction("Index");
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetGuideSelectList(Guid? selectedId)
        {
            // 只需要 GuideId / Name / DisplaySeq,不需要 Include 一堆關聯表 → 用 All() 而非 IncludeAll()
            return await uow.GuideRepo.All()
                .OrderBy(p => p.DisplaySeq)
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.GuideId.ToString(),
                    Text = p.Name,
                    Selected = selectedId.HasValue && p.GuideId == selectedId.Value
                })
                .ToListAsync();
        }
    }
}
