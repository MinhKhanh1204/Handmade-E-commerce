using BussinessObject;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAccountService
    {
        Account Login(LoginDTO account);
		bool Register(RegisterDTO account);
	}
}
