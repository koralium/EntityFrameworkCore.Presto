using Data.Presto.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Tests
{
    class DecoderHelperTests
    {
        [Test]
        public void TestReadId()
        {
            string expected = "20210221_145601_19513_ihh5f";
            var inData = $"{{\"id\":\"{expected}\"}}";

            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(inData).AsSpan());

            string actual = JsonDecoderHelper.ReadResultId(ref reader);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestFindNextUri()
        {
            var expected = "http://10.202.240.173:8080/v1/statement/executing/20210221_145601_19513_ihh5f/y89c84196a049fe24c8bc4d5130255ac43a5cbdbf/1";
            var inData = $"{{\"id\":\"20210221_145601_19513_ihh5f\",\"infoUri\":\"http://10.202.240.173:8080/ui/query.html?20210221_145601_19513_ihh5f\",\"partialCancelUri\":\"http://10.202.240.173:8080/v1/statement/executing/partialCancel/20210221_145601_19513_ihh5f/0/y89c84196a049fe24c8bc4d5130255ac43a5cbdbf/1\",\"nextUri\":\"{expected}\"}}";

            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(inData).AsSpan());

            var nextProperty = JsonDecoderHelper.FindNextUriColumnsDataOrStats(ref reader);

            Assert.AreEqual(JsonDecoderHelper.PrestoResultPropertyType.NextUri, nextProperty);

            Assert.True(reader.Read());
            Assert.AreEqual(JsonTokenType.String, reader.TokenType);
            var nextUri = reader.GetString();

            Assert.AreEqual(expected, nextUri);
        }

        [Test]
        public void TestParsePageNextUri()
        {
            var expected = "http://10.202.240.173:8080/v1/statement/executing/20210221_145601_19513_ihh5f/y89c84196a049fe24c8bc4d5130255ac43a5cbdbf/1";
            var inData = $"{{\"id\":\"20210221_145601_19513_ihh5f\",\"infoUri\":\"http://10.202.240.173:8080/ui/query.html?20210221_145601_19513_ihh5f\",\"partialCancelUri\":\"http://10.202.240.173:8080/v1/statement/executing/partialCancel/20210221_145601_19513_ihh5f/0/y89c84196a049fe24c8bc4d5130255ac43a5cbdbf/1\",\"nextUri\":\"{expected}\"}}";

            var memory = Encoding.UTF8.GetBytes(inData).AsMemory();
            var result = JsonDecoderHelper.NewPage(memory);

            Assert.AreEqual(expected, result.NextUri);
        }

        [Test]
        public void TestParsePageId()
        {
            string expected = "20210221_145601_19513_ihh5f";
            var inData = $"{{\"id\":\"{expected}\"}}";
            var memory = Encoding.UTF8.GetBytes(inData).AsMemory();

            var result = JsonDecoderHelper.NewPage(memory);

            Assert.AreEqual(expected, result.Id);
        }

        [Test]
        public void TestParseColumns()
        {
            var inData = $"{{\"id\":\"20210225_092330_00000_9j99x\",\"columns\":[{{\"name\":\"table_name\",\"type\":\"varchar\",\"typeSignature\":{{\"rawType\":\"varchar\",\"arguments\":[{{\"kind\":\"LONG\",\"value\":2147483647}}]}}}}]}}";
            var memory = Encoding.UTF8.GetBytes(inData).AsMemory();
            var result = JsonDecoderHelper.NewPage(memory);
        }

        /// <summary>
        /// Simple test for data parsing, this test does not focus on individual data type decoders
        /// </summary>
        [Test]
        public void TestParseData()
        {
            var inData = $"{{\"id\":\"20210225_092330_00000_9j99x\",\"columns\":[{{\"name\":\"table_name\",\"type\":\"varchar\",\"typeSignature\":{{\"rawType\":\"varchar\",\"arguments\":[{{\"kind\":\"LONG\",\"value\":2147483647}}]}}}}], \"data\": [[\"division_main_lvs5druy_vqalc2hx_1g3dk1v\"],[\"employee_main_ge0jedtr_2ze5duz0_1g3dk20\"]]}}";
            var memory = Encoding.UTF8.GetBytes(inData).AsMemory();
            var result = JsonDecoderHelper.NewPage(memory);

            Assert.AreEqual(1, result.ColumnDecoders.Count);

            Assert.AreEqual(2, result.RowCount);
            Assert.AreEqual("division_main_lvs5druy_vqalc2hx_1g3dk1v", result.ColumnDecoders[0].GetString(0));
            Assert.AreEqual("employee_main_ge0jedtr_2ze5duz0_1g3dk20", result.ColumnDecoders[0].GetString(1));
        }
    }
}
