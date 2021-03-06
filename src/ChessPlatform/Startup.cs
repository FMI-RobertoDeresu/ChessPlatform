﻿using System;
using AutoMapper;
using ChessPlatform.DBContexts;
using ChessPlatform.Entities;
using ChessPlatform.Extensions;
using ChessPlatform.Filters;
using ChessPlatform.Logging;
using ChessPlatform.Repositories;
using ChessPlatform.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ChessPlatform
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
                builder = builder.AddUserSecrets("cheesplatformsecrets");

            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //auto mapper
            services.AddAutoMapper();

            //mvc
            services.AddMvc(
                    options =>
                    {
                        if (bool.Parse(Configuration["app:requireHttpsFilterEnabled"]))
                            options.Filters.Add(new RequireHttpsAttribute { Permanent = true });

                        if (bool.Parse(Configuration["app:applicationExceptionFilterEnabled"]))
                            options.Filters.Add(typeof(ApplicationExceptionFilter));

                        if (bool.Parse(Configuration["app:requestHistoryLogFilterEnabled"]))
                            options.Filters.Add(typeof(RequestHistoryLogFilterAttribute));
                    })
                .AddJsonOptions(
                    option =>
                    {
                        option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        option.SerializerSettings.Formatting = Formatting.Indented;
                        option.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });

            //entity framework
            services.AddDbContext<ChessContext>(
                options => options.UseSqlServer(Configuration["db:default"]));

            //identity
            services.AddIdentity<User, IdentityRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequiredLength = 6;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ChessContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/authenticate";
                options.LogoutPath = "/auth/logout";
                options.SlidingExpiration = true;
                options.Cookie.Name = "_ChessPlatformAuth";
                options.Cookie.Expiration = TimeSpan.FromDays(10);
            });

            //configuration
            services.AddSingleton(Configuration);

            //repositories
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IGameRepository, GameRepository>();

            //services
            services.AddTransient<IUserService, UserService>();

            //loggers
            services.AddTransient<IApplicationLogger, ApplicationLogger>();
            services.AddTransient<IRequestHistoryLogger, RequestHistoryLogger>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //logger
            app.ConfigureNLog(env, loggerFactory, Configuration["db:default"]);

            //env options
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/error");

            //static files
            app.UseStaticFiles();

            //web sockets
            app.UseWebSockets();
            app.UseChessWebSocket();

            // auth
            app.UseAuthentication();

            //mvc
            app.UseMvc(config => config.MapRoute("Default", "{controller=Auth}/{action=Authenticate}/{id?}"));
        }
    }
}