using System.Data.SqlTypes;
using UDAs;

namespace UDA_Tests
{
    [TestClass]
    public sealed class RangeTests
    {
        public UDAs.Range agg;

        [TestInitialize]
        public void InitialTestData()
        {
            agg = new UDAs.Range();
            agg.Init();
        }

        [TestMethod]
        public void IgnoresNulls()
        {
            agg.Accumulate(SqlDouble.Null);
            agg.Accumulate(new SqlDouble(25.26));
            agg.Accumulate(SqlDouble.Null);

            Assert.AreEqual(0.0, agg.Terminate().Value);
        }

        [TestMethod]
        public void EmptyInput_ReturnsNull()
        {
            var result = agg.Terminate();
            Assert.IsTrue(result.IsNull);
        }


        [TestMethod]
        public void SingleValue_ReturnsZero()
        {
            agg.Accumulate(new SqlDouble(12.24));
            Assert.AreEqual(0.0, agg.Terminate().Value);
        }

        [TestMethod]
        public void TwoValues_ReturnsCorrectRange()
        {
            agg.Accumulate(new SqlDouble(7.20));
            agg.Accumulate(new SqlDouble(7.11));
            Assert.AreEqual(0.09, agg.Terminate().Value);
        }

        [TestMethod]
        public void Merge_TwoAggregates_CombinesCorrectly()
        {
            agg.Accumulate(new SqlDouble(1.00));
            agg.Accumulate(new SqlDouble(5.00));

            UDAs.Range agg2 = new UDAs.Range();
            agg2.Init();
            agg2.Accumulate(new SqlDouble(2.00));
            agg2.Accumulate(new SqlDouble(10.00));
            agg.Merge(agg2);

            Assert.AreEqual(9.00, agg.Terminate().Value);
        }

        [TestMethod]
        public void Merge_EmptyRange_CombinesCorrectly()
        {
            agg.Accumulate(new SqlDouble(1.00));
            agg.Accumulate(new SqlDouble(5.00));

            UDAs.Range agg2 = new UDAs.Range();
            agg2.Init();
            agg.Merge(agg2);

            Assert.AreEqual(4.00, agg.Terminate().Value);
        }

        [TestMethod]
        public void AllSameValues_ReturnZero()
        {
            agg.Accumulate(new SqlDouble(1.50));
            agg.Accumulate(new SqlDouble(1.50));
            agg.Accumulate(new SqlDouble(1.50));
            Assert.AreEqual(0.00, agg.Terminate().Value);
        }

    }
}
