using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Dapper;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public enum DatabaseType
  {
    PEngine,
    Misc,
    Forum
  }

  public static class Database
  {
    private static IDataProvider _dataProvider;
    private static Dictionary<DatabaseType, ConcurrentQueue<int?>> _singleWriteQueue = new Dictionary<DatabaseType, ConcurrentQueue<int?>>();
    private static Dictionary<DatabaseType, int?> _singleWriteQueueCurrentThreadId = new Dictionary<DatabaseType, int?>();

    public static void Startup(string contentRootPath, IDataProvider dataProvider)
    {
      ContentRootPath = contentRootPath;
      var databaseTypes = Enum.GetValues(typeof(DatabaseType));
      foreach (var databaseType in databaseTypes)
      {
       _singleWriteQueue.Add((DatabaseType)databaseType, new ConcurrentQueue<int?>());
       _singleWriteQueueCurrentThreadId.Add((DatabaseType)databaseType, null);
      }
      _dataProvider = dataProvider;
      dataProvider.Init(dataProvider.RequiresFolder ? DatabaseFolderPath : null);
      Update();
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

            using (var ct = OpenTransaction(databaseType, false))
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
    
    public static ConnTranWrapper OpenConnection(DatabaseType type, bool readOnly)
    {
      var threadId = Thread.CurrentThread.ManagedThreadId;
      if (!readOnly && _dataProvider.SingleWrite)
      {
        WaitForSingleWriteAccess(type, threadId);
        return new ConnTranWrapper(_dataProvider.GetConnection(type, readOnly), type, true, threadId, _dataProvider.Name);
      }
      return new ConnTranWrapper(_dataProvider.GetConnection(type, readOnly), type, false, threadId, _dataProvider.Name);
    }

    public static ConnTranWrapper OpenTransaction(DatabaseType type, bool readOnly)
    {
      var threadId = Thread.CurrentThread.ManagedThreadId;
      if (!readOnly && _dataProvider.SingleWrite)
      {
        WaitForSingleWriteAccess(type, threadId);
        return new ConnTranWrapper(_dataProvider.GetTransaction(type, readOnly), type, true, threadId, _dataProvider.Name);
      }
      return new ConnTranWrapper(_dataProvider.GetTransaction(type, false), type, false, threadId, _dataProvider.Name);
    }

    private static void WaitForSingleWriteAccess(DatabaseType type, int threadId)
    {
      if (_singleWriteQueueCurrentThreadId[type].HasValue)
      {
        _singleWriteQueue[type].Enqueue(threadId);
      }
      else
      {
        _singleWriteQueueCurrentThreadId[type] = threadId;
      }
      while (_singleWriteQueueCurrentThreadId[type].HasValue && _singleWriteQueueCurrentThreadId[type].Value != threadId)
      {
        Thread.Sleep(100);
      }
    }

    public static void ReleaseSingleWriteAccess(DatabaseType type, int threadId)
    {
      int? currentThreadId = _singleWriteQueueCurrentThreadId[type];
      if (currentThreadId.HasValue && currentThreadId.Value == threadId)
      {
        int? nextThreadId = null;
        while (_singleWriteQueue[type].Count > 0 && !_singleWriteQueue[type].TryDequeue(out nextThreadId));
        _singleWriteQueueCurrentThreadId[type] = nextThreadId;
      }
      else
      {
        if (currentThreadId.HasValue)
        {
          throw new Exception($"Thead {threadId} attempted to release single write access lock belonging to {currentThreadId.Value}");
        }
        else
        {
          throw new Exception($"Thead {threadId} attempted to release non-existent single write access lock");
        }
      }
    }

    public static IEnumerable<string> DatabaseUpdateFiles(DatabaseType type)
    {
      return System.IO.Directory.EnumerateFiles(DatabaseUpdatePath(type), "*.sql");
    }

    private static string DatabaseUpdatePath(DatabaseType type)
    {
      var databaseUpdatePath = $"{ContentRootPath}sql{System.IO.Path.DirectorySeparatorChar}";
      if (!System.IO.Directory.Exists(databaseUpdatePath))
      {
        System.IO.Directory.CreateDirectory(databaseUpdatePath);
      }
      databaseUpdatePath += $"{_dataProvider.Name}{System.IO.Path.DirectorySeparatorChar}";
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
        case DatabaseType.Forum:
          return $"{databaseUpdatePath}forum{System.IO.Path.DirectorySeparatorChar}";
      }
      return null;
    }

    private static string DatabaseFolderPath
    {
      get
      {
        var databasePath = $"{ContentRootPath}data{System.IO.Path.DirectorySeparatorChar}";
        if (!System.IO.Directory.Exists(databasePath))
        {
          System.IO.Directory.CreateDirectory(databasePath);
        }
        databasePath += $"{_dataProvider.Name}{System.IO.Path.DirectorySeparatorChar}";
        if (!System.IO.Directory.Exists(databasePath))
        {
          System.IO.Directory.CreateDirectory(databasePath);
        }
        return databasePath;
      }
    }

    private static string _contentRootPath;
    public static string ContentRootPath
    {
      get
      {
        return _contentRootPath;
      }
      private set 
      {
        _contentRootPath = value + (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? string.Empty : System.IO.Path.DirectorySeparatorChar.ToString());
      }
    }
  }
}
