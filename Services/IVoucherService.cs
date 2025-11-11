using DTO;
using System.Collections.Generic;

namespace Services
{
    public interface IVoucherService
    {
        List<VoucherDTO> GetAllVouchers();
        VoucherDTO? GetVoucherById(int voucherId);
        List<VoucherDTO> GetActiveVouchers();
    }
}

