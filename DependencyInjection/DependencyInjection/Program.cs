using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            DependenciesConfig c = new DependenciesConfig();
            c.Register<MyRepository, MyRepository>(false);
            c.Register(typeof(ServiceImpl<>), typeof(ServiceImpl<>), false);
            try
            {
                DependencyProvider p = new DependencyProvider(c);
                ServiceImpl<MyRepository> example = p.Resolve<ServiceImpl<MyRepository>>();
                Console.WriteLine(example.GetNum());
                Console.ReadLine();
                Console.ReadLine();
                Console.ReadLine();
                Console.ReadLine();
            }
            catch (ConfigurationValidationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ConstructorNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (CycleDependencyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (TypeNotRegisterException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
