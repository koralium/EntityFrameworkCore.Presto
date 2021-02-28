using Data.Presto.DataReaders;
using Data.Presto.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Tests
{
    class PageDataReaderTests
    {
        [Test]
        public void BasicTest()
        {
            var inData = $"{{\"id\":\"20210225_092330_00000_9j99x\",\"columns\":[{{\"name\":\"table_name\",\"type\":\"varchar\",\"typeSignature\":{{\"rawType\":\"varchar\",\"arguments\":[{{\"kind\":\"LONG\",\"value\":2147483647}}]}}}}], \"data\": [[\"division_main_lvs5druy_vqalc2hx_1g3dk1v\"],[\"employee_main_ge0jedtr_2ze5duz0_1g3dk20\"]]}}";
            var memory = Encoding.UTF8.GetBytes(inData).AsMemory();
            var result = JsonDecoderHelper.NewPage(memory);

            PrestoPageDataReader prestoPageDataReader = new PrestoPageDataReader(result.Columns, result.RowCount, result.ColumnDecoders);

            Assert.AreEqual(0, prestoPageDataReader.GetOrdinal("table_name"));
            Assert.AreEqual("table_name", prestoPageDataReader.GetName(0));
            Assert.True(prestoPageDataReader.Read());
            Assert.AreEqual("division_main_lvs5druy_vqalc2hx_1g3dk1v", prestoPageDataReader.GetString(0));
            Assert.AreEqual("division_main_lvs5druy_vqalc2hx_1g3dk1v", prestoPageDataReader["table_name"]);
            Assert.AreEqual("division_main_lvs5druy_vqalc2hx_1g3dk1v", prestoPageDataReader[0]);
            Assert.True(prestoPageDataReader.Read());
            Assert.AreEqual("employee_main_ge0jedtr_2ze5duz0_1g3dk20", prestoPageDataReader.GetString(0));
            Assert.False(prestoPageDataReader.Read());
        }

        [Test]
        public void BasicJsonWriteTest()
        {
            var inData = $"{{\"id\":\"20210225_092330_00000_9j99x\",\"columns\":[{{\"name\":\"table_name\",\"type\":\"varchar\",\"typeSignature\":{{\"rawType\":\"varchar\",\"arguments\":[{{\"kind\":\"LONG\",\"value\":2147483647}}]}}}}], \"data\": [[\"division_main_lvs5druy_vqalc2hx_1g3dk1v\"],[\"employee_main_ge0jedtr_2ze5duz0_1g3dk20\"]]}}";
            var memory = Encoding.UTF8.GetBytes(inData).AsMemory();
            var result = JsonDecoderHelper.NewPage(memory);

            PrestoPageDataReader prestoPageDataReader = new PrestoPageDataReader(result.Columns, result.RowCount, result.ColumnDecoders);

            using MemoryStream memoryStream = new MemoryStream();

            var writer = new Utf8JsonWriter(memoryStream);
            writer.WriteStartArray();
            while (prestoPageDataReader.Read())
            {
                prestoPageDataReader.ToJson(writer);
            }
            writer.WriteEndArray();
            writer.Flush();
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            
            var actual = reader.ReadToEnd();
            Assert.AreEqual("[{\"table_name\":\"division_main_lvs5druy_vqalc2hx_1g3dk1v\"},{\"table_name\":\"employee_main_ge0jedtr_2ze5duz0_1g3dk20\"}]", actual);
        }
    }
}
