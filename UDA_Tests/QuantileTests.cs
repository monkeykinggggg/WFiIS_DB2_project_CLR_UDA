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
    public sealed class QuantileTests
    {
        public Quantile agg;
        [TestInitialize]
        public void InitialTestData()
        {
            agg = new Quantile();
            agg.Init();
        }

        [TestMethod]
        public void EmptyInput_ReturnsNull()
        {
            Assert.IsTrue(agg.Terminate().IsNull);
        }

        [TestMethod]
        public void NullValues_IgnoresThem()
        {
            agg.Accumulate(SqlDouble.Null, new SqlDouble(0.6));
            agg.Accumulate(new SqlDouble(10.00), new SqlDouble(0.6));
            agg.Accumulate(SqlDouble.Null, new SqlDouble(0.6));
            Assert.AreEqual(10.00, agg.Terminate().Value);
        }

        [TestMethod]
        public void SingleValue_ReturnsThatValue()
        {
            agg.Accumulate(new SqlDouble(10.5), SqlDouble.Null);
            Assert.AreEqual(10.5, agg.Terminate().Value);
        }

        [TestMethod]
        public void MedianQuantile_ByDefault()
        {
            agg.Accumulate(new SqlDouble(10.56), SqlDouble.Null);
            agg.Accumulate(new SqlDouble(20.99), SqlDouble.Null);
            agg.Accumulate(new SqlDouble(30.23), SqlDouble.Null);
            Assert.AreEqual(20.99, agg.Terminate().Value);
        }

        [TestMethod]
        public void ZeroQuantile_ReturnsMinimum()
        {
            agg.Accumulate(new SqlDouble(10.56), new SqlDouble(0.0));
            agg.Accumulate(new SqlDouble(20.99), new SqlDouble(0.0));
            agg.Accumulate(new SqlDouble(30.23), new SqlDouble(0.0));
            Assert.AreEqual(10.56, agg.Terminate().Value);
        }

        [TestMethod]
        public void OneQuantile_ReturnsMaximum()
        {
            agg.Accumulate(new SqlDouble(10.56), new SqlDouble(1.0));
            agg.Accumulate(new SqlDouble(20.99), new SqlDouble(1.0));
            agg.Accumulate(new SqlDouble(30.23), new SqlDouble(1.0));
            Assert.AreEqual(30.23, agg.Terminate().Value);
        }


        [TestMethod]
        public void AverageFlow()
        {
            agg.Accumulate(new SqlDouble(1.50), new SqlDouble(0.25));
            agg.Accumulate(new SqlDouble(0.50), new SqlDouble(0.25));
            agg.Accumulate(new SqlDouble(3.50), new SqlDouble(0.25));
            agg.Accumulate(new SqlDouble(0.20), new SqlDouble(0.25));
            // [0.2, 0.5, 1.5, 3.5] -> q0.25 -> 0.50
            Assert.AreEqual(0.50, agg.Terminate().Value);
        }

        [TestMethod]
        public void AfterRangeChange_QuantileChanges()
        {
            agg.Accumulate(new SqlDouble(10.44), new SqlDouble(0.0));
            agg.Accumulate(new SqlDouble(20.89), new SqlDouble(1.0));  // changes quantile from 0.0 to 1.0
            agg.Accumulate(new SqlDouble(30.11), SqlDouble.Null);      // keeps last quantile
            Assert.AreEqual(30.11, agg.Terminate().Value);
        }

        [TestMethod]
        public void Merge_TwoAggregates_CombinesCorrectly()
        {
            agg.Accumulate(new SqlDouble(20), new SqlDouble(0.5));
            agg.Accumulate(new SqlDouble(30), new SqlDouble(0.5));

            Quantile agg2 = new Quantile();
            agg2.Init();
            agg2.Accumulate(new SqlDouble(1), new SqlDouble(0.0));
            agg2.Accumulate(new SqlDouble(50), new SqlDouble(0.0));
            agg.Merge(agg2);
            // [1,20,30,50] -> q0 -> 1
            Assert.AreEqual(1.00, agg.Terminate().Value);
        }

        [TestMethod]
        public void SerializationPreservesData()
        {
            agg.Accumulate(new SqlDouble(1.50), new SqlDouble(0.75));
            agg.Accumulate(new SqlDouble(0.50), new SqlDouble(0.75));
            agg.Accumulate(new SqlDouble(3.50), new SqlDouble(0.75));
            agg.Accumulate(new SqlDouble(0.20), new SqlDouble(0.75));

            byte[] serialized;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                agg.Write(writer);
                serialized = ms.ToArray();
            }

            var deserialized = new Quantile();
            using (var ms = new MemoryStream(serialized))
            using (var reader = new BinaryReader(ms))
            {
                deserialized.Read(reader);
            }
            // [0.2, 0.5, 1.5, 3.5] -> q0.75 -> 1.50
            Assert.AreEqual(1.50, deserialized.Terminate().Value);
        }
    }
}
