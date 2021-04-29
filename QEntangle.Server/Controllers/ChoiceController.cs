using Microsoft.AspNetCore.Mvc;


namespace QEntangle.Server.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ChoiceController : Controller
  {
    [HttpGet]
    public IActionResult Get()
    {
      return this.Ok();
    }
  }
}
