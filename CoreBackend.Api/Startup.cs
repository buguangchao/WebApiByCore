using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using CoreBackend.Api.Service;
using Microsoft.Extensions.Configuration;
using CoreBackend.Api.Entity;
using CoreBackend.Api.Dto;
using Microsoft.EntityFrameworkCore;
using CoreBackend.Api.Repositories;

namespace CoreBackend.Api
{
    public class Startup
    {
        public static IConfiguration _configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //返回json大写
            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    {
            //        if (options.SerializerSettings.ContractResolver is DefaultContractResolver resolver)
            //        {
            //            resolver.NamingStrategy = null;
            //        }
            //    }); 

            //修改返回格式为xml
            //services.AddMvc()
            //    .AddMvcOptions(options =>
            //    {
            //        options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            //    });

            //services.AddTransient<MailService>();

            #if DEBUG
            services.AddTransient<IMailService, MailService>();
#else
            services.AddTransient<IMailService, NewMailService>();
#endif

            //var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ProductDB11;Trusted_Connection=True";
            var connectionString = _configuration["connectionStrings:productionInfoDbConnectionString"];
            services.AddDbContext<MyContext>(p=>p.UseSqlServer(connectionString));

            services.AddScoped<IProductRepository, ProductRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory, MyContext myContext)
        {
            //loggerFactory.AddProvider(new NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //生成环境异常处理
                app.UseExceptionHandler();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            AutoMapper.Mapper.Initialize(config=> {
                config.CreateMap<Products, ProductWithoutMaterialDto>();
                config.CreateMap<Products, ProductDto>();
                config.CreateMap<ProductCreation, Products>();
                config.CreateMap<ProductModification, Products>();
                config.CreateMap<Products, ProductModification > ();
                config.CreateMap<Materials, MaterialDto>();
            });

            myContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            app.UseMvc();
        }
    }
}
