using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_app.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Books (ISBN, Title, Author, PublicationYear, AvailableForLoan)
                VALUES
                ('978-0-321-12345-6', 'Whispers of the Forest', 'Alice Green', 2015, 1),
                ('978-0-432-98765-4', 'Journey to the Stars', 'Bob Carter', 2018, 1),
                ('978-0-543-87654-3', 'The Silent River', 'Catherine Holt', 2012, 1),
                ('978-1-234-56789-0', 'Legends Reborn', 'Daniel Storm', 2020, 1),
                ('978-1-345-67890-1', 'Shadows of the Past', 'Eleanor Frost', 2016, 1),
                ('978-1-456-78901-2', 'Winds of Destiny', 'Franklin Knight', 2019, 1),
                ('978-1-567-89012-3', 'Beneath Crimson Skies', 'Grace Winter', 2021, 1),
                ('978-1-678-90123-4', 'Fragments of Light', 'Henry Brooks', 2017, 1),
                ('978-1-789-01234-5', 'The Last Horizon', 'Isabelle West', 2013, 1),
                ('978-1-890-12345-6', 'Echoes of Eternity', 'Jack Rivers', 2022, 1),
                ('978-2-001-23456-7', 'Threads of Time', 'Karen Miles', 2014, 1),
                ('978-2-112-34567-8', 'Bound by Honor', 'Leo Grant', 2011, 1),
                ('978-2-223-45678-9', 'Dreams of Tomorrow', 'Mia Dawson', 2015, 1),
                ('978-2-334-56789-0', 'Stormbreaker', 'Noah Cross', 2018, 1),
                ('978-2-445-67890-1', 'Fallen Kingdom', 'Olivia Snow', 2016, 1),
                ('978-2-556-78901-2', 'The Crystal Code', 'Patrick Moon', 2020, 1),
                ('978-2-667-89012-3', 'Voyage Beyond', 'Quinn Harper', 2019, 1),
                ('978-2-778-90123-4', 'Serpent’s Curse', 'Rachel Blaze', 2017, 1),
                ('978-2-889-01234-5', 'Rise of the Phoenix', 'Samuel Drake', 2012, 1),
                ('978-3-001-12345-6', 'The Golden Compass', 'Tina Meadows', 2014, 1),
                ('978-3-112-23456-7', 'Garden of Shadows', 'Ulysses Crane', 2011, 1),
                ('978-3-223-34567-8', 'Ashes and Embers', 'Victoria Lane', 2015, 1),
                ('978-3-334-45678-9', 'Heart of Ice', 'William Frost', 2016, 1),
                ('978-3-445-56789-0', 'Pathfinder', 'Xavier Knight', 2020, 1),
                ('978-3-556-67890-1', 'Beyond the Veil', 'Yasmine Brooks', 2018, 1);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Books WHERE Id BETWEEN 1 AND 25;
            ");
        }
    }
}
