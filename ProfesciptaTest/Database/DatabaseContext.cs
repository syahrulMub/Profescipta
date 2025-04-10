using Microsoft.EntityFrameworkCore;

namespace ProfesciptaTest.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

}
