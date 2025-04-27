using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_app.Migrations
{
    /// <inheritdoc />
    public partial class SeedMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Members (FirstName, LastName, DOB, Email)
                VALUES
                ('John', 'Doe', '1990-05-10', 'john.doe@gmail.com'),
                ('Jane', 'Smith', '1985-11-20', 'jane.smith@yahoo.com'),
                ('David', 'Johnson', '1992-08-15', 'david.johnson@outlook.com'),
                ('Emily', 'Brown', '1988-04-05', 'emily.brown@aol.com'),
                ('Michael', 'Davis', '1995-01-25', 'michael.davis@icloud.com'),
                ('Sarah', 'Miller', '1987-09-13', 'sarah.miller@live.com'),
                ('Daniel', 'Wilson', '1993-06-30', 'daniel.wilson@protonmail.com'),
                ('Sophia', 'Moore', '1991-03-22', 'sophia.moore@mail.com'),
                ('Matthew', 'Taylor', '1989-12-14', 'matthew.taylor@hotmail.com'),
                ('Olivia', 'Anderson', '1994-07-02', 'olivia.anderson@gmail.com'),
                ('James', 'Thomas', '1982-10-17', 'james.thomas@outlook.com'),
                ('Isabella', 'Jackson', '1996-02-09', 'isabella.jackson@yahoo.com'),
                ('Alexander', 'White', '1984-11-03', 'alexander.white@icloud.com'),
                ('Mia', 'Harris', '1997-05-21', 'mia.harris@zoho.com'),
                ('Lucas', 'Clark', '1990-08-12', 'lucas.clark@mail.com');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Members WHERE Id BETWEEN 1 AND 15;
            ");
        }
    }
}
