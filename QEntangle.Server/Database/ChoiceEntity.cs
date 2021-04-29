using System;

namespace QEntangle.Server.Database
{
  public class ChoiceEntity
  {
    #region Properties

    public string DefinitiveOption { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Options { get; set; }
    public UserEntity User { get; set; }
    public Guid UserId { get; set; }

    #endregion Properties
  }
}