using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Tests
{
    /// <summary>
    /// Tests regarding streaming pages, where the user does not have to call NextResult to parse a new page.
    /// This is the default setting and is more inline with regular usage of DbDataReader
    /// </summary>
    class StreamingTests : FunctionalTestsBase
    {
        [Test]
        public void TestStream()
        {
            var dbconn = GetConnection();

            var cmd = dbconn.CreateCommand();
            cmd.CommandText = "SELECT * from tpch.tiny.lineitem";
            var reader = cmd.ExecuteReader();

            int actualCount = 0;
            while (reader.Read())
            {
                actualCount++;
            }

            Assert.AreEqual(60175, actualCount);
        }

        [Test]
        public void NonStreamingTest()
        {
            //Test get a result by having each page in a different result set.
            //This is useful when giving out the result in say an OData stream

            var connStr = ConnectionString;
            connStr.Streaming = false;

            PrestoConnection prestoConnection = new PrestoConnection()
            {
                ConnectionOptions = connStr
            };

            var cmd = prestoConnection.CreateCommand();
            cmd.CommandText = "SELECT * from tpch.tiny.lineitem";
            var reader = cmd.ExecuteReader();

            int actualCount = 0;
            do
            {
                while (reader.Read())
                {
                    actualCount++;
                }
            } while (reader.NextResult());


            Assert.AreEqual(60175, actualCount);
        }
    }
}
