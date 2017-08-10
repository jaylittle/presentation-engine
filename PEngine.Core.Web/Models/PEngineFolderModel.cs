using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PEngine.Core.Shared;
using Newtonsoft.Json;

namespace PEngine.Core.Web.Models
{
  public class PEngineFolderModel
  {
    public string Name { get; set; }
    [JsonIgnore]
    public bool Valid { get; set; }
    [JsonIgnore]
    public string FullPath { get; set; }
    [JsonIgnore]
    public PEngineFolderModel Parent { get; set; }
    public string AbsolutePath
    {
      get
      {
        var rootPath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}");
        var relativePath = FullPath.Substring(rootPath.TrimEnd(Path.DirectorySeparatorChar).Length).Replace(Path.DirectorySeparatorChar, '/');
        return $"{Settings.Current.ExternalBaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
      } 
    }
    public string RelativePath
    {
      get
      {
        var rootPath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}");
        return $".{FullPath.Substring(rootPath.TrimEnd(Path.DirectorySeparatorChar).Length).Replace(Path.DirectorySeparatorChar, '/')}";
      } 
    }
    public List<PEngineFileModel> Files { get; set; }
    public List<PEngineFolderModel> Folders { get; set; }

    public PEngineFolderModel(string webFolderPath, bool populateChildren = true)
    {
      var relativePath = webFolderPath.Replace('/', Path.DirectorySeparatorChar).TrimStart('/');
      var fullFolderPath = $"{Startup.ContentRootPath.TrimEnd(Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}wwwroot{Path.DirectorySeparatorChar}{relativePath}";
      if (System.IO.File.Exists(fullFolderPath))
      {
        Init(new DirectoryInfo(fullFolderPath), populateChildren);
      }
      else
      {
        Valid = false;
      }
    }

    public PEngineFolderModel(DirectoryInfo current, bool populateChildren = true)
    {
      Init(current, populateChildren);
    }

    private void Init (DirectoryInfo current, bool populateChildren = true)
    {
      Name = current.Name.Equals("wwwroot", StringComparison.OrdinalIgnoreCase) ? "[Root]" : current.Name;
      FullPath = current.FullName;
      if (populateChildren)
      {
        Files = current.GetFiles().Select(f => new PEngineFileModel(f)).ToList();
        Folders = current.GetDirectories().Select(d => new PEngineFolderModel(d, false)).ToList();
        Parent = new PEngineFolderModel(current.Parent, false);
      }
      Valid = true;
    }
  }
}