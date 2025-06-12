using System.Data.SqlTypes;
using UDAs;

namespace UDA_Tests
{
    [TestClass]
    public sealed class ModeTests
    {
        public Mode agg;
        [TestInitialize]
        public void InitialTestData()
        {
            agg = new Mode();
            agg.Init();
        }

        [TestMethod]
        public void IgnoresNulls()
        {
            agg.Accumulate(SqlDouble.Null);
            agg.Accumulate(new SqlDouble(25.26));
            agg.Accumulate(SqlDouble.Null);

            Assert.AreEqual(25.26, agg.Terminate().Value);
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
        public void NoDuplicates_ReturnsFirstValue()
        {
            agg.Accumulate(new SqlDouble(7.20));
            agg.Accumulate(new SqlDouble(7.11));
            agg.Accumulate(new SqlDouble(2.10));
            Assert.AreEqual(7.20, agg.Terminate().Value);
        }

        [TestMethod]
        public void WithDuplicates_ReturnsMostFrequent()
        {
            agg.Accumulate(new SqlDouble(7.26));
            agg.Accumulate(new SqlDouble(7.26));
            agg.Accumulate(new SqlDouble(2.10));
            Assert.AreEqual(7.26, agg.Terminate().Value);
        }

        [TestMethod]
        public void WithTies_ReturnsFirstMode()
        {
            agg.Accumulate(new SqlDouble(7.26));
            agg.Accumulate(new SqlDouble(7.26));
            agg.Accumulate(new SqlDouble(2.10));
            agg.Accumulate(new SqlDouble(2.10));
            agg.Accumulate(new SqlDouble(2.11));
            Assert.AreEqual(7.26, agg.Terminate().Value);
        }

        [TestMethod]
        public void RoundingWorks()
        {
            agg.Accumulate(new SqlDouble(1.001));
            agg.Accumulate(new SqlDouble(1.002)); 
            agg.Accumulate(new SqlDouble(1.003)); 
            Assert.AreEqual(1.00, agg.Terminate().Value);
        }

        [TestMethod]
        public void Merge_TwoAggregates_CombinesFreqCorrectly()
        {
            agg.Accumulate(new SqlDouble(1.00));
            agg.Accumulate(new SqlDouble(1.00));

            Mode agg2 = new Mode();
            agg2.Init();
            agg2.Accumulate(new SqlDouble(1.00));
            agg2.Accumulate(new SqlDouble(2.00));
            agg.Merge(agg2);

            Assert.AreEqual(1.00, agg.Terminate().Value);
        }

        [TestMethod]
        public void SerializationPreservesData()
        {
            agg.Accumulate(new SqlDouble(1.50));
            agg.Accumulate(new SqlDouble(1.50));
            agg.Accumulate(new SqlDouble(3.50));

            byte[] serialized;
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                agg.Write(writer);
                serialized = ms.ToArray();
            }

            var deserialized = new Mode();
            using (var ms = new MemoryStream(serialized))
            using (var reader = new BinaryReader(ms))
            {
                deserialized.Read(reader);
            }

            Assert.AreEqual(1.50, deserialized.Terminate().Value);
        }

    }
}
