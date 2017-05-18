using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data
{
  public class Database
  {

    public enum DatabaseType
    {
      PEngine,
      Misc
    }

    public static void Startup(string contentRootPath)
    {
      ContentRootPath = contentRootPath;
      SetupDapper();
      Update();
    }

    public static void SetupDapper()
    {
      Dapper.SqlMapper.AddTypeHandler(typeof(Guid), new SqliteGuidTypeHandler());
    }

    public class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
      public override Guid Parse(object value)
      {
        return value != null ? new Guid((byte[])value) : Guid.Empty;
      }

      public override void SetValue(System.Data.IDbDataParameter parameter, Guid value)
      {
        parameter.Value = value.ToByteArray();
      }
    }

    public static void Update()
    {
      var databases = Enum.GetValues(typeof(DatabaseType));
      foreach (var database in databases)
      {
        var databaseType = (DatabaseType)database;
        var currentVersion = new VersionDal().GetCurrentVersion(databaseType);
        var updates = DatabaseUpdateFiles(databaseType)
          .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

        Console.WriteLine($"Database {databaseType}: Current Version is {currentVersion}");
          
        foreach (var update in updates)
        {
          var updateVersion = VersionModel.FromFileName(update);
          if (updateVersion.Combined > currentVersion.Combined)
          {
            Console.WriteLine($"Database {databaseType}: Updating to Version {updateVersion}");
            var updateCommands = System.IO.File.ReadAllText(update)
              .Split(new string[] { ";\n" }, StringSplitOptions.None)
              .Where(uc => !string.IsNullOrWhiteSpace(uc));

            using (var ct = OpenTransaction(databaseType))
            {
              var versionDal = new VersionDal();
              versionDal.AddTransaction(databaseType, ct);
              var lastUpdateCommand = string.Empty;
              try
              {
                foreach (var updateCommand in updateCommands)
                {
                  lastUpdateCommand = updateCommand;
                  ct.DbConnection.Execute(updateCommand);
                }
                lastUpdateCommand = "Standard_InsertVersion";
                versionDal.InsertVersion(databaseType, updateVersion);
                lastUpdateCommand = "Standard_CommitTransaction";
                ct.CommitTransaction();
              }
              catch (Exception ex)
              {
                ct.RollBackTransaction();
                throw new Exception($"{databaseType} version {updateVersion.ToString()} update failed while processing command {lastUpdateCommand}", ex);
              }
              finally
              {
                versionDal.RemoveTransaction(databaseType);
              }
            }
            currentVersion = updateVersion;
          }
        }
      }
    }
    
    public static ConnTranWrapper OpenConnection(DatabaseType type)
    {
      var conn = new SqliteConnection(ConnectionString(type));
      conn.Open();
      return new ConnTranWrapper(conn);
    }

    public static ConnTranWrapper OpenTransaction(DatabaseType type)
    {
      var conn = new SqliteConnection(ConnectionString(type));
      conn.Open();
      var transaction = conn.BeginTransaction();
      return new ConnTranWrapper(transaction);
    }

    public static string ConnectionString(DatabaseType type)
    {
      string relativePath = string.Empty;
      switch (type)
      {
        case DatabaseType.PEngine:
          relativePath = $"{DatabaseFolderPath}pengine.db";
          break;
        case DatabaseType.Misc:
          relativePath = $"{DatabaseFolderPath}misc.db";
          break;
      }
      return $"Data Source={System.IO.Path.Combine(ContentRootPath, relativePath)}";
    }

    public static string DatabaseFolderPath
    {
      get
      {
        var databasePath = $"{ContentRootPath}data{System.IO.Path.DirectorySeparatorChar}";
        if (!System.IO.Directory.Exists(databasePath))
        {
          System.IO.Directory.CreateDirectory(databasePath);
        }
        return databasePath;
      }
    }

    public static IEnumerable<string> DatabaseUpdateFiles(DatabaseType type)
    {
      return System.IO.Directory.EnumerateFiles(DatabaseUpdatePath(type), "*.sql");
    }

    public static string DatabaseUpdatePath(DatabaseType type)
    {
      var databaseUpdatePath = $"{ContentRootPath}sql{System.IO.Path.DirectorySeparatorChar}";
      if (!System.IO.Directory.Exists(databaseUpdatePath))
      {
        System.IO.Directory.CreateDirectory(databaseUpdatePath);
      }
      switch (type)
      {
        case DatabaseType.PEngine:
          return $"{databaseUpdatePath}pengine{System.IO.Path.DirectorySeparatorChar}";
        case DatabaseType.Misc:
          return $"{databaseUpdatePath}misc{System.IO.Path.DirectorySeparatorChar}";
      }
      return null;
    }

    private static string _contentRootPath;
    public static string ContentRootPath
    {
      get
      {
        return _contentRootPath;
      }
      set 
      {
        _contentRootPath = value + (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? string.Empty : System.IO.Path.DirectorySeparatorChar.ToString());
      }
    }
  }
}
