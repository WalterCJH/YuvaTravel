using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    /// <summary>
    /// 新版 Repository 基底實作。直接持有 DbContext,不再透過 IUnitOfWork 取得。
    /// </summary>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly YuvaTravelDbContext _db;

        protected BaseRepository(YuvaTravelDbContext db)
        {
            _db = db;
        }

        private DbSet<T> _objectset;

        private DbSet<T> ObjectSet
        {
            get
            {
                if (_objectset == null)
                {
                    _objectset = _db.Set<T>();
                }
                return _objectset;
            }
        }

        public virtual IQueryable<T> BaseAll() => ObjectSet.AsQueryable();

        public IQueryable<T> Where(Expression<Func<T, bool>> expression) => ObjectSet.Where(expression);

        public virtual void Add(T entity) => ObjectSet.Add(entity);

        public virtual void Delete(T entity) => ObjectSet.Remove(entity);
    }
}
