using AutoMapper;
using library_app.Contracts;
using library_app.Data.Data;
using library_app.Models.MemberDtos;
using library_app.Service;
using Moq;
using Shouldly;

namespace library_app.Tests.UnitTests
{
    public class MembersServiceTest
    {
        private readonly BooksService _booksService;
        private readonly MembersService _membersService;
        private readonly Mock<IBooksRepository> _booksRepository;
        private readonly Mock<IMembersRepository> _membersRepository;
        private readonly Mock<IMapper> _mapper;
        private List<Member> _members;
        private List<MemberDto> _membersDtos;
        private List<MemberFullDto> _membersFullDtos;
        private List<Book> _books;

        public MembersServiceTest()
        {
            _booksRepository = new Mock<IBooksRepository>();
            _membersRepository = new Mock<IMembersRepository>();
            _mapper = new Mock<IMapper>();
            _booksService = new BooksService(_booksRepository.Object, _mapper.Object);
            _membersService = new MembersService(_membersRepository.Object, _booksRepository.Object, _mapper.Object);
            _members = new List<Member>()
            {
                new Member
                {
                    Id = 1,
                    FirstName = "Anonym",
                    LastName = "Tester",
                    DOB = new DateTime(1990, 6, 1),
                    Email = "anonym.tester@gmail.com",
                    BooksLoaned = null
                }

            };
            _membersDtos = new List<MemberDto>()
            {
                new MemberDto
                {
                    Id = 1,
                    FirstName = "Anonym",
                    LastName = "Tester",
                    DOB = new DateTime(1990, 6, 1),
                    Email = "anonym.tester@gmail.com"
                }
            };
            _membersFullDtos = new List<MemberFullDto>()
            {
                new MemberFullDto
                {
                    Id = 1,
                    FirstName = "Anonym",
                    LastName = "Tester",
                    DOB = new DateTime(1990, 6, 1),
                    Email = "anonym.tester@gmail.com",
                    BooksLoaned = null
                }
            };
            _books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    ISBN = "123-456-789",
                    Title = "Tester's book",
                    Author = "Mr. Tester",
                    PublicationYear = 2025,
                    AvailableForLoan = true,
                    MemberId = null,
                    Member = null
                },
                new Book
                {
                    Id = 2,
                    ISBN = "987-654-321",
                    Title = "Tester's book - the update",
                    Author = "Mrs. Tester",
                    PublicationYear = 2026,
                    AvailableForLoan = false,
                    MemberId = null,
                    Member = null
                }
            };
            _membersRepository.Setup(r => r.GetAsync(_members[0].Id))
                .ReturnsAsync(_members[0]);
        }

        [Fact]
        public async Task ShouldReturnAllMembers()
        {
            /*
             * Arrange:
             * Subscribe to the GetAllAync method that returns a Member List.
             * Subscribe to the Map method that returns a MemberDto List. 
            */
            _membersRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_members);
            _mapper.Setup(m => m.Map<List<MemberDto>>(_members))
                .Returns(_membersDtos);

            /* Act:
             * Call the actual function through the service
            */
            var result = await _membersService.GetAllMembersAsync();

            /* Assert:
             * Test if the returned results are as expected
            */
            _membersRepository.Verify(s => s.GetAllAsync(), Times.Once);
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe(1);
            result[0].FirstName.ShouldBe("Anonym");
            result[0].LastName.ShouldBe("Tester");
            result[0].DOB.Date.ShouldBe(new DateTime(1990, 6, 1));
            result[0].Email.ShouldBe("anonym.tester@gmail.com");
        }

        [Fact]
        public async Task ShouldReturnOneMember()
        {
            _membersRepository.Setup(r => r.FindMemberWithBooksLoanedAsync(_members[0].Id))
                .ReturnsAsync(_members[0]);
            _mapper.Setup(m => m.Map<MemberFullDto>(_members[0]))
                .Returns(_membersFullDtos[0]);

            await _membersService.GetMemberAsync(_members[0].Id);

            _membersRepository.Verify(s => s.FindMemberWithBooksLoanedAsync(_members[0].Id), Times.Once);
            _mapper.Verify(m => m.Map<MemberFullDto>(_members[0]), Times.Once);
        }

        [Fact]
        public async Task ShouldLoanBookToMember()
        {
            _booksRepository.Setup(r => r.FindByISBNAsync(_books[0].ISBN))
                .ReturnsAsync(_books[0]);
            _membersRepository.Setup(r => r.LoanOutBookToMemberAsync(_books[0], _members[0]));

            await _membersService.LoanBookAsync(_members[0].Id, _books[0].ISBN);

            _membersRepository.Verify(r => r.GetAsync(_members[0].Id), Times.Once);
            _membersRepository.Verify(r => r.LoanOutBookToMemberAsync(_books[0], _members[0]), Times.Once);
            _booksRepository.Verify(r => r.FindByISBNAsync(_books[0].ISBN), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnBookToLibrary()
        {
            _booksRepository.Setup(r => r.FindByISBNAsync(_books[1].ISBN))
                .ReturnsAsync(_books[1]);
            _membersRepository.Setup(r => r.ReturnBookToLibraryAsync(_books[1], _members[0]));

            await _membersService.ReturnBookAsync(_members[0].Id, _books[1].ISBN);

            _membersRepository.Verify(r => r.GetAsync(_members[0].Id), Times.Once);
            _membersRepository.Verify(r => r.ReturnBookToLibraryAsync(_books[1], _members[0]), Times.Once);
            _booksRepository.Verify(r => r.FindByISBNAsync(_books[1].ISBN), Times.Once);
        }

    }
}
