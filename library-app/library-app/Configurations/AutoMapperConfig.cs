using AutoMapper;
using library_app.Data;
using library_app.Models.Book;
using library_app.Models.Member;

namespace library_app.Configurations
{
    public class AutoMapperConfig: Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Book, CreateBookDto>().ReverseMap();  
            CreateMap<Book, BookDto>().ReverseMap();

            CreateMap<Member, MemberDto>().ReverseMap();
        }
    }
}
