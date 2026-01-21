using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUserService
    {
        Task<List<User>> SearchAsync(string term);
        Task<User?> GetByIdAsync(string id);
    }
}
