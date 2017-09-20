using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using PEngine.Core.Shared;

namespace PEngine.Core.Web.Controllers
{
  [Route("hash")]
  public class HashController : Controller
  {
    private Regex _md5HashRegex = new Regex(@"(?:[0-9]|[A-F]){32}");
    private Regex _filePathRegex = new Regex(@"^[\w\d]");
    
    public HashController()
    {
    }

    [HttpGet("{hash}/{*filePath}")]
    public async Task<IActionResult> GetHashedFileName(string hash, string filePath)
    {
      Console.WriteLine($"Hash Request Received for {hash}/{filePath}");
      filePath = System.Net.WebUtility.UrlDecode(filePath);
      if (_md5HashRegex.Matches(hash).Count == 1 && !string.IsNullOrWhiteSpace(filePath) 
        && !filePath.Contains("..") && _filePathRegex.IsMatch(filePath))
      {
        var hashEntry = await ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", filePath, true);
        if (hashEntry != null)
        {
          if (hashEntry.Hash.Equals(hash))
          {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(hashEntry.FullPath, out contentType);
            return this.File(System.IO.File.ReadAllBytes(hashEntry.FullPath), contentType);
          }
          else
          {
            var urlHelper = new UrlHelper(ControllerContext);
            var hashUrl = System.Net.WebUtility.UrlDecode(urlHelper.Action("GetHashedFileName", "hash", new {
              hash = hashEntry.Hash,
              filePath = hashEntry.WebPath
            }));
            return RedirectPermanent(hashUrl);
          }
        }
        return NotFound();
      }
      return BadRequest();
    }
  }
}