using AutoMapper;
using library_app.Contracts;
using library_app.Data.Data;
using library_app.Models.MemberDtos;
using library_app.Service;
using Moq;
using Shouldly;

namespace library_app.Tests
{
    public class MembersServiceTest
    {
        private readonly BooksService _booksService;
        private readonly MembersService _membersService;
        private readonly Mock<IBooksRepository> _booksRepository;
        private readonly Mock<IMembersRepository> _membersRepository;
        private readonly Mock<IMapper> _mapper;
        private static List<Member> _members;
        private List<MemberDto> _membersDtos;
        private List<MemberFullDto> _membersFullDtos;
        private static List<Book> _books;
        private readonly string _bookNotFoundMessage;
        private readonly string _memberNotFoundMessage;
        private readonly string _emptyIsbn;
        private readonly int _nonExistentMemberId;

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
            _bookNotFoundMessage = "Book not found";
            _memberNotFoundMessage = "Member not found";
            _emptyIsbn = "";
            _nonExistentMemberId = 999;
        }

        public static IEnumerable<object[]> ReturnBookTestCases =>
            new List<object[]>
            {
                new object[] { _books[1], _members[0] },
                new object[] { _books[1], null },
                new object[] { null, _members[0] }
            };

        public static IEnumerable<object[]> LoanBookTestCases =>
            new List<object[]>
            {
                new object[] { _books[0], _members[0] },
                new object[] { _books[0], null },
                new object[] { null, _members[0] }
            };


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

        [Theory]
        [MemberData(nameof(LoanBookTestCases))]
        public async Task ShouldLoanBookToMember(Book book, Member member)
        {
            if(book == null)
            {
                _membersRepository.Setup(r => r.GetAsync(member.Id))
                                    .ReturnsAsync(member);
                _booksRepository.Setup(r => r.FindByISBNAsync(_emptyIsbn))
                    .ReturnsAsync((Book)null);

                var result = await _membersService.LoanBookAsync(member.Id, _emptyIsbn);

                result.ShouldBe(_bookNotFoundMessage);
                _membersRepository.Verify(r => r.GetAsync(member.Id), Times.Once);
                _booksRepository.Verify(r => r.FindByISBNAsync(_emptyIsbn), Times.Once);
            }
            else if(member == null)
            {
                _membersRepository.Setup(r => r.GetAsync(_nonExistentMemberId))
                    .ReturnsAsync((Member)null);
                _booksRepository.Setup(r => r.FindByISBNAsync(book.ISBN))
                    .ReturnsAsync(book);

                var result = await _membersService.LoanBookAsync(_nonExistentMemberId, book.ISBN);

                result.ShouldBe(_memberNotFoundMessage);
                _membersRepository.Verify(r => r.GetAsync(_nonExistentMemberId), Times.Once);
                _booksRepository.Verify(r => r.FindByISBNAsync(book.ISBN), Times.Once);
            }
            else
            {
                _membersRepository.Setup(r => r.GetAsync(member.Id))
                    .ReturnsAsync(member);
                _booksRepository.Setup(r => r.FindByISBNAsync(book.ISBN))
                    .ReturnsAsync(book);
                _membersRepository.Setup(r => r.LoanOutBookToMemberAsync(book, member));

                var result = await _membersService.LoanBookAsync(member.Id, book.ISBN);

                result.ShouldBeNull();
                _membersRepository.Verify(r => r.LoanOutBookToMemberAsync(book, member), Times.Once);
                _membersRepository.Verify(r => r.GetAsync(member.Id), Times.Once);
                _booksRepository.Verify(r => r.FindByISBNAsync(book.ISBN), Times.Once);
            }
        }

        [Theory]
        [MemberData(nameof(ReturnBookTestCases))]
        public async Task ShouldReturnBookToLibrary(Book book, Member member)
        {
            if(book == null)
            {
                _membersRepository.Setup(r => r.GetAsync(member.Id))
                    .ReturnsAsync(member);
                _booksRepository.Setup(r => r.FindByISBNAsync(_emptyIsbn))
                    .ReturnsAsync((Book)null);

                var result = await _membersService.ReturnBookAsync(member.Id, _emptyIsbn);

                result.ShouldBe(_bookNotFoundMessage);
                _membersRepository.Verify(r => r.GetAsync(member.Id), Times.Once);
                _booksRepository.Verify(r => r.FindByISBNAsync(_emptyIsbn), Times.Once);
            }
            else if(member == null)
            {
                _membersRepository.Setup(r => r.GetAsync(_nonExistentMemberId))
                    .ReturnsAsync((Member)null);
                _booksRepository.Setup(r => r.FindByISBNAsync(book.ISBN))
                    .ReturnsAsync(book);

                var result = await _membersService.ReturnBookAsync(_nonExistentMemberId, book.ISBN);

                result.ShouldBe(_memberNotFoundMessage);
                _membersRepository.Verify(r => r.GetAsync(_nonExistentMemberId), Times.Once);
                _booksRepository.Verify(r => r.FindByISBNAsync(book.ISBN), Times.Once);
            }
            else
            {
                _membersRepository.Setup(r => r.GetAsync(member.Id))
                    .ReturnsAsync(member);
                _booksRepository.Setup(r => r.FindByISBNAsync(book.ISBN))
                    .ReturnsAsync(book);
                _membersRepository.Setup(r => r.ReturnBookToLibraryAsync(book, member));

                var result = await _membersService.ReturnBookAsync(member.Id, book.ISBN);

                result.ShouldBeNull();
                _membersRepository.Verify(r => r.ReturnBookToLibraryAsync(book, member), Times.Once);
                _membersRepository.Verify(r => r.GetAsync(member.Id), Times.Once);
                _booksRepository.Verify(r => r.FindByISBNAsync(book.ISBN), Times.Once);
            }            
        }
    }
}
