using ClassLibrary1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestProject1;
using System.Data.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestRealDatabase();
            test.TestMemoryLeaks();
            //var test = new HeterogeneousUserTests();
            //test.Initialize();
            //test.AbsoluteErrorWithinBoundsWhenHeterogeneousUsersRateIterationsofTblRows();
        }
    }
}
