using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

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
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.ServiceHosting.Tools.DevelopmentStorage;
using Microsoft.ServiceHosting.Tools.DevelopmentFabric;
using System.Threading;
using System.Diagnostics;


namespace TestProject1
{
    [TestClass]
    public class TestTemp
    {
        [TestMethod]
        public void TestLoad()
        {
            var dc = GetIR8RDataContext.New();
            Guid firstRating = dc.GetTable<Rating>().OrderBy(x => x.RatingGroup.WhenCreated).First().RatingID;
            User admin = dc.GetTable<User>().Where(x => x.SuperUser).First();
            RatingsAndRelatedInfoLoader.Load(dc, new List<Guid> { firstRating }, admin);
        }
    }
}
