using AutoMapper;
using Project.DTO_s.Account;
using Project.DTO_s.Comment;
using Project.DTO_s.Like;
using Project.DTO_s.Post;
using Project.DTO_s.Relationship;
using Project.Entities;

namespace Project.Automapper
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<Post, PostGetDto>();
            CreateMap<Post, PostDetailedGetDto>().ForMember((dest => dest.UserName), (opt) => opt.MapFrom(s => s.User.UserName));
            CreateMap<PostPutDto, Post>();
            CreateMap<Comment, CommentGetDto>().ForMember((dest => dest.UserName), (opt) => opt.MapFrom(s => s.User.UserName));
            CreateMap<UserPutDto, User>();
            CreateMap<Post, PostDto>();
            CreateMap<Post, PostGetDto>();
            CreateMap<User, UserDetailedGetDto>();
            CreateMap<User, UserGetDto>();
            CreateMap<Like, LikeDto>();

        }
    }
}
