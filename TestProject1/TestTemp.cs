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
        public void TestProjections()
        {
            GetIR8RDataContext.UseRealDatabase = true;
            IR8RDataContext myDataContext = GetIR8RDataContext.New();
            R8RContext dbContext = (R8RContext)((R8REFDataContext)myDataContext).UnderlyingDbContext;
            var myQuery = from bp in dbContext.MyBackpacks.Include(x => x.ContainerInBackpack)
                          select new
                          {
                              myContainer = bp.ContainerInBackpack,
                              myContainerContents = (bp.ContainerInBackpack == null) ? null : bp.ContainerInBackpack.Contents.Where(x => x.ContentsString == "Beverage"),
                          };
            string sqlString = myQuery.ToString();
            var myList = myQuery.ToList();
        }

        [TestMethod]
        public void TestProjections2()
        {
            GetIR8RDataContext.UseRealDatabase = true;
            IR8RDataContext myDataContext = GetIR8RDataContext.New();
            R8RContext dbContext = (R8RContext)((R8REFDataContext)myDataContext).UnderlyingDbContext;
            foreach (var x in dbContext.MyContents.Where(x => true).ToList())
                dbContext.MyContents.Remove(x); 
            foreach (var x in dbContext.MyContainers.Where(x => true).ToList())
                dbContext.MyContainers.Remove(x); 
            foreach (var x in dbContext.MyBackpacks.Where(x => true).ToList())
                dbContext.MyBackpacks.Remove(x);
            dbContext.SaveChanges();
            myDataContext = GetIR8RDataContext.New();
            dbContext = (R8RContext)((R8REFDataContext)myDataContext).UnderlyingDbContext;
            MyBackpack bp1 = new MyBackpack() { MyBackpackID = Guid.NewGuid(), ContainerInBackpack = new MyContainer() { MyContainerID = Guid.NewGuid(), Contents = new List<MyContainerContents>() { new MyContainerContents() { MyContainerContentsID = Guid.NewGuid(), ContentsString = "Beverage" }, new MyContainerContents() { MyContainerContentsID = Guid.NewGuid(), ContentsString = "Food1" } } } };
            MyBackpack bp2 = new MyBackpack() { MyBackpackID = Guid.NewGuid(), };
            dbContext.MyBackpacks.Add(bp1);
            dbContext.MyBackpacks.Add(bp2);
            dbContext.SaveChanges();
            myDataContext = GetIR8RDataContext.New();
            dbContext = (R8RContext)((R8REFDataContext)myDataContext).UnderlyingDbContext;
            var bps = dbContext.MyBackpacks.ToList();
            foreach (var bp in bps)
            {
                var myContainerContents = bp.ContainerInBackpack.Contents.Where(x => bp.ContainerInBackpack != null && x.ContentsString == "Beverage");
                //List<MyContainerContents> myContainerContents;
                //if (bp.ContainerInBackpackID != null)
                //{
                //    myContainerContents = bp.ContainerInBackpack.Contents.Where(x => x.ContentsString == "Beverage").ToList();
                //}
            }
        }

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
