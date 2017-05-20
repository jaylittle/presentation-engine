using System;
using System.Data;

namespace PEngine.Core.Data
{
  public class ConnTranWrapper : IDisposable
  {
    private DatabaseType _databaseType;
    private bool _isSingleWrite;
    private int _originalThreadId;

    public ConnTranWrapper(IDbConnection connection, DatabaseType databaseType, bool isSingleWrite, int originalThreadId, string providerName)
    {
      _databaseType = databaseType;
      _isSingleWrite = isSingleWrite;
      _originalThreadId = originalThreadId;
      DbConnection = connection;
      DbTransaction = null;
      ProviderName = providerName;
    }

    public ConnTranWrapper(IDbTransaction transaction, DatabaseType databaseType, bool isSingleWrite, int originalThreadId, string providerName)
    {
      _databaseType = databaseType;
      _isSingleWrite = isSingleWrite;
      _originalThreadId = originalThreadId;
      DbConnection = transaction.Connection;
      DbTransaction = transaction;
      ProviderName = providerName;
    }

    public IDbConnection DbConnection { get; set; }
    public IDbTransaction DbTransaction { get; set; }

    public string ProviderName { get; private set; }

    public void Dispose()
    {
      if (DbTransaction == null)
      {
        DbConnection.Dispose();
        if (_isSingleWrite)
        {
          Database.ReleaseSingleWriteAccess(_databaseType, _originalThreadId);
        }
      }
    }

    public void RollBackTransaction()
    {
      if (!HasBeenRolledBack && !HasBeenCommited)
      {
        DbTransaction.Rollback();
        DbConnection.Close();
        DbTransaction.Dispose();
        DbConnection.Dispose();
        HasBeenRolledBack = true;
        if (_isSingleWrite)
        {
          Database.ReleaseSingleWriteAccess(_databaseType, _originalThreadId);
        }
      }
    }

    public void CommitTransaction()
    {
      if (!HasBeenRolledBack && !HasBeenCommited)
      {
        DbTransaction.Commit();
        DbConnection.Close();
        DbTransaction.Dispose();
        DbConnection.Dispose();
        HasBeenCommited = true;
        if (_isSingleWrite)
        {
          Database.ReleaseSingleWriteAccess(_databaseType, _originalThreadId);
        }
      }
    }

    public bool HasBeenCommited { get; set; }
    public bool HasBeenRolledBack { get; set; }
  }
}