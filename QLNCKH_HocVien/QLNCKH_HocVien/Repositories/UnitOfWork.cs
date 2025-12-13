using Microsoft.EntityFrameworkCore.Storage;
using QLNCKH_HocVien.Data;

namespace QLNCKH_HocVien.Repositories
{
    /// <summary>
    /// Unit of Work Implementation - Quản lý tất cả repositories và transaction
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization của repositories
        private ISinhVienRepository? _sinhViens;
        private IGiaoVienRepository? _giaoViens;
        private IChuyenDeRepository? _chuyenDes;
        private INopSanPhamRepository? _nopSanPhams;
        private IHoiDongRepository? _hoiDongs;
        private IKetQuaRepository? _ketQuas;
        private IXepGiaiRepository? _xepGiais;
        private INguoiDungRepository? _nguoiDungs;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ISinhVienRepository SinhViens
            => _sinhViens ??= new SinhVienRepository(_context);

        public IGiaoVienRepository GiaoViens
            => _giaoViens ??= new GiaoVienRepository(_context);

        public IChuyenDeRepository ChuyenDes
            => _chuyenDes ??= new ChuyenDeRepository(_context);

        public INopSanPhamRepository NopSanPhams
            => _nopSanPhams ??= new NopSanPhamRepository(_context);

        public IHoiDongRepository HoiDongs
            => _hoiDongs ??= new HoiDongRepository(_context);

        public IKetQuaRepository KetQuas
            => _ketQuas ??= new KetQuaRepository(_context);

        public IXepGiaiRepository XepGiais
            => _xepGiais ??= new XepGiaiRepository(_context);

        public INguoiDungRepository NguoiDungs
            => _nguoiDungs ??= new NguoiDungRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

