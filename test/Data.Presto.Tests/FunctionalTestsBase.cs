using NUnit.Framework;
using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Data.Presto.Tests
{
    public class FunctionalTestsBase
    {
        private IContainer testContainer;
        private MemoryStream memoryStream;
        [OneTimeSetUp]
        public async Task Setup()
        {
            memoryStream = new MemoryStream();
            var testcontainersBuilder = new ContainerBuilder()
                .WithImage("trinodb/trino")
                .WithName("trino")
                .WithPortBinding(8080)
                .WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole())
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("======== SERVER STARTED ========"));
            
            testContainer = testcontainersBuilder.Build();

            try
            {
                
                await testContainer.StartAsync();
            }
            catch(Exception e)
            {

            }
            //sometimes just after container starts there is an error "nodes is empty" in streaming tests
            await Task.Delay(5000);
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
