using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Newtonsoft.Json;
using WebApplication1.Database;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Context>(o =>
            {
                o.UsePresto("Data Source=localhost:8080; Catalog=tpch; Schema=tiny;");
            });


            services.AddMvc(opt =>
            {
                opt.MaxIAsyncEnumerableBufferLimit = 256000;
            });


            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Select().Filter().SkipToken().Expand().Count().OrderBy().MaxTop(null);
                endpoints.GetDefaultQuerySettings();
                endpoints.MapODataRoute("odata", "odata", GetEdmModel());

                endpoints.EnableDependencyInjection();
            });
        }

        static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.Namespace = "sample";
            odataBuilder.ContainerName = "sample";

            odataBuilder.EntitySet<Customer>("Customers")
                .EntityType
                .HasKey(x => x.Custkey);

            odataBuilder.EntitySet<Order>("Orders")
                .EntityType
                .HasKey(x => x.Orderkey);

            var model = odataBuilder.GetEdmModel();
            return model;
        }
    }
}
