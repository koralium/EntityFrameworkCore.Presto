using Data.Presto.Client;
using Data.Presto.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Presto.Tests
{
    class TestPrestoClient : FunctionalTestsBase
    {
        [Test]
        public async Task TestSelect()
        {
            PrestoClient prestoClient = new PrestoClient(ConnectionString);

            var decodeResults = await prestoClient.Query("SELECT 'test' AS c1", default);

            Assert.AreEqual(1, decodeResults.RowCount);
        }

        [Test]
        public async Task TestError()
        {
            PrestoClient prestoClient = new PrestoClient(ConnectionString);

            var decodeResults = await prestoClient.Query("SELEC", default);

            Assert.AreEqual(PrestoState.Failed, decodeResults.State);
            Assert.AreEqual(
                "line 1:1: mismatched input 'SELEC'. Expecting: 'ALTER', 'ANALYZE', 'CALL', 'COMMENT', 'COMMIT', 'CREATE', 'DEALLOCATE', 'DELETE', 'DESC', 'DESCRIBE', 'DROP', 'EXECUTE', 'EXPLAIN', 'GRANT', 'INSERT', 'PREPARE', 'REFRESH', 'RESET', 'REVOKE', 'ROLLBACK', 'SET', 'SHOW', 'START', 'UPDATE', 'USE', <query>",
                decodeResults.ErrorMessage);
        }
    }
}
