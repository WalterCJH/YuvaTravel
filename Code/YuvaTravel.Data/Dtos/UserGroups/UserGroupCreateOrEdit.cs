using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.UserGroups
{

    public class UserGroupCreateOrEdit
    {
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代號")]
        public string UserGroupId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = StrText.RequireSelectd)]
        [Display(Name = "權限等級")]
        public AuthorizeLevel AuthorizeLevel { get; set; }

        [Display(Name = "權限等級")]
        public AuthorizeLevel AuthorizeLevelView { get { return AuthorizeLevel; } }

        [Display(Name = "全選")]
        public bool CheckAll { get; set; }

        public List<FuncGroupDto> FuncGroups { get; set; }

        public List<FuncGroup> _TempFuncGroups { get; }

        public UserGroup _UserGroup { get; }

        public UserGroupCreateOrEdit()
        {
            FuncGroups = new List<FuncGroupDto>();
        }

        //public UserGroupCreateOrEdit(List<FuncGroup> TempFuncGroups, string userId)
        //{
        //    FuncGroups = new List<FuncGroupDto>();

        //    _TempFuncGroups = TempFuncGroups;
        //    foreach (var funcGroup in _TempFuncGroups)
        //    {
        //        FuncGroupDto funcGroupDto = new FuncGroupDto();
        //        FuncGroups.Add(funcGroupDto);
        //        funcGroupDto.Name = funcGroup.Name;

        //        foreach (var funcProgram in funcGroup.FuncPrograms)
        //        {
        //            if (funcProgram.OnlyAdmin && userId.ToLower() != StrText.AdminUserId.ToLower())
        //                continue;
        //            FuncProgramDto funcProgramDto = new FuncProgramDto();
        //            funcGroupDto.FuncPrograms.Add(funcProgramDto);
        //            funcProgramDto.ProgramId = funcProgram.ProgramId;
        //            funcProgramDto.Name = funcProgram.Name;
        //            funcProgramDto.DisplaySeq = funcProgram.DisplaySeq;
        //        }
        //    }
        //}

        public UserGroupCreateOrEdit(List<FuncGroup> TempFuncGroups, UserGroup UserGroup, string userId, AuthorizeLevel userAuthorizeLevel)
        {
            FuncGroups = new List<FuncGroupDto>();

            _UserGroup = UserGroup;
            _TempFuncGroups = TempFuncGroups;
            CheckAll = true;

            foreach (var funcGroup in _TempFuncGroups.OrderBy(p => p.DisplaySeq))
            {
                FuncGroupDto funcGroupDto = new FuncGroupDto();
                FuncGroups.Add(funcGroupDto);
                funcGroupDto.Name = funcGroup.Name;
                funcGroupDto.FuncGroupCheckAll = true;

                foreach (var funcProgram in funcGroup.FuncPrograms.OrderBy(p => p.DisplaySeq))
                {
                    if (funcProgram.OnlyAdmin && userId.ToLower() != StrText.AdminUserId.ToLower())
                        continue;
                    FuncProgramDto funcProgramDto = new FuncProgramDto();
                    funcGroupDto.FuncPrograms.Add(funcProgramDto);
                    funcProgramDto.ProgramId = funcProgram.ProgramId;
                    funcProgramDto.Name = funcProgram.Name;
                    funcProgramDto.DisplaySeq = funcProgram.DisplaySeq;

                    if (userAuthorizeLevel == AuthorizeLevel.系統管理員)
                    {
                        funcProgramDto.IsActiveCreate = funcProgram.IsActiveCreate;
                        funcProgramDto.IsActiveEdit = funcProgram.IsActiveEdit;
                        funcProgramDto.IsActiveDelete = funcProgram.IsActiveDelete;
                        funcProgramDto.IsActiveDetails = funcProgram.IsActiveDetails;
                        funcProgramDto.IsActiveImport = funcProgram.IsActiveImport;
                        funcProgramDto.IsActiveExport = funcProgram.IsActiveExport;
                    }
                    else
                    {
                        funcProgramDto.IsActiveCreate = (funcProgram.IsActiveCreate == false) ? false : funcProgram.IsChangeCreate;
                        funcProgramDto.IsActiveEdit = (funcProgram.IsActiveEdit == false) ? false : funcProgram.IsChangeEdit;
                        funcProgramDto.IsActiveDelete = (funcProgram.IsActiveDelete == false) ? false : funcProgram.IsChangeDelete;
                        funcProgramDto.IsActiveDetails = (funcProgram.IsActiveDetails == false) ? false : funcProgram.IsChangeDetails;
                        funcProgramDto.IsActiveImport = (funcProgram.IsActiveImport == false) ? false : funcProgram.IsChangeImport;
                        funcProgramDto.IsActiveExport = (funcProgram.IsActiveExport == false) ? false : funcProgram.IsChangeExport;
                    }

                    if (_UserGroup != null)
                    {
                        var UserGroupFuncProgram = _UserGroup.UserGroupFuncPrograms.FirstOrDefault(p => p.FuncProgramId == funcProgram.FuncProgramId.ToString());
                        if (UserGroupFuncProgram != null)
                        {
                            //funcProgramDto.IsActive = UserGroupFuncProgram.IsActive;
                            funcProgramDto.IsCreate = UserGroupFuncProgram.IsCreate;
                            funcProgramDto.IsEdit = UserGroupFuncProgram.IsEdit;
                            funcProgramDto.IsDelete = UserGroupFuncProgram.IsDelete;
                            funcProgramDto.IsDetails = UserGroupFuncProgram.IsDetails;
                            funcProgramDto.IsImport = UserGroupFuncProgram.IsImport;
                            funcProgramDto.IsExport = UserGroupFuncProgram.IsExport;
                        }
                    }

                    funcProgramDto.IsActive = true;
                    if (funcProgramDto.IsActiveCreate && !funcProgramDto.IsCreate)
                        funcProgramDto.IsActive = false;
                    else if (funcProgramDto.IsActiveEdit && !funcProgramDto.IsEdit)
                        funcProgramDto.IsActive = false;
                    else if (funcProgramDto.IsActiveDelete && !funcProgramDto.IsDelete)
                        funcProgramDto.IsActive = false;
                    else if (funcProgramDto.IsActiveDetails && !funcProgramDto.IsDetails)
                        funcProgramDto.IsActive = false;
                    else if (funcProgramDto.IsActiveImport && !funcProgramDto.IsImport)
                        funcProgramDto.IsActive = false;
                    else if (funcProgramDto.IsActiveExport && !funcProgramDto.IsExport)
                        funcProgramDto.IsActive = false;
                }

                if (funcGroupDto.FuncPrograms.Any(p => !p.IsActive))
                {
                    funcGroupDto.FuncGroupCheckAll = false;
                    CheckAll = false;
                }
            }
        }
    }

}
