using AutoMapper;
using ChessPlatform.Entities;
using ChessPlatform.ViewModels.Auth;
using ChessPlatform.ViewModels.Game;
using ChessPlatform.ViewModels.User;

namespace ChessPlatform.Config
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Game, GameInfoViewModel>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString()))
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt.ToString()))
                .ForMember(dest => dest.EndedAt, opt => opt.MapFrom(src => src.EndedAt.ToString()))
                .ForMember(dest => dest.Player1, opt => opt.MapFrom(src => src.Player1.Profile.Nickname))
                .ForMember(dest => dest.Player2, opt => opt.MapFrom(src => src.Player2.Profile.Nickname))
                .ForMember(dest => dest.Winner, opt => opt.MapFrom(src => src.Winner.Profile.Nickname))
                .ForMember(dest => dest.RequirePassword, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Password)));

            CreateMap<RegisterViewModel, User>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src));

            CreateMap<RegisterViewModel, UserProfile>();

            CreateMap<User, UserViewModel>();
            CreateMap<UserProfile, UserProfileViewModel>();
            CreateMap<Notification, NotificationViewModel>();
        }
    }
}