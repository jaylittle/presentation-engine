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
  [ResponseCache(CacheProfileName = "Content")]
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
      filePath = System.Net.WebUtility.UrlDecode(filePath);
      if (_md5HashRegex.Matches(hash).Count == 1 && !string.IsNullOrWhiteSpace(filePath) 
        && !filePath.Contains("..") && _filePathRegex.IsMatch(filePath))
      {
        var hashEntry = await ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", filePath, Helpers.Html.GetAbsoluteHashPath, true);
        if (hashEntry != null)
        {
          if (hashEntry.Hash.Equals(hash))
          {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(hashEntry.FullPath, out contentType);
            return this.File(hashEntry.Transformable ? hashEntry.Transformation : System.IO.File.ReadAllBytes(hashEntry.FullPath), contentType);
          }
          else
          {
            return RedirectPermanent(Helpers.Html.GetAbsoluteHashPath(hashEntry.Hash, hashEntry.WebPath));
          }
        }
        return NotFound();
      }
      return BadRequest();
    }
  }
}