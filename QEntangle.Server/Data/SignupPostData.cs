namespace QEntangle.Server.Data
{
  /// <summary>
  /// The data to signup a new user.
  /// </summary>
  public class SignupPostData
  {
    #region Properties

    /// <summary>
    /// The password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// The username.
    /// </summary>
    public string Username { get; set; }

    #endregion Properties
  }
}