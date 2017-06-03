using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PEngine.Core.Shared;
using System.Text.RegularExpressions;

namespace PEngine.Core.Web.Controllers
{
  [Route("[controller]")]
  public class HashController : Controller
  {
    private Regex _md5HashRegex = new Regex(@"(?:[0-9]|[A-F]){32}");
    public HashController()
    {
    }

    [HttpGet("{*hashedPath}")]
    public IActionResult GetHashedFileName(string hashedPath)
    {
      if (!string.IsNullOrWhiteSpace(hashedPath))
      {
        string[] elements = hashedPath.Split('.');
        string originalPath = hashedPath;
        switch (elements.Last().ToLower())
        {
          //Certain file types are assumed to be exempt from hash mapping, including .map files
          case "map":
            break;
          default:
            originalPath = String.Join(".", elements.Where((e) => _md5HashRegex.Matches(e).Count != 1));
            break;
        }
        string originalFullPath = System.IO.Path.Combine(Startup.ContentRootPath, "wwwroot", originalPath);
        if (System.IO.File.Exists(originalFullPath))
        {
          string contentType;
          new FileExtensionContentTypeProvider().TryGetContentType(originalFullPath, out contentType);
          return File(System.IO.File.OpenRead(originalFullPath), contentType ?? "application/octet-stream");
        }
        return NotFound();
      }
      return BadRequest();
    }
  }
}