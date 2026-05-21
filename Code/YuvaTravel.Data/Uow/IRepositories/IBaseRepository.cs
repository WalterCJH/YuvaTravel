using System;
using System.Linq;
using System.Linq.Expressions;

namespace YuvaTravel.Data.Uow.IRepositories
{
    /// <summary>
    /// 新版 Repository 基底介面 (Phase B 漸進遷移)。
    /// 不再持有 IUnitOfWork 參考 — commit 統一在 IUnitOfWork.CommitAsync 進行。
    /// </summary>
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> BaseAll();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void Delete(T entity);
    }
}
