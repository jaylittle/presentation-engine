using System;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data.Providers
{
  public class SQLiteDataProvider : IDataProvider
  {
    public string Name
    {
      get
      {
        return "sqlite";
      }
    }

    public bool RequiresFolder
    {
      get
      {
        return true;
      }
    }

    public bool SingleWrite
    {
      get
      {
        return true;
      }
    }

    private string _databaseFolderPath = null;

    public void Init(string databaseFolderPath = null)
    {
      _databaseFolderPath = databaseFolderPath;
      SetupDapper();
    }

    private void SetupDapper()
    {
      Dapper.SqlMapper.AddTypeHandler(typeof(Guid?), new SqliteGuidTypeHandler());
      Dapper.SqlMapper.AddTypeHandler(typeof(Guid), new SqliteGuidTypeHandler());
      Dapper.SqlMapper.AddTypeHandler(typeof(long?), new SqliteNLongTypeHandler());
      Dapper.SqlMapper.AddTypeHandler(typeof(long), new SqliteLongTypeHandler());
    }

    public class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
      public override Guid Parse(object value)
      {
        if (value is byte[])
        {
          return value != null ? new Guid((byte[])value) : Guid.Empty;
        }
        return value != null ? Guid.Parse((string)value) : Guid.Empty;
      }

      public override void SetValue(System.Data.IDbDataParameter parameter, Guid value)
      {
        parameter.Value = value.ToString();
      }
    }

    public class SqliteNLongTypeHandler : SqlMapper.TypeHandler<long?>
    {
      public override long? Parse(object value)
      {
        return value != null ? (long?)value : null;
      }

      public override void SetValue(System.Data.IDbDataParameter parameter, long? value)
      {
        parameter.Value = value;
      }
    }

    public class SqliteLongTypeHandler : SqlMapper.TypeHandler<long>
    {
      public override long Parse(object value)
      {
        return value != null ? (long)value : default(long);
      }

      public override void SetValue(System.Data.IDbDataParameter parameter, long value)
      {
        parameter.Value = value;
      }
    }

    public DbConnection GetConnection(DatabaseType type, bool readOnly = true)
    {
      var conn = new SqliteConnection(ConnectionString(type));
      conn.Open();
      return conn;
    }

    public DbTransaction GetTransaction(DatabaseType type, bool readOnly = false)
    {
      var conn = new SqliteConnection(ConnectionString(type));
      conn.Open();
      return conn.BeginTransaction();
    }

    private string ConnectionString(DatabaseType type)
    {
      string databasePath = string.Empty;
      switch (type)
      {
        case DatabaseType.PEngine:
          databasePath = $"{_databaseFolderPath}pengine.db";
          break;
        case DatabaseType.Misc:
          databasePath = $"{_databaseFolderPath}misc.db";
          break;
      }
      return $"Data Source={databasePath}";
    }
  }
}
