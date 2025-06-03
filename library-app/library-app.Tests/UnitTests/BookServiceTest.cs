using AutoMapper;
using library_app.Contracts;
using library_app.Data.Data;
using library_app.Models.BookDtos;
using library_app.Service;
using Moq;
using Shouldly;

namespace library_app.Tests.UnitTests
{
    public class BookServiceTest
    {
        private readonly BooksService _booksService;
        private readonly Mock<IBooksRepository> _booksRepository;
        private readonly Mock<IMapper> _mapper;
        private List<Book> _books;
        private List<BookDto> _bookDtos;
        private CreateBookDto _createBookDto;

        public BookServiceTest()
        {
            _booksRepository = new Mock<IBooksRepository>();
            _mapper = new Mock<IMapper>();
            _booksService = new BooksService(_booksRepository.Object, _mapper.Object);
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
            _bookDtos = new List<BookDto>
            {
                new BookDto
                {
                    ISBN = "123-456-789",
                    Title = "Tester's book",
                    Author = "Mr. Tester",
                    PublicationYear = 2025,
                    AvailableForLoan = true
                },
                new BookDto
                {
                    ISBN = "987-654-321",
                    Title = "Tester's book - the update",
                    Author = "Mrs. Tester",
                    PublicationYear = 2026,
                    AvailableForLoan = false
                }
            };
            _createBookDto = new CreateBookDto()
            {
                ISBN = "147-258-369",
                Title = "Practical .NET - creating DTOs",
                Author = "Mr. Tester",
                PublicationYear = 2027
            };
        }

        [Fact]
        public async Task ShouldReturnAllBooksFromDatabase()
        {
            /*
             * Arrange:
             * Subscribe to the GetAllAync method that returns a Book List.
             * Subscribe to the Map method that returns a BookDto List. 
            */      
            _booksRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_books);
            _mapper.Setup(m => m.Map<List<BookDto>>(_books))
                .Returns(_bookDtos);

            /* Act:
             * Call the actual function through the service
            */
            var result = await _booksService.GetAllBooksAsync();

            /* Assert:
             * Test if the returned results are as expected
            */
            _booksRepository.Verify(s => s.GetAllAsync(), Times.Once);
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].ISBN.ShouldBe("123-456-789");
            result[0].Title.ShouldBe("Tester's book");
            result[0].Author.ShouldBe("Mr. Tester");
            result[0].PublicationYear.ShouldBe(2025);
            result[0].AvailableForLoan.ShouldBe(true);
        }

        [Fact]
        public async Task ShouldUpdateBookInDatabase()
        {
            _mapper.Setup(m => m.Map(
                It.Is<BookDto>(src => src == _bookDtos[1]),
                It.Is<Book>(dest => dest == _books[0])
             ));
            _booksRepository.Setup(r => r.UpdateAsync(_books[1]));

            await _booksService.UpdateBook(_books[0], _bookDtos[1]);

            _booksRepository.Verify(s => s.UpdateAsync(_books[0]), Times.Once);
            _mapper.Verify(m => m.Map(
                It.Is<BookDto>(src => src == _bookDtos[1]),
                It.Is<Book>(dest => dest == _books[0])),
                Times.Once
             );
        }

        [Fact]
        public async Task ShouldAddBookToDatabase()
        {
            _booksRepository.Setup(r => r.AddAsync(_books[0]));
            _mapper.Setup(m => m.Map<Book>(_createBookDto))
                .Returns(_books[0]);

            await _booksService.AddBook(_createBookDto);

            _booksRepository.Verify(s => s.AddAsync(_books[0]), Times.Once);
            _mapper.Verify(m => m.Map<Book>(_createBookDto), Times.Once);
        }
    }
}
