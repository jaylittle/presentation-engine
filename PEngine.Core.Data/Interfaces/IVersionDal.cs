using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
  public interface IVersionDal
  {
    IEnumerable<VersionModel> ListVersions(Database.DatabaseType type);
    VersionModel GetCurrentVersion(Database.DatabaseType type);
    void InsertVersion(Database.DatabaseType type, VersionModel version);
  }
}