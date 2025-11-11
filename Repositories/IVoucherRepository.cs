using BussinessObject;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public interface IVoucherRepository
    {
        IQueryable<Voucher> GetAllVouchers();
        Voucher? GetVoucherById(int voucherId);
        List<Voucher> GetActiveVouchers();
    }
}

