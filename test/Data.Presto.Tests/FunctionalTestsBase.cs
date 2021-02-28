using Data.Presto.DataReaders;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Data.Presto.Tests
{
    public class FunctionalTestsBase
    {
        private TestcontainersContainer testContainer;
        private MemoryStream memoryStream;
        [OneTimeSetUp]
        public async Task Setup()
        {
            memoryStream = new MemoryStream();
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("trinodb/trino")
              .WithName("trino")
              .WithPortBinding(8080)
              .WithOutputConsumer(Consume.RedirectStdoutAndStderrToStream(memoryStream, memoryStream))
              .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(memoryStream, "======== SERVER STARTED ========"));

            
            testContainer = testcontainersBuilder.Build();

            try
            {
                
                await testContainer.StartAsync();
            }
            catch(Exception e)
            {

            }
            
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await memoryStream.DisposeAsync();
            await testContainer.StopAsync();
            await testContainer.DisposeAsync();
        }

        public PrestoConnection GetConnection()
        {
            return new PrestoConnection()
            {
                ConnectionOptions = GetPrestoConnection()
            };
        }

        public DbCommand CreateCommand(string statement)
        {
            var cmd = GetConnection().CreateCommand();
            cmd.CommandText = statement;
            return cmd;
        }

        public DbDataReader ExecuteReader(string statement)
        {
            return CreateCommand(statement).ExecuteReader();
        } 


        private PrestoConnectionStringBuilder GetPrestoConnection()
        {
            PrestoConnectionStringBuilder connection = new PrestoConnectionStringBuilder();
            connection.DataSource = "localhost:8080";
            connection.User = "root";
            connection.Catalog = "memory";
            connection.Schema = "default";
            connection.Trino = true;
            return connection;
        }

        public PrestoConnectionStringBuilder ConnectionString => GetPrestoConnection();
    }
}
