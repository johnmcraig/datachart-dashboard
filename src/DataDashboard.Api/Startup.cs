﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashboardApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DashboardApi
{
    public class Startup
    {
        private string _connectionString = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", c => c.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });

            _connectionString = Configuration["secretConnectionString"];
            
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ApiContext>(opt => opt.UseNpgsql(_connectionString));
                
            //transient service to call DataSeed program class on starup
            services.AddTransient<DataSeed>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DataSeed seed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("CorsPolicy");
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            seed.SeedData(20, 1000); //(x,y) called from service DataSeed that will populate DB with X customers and Y orders
            
            app.UseMvc(routes => routes.MapRoute(
                "default", "api/{controller}/{action}/{id}"
            ));
            
        }
    }
}