using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;

namespace UDAs
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
       Format.Native,
       IsNullIfEmpty = true,           
       IsInvariantToDuplicates = true,  
       IsInvariantToNulls = true,
       IsInvariantToOrder = true
   )]
    public struct Range
    {
        private double min, max;
        private bool hasVals;
        public void Init()
        {
            this.min = double.MaxValue;
            this.max = double.MinValue;
            this.hasVals = false;
        }
        public void Accumulate(SqlDouble number)
        {
            if (!number.IsNull)
            {
                if (number.Value < this.min)
                {
                    this.min = number.Value;
                }
                if (number.Value > this.max)
                {
                    this.max = number.Value;
                }
                this.hasVals = true;

            }
        }
        public void Merge(Range group)
        {
            if (group.min < this.min)
            {
                this.min = group.min;
            }
            if (group.max > this.max)
            {
                this.max = group.max;
            }
            this.hasVals = group.hasVals || this.hasVals;
        }
        public SqlDouble Terminate()
        {
            SqlDouble result = SqlDouble.Null;
            if (this.hasVals)
            {
                double value = Math.Round(max - min, 2);
                result = new SqlDouble(value);
            }
            return result;
        }
    }
}
