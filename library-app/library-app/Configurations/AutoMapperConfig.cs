using AutoMapper;
using library_app.Data;
using library_app.Identity;
using library_app.Models.BookDtos;
using library_app.Models.MemberDtos;
using library_app.Models.UserDtos;

namespace library_app.Configurations
{
    public class AutoMapperConfig: Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Book, CreateBookDto>().ReverseMap();  
            CreateMap<Book, BookDto>().ReverseMap();

            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<Member, MemberFullDto>();

            CreateMap<LibraryUserDto, LibraryUser>().ReverseMap();
        }
    }
}
