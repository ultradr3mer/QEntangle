using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QEntangle.Server.Data;
using QEntangle.Server.Database;
using QEntangle.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QEntangle.Server.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ChoiceController : Controller
  {
    #region Fields

    private readonly IAuthService authService;
    private readonly QrngAnuClient qrng;

    #endregion Fields

    #region Constructors

    public ChoiceController(IAuthService authService, QrngAnuClient qrng)
    {
      this.authService = authService;
      this.qrng = qrng;
    }

    #endregion Constructors

    #region Methods

    [HttpPost("Execute")]
    public async Task<ChoiceGetData> ExecuteAsync(Guid choiceId)
    {
      using var connection = authService.Connection;
      var context = new DatabaseContext(connection);
      var entity = await context.Choice.FindAsync(choiceId);

      if(entity.UserId != authService.UserId)
      {
        throw new Exception("Choice does not belong to this user.");
      }

      if (!string.IsNullOrEmpty(entity.DefinitiveOption))
      {
        throw new Exception("Choices can only be executed once.");
      }

      var options = entity.Options.Split(',');

      var qrngResult = await this.qrng.JsonIphpAsync(Services.Type.Uint8, 1, null);
      var number = qrngResult.Data.Single();

      int definitiveOptionNumber = number * options.Length / 256;
      string definitiveOptionString = options[definitiveOptionNumber];

      entity.DefinitiveOption = definitiveOptionString;
      await context.SaveChangesAsync();
      await connection.CloseAsync();
      var result = CreateGetDataFromEntity(entity);

      return result;
    }

    /// <summary>
    /// Gets all choices of the current user.
    /// </summary>
    /// <returns>The users choices.</returns>
    [HttpGet]
    public async Task<IList<ChoiceGetData>> Get()
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);

      var entities = await (from c in context.Choice
                            where c.UserId == authService.UserId
                            select c).ToListAsync();

      var result = entities.Select(CreateGetDataFromEntity).ToList();

      return result;
    }

    /// <summary>
    /// Posts a new choice.
    /// </summary>
    /// <returns>The choice data.</returns>
    [HttpPost]
    public async Task<ChoiceGetData> PostAsync(ChoicePostData data)
    {
      using var connection = authService.Connection;

      var optionsArray = data.Options.Split(",").Select(o => o.Trim()).ToArray();
      if (optionsArray.Length < 2)
      {
        throw new Exception("At least 2 options are required.");
      }
      string optionsString = string.Join(",", optionsArray);

      var entity = new ChoiceEntity()
      {
        Name = data.Name.Trim(),
        Options = optionsString,
        UserId = authService.UserId
      };

      var context = new DatabaseContext(connection);
      await context.AddAsync(entity);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return CreateGetDataFromEntity(entity);
    }

    private static ChoiceGetData CreateGetDataFromEntity(ChoiceEntity entity)
    {
      return new ChoiceGetData()
      {
        Id = entity.Id,
        Name = entity.Name,
        Options = entity.Options.Split(","),
        DefinitiveOption = entity.DefinitiveOption
      };
    }

    #endregion Methods
  }
}