using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.UserGroups;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UserGroupFuncProgramRepository : BaseRepository<UserGroupFuncProgram>, IUserGroupFuncProgramRepository
    {
        public UserGroupFuncProgramRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<UserGroupFuncProgram> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public UserGroupFuncProgram Find(Guid id) => BaseAll().FirstOrDefault(p => p.UserGroupFuncProgramId == id);

        public async Task<UserGroupFuncProgram> FindAsync(Guid id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.UserGroupFuncProgramId == id);
        }

        public async Task UpdateUserGroupFuncProgram(List<FuncGroupDto> funcGroups, UserGroup userGroup, string userId)
        {
            var userGroupFuncPrograms = await BaseAll().Where(p => p.UserGroupId == userGroup.UserGroupId).ToListAsync();

            foreach (var funcGroup in funcGroups)
            {
                foreach (var program in funcGroup.FuncPrograms.Where(p => p.IsCreate || p.IsEdit || p.IsDelete || p.IsDetails || p.IsImport || p.IsExport))
                {
                    var userGroupFuncProgram = userGroupFuncPrograms.FirstOrDefault(p => p.UserGroupId == userGroup.UserGroupId && p.FuncProgramId == program.ProgramId.ToString());

                    if (userGroupFuncProgram != null)
                    {
                        userGroupFuncProgram.UserLog.UpdateTime = DateTime.Now;
                        userGroupFuncProgram.UserLog.UpdateUserId = userId;
                        userGroupFuncPrograms.Remove(userGroupFuncProgram);
                    }
                    else
                    {
                        userGroupFuncProgram = new UserGroupFuncProgram
                        {
                            UserGroupFuncProgramId = Guid.NewGuid(),
                            UserGroupId = userGroup.UserGroupId,
                            FuncProgramId = program.ProgramId.ToString()
                        };
                        userGroupFuncProgram.UserLog.CreateTime = DateTime.Now;
                        userGroupFuncProgram.UserLog.CreateUserId = userId;
                        Add(userGroupFuncProgram);
                    }
                    userGroupFuncProgram.IsCreate = program.IsCreate;
                    userGroupFuncProgram.IsEdit = program.IsEdit;
                    userGroupFuncProgram.IsDelete = program.IsDelete;
                    userGroupFuncProgram.IsDetails = program.IsDetails;
                    userGroupFuncProgram.IsImport = program.IsImport;
                    userGroupFuncProgram.IsExport = program.IsExport;
                }
            }

            foreach (var userGroupFuncProgram in userGroupFuncPrograms)
                Delete(userGroupFuncProgram);
        }
    }
}
