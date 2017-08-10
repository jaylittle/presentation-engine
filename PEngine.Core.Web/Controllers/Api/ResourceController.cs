using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;
using PEngine.Core.Web.Constraints;
using PEngine.Core.Web.Models;
using System.IO;

namespace PEngine.Core.Web.Controllers.Api
{
  [Route("api/[controller]")]
  public class ResourceController : Controller
  {
    public string[] RESTRICTED_PATHS = { "dist", "styles", "themes" };

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet("folder")]
    public IActionResult Get()
    {
      return Get(string.Empty);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet("folder/{*folderPath}")]
    public IActionResult Get(string folderPath)
    {
      folderPath = folderPath ?? string.Empty;
      if (!IsRestrictedPath(folderPath))
      {
        var folder = new PEngineFolderModel(folderPath);
        if (folder.Valid)
        {
          return this.Ok(folder);
        }
      }
      return this.BadRequest();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost("file/{*folderPath}")]
    public IActionResult UploadFile(string folderPath)
    {
      if (!IsRestrictedPath(folderPath) && Request.Form.Files.Any() && !Request.Form.Files.Any(f => f.Name.Contains("..")))
      {
        var folder = new PEngineFolderModel(folderPath);
        if (folder.Valid)
        {
          foreach (var file in Request.Form.Files)
          {
            var fullFilePath = Path.Combine(folder.FullPath, file.FileName);
            if (System.IO.File.Exists(fullFilePath))
            {
              System.IO.File.Delete(fullFilePath);
            }
            using (var writeStream = System.IO.File.OpenWrite(fullFilePath))
            {
              file.CopyTo(writeStream);
            }
          }
          return this.Ok();
        }
      }
      return this.BadRequest();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpDelete("folder/{*folderPath}")]
    public IActionResult DeleteFolder(string folderPath)
    {
      if (!IsRestrictedPath(folderPath))
      {
        var folder = new PEngineFolderModel(folderPath);
        if (folder.Valid)
        {
          System.IO.Directory.Delete(folder.FullPath);
          return this.Ok();
        }
        return this.NotFound();
      }
      return this.BadRequest();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost("folder/{*parentFolderPath}")]
    public IActionResult CreateFolder(string parentFolderPath, [FromQuery]string newFolderName)
    {
      if (!IsRestrictedPath(parentFolderPath))
      {
        var parentFolder = new PEngineFolderModel(parentFolderPath);
        if (parentFolder.Valid)
        {
          var newFolderPath = $"{parentFolder.FullPath.TrimEnd(Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}{newFolderName}";
          if (!System.IO.Directory.Exists(newFolderPath))
          {
            System.IO.Directory.CreateDirectory(newFolderPath);
            return this.Ok();
          }
          return this.BadRequest();
        }
        return this.NotFound();
      }
      return this.BadRequest();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPut("folder/{*folderPath}")]
    public IActionResult RenameFolder(string folderPath, [FromQuery]string newFolderName)
    {
      if (!IsRestrictedPath(folderPath))
      {
        var folder = new PEngineFolderModel(folderPath);
        if (folder.Valid)
        {
          var newFolderPath = $"{folder.Parent.FullPath.TrimEnd(Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}{newFolderName}";
          if (!System.IO.Directory.Exists(newFolderPath))
          {
            System.IO.Directory.Move(folder.FullPath, newFolderPath);
            return this.Ok();
          }
          return this.BadRequest();
        }
        return this.NotFound();
      }
      return this.BadRequest();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpDelete("file/{*filePath}")]
    public IActionResult DeleteFile(string filePath)
    {
      if (!IsRestrictedPath(filePath))
      {
        var file = new PEngineFileModel(filePath);
        if (file.Valid)
        {
          System.IO.File.Delete(file.FullPath);
          return this.Ok();
        }
        return this.NotFound();
      }
      return this.BadRequest();
    }

    private bool IsRestrictedPath(string path)
    {
      return path.Contains("..") || RESTRICTED_PATHS.Any(rp => path.StartsWith($"{rp}{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));
    }
  }
}
