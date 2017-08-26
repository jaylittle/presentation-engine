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

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost("copyTo/{*folderPath}")]
    public IActionResult CopyToFolder(string folderPath, [FromBody]PEngineResourceSelectionModel selections)
    {
      return CopyMoveResource(folderPath, selections, false);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost("moveTo/{*folderPath}")]
    public IActionResult MoveToFolder(string folderPath, [FromBody]PEngineResourceSelectionModel selections)
    {
      return CopyMoveResource(folderPath, selections, true);
    }

    private IActionResult CopyMoveResource(string targetFolderPath, PEngineResourceSelectionModel selections, bool moveFlag)
    {
      if (!IsRestrictedPath(targetFolderPath))
      {
        var targetFolder = new PEngineFolderModel(targetFolderPath);
        if (targetFolder.Valid && selections != null)
        {
          bool valid = false;
          if (selections.FilePaths != null && selections.FilePaths.Any())
          {
            foreach (var filePath in selections.FilePaths)
            {
              if (!IsRestrictedPath(filePath))
              {
                var file = new PEngineFileModel(filePath);
                var targetFullPath = $"{targetFolder.FullPath}{System.IO.Path.DirectorySeparatorChar}{file.Name}";
                if (file.Valid && !moveFlag)
                {
                  System.IO.File.Copy(file.FullPath, targetFullPath);
                  valid = true;
                }
                if (file.Valid && moveFlag)
                {
                  System.IO.File.Move(file.FullPath, targetFullPath);
                  valid = true;
                }
              }
            }
          }
          if (selections.FolderPaths != null && selections.FolderPaths.Any())
          {
            foreach (var folderPath in selections.FolderPaths)
            {
              if (!IsRestrictedPath(folderPath))
              {
                var folder = new PEngineFolderModel(folderPath);
                var targetFullPath = $"{targetFolder.FullPath}{System.IO.Path.DirectorySeparatorChar}{folder.Name}";
                if (folder.Valid && !moveFlag)
                {
                  CopyFolder(folder.FullPath, targetFullPath);
                  valid = true;
                }
                if (folder.Valid && moveFlag)
                {
                  System.IO.Directory.Move(folder.FullPath, targetFullPath);
                  valid = true;
                }
              }
            }
          }
          if (valid)
          {
            return Get(targetFolderPath);
          }
        }
      }
      return this.BadRequest();
    }

    static public void CopyFolder(string sourceFolder, string destFolder)
    {
      if (!Directory.Exists(destFolder))
      {
        Directory.CreateDirectory(destFolder);
      }

      string[] files = Directory.GetFiles(sourceFolder);
      foreach (string file in files)
      {
        string name = Path.GetFileName(file);
        string dest = Path.Combine(destFolder, name);
        System.IO.File.Copy(file, dest);
      }

      string[] folders = Directory.GetDirectories(sourceFolder);
      foreach (string folder in folders)
      {
        string name = Path.GetFileName(folder);
        string dest = Path.Combine(destFolder, name);
        CopyFolder(folder, dest);
      }
    }

    private bool IsRestrictedPath(string path)
    {
      if (path != string.Empty)
      {
        path = path.TrimStart('.').TrimStart('/');
        return path.Contains("..") || RESTRICTED_PATHS.Any(rp => path.StartsWith($"{rp}{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));
      }
      return false;
    }
  }
}
