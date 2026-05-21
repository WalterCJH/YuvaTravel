using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Base
{
    public class AdminNavBarDto
    {
        public List<AdminNavMain> AdminNavMains { get; set; }

        public string DashBoard { get; set; }

        public string UserName { get; set; }

        public AdminNavBarDto()
        {
            AdminNavMains = new List<AdminNavMain>();
        }

        public AdminNavBarDto(string path, List<FuncGroupDto> funcGroups,string userName)
        {
            UserName = userName;

            bool isDashboard = true;
            AdminNavMains = new List<AdminNavMain>();

            foreach (var funcGroup in funcGroups)
            {
                AdminNavMain navMain = new AdminNavMain();
                AdminNavMains.Add(navMain);
                navMain.Name = funcGroup.Name;
                navMain.IconClass = funcGroup.IconClass;

                foreach (var funcProgram in funcGroup.FuncPrograms)
                {
                    AdminNavSub navSub = new AdminNavSub();
                    navMain.AdminNavSubs.Add(navSub);
                    navSub.Name = funcProgram.Name;
                    navSub.Url = funcProgram.Url;

                    if (path.Contains(navSub.Url))
                    {
                        navMain.Status = "active";
                        navSub.Status = "active";
                        isDashboard = false;
                    }
                }
            }

            if (isDashboard)
            {
                DashBoard = "active";
            }
        }
    }

    public class AdminNavMain
    {
        public string Status { get; set; }
        public string OpenStatus { get { if (AdminNavSubs.Any(p => !string.IsNullOrWhiteSpace(p.Status))) return "menu-open"; else return ""; } }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public List<AdminNavSub> AdminNavSubs { get; set; }
        public AdminNavMain()
        {
            AdminNavSubs = new List<AdminNavSub>();
        }
    }

    public class AdminNavSub
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
