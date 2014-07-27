using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
#else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif

using FluentAssertions;
using ClassLibrary1.Nonmodel_Code;

namespace TestProject1
{
    [TestClass]
    public class TestDictionaryByType
    {
        [TestMethod]
        public void DictionaryByTypeWorks()
        {
            DictionaryByType dbt = new DictionaryByType();
            dbt.Add(3);
            dbt.Add(4);
            dbt.Add("hello");
            dbt.Add(dbt);
            dbt.GetCollectionOfType<DictionaryByType>().Count().Should().Equals(1);
            dbt.GetCollectionOfType<int>().Count().Should().Equals(2);
            dbt.GetCollectionOfType<string>().Count().Should().Equals(1);
            dbt.GetCollectionOfType<List<string>>().Count().Should().Equals(0);
        }
    }
}
