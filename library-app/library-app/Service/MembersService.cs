using AutoMapper;
using library_app.Models.MemberDtos;
using library_app.Contracts;

namespace library_app.Service
{
    public class MembersService
    {
        private readonly IMembersRepository _membersRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public MembersService(IMembersRepository membersRepository,IBooksRepository booksRepository, IMapper mapper)
        {
            _membersRepository = membersRepository;
            _booksRepository = booksRepository;
            _mapper = mapper;   
        }

        public async Task<MemberFullDto> GetMemberAsync(int id)
        {
            var result = await _membersRepository.FindMemberWithBooksLoanedAsync(id);
            if (result == null)
            {
                return null;
            }
            var member = _mapper.Map<MemberFullDto>(result);
            return member;
        }

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var members = await _membersRepository.GetAllAsync();
            return _mapper.Map<List<MemberDto>>(members);
        }

        public async Task<string?> LoanBookAsync(int id, string isbn)
        {
            var member = await _membersRepository.GetAsync(id);
            var book = await _booksRepository.FindByISBNAsync(isbn);

            if (member == null) return "Member not found";
            if (book == null) return "Book not found";
            if (!book.AvailableForLoan) return "Book already loaned";

            await _membersRepository.LoanOutBookToMemberAsync(book, member);
            return null;
        }

        public async Task<string?> ReturnBookAsync(int id, string isbn)
        {
            var member = await _membersRepository.GetAsync(id);
            var book = await _booksRepository.FindByISBNAsync(isbn);

            if (member == null) return "Member not found";
            if (book == null) return "Book not found";
            if (book.AvailableForLoan) return "Book already available";

            await _membersRepository.ReturnBookToLibraryAsync(book, member);
            return null;
        }
    }
}
