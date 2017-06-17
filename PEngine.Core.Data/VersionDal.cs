using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class VersionDal : BaseDal<VersionDal>, IVersionDal
  {
    public async Task<IEnumerable<VersionModel>> ListVersions(DatabaseType type)
    {
      using (var ct = GetConnection(type, true))
      {
        return await ct.DbConnection.QueryAsync<VersionModel>(ReadQuery("ListVersions", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task<VersionModel> GetCurrentVersion(DatabaseType type)
    {
      IEnumerable<VersionModel> versions = null;
      var defaultVersionModel = new VersionModel();
      try
      {
        versions = await ListVersions(type);
        Console.WriteLine($"Database {type}: Has {versions.Count()} version records");
      }
      catch
      {
        //Just eat it - either DB doesnt exist or it hasn't been initialized
        Console.WriteLine($"Database {type}: Has no version records");
      }
      if (versions != null && versions.Any())
      {
        return versions.OrderByDescending(v => v.Combined).First();
      }
      return defaultVersionModel;
    }

    public async Task InsertVersion(DatabaseType type, VersionModel version)
    {
      version.UpdateGuid();
      version.UpdateTimestamps(true);

      using (var ct = GetConnection(type, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertVersion", ct.ProviderName), version, transaction: ct.DbTransaction);
      }
    }
  }
}
