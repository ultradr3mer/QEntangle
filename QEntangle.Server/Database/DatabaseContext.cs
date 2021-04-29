using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace QEntangle.Server.Database
{
  public class DatabaseContext : DbContext
  {
    #region Fields

    private readonly DbConnection connection;

    #endregion Fields

    #region Constructors

    public DatabaseContext(DbConnection connection)
    {
      this.connection = connection;
    }

    #endregion Constructors

    #region Properties

    public DbSet<UserEntity> QeUser { get; set; }
    public DbSet<ChoiceEntity> Choice { get; set; }

    #endregion Properties

    #region Methods

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(this.connection);
    }

    #endregion Methods
  }
}