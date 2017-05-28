using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using ChessPlatform.DBContexts;
using ChessPlatform.Entities;
using ChessPlatform.Extensions;
using ChessPlatform.Filters;
using ChessPlatform.Logging;
using ChessPlatform.Repositories;
using ChessPlatform.Services;
using ChessPlatform.ViewModels.Auth;
using ChessPlatform.ViewModels.Game;
using ChessPlatform.ViewModels.User;
using ChessPlatform.WebSockets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
                builder = builder.AddUserSecrets("cheesplatformsecrets");

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //mvc
            services.AddMvc(
                    options =>
                    {
                        options.Filters.Add(typeof(ApplicationExceptionFilter));
                        if (bool.Parse(_configuration["app:requestHistoryLoggerEnabled"]))
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
                options => options.UseSqlServer(_configuration["db:default"]));

            //identity
            services.AddIdentity<User, IdentityRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequiredLength = 6;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;

                    config.Cookies.ApplicationCookie.LoginPath = "/Auth/Authenticate";
                    config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = x =>
                        {
                            x.Response.Redirect(x.RedirectUri);

                            return Task.FromResult(0);
                        }
                    };
                })
                .AddEntityFrameworkStores<ChessContext>();

            //configuration
            services.AddSingleton(_configuration);

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
            app.ConfigureNLog(env, loggerFactory, _configuration["db:default"]);

            //env options
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/error");

            //static files
            app.UseStaticFiles();

            //web sockets
            ConfigureWebSockets(app);

            //identity
            app.UseIdentity();

            //mvc
            app.UseMvc(config => config.MapRoute("Default", "{controller=Auth}/{action=Authenticate}/{id?}"));

            //mappings
            RegisterMappings();
        }

        public void ConfigureWebSockets(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(async (http, next) =>
            {
                if (http.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await http.WebSockets.AcceptWebSocketAsync();

                    if (webSocket != null && webSocket.State == WebSocketState.Open)
                    {
                        await SocketListener.Handle(webSocket);
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        public void RegisterMappings()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<Game, GameInfoViewModel>()
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString()))
                    .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt.ToString()))
                    .ForMember(dest => dest.EndedAt, opt => opt.MapFrom(src => src.EndedAt.ToString()))
                    .ForMember(dest => dest.Player1, opt => opt.MapFrom(src => src.Player1.Profile.Nickname))
                    .ForMember(dest => dest.Player2, opt => opt.MapFrom(src => src.Player2.Profile.Nickname))
                    .ForMember(dest => dest.Winner, opt => opt.MapFrom(src => src.Winner.Profile.Nickname))
                    .ForMember(dest => dest.RequirePassword, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Password)));

                x.CreateMap<RegisterViewModel, User>()
                    .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src));

                x.CreateMap<RegisterViewModel, UserProfile>();

                x.CreateMap<User, UserViewModel>();
                x.CreateMap<UserProfile, UserProfileViewModel>();
                x.CreateMap<Notification, NotificationViewModel>();
            });
        }
    }
}