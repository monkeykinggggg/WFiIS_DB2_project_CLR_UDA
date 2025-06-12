using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Server;

namespace UDAs
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
        Format.UserDefined,
        IsNullIfEmpty = true,               // no values are given, then result is null
        IsInvariantToDuplicates = false,     // when getting duplicates doesn't change the result
        IsInvariantToNulls = true,          // receiving null doesn't change the result
        IsInvariantToOrder = false,
        MaxByteSize = 8000                  // musi byc okreslone przy serializacji UserDefined
    )] 
    [StructLayout(LayoutKind.Sequential)]   // member objs in order whe exported
    public struct Median : IBinarySerialize
    {
        private List<double> temp;
        public void Init()
        {
            this.temp = new List<double>();
        }
        public void Accumulate(SqlDouble number)
        {
            if (!number.IsNull)
            {
                this.temp.Add(number.Value);
            }
        }
        public void Merge(Median group)
        {
            this.temp.InsertRange(this.temp.Count, group.temp);  // inserting the collection at specified index (1st argument)
        }
        public SqlDouble Terminate()
        {
            SqlDouble result = SqlDouble.Null;
            if (this.temp.Count > 0)
            {
                this.temp.Sort();
                int count = this.temp.Count;
                double medianValue;
                if(count%2 == 1)
                {
                    medianValue = this.temp[count / 2];
                }
                else
                {
                    int first = count / 2 - 1;
                    int second = first + 1;
                    medianValue = (this.temp[first] + this.temp[second]) / 2.0;
                }
                medianValue = Math.Round(medianValue, 3);
                result = new SqlDouble(medianValue);
            }

            return result;
        }
        // metoda do wczytywania serializowanych danych, potrzebna bo korzystamy z typów referencyjnych wewnątrz
        #region IBinarySerialize Members
        public void Read(BinaryReader r)
        {
            this.temp = new List<double>();
            int elements = r.ReadInt32();
            for(int i = 0; i < elements; i++)
            {
                this.temp.Add(r.ReadDouble());
            }
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.temp.Count);
            foreach (double value in this.temp)
            {
                w.Write(value);
            }
        }
        #endregion
    }
}
