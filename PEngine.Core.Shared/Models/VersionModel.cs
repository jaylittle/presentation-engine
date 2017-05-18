using System;
using PEngine.Core.Shared.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Shared.Models
{
  public class VersionModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; } = Guid.Empty;
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Build { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }

    public int Combined
    {
      get
      {
        return (Major * 1000000) + (Minor * 1000) + Build;
      }
    }

    public static VersionModel FromFileName(string filePath)
    {
      string fileName = filePath.Contains(System.IO.Path.DirectorySeparatorChar.ToString()) ? System.IO.Path.GetFileName(filePath) : filePath;
      var fileElements = fileName.Split('.');
      if (fileElements.Length >= 3)
      {
        var currentDT = DateTime.UtcNow;
        return new VersionModel()
        {
          Guid = Guid.NewGuid(),
          Major = int.Parse(fileElements[0]),
          Minor = int.Parse(fileElements[1]),
          Build = int.Parse(fileElements[2])
        };
      }
      return null;
    }

    public override string ToString()
    {
      return $"{Major}.{Minor}.{Build}";
    }
  }
}