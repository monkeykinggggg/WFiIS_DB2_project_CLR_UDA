using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using UDAs;

namespace UDA_Tests
{
    [TestClass]
    public sealed class MedianTests
    {
        public Median agg;
        [TestInitialize]
        public void InitialTestData()
        {
            agg = new Median();
            agg.Init();
        }

        [TestMethod]
        public void OddNumberOfValues_ReturnsMiddleValue()
        {
            agg.Accumulate(new SqlDouble(30.75));
            agg.Accumulate(new SqlDouble(10.50));
            agg.Accumulate(new SqlDouble(20.25));

            Assert.AreEqual(20.25, agg.Terminate().Value);
        }

        [TestMethod]
        public void EvenCount_ReturnsAverageOfMiddleTwo()
        {
            agg.Accumulate(new SqlDouble(30.41));
            agg.Accumulate(new SqlDouble(10.10));
            agg.Accumulate(new SqlDouble(20.21));
            agg.Accumulate(new SqlDouble(40.45));

            // Assert (20.21 + 30.41) / 2 = 25.31
            Assert.AreEqual(25.31, agg.Terminate().Value);
        }

        [TestMethod]
        public void IgnoresNulls()
        {
            agg.Accumulate(SqlDouble.Null);
            agg.Accumulate(new SqlDouble(15.75));
            agg.Accumulate(new SqlDouble(25.26));
            agg.Accumulate(SqlDouble.Null);

            // Assert (15.75 + 25.65) / 2 = 20.505
            Assert.AreEqual(20.505, agg.Terminate().Value);
        }

        [TestMethod]
        public void EmptyInput_ReturnsNull()
        {
            var result = agg.Terminate();
            Assert.IsTrue(result.IsNull);
        }


        [TestMethod]
        public void SingleValue_ReturnsThatValue()
        {
            agg.Accumulate(new SqlDouble(12.24));
            Assert.AreEqual(12.24, agg.Terminate().Value);
        }

        [TestMethod]
        public void NegativeValues_CalculatesCorrectly()
        {
            agg.Accumulate(new SqlDouble(-10.50));
            agg.Accumulate(new SqlDouble(-20.25));
            agg.Accumulate(new SqlDouble(-20.27));
            agg.Accumulate(new SqlDouble(-30.75));
            // Assert (-20.25 -20.27) / 2 = -20.26
            Assert.AreEqual(-20.26, agg.Terminate().Value);
        }

        [TestMethod]
        public void SmallAndLargeValues_CalculatesCorrectly()
        {
            agg.Accumulate(new SqlDouble(0.01));
            agg.Accumulate(new SqlDouble(99999.99));
            // Assert (99999.99 + 0.01) / 2 = 50 000
            Assert.AreEqual(50000, agg.Terminate().Value);
        }

        [TestMethod]
        public void Merge_TwoAggregates_CombinesValuesCorrectly()
        {
            agg.Accumulate(new SqlDouble(20.25));
            agg.Accumulate(new SqlDouble(10.11));

            Median agg2 = new Median();
            agg2.Init();
            agg2.Accumulate(new SqlDouble(30.38));
            agg2.Accumulate(new SqlDouble(40.49));

            agg.Merge(agg2);

            // [10.11, 20.25, 30.38, 40.49] -> Assert (20.25 + 30.38) / 2 = 25.315
            Assert.AreEqual(25.315, agg.Terminate().Value);
        }

        [TestMethod]
        public void SerializationPreservesData()
        {
            agg.Accumulate(new SqlDouble(15.75));
            agg.Accumulate(new SqlDouble(25.25));

            byte[] serialized;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                agg.Write(writer);
                serialized = ms.ToArray();
            }

            var deserialized = new Median();
            using (var ms = new MemoryStream(serialized))
            using (var reader = new BinaryReader(ms))
            {
                deserialized.Read(reader);
            }

            Assert.AreEqual(20.50, deserialized.Terminate().Value);
        }

    }
}
