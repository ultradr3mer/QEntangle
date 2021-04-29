namespace QEntangle.Server.Data
{
  /// <summary>
  /// The data to post a new choice.
  /// </summary>
  public class ChoicePostData
  {
    #region Properties

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The options, seperated by ",".
    /// </summary>
    public string Options { get; set; }

    #endregion Properties
  }
}