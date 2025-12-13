using System.Linq.Expressions;

namespace QLNCKH_HocVien.Repositories
{
    /// <summary>
    /// Generic Repository Interface - Cung cấp các thao tác CRUD cơ bản
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // Query
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        // Pagination
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        // Commands
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Save
        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// Unit of Work Interface - Quản lý transaction và các repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        ISinhVienRepository SinhViens { get; }
        IGiaoVienRepository GiaoViens { get; }
        IChuyenDeRepository ChuyenDes { get; }
        INopSanPhamRepository NopSanPhams { get; }
        IHoiDongRepository HoiDongs { get; }
        IKetQuaRepository KetQuas { get; }
        IXepGiaiRepository XepGiais { get; }
        INguoiDungRepository NguoiDungs { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

