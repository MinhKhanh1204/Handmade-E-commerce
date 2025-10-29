using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly MyStoreContext _context;

        public FeedbackRepository(MyStoreContext context)
        {
            _context = context;
        }
    }
}
