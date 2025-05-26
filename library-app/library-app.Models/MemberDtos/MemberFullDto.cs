using library_app.Models.BookDtos;

namespace library_app.Models.MemberDtos
{
    public class MemberFullDto: MemberDto
    {
        public IList<BookDto> BooksLoaned { get; set; }
    }
}
