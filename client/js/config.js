const urls = {
	base: "http://localhost:5000/api",
	book: {
		getBooks: "/books",
		addBook: "/books",
		removeBook: "/books/",
		searchBookByIsbn: "/books/isbn/"
	},
	member: {
		getMembers: "/members",
		getMemberById: "/members/",
		loanBookToMember: "/members/loan/",
		returnBookToLibrary: "/members/return/"
	},
	authentication: {
		register: "/user/register",
		login: "/user/login"
	}
}