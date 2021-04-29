using System;

namespace QEntangle.Server.Data
{
  /// <summary>
  /// The retruned data when getting a choice.
  /// </summary>
  public class ChoiceGetData
  {
    #region Properties

    /// <summary>
    /// The Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The options.
    /// </summary>
    public string[] Options { get; set; }

    #endregion Properties
  }
}