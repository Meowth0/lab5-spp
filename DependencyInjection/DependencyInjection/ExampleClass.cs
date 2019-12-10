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
    public class MyRepository : IRepository
    {
        public int GetNum()
        {
            return 111;
        }
    }

    public class ServiceImpl<TRepository> where TRepository : IRepository
    {
        TRepository rep;

        public ServiceImpl(TRepository repository)
        {
            rep = repository;
        }

        public int GetNum()
        {
            return rep.GetNum();
        }
    }
}
