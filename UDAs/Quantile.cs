using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;

namespace UDAs
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
        Format.UserDefined,
        IsNullIfEmpty = true,                
        IsInvariantToDuplicates = false,    
        IsInvariantToNulls = true,
        IsInvariantToOrder = false,
        MaxByteSize = 8000                   
    )]
    public struct Quantile : IBinarySerialize
    {
        private List<double> values;
        private double quantile;
        public void Init()
        {
            this.values = new List<double>();
            this.quantile = 0.5;    // by default
        }
        public void Accumulate(SqlDouble number, SqlDouble quantilePrompt)
        {
            if (!number.IsNull)
            {
                this.values.Add(number.Value);
                if (!quantilePrompt.IsNull)
                {
                    this.quantile = quantilePrompt.Value;
                }
            }
        }
        public void Merge(Quantile group)
        {
            this.values.AddRange(group.values);
            this.quantile = group.quantile;             // we take the quantile from the other group
        }
        public SqlDouble Terminate()
        {
            SqlDouble result = SqlDouble.Null;
            if (this.values.Count > 0)
            {
                this.values.Sort();
                double q = Math.Max(0.0, Math.Min(1.0, this.quantile)); // validating quantile range
                int idx = (int)Math.Round((q * (this.values.Count-1)));
                result = this.values[idx];
            }
            return result;
        }

        public void Read(BinaryReader r)
        {
            int elements = r.ReadInt32();
            this.values = new List<double>();
            for(int i = 0; i < elements; i++)
            {
                this.values.Add(r.ReadDouble());
            }
            this.quantile = r.ReadDouble();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(values.Count);
            foreach(double value in values)
            {
                w.Write(value);
            }
            w.Write(quantile);
        }
    }
}
