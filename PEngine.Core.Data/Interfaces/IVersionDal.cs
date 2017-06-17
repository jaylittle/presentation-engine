using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;

namespace PEngine.Core.Data.Interfaces
{
  public interface IVersionDal : IBaseDal
  {
    Task<IEnumerable<VersionModel>> ListVersions(DatabaseType type);
    Task<VersionModel> GetCurrentVersion(DatabaseType type);
    Task InsertVersion(DatabaseType type, VersionModel version);
  }
}