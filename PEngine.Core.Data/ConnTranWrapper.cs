using System;
using System.Data;

namespace PEngine.Core.Data
{
  public class ConnTranWrapper : IDisposable
  {
    public ConnTranWrapper(IDbConnection connection)
    {
      DbConnection = connection;
      DbTransaction = null;
    }

    public ConnTranWrapper(IDbTransaction transaction)
    {
      DbConnection = transaction.Connection;
      DbTransaction = transaction;
    }
    public IDbConnection DbConnection { get; set; }
    public IDbTransaction DbTransaction { get; set; }

    public void Dispose()
    {
      if (DbTransaction == null)
      {
        DbConnection.Dispose();
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
      }
    }

    public bool HasBeenCommited { get; set; }
    public bool HasBeenRolledBack { get; set; }
  }
}