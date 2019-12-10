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

    public interface IExample
    {
        void Print();
    }

    public class ExampleClass1 : IExample
    {
        public ExampleClass2 example { get; set; }

        public ExampleClass1(ExampleClass2 example)
        {
            this.example = example;
        }

        public ExampleClass1()
        {
        }

        public void Print()
        {
            Console.WriteLine("1 -> 2");
            if (example != null)
                example.Print();
        }
    }

    public class ExampleClass2 : IExample
    {
        public ExampleClass3 example { get; set; }

        public ExampleClass2(ExampleClass3 example)
        {
            this.example = example;
        }

        public ExampleClass2()
        {
        }

        public void Print()
        {
            Console.WriteLine("2 -> 3");
            if (example != null)
                example.Print();
        }
    }

    public class ExampleClass3
    {
        public ExampleClass1 example { get; set; }

        public ExampleClass3(ExampleClass1 example)
        {
            this.example = example;
        }

        public ExampleClass3()
        {
        }

        public void Print()
        {
            Console.WriteLine("3 -> 1");
        }
    }
}
