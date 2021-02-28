using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Tests
{
    class DataTypeTests : FunctionalTestsBase
    {
        [Test]
        public void TestSelectString()
        {
            var reader = ExecuteReader("SELECT 'test' AS c1");
            reader.Read();
            Assert.IsFalse(reader.IsDBNull(0));
            var actual = reader.GetString(0);

            Assert.AreEqual("test", actual);
        }

        [Test]
        public void TestSelectNullString()
        {
            var reader = ExecuteReader("SELECT CAST(null as varchar) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
            var actual = reader.GetString(0);

            Assert.AreEqual(null, actual);
        }

        [Test]
        public void TestSelectBoolTrue()
        {
            var reader = ExecuteReader("SELECT true AS c1");
            reader.Read();
            Assert.IsFalse(reader.IsDBNull(0));
            var actual = reader.GetBoolean(0);

            Assert.AreEqual(true, actual);
        }

        [Test]
        public void TestSelectBoolFalse()
        {
            var reader = ExecuteReader("SELECT false AS c1");
            reader.Read();
            Assert.IsFalse(reader.IsDBNull(0));
            var actual = reader.GetBoolean(0);

            Assert.AreEqual(false, actual);
        }

        [Test]
        public void TestSelectNullBool()
        {
            var reader = ExecuteReader("SELECT CAST(null AS BOOLEAN) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectTinyint()
        {
            var reader = ExecuteReader("SELECT CAST(17 AS TINYINT) AS c1");
            reader.Read();
            var actualByte = reader.GetByte(0);
            Assert.AreEqual(17, actualByte);

            var actualShort = reader.GetInt16(0);
            Assert.AreEqual(17, actualShort);

            var actualInt = reader.GetInt32(0);
            Assert.AreEqual(17, actualInt);

            var actualLong = reader.GetInt64(0);
            Assert.AreEqual(17, actualLong);
        }

        [Test]
        public void TestSelectNullTinyint()
        {
            var reader = ExecuteReader("SELECT CAST(null AS TINYINT) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectSmallint()
        {
            var reader = ExecuteReader("SELECT CAST(17 AS SMALLINT) AS c1");
            reader.Read();
            var actualByte = reader.GetByte(0);
            Assert.AreEqual(17, actualByte);

            var actualShort = reader.GetInt16(0);
            Assert.AreEqual(17, actualShort);

            var actualInt = reader.GetInt32(0);
            Assert.AreEqual(17, actualInt);

            var actualLong = reader.GetInt64(0);
            Assert.AreEqual(17, actualLong);
        }

        [Test]
        public void TestSelectNullSmallint()
        {
            var reader = ExecuteReader("SELECT CAST(null AS SMALLINT) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectInteger()
        {
            var reader = ExecuteReader("SELECT CAST(17 AS INTEGER) AS c1");
            reader.Read();
            var actualByte = reader.GetByte(0);
            Assert.AreEqual(17, actualByte);

            var actualShort = reader.GetInt16(0);
            Assert.AreEqual(17, actualShort);

            var actualInt = reader.GetInt32(0);
            Assert.AreEqual(17, actualInt);

            var actualLong = reader.GetInt64(0);
            Assert.AreEqual(17, actualLong);
        }

        [Test]
        public void TestSelectNullInteger()
        {
            var reader = ExecuteReader("SELECT CAST(null AS INTEGER) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectBigint()
        {
            var reader = ExecuteReader("SELECT CAST(17 AS BIGINT) AS c1");
            reader.Read();
            var actualByte = reader.GetByte(0);
            Assert.AreEqual(17, actualByte);

            var actualShort = reader.GetInt16(0);
            Assert.AreEqual(17, actualShort);

            var actualInt = reader.GetInt32(0);
            Assert.AreEqual(17, actualInt);

            var actualLong = reader.GetInt64(0);
            Assert.AreEqual(17, actualLong);
        }

        [Test]
        public void TestSelectNullBigint()
        {
            var reader = ExecuteReader("SELECT CAST(null AS BIGINT) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectReal()
        {
            var reader = ExecuteReader("SELECT CAST(17.0 AS REAL) AS c1");
            reader.Read();
            var actualByte = reader.GetFloat(0);
            Assert.AreEqual(17.0f, actualByte);

            var actualShort = reader.GetDouble(0);
            Assert.AreEqual(17.0d, actualShort);

            var actualInt = reader.GetDecimal(0);
            Assert.AreEqual(17.0m, actualInt);
        }

        [Test]
        public void TestSelectNullReal()
        {
            var reader = ExecuteReader("SELECT CAST(null AS REAL) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectDouble()
        {
            var reader = ExecuteReader("SELECT CAST(17.0 AS DOUBLE) AS c1");
            reader.Read();
            var actualByte = reader.GetFloat(0);
            Assert.AreEqual(17.0f, actualByte);

            var actualShort = reader.GetDouble(0);
            Assert.AreEqual(17.0d, actualShort);

            var actualInt = reader.GetDecimal(0);
            Assert.AreEqual(17.0m, actualInt);
        }

        [Test]
        public void TestSelectNullDouble()
        {
            var reader = ExecuteReader("SELECT CAST(null AS DOUBLE) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectDecimal()
        {
            var reader = ExecuteReader("SELECT CAST(17.0 AS DECIMAL) AS c1");
            reader.Read();
            var actualByte = reader.GetFloat(0);
            Assert.AreEqual(17.0f, actualByte);

            var actualShort = reader.GetDouble(0);
            Assert.AreEqual(17.0d, actualShort);

            var actualInt = reader.GetDecimal(0);
            Assert.AreEqual(17.0m, actualInt);
        }

        [Test]
        public void TestSelectNullDecimal()
        {
            var reader = ExecuteReader("SELECT CAST(null AS DECIMAL) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectTimestamp()
        {
            var reader = ExecuteReader("SELECT CAST('1990-02-03' AS TIMESTAMP) AS c1");
            reader.Read();
            var actual = reader.GetDateTime(0);
            Assert.AreEqual(DateTime.Parse("1990-02-03"), actual);
        }

        [Test]
        public void TestSelectNullTimestamp()
        {
            var reader = ExecuteReader("SELECT CAST(null AS TIMESTAMP) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }

        [Test]
        public void TestSelectArray()
        {
            var reader = ExecuteReader("SELECT ARRAY[1, 2, 3] AS c1");
            reader.Read();
            var actual = reader.GetFieldValue<List<int>>(0);
            Assert.AreEqual(new List<int>() { 1, 2, 3 }, actual);
        }

        [Test]
        public void TestSelectNullArray()
        {
            var reader = ExecuteReader("SELECT CAST(null AS array(integer)) AS c1");
            reader.Read();
            var actual = reader.GetFieldValue<List<int>>(0);
            Assert.AreEqual(null, actual);
        }

        private class RowTest
        {
            public long X { get; set; }

            public double Y { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is RowTest other)
                {
                    return Equals(X, other.X) &&
                        Equals(Y, other.Y);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }
        }

        [Test]
        public void TestSelectRow()
        {
            var reader = ExecuteReader("SELECT CAST(ROW(1, 2e0) AS ROW(x BIGINT, y DOUBLE)) AS c1");
            reader.Read();
            var actual = reader.GetFieldValue<RowTest>(0);
            Assert.AreEqual(new RowTest() { X = 1, Y = 2.0 }, actual);
        }

        [Test]
        public void TestSelectNullRow()
        {
            var reader = ExecuteReader("SELECT CAST(null AS ROW(x BIGINT, y DOUBLE)) AS c1");
            reader.Read();
            var actual = reader.GetFieldValue<RowTest>(0);
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void TestSelectGuid()
        {
            var reader = ExecuteReader("SELECT CAST('12151fd2-7586-11e9-8f9e-2a86e4085a59' AS UUID) AS c1");
            reader.Read();
            Assert.IsFalse(reader.IsDBNull(0));
            var actual = reader.GetGuid(0);
            Assert.AreEqual(Guid.Parse("12151fd2-7586-11e9-8f9e-2a86e4085a59"), actual);
        }

        [Test]
        public void TestSelectNullGuid()
        {
            var reader = ExecuteReader("SELECT CAST(null AS UUID) AS c1");
            reader.Read();
            Assert.IsTrue(reader.IsDBNull(0));
        }
    }
}
