using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

namespace UDAs
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
        Format.UserDefined,
        IsNullIfEmpty = true,                
        IsInvariantToDuplicates = false,     
        IsInvariantToNulls = true,   
        IsInvariantToOrder = true,
        MaxByteSize = 8000                   
    )]
    public struct Mode : IBinarySerialize
    {
        private Dictionary<double, int> freqDict;
        public void Init()
        {
            this.freqDict = new Dictionary<double, int>();
        }
        public void Accumulate(SqlDouble number)
        {
            if (!number.IsNull)
            {
                double key = Math.Round(number.Value, 1);    //! round for better results!! If needed -> change!!!
                if (this.freqDict.ContainsKey(key))
                    this.freqDict[key]++;
                else
                    this.freqDict.Add(key, 1);
            }
        }
        public void Merge(Mode group)
        {
            foreach( var pair in group.freqDict)
            {
                if (this.freqDict.ContainsKey(pair.Key))
                {
                    this.freqDict[pair.Key]+=pair.Value;
                }
                else
                {
                    this.freqDict.Add(pair.Key, pair.Value);
                }
            }
        }
        public SqlDouble Terminate()
        {
            SqlDouble result = SqlDouble.Null;
            if( this.freqDict.Count > 0)
            {
                double firstModeKey = 0;
                int maxFreq = 0;
                bool isFirst = true;    // zmienna pomocnicza   
                foreach(var pair in this.freqDict)
                {
                    if(isFirst || pair.Value > maxFreq) // if we find bigger frequency and only, we add the next mode, if the frequency remains the same, we don't add anything
                    {   
                        firstModeKey = pair.Key;
                        maxFreq = pair.Value;
                        isFirst = false;
                    }// finally : we got first mode when tie

                }

                result = new SqlDouble(firstModeKey);
            }
            return result;
        }
        // metoda do wczytywania serializowanych danych, potrzebna bo korzystamy z typów referencyjnych wewnątrz
        #region IBinarySerialize Members
        public void Read(BinaryReader r)
        {
            this.freqDict = new Dictionary<double, int>();
            int elements = r.ReadInt32();
            for (int i = 0; i < elements; i++)
            {
                double key = r.ReadDouble();
                int value = r.ReadInt32();
                this.freqDict.Add(key, value);
            }
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.freqDict.Count);
            foreach (var pair in this.freqDict)
            {
                w.Write(pair.Key);
                w.Write(pair.Value);
            }
        }
        #endregion
    }
}
