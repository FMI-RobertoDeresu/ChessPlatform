using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using ChessPlatform.DBContexts;
using ChessPlatform.Entities;
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
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.WebRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{appEnv.EnvironmentName}.json", true)
                .AddUserSecrets("cheesplatformsecrets")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(
                    option =>
                    {
                        option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        option.SerializerSettings.Formatting = Formatting.Indented;
                        option.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });
            services.AddLogging();

            services.AddDbContext<ChessContext>(
                setup => setup.UseSqlServer(Configuration["db:default"]));

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

            //Add repositories
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRepository, GameRepository>();

            //Add services
            services.AddScoped<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug(LogLevel.Error);

            applicationBuilder.UseDeveloperExceptionPage();
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseIdentity();

            ConfigureWebSockets(applicationBuilder);
            ConfigureMVC(applicationBuilder);
            RegisterMappings();
        }

        public void ConfigureWebSockets(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseWebSockets();
            applicationBuilder.Use(async (http, next) =>
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

        public void ConfigureMVC(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMvc(config =>
            {
                config.MapRoute(
                    "Default",
                    "{controller}/{action}/{id?}",
                    new { controller = "Auth", action = "Authenticate" }
                );
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