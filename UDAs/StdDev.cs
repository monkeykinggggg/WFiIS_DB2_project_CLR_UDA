using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;

namespace UDAs
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
       Format.Native,
       IsNullIfEmpty = true,
       IsInvariantToDuplicates = false,
       IsInvariantToNulls = true,
       IsInvariantToOrder = true
    )]
    public struct StdDev
    {
        private long count;
        private double sum;
        private double squareSum;
        public void Init()
        {
            count = 0;
            sum = 0.0;
            squareSum = 0.0;
        }
        public void Accumulate(SqlDouble Value)
        {
            if (!Value.IsNull)  // ignores null values
            {
                count++;
                sum += Value.Value;
                squareSum += Value.Value * Value.Value;
            }
        }

        public void Merge(StdDev Group)
        {
            this.count += Group.count;
            this.sum += Group.sum;
            this.squareSum += Group.squareSum;
        }

        public SqlDouble Terminate()
        {
            if (count < 2)
            {
                return SqlDouble.Null;
            }
            double mean = sum / count;
            double variance = (squareSum - count * mean * mean)/(count-1) ;
            variance = (variance < 0.0) ? 0.0 : variance;
            return new SqlDouble(Math.Sqrt(variance));
        }
    }
}
