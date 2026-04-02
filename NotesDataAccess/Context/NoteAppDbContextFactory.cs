using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotesDataAccess.Context
{
    public class NoteAppDbContextFactory : IDesignTimeDbContextFactory<NoteAppDbContext>
    {
        public NoteAppDbContext CreateDbContext(string[] args)
        {
            var connString = Environment.GetEnvironmentVariable
                             ("ConnectionStrings__NotesConnection");

            Console.WriteLine($"Using connection: {connString}");

            var optionsBuilder = new DbContextOptionsBuilder<NoteAppDbContext>();
            optionsBuilder.UseNpgsql(connString);

            return new NoteAppDbContext(optionsBuilder.Options);
        }
    }
}