// <copyright file="PMAdjustmentFactorTest.cs">Copyright ©  2010</copyright>
using System;
using ClassLibrary1.Model;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassLibrary1.Model
{
    /// <summary>This class contains parameterized unit tests for PMAdjustmentFactor</summary>
    [PexClass(typeof(PMAdjustmentFactor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class PMAdjustmentFactorTest
    {
        /// <summary>Test stub for CalculateAdjustmentFactor(Decimal, Decimal, Decimal, Nullable`1&lt;Decimal&gt;)</summary>
        [PexMethod]
        public float CalculateAdjustmentFactor(
            decimal adjustedValue,
            decimal value,
            decimal basisValue,
            decimal? logBase
        )
        {
            float result = PMAdjustmentFactor.CalculateAdjustmentFactor
                               (adjustedValue, value, basisValue, logBase);
            return result;
            // TODO: add assertions to method PMAdjustmentFactorTest.CalculateAdjustmentFactor(Decimal, Decimal, Decimal, Nullable`1<Decimal>)
        }
    }
}
