using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    abstract class ColumnDecoder
    {
        protected uint Ordinal { get; private set; }

        internal void SetOrdinal(uint ordinal)
        {
            Ordinal = ordinal;
        }

        public abstract int RowCount { get; }

        public abstract string GetDataTypeName();

        public abstract Type GetFieldType();

        internal abstract void NewBatch(Memory<byte> memory);

        internal abstract void DecodeValue(ref Utf8JsonReader utf8JsonReader);

        public virtual string GetString(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a string value");
        }

        public abstract void AppendOffset();

        public abstract void WriteJson(in int index, in Utf8JsonWriter jsonWriter);

        public abstract object GetFieldValue(in int index, Type type);

        public abstract object GetValue(in int index);

        public abstract bool IsDbNull(in int index);

        public virtual bool GetBoolean(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a boolean value");
        }

        public virtual int GetInt32(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a int32 value");
        }

        public virtual short GetInt16(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a int16 value");
        }

        public virtual long GetInt64(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a int64 value");
        }

        public virtual byte GetByte(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a byte value");
        }

        public virtual float GetFloat(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a float value");
        }

        public virtual double GetDouble(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a double value");
        }

        public virtual decimal GetDecimal(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a decimal value");
        }

        public virtual DateTime GetDateTime(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a DateTime value.");
        }

        public virtual T GetFieldValue<T>(in int index)
        {
            return (T)GetFieldValue(index, typeof(T));
        }

        public virtual Guid GetGuid(in int index)
        {
            throw new NotSupportedException($"Column with ordinal {Ordinal} and type {GetDataTypeName()} cant get a guid value");
        }
    }
}
