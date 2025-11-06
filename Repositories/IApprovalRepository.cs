using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IApprovalRepository
    {
        Task UpdateStatusAsync(string entityType, string id, string newStatus, string approvedBy);
    }


}
