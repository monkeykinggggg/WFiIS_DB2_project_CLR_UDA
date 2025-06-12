using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDAs;

namespace UDA_Tests
{
    [TestClass]
    public sealed class StdDevTests
    {
        private StdDev agg;

        [TestInitialize]
        public void Setup()
        {
            agg = new StdDev();
            agg.Init();
        }

        [TestMethod]
        public void Empty_ReturnsNull()
        {
            var result = agg.Terminate();
            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void SingleValue_ReturnsNull()
        {
            agg.Accumulate(new SqlDouble(10.0));
            var result = agg.Terminate();
            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void MultipleValues_CorrectStdDev()
        {
            agg.Accumulate(new SqlDouble(2.54));
            agg.Accumulate(new SqlDouble(4.66));
            agg.Accumulate(new SqlDouble(4.67));
            agg.Accumulate(new SqlDouble(4.3));
            agg.Accumulate(new SqlDouble(5.99));
            agg.Accumulate(new SqlDouble(5.1));
            agg.Accumulate(new SqlDouble(7.0));
            agg.Accumulate(new SqlDouble(9.77));

            var result = agg.Terminate();
            Assert.AreEqual(2.15403, Math.Round(result.Value,5));   // from wolfram alpha
        }

        [TestMethod]
        public void IgnoresNulls()
        {
            agg.Accumulate(SqlDouble.Null);
            agg.Accumulate(new SqlDouble(4.0));
            agg.Accumulate(SqlDouble.Null);
            agg.Accumulate(new SqlDouble(6.0));

            var result = agg.Terminate();
            Assert.AreEqual(1.41421, Math.Round(result.Value, 5));  // from wolfram alpha
        }

        [TestMethod]
        public void Merge_WorksCorrectly()
        {
            agg.Accumulate(new SqlDouble(1.0));
            agg.Accumulate(new SqlDouble(2.0));
            agg.Accumulate(new SqlDouble(3.0));

            StdDev agg2 = new StdDev();
            agg2.Init();
            agg2.Accumulate(new SqlDouble(4.0));
            agg2.Accumulate(new SqlDouble(5.0));
            agg2.Accumulate(new SqlDouble(6.5));

            agg.Merge(agg2);
            var result = agg.Terminate();

            Assert.AreEqual(2.01039, Math.Round(result.Value, 5));  // from wolfram alpha
        }
    }
}
