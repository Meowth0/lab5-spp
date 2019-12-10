using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
 
    public interface IRepository
    {
        int GetNum();
    }
    public class MySQLRepository : IRepository
    {
        public int GetNum()
        {
            return 111;
        }
    }
}
