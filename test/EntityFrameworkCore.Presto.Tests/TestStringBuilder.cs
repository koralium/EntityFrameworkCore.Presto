using Data.Presto;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace EntityFrameworkCore.Presto.Tests
{
    public class TestStringBuilder
    {
        [Fact]
        public void TestExtraCredentialsParsing()
        {
            var connectionString = "ExtraCredentials=key:value,key2:value2";
            var prestoConnectionStringBuilder = new PrestoConnectionStringBuilder(connectionString);

            Assert.Equal(prestoConnectionStringBuilder.ExtraCredentials,
                ImmutableList.Create(new KeyValuePair<string, string>("key", "value"), new KeyValuePair<string, string>("key2", "value2")));
            Assert.Equal(connectionString, prestoConnectionStringBuilder.ConnectionString);
        }

        [Fact]
        public void TestAddExtraCredentials()
        {
            var prestoConnectionStringBuilder = new PrestoConnectionStringBuilder();
            prestoConnectionStringBuilder.ExtraCredentials = ImmutableList.Create<KeyValuePair<string, string>>(new KeyValuePair<string, string>("key1", "value1"));
        }
    }
}
