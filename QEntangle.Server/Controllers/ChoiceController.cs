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
    private readonly IAuthService authService;

    public ChoiceController(IAuthService authService)
    {
      this.authService = authService;
    }

    #region Methods

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

      var result = entities.Select(c => new ChoiceGetData()
      {
        Id = c.Id,
        Name = c.Name,
        Options = c.Options.Split(",")
      }).ToList();

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

      return new ChoiceGetData()
      {
        Id = entity.Id,
        Name = entity.Name,
        Options = optionsArray
      };
    }

    #endregion Methods
  }
}