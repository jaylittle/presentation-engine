using System;
using System.Data;
using System.Collections.Generic;
using PEngine.Core.Data;
using PEngine.Core.Shared;

namespace PEngine.Core.Data
{
  public interface IBaseDal
  {
    ConnTranWrapper GetConnection(DatabaseType databaseType, bool readOnly);
    ConnTranWrapper OpenTransaction(DatabaseType databaseType, bool readOnly);
    void AddTransaction(DatabaseType databaseType, ConnTranWrapper wrapper);
    void RollBackTransaction(DatabaseType databaseType);
    void CommitTransaction(DatabaseType databaseType);
    void RemoveTransaction(DatabaseType databaseType);
  }

  public abstract class BaseDal<TService> : IBaseDal
  {
    protected Dictionary<DatabaseType, ConnTranWrapper> Transactions = new Dictionary<DatabaseType, ConnTranWrapper>();
    
    protected string ReadQuery(string queryName, string providerName)
    {
      return Resources.ReadQueryResource<TService>(queryName, providerName);
    }

    public ConnTranWrapper GetConnection(DatabaseType databaseType, bool readOnly)
    {
      if (Transactions.ContainsKey(databaseType))
      {
        return Transactions[databaseType];
      }
      else
      {
        return Database.OpenConnection(databaseType, readOnly);
      }
    }

    public ConnTranWrapper OpenTransaction(DatabaseType databaseType, bool readOnly)
    {
      var wrapper = Database.OpenTransaction(databaseType, readOnly);
      AddTransaction(databaseType, wrapper);
      return wrapper;
    }

    public void AddTransaction(DatabaseType databaseType, ConnTranWrapper wrapper)
    {
      Transactions.Add(databaseType, wrapper);
    }

    public void RollBackTransaction(DatabaseType databaseType)
    {
      Transactions[databaseType].RollBackTransaction();
      Transactions.Remove(databaseType);
    }

    public void CommitTransaction(DatabaseType databaseType)
    {
      Transactions[databaseType].CommitTransaction();
      Transactions.Remove(databaseType);
    }

    public void RemoveTransaction(DatabaseType databaseType)
    {
      Transactions.Remove(databaseType);
    }
  }
}