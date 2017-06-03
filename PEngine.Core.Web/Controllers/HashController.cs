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

    [HttpGet("{hash}/{*filePath}")]
    public IActionResult GetHashedFileName(string hash, string filePath)
    {
      if (_md5HashRegex.Matches(hash).Count == 1 && !string.IsNullOrWhiteSpace(filePath))
      {
        string fullPath = System.IO.Path.Combine(Startup.ContentRootPath, "wwwroot", filePath);
        if (System.IO.File.Exists(fullPath))
        {
          string contentType;
          new FileExtensionContentTypeProvider().TryGetContentType(fullPath, out contentType);
          return File(System.IO.File.OpenRead(fullPath), contentType ?? "application/octet-stream");
        }
        return NotFound();
      }
      return BadRequest();
    }
  }
}