// <copyright file="PexAssemblyInfo.cs">Copyright ©  2010</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Moles;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;
using Microsoft.Pex.Linq;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("ClassLibrary1")]
[assembly: PexInstrumentAssembly("Microsoft.WindowsAzure.ServiceRuntime")]
[assembly: PexInstrumentAssembly("System.Configuration")]
[assembly: PexInstrumentAssembly("System.Data.Services.Client")]
[assembly: PexInstrumentAssembly("System.Transactions")]
[assembly: PexInstrumentAssembly("System.Runtime.Serialization")]
[assembly: PexInstrumentAssembly("Microsoft.WindowsAzure.CloudDrive")]
[assembly: PexInstrumentAssembly("mapquest20")]
[assembly: PexInstrumentAssembly("GMapGeocoder")]
[assembly: PexInstrumentAssembly("Microsoft.ServiceHosting.Tools")]
[assembly: PexInstrumentAssembly("System.Xml.Linq")]
[assembly: PexInstrumentAssembly("System.Web.DataVisualization")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("System.Web")]
[assembly: PexInstrumentAssembly("System.Data.Linq")]
[assembly: PexInstrumentAssembly("Microsoft.WindowsAzure.StorageClient")]
[assembly: PexInstrumentAssembly("System.Web.ApplicationServices")]
[assembly: PexInstrumentAssembly("Microsoft.SqlServer.Types")]
[assembly: PexInstrumentAssembly("System.Web.Extensions")]
[assembly: PexInstrumentAssembly("System.Data")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.WindowsAzure.ServiceRuntime")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Configuration")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Data.Services.Client")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Transactions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime.Serialization")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.WindowsAzure.CloudDrive")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "mapquest20")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "GMapGeocoder")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.ServiceHosting.Tools")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Xml.Linq")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Web.DataVisualization")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Web")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Data.Linq")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.WindowsAzure.StorageClient")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Web.ApplicationServices")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.SqlServer.Types")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Web.Extensions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Data")]

// Microsoft.Pex.Framework.Moles
[assembly: PexAssumeContractEnsuresFailureAtBehavedSurface]
[assembly: PexChooseAsBehavedCurrentBehavior]

// Microsoft.Pex.Linq
[assembly: PexLinqPackage]

