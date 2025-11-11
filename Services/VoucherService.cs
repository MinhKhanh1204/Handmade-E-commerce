using DTO;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public List<VoucherDTO> GetAllVouchers()
        {
            var vouchers = _voucherRepository.GetAllVouchers()
                .OrderByDescending(v => v.ExpiryDate)
                .ToList();

            return vouchers.Select(v => VoucherDTO.FromEntity(v)).ToList();
        }

        public VoucherDTO? GetVoucherById(int voucherId)
        {
            var voucher = _voucherRepository.GetVoucherById(voucherId);
            if (voucher == null)
                return null;

            return VoucherDTO.FromEntity(voucher);
        }

        public List<VoucherDTO> GetActiveVouchers()
        {
            var vouchers = _voucherRepository.GetActiveVouchers();
            return vouchers.Select(v => VoucherDTO.FromEntity(v)).ToList();
        }
    }
}

