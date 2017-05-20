using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;

namespace PEngine.Core.Data.Interfaces
{
  public interface IVersionDal
  {
    IEnumerable<VersionModel> ListVersions(DatabaseType type);
    VersionModel GetCurrentVersion(DatabaseType type);
    void InsertVersion(DatabaseType type, VersionModel version);
  }
}