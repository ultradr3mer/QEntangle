﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QEntangle.Server.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace QEntangle.Server.Services
{
  public interface IAuthService
  {
    #region Properties

    SqlConnection Connection { get; }
    string Login { get; }
    Guid UserId { get; }

    #endregion Properties

    #region Methods

    Task<UserEntity> Authenticate(string login, string password);
    Task<SqlConnection> Connect();

    #endregion Methods
  }

  internal class AuthService : IAuthService
  {
    #region Fields

    private readonly IConfiguration configuration;
    private readonly UserSevice userSevice;

    #endregion Fields

    #region Constructors

    public AuthService(IConfiguration configuration, UserSevice userSevice)
    {
      this.configuration = configuration;
      this.userSevice = userSevice;
    }

    #endregion Constructors

    #region Properties

    public SqlConnection Connection { get; private set; }
    public string Login { get; private set; }
    public Guid UserId { get; private set; }

    #endregion Properties

    #region Methods

    public async Task<UserEntity> Authenticate(string login, string password)
    {
      var connection = await this.Connect();
      var user = await this.GetUser(login, connection);
      bool success = this.userSevice.CheckPassword(password, user.Salt, user.Password);
      if (!success)
      {
        throw new Exception("Password incorrect.");
      }

      this.Connection = connection;
      this.Login = login;
      this.UserId = user.Id;

      return user;
    }

    public async Task<SqlConnection> Connect()
    {
      return await Task.Run(() =>
      {
        string connectionString = this.configuration["ConnectionString"];
        SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
      });
    }

    public async Task<UserEntity> GetUser(string login, SqlConnection connection)
    {
      using DatabaseContext context = new DatabaseContext(connection);

      var result = await (from s in context.QeUser
                          where s.Login == login
                          select s).FirstAsync();

      return result;
    }

    #endregion Methods
  }
}