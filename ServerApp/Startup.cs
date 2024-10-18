using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using ServerApp.Models;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ServerApp
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
            services.AddControllers();
            services.AddMvc()
                    .AddXmlSerializerFormatters();

            services.AddDbContext<ServerAppContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Assignment3Context")));
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

            // Checking the header Accept parameter define the return type
            app.Use(async (context, next) =>
            {
                StringValues type;
                if (context.Request.Headers.TryGetValue("Accept", out type))
                {
                    if (type.FirstOrDefault() != "application/json" && type.FirstOrDefault() != "application/xml")
                    {
                        context.Request.Headers.Remove("Accept");
                        context.Request.Headers.Add("Accept", "application/json");   // setting default
                    }
                }
                else
                {      // returning error becasue of no accept header
                    context.Response.StatusCode = 406;
                    context.Response.ContentType = "application/json;charset=UTF-8";
                    ErrorMessage e = new ErrorMessage(406, "Please enter valid accept header json/xml");
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(e.ToString()));
                    await context.Response.WriteAsync(e.ToString());
                }
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
