using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Data.Dtos.Users;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<User> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<User> IncludeAll(bool isANT = true)
        {
            var query = BaseAll()
                .Include(p => p.UserGroup)
                .Include(p => p.UserLoginProviders)
                .AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public User FindGuid(Guid id) => IncludeAll(false).FirstOrDefault(p => p.UserGuid == id);
        public async Task<User> FindGuidAsync(Guid id, bool isANT = true) => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.UserGuid == id);

        public User Find(string id) => IncludeAll(false).FirstOrDefault(p => p.UserId == id);
        public async Task<User> FindAsync(string id, bool isANT = true) => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.UserId == id);

        public User FindEmail(string email) => IncludeAll(false).FirstOrDefault(p => p.Email == email);

        public IQueryable<User> Search(UserFilter filter, string userGroupId)
        {
            var data = IncludeAll(true);

            if (filter.IsActive == WhetherType.否)
                data = data.Where(p => p.IsActive == false);
            else if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive == true);

            if (userGroupId.ToLower() != "systemg")
                data = data.Where(p => p.UserGroupId.ToLower() != "systemg");

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.UserId.Contains(filter.Keyword) || p.Email.Contains(filter.Keyword));

            if (filter.UserGroupId != null)
                data = data.Where(p => p.UserGroupId == filter.UserGroupId);

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public bool IsUserIdRepeat(string userId)
        {
            var data = All();
            if (!string.IsNullOrEmpty(userId))
                data = data.Where(p => p.UserId == userId);
            return data.Any();
        }

        public bool IsUserEmailRepeat(string email, Guid? userGuid = null)
        {
            var data = All();
            if (!string.IsNullOrEmpty(email))
                data = data.Where(p => p.Email == email);
            if (userGuid != null)
                data = data.Where(p => p.UserGuid != userGuid);
            return data.Any();
        }

        public bool PasswordCheck(string email, string password)
        {
            var user = FindEmail(email);
            if (user == null) return false;
            if (string.IsNullOrWhiteSpace(user.Password)) return false;
            return VerifyHashedPassword(user.Password, password);
        }

        public bool PasswordCheck(Guid userGuid, string password)
        {
            var user = FindGuid(userGuid);
            if (user == null) return false;
            return VerifyHashedPassword(user.Password, password);
        }

        public bool IsNoPassword(Guid userGuid)
        {
            var user = FindGuid(userGuid);
            if (user == null) return false;
            return string.IsNullOrWhiteSpace(user.Password);
        }

        public bool IsActive(string email)
        {
            var user = FindEmail(email);
            if (user == null) return false;
            return user.IsActive;
        }

        public List<FuncGroupDto> QueryFuncGroupForAuthorize(Guid userGuid)
        {
            var dto = new List<FuncGroupDto>();
            var user = BaseAll().AsNoTracking()
                .Include(p => p.UserGroup)
                    .ThenInclude(g => g.UserGroupFuncPrograms)
                        .ThenInclude(ugfp => ugfp.FuncProgram)
                            .ThenInclude(fp => fp.FuncGroup)
                .FirstOrDefault(p => p.UserGuid == userGuid);

            if (user?.UserGroup != null)
            {
                var funcPrograms = user.UserGroup.UserGroupFuncPrograms
                    .OrderBy(p => p.FuncProgram.FuncGroup.DisplaySeq)
                    .ThenBy(p => p.FuncProgram.DisplaySeq)
                    .ToList();

                foreach (var prg in funcPrograms)
                {
                    var funcGroup = dto.FirstOrDefault(p => p.Name == prg.FuncProgram.FuncGroup.Name);
                    if (funcGroup == null)
                    {
                        funcGroup = new FuncGroupDto
                        {
                            Name = prg.FuncProgram.FuncGroup.Name,
                            IconClass = prg.FuncProgram.FuncGroup.IconClass,
                            DisplaySeq = prg.FuncProgram.FuncGroup.DisplaySeq,
                            FuncPrograms = new List<FuncProgramDto>()
                        };
                        dto.Add(funcGroup);
                    }

                    funcGroup.FuncPrograms.Add(new FuncProgramDto
                    {
                        Name = prg.FuncProgram.Name,
                        Url = prg.FuncProgram.Url,
                        OnlyAdmin = prg.FuncProgram.OnlyAdmin,
                        DisplaySeq = prg.FuncProgram.DisplaySeq
                    });
                }
            }

            return dto;
        }

        public string GetNotRepeatUserId(string userId)
        {
            string tmpUserId = userId;
            int num = 1;
            while (All().Any(p => p.UserId == userId))
            {
                userId = $"{tmpUserId}{num++}";
            }
            return userId;
        }

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] subkey = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, 10000, HashAlgorithmName.SHA256, 32);
            byte[] output = new byte[49];
            output[0] = 0x01;
            salt.CopyTo(output, 1);
            subkey.CopyTo(output, 17);
            return Convert.ToBase64String(output);
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] decoded;
            try { decoded = Convert.FromBase64String(hashedPassword); }
            catch { return false; }
            if (decoded.Length != 49 || decoded[0] != 0x01) return false;
            byte[] salt = decoded[1..17];
            byte[] expectedSubkey = decoded[17..49];
            byte[] actualSubkey = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, 10000, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
    }
}
