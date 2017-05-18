using System;
using System.Data;
using System.Collections.Generic;
using PEngine.Core.Data;
using PEngine.Core.Shared;

namespace PEngine.Core.Data
{
  public interface IBaseDal
  {
    ConnTranWrapper GetConnection(Database.DatabaseType databaseType);
    ConnTranWrapper OpenTransaction(Database.DatabaseType databaseType);
    void AddTransaction(Database.DatabaseType databaseType, ConnTranWrapper wrapper);
    void RollBackTransaction(Database.DatabaseType databaseType);
    void CommitTransaction(Database.DatabaseType databaseType);
    void RemoveTransaction(Database.DatabaseType databaseType);
  }

  public abstract class BaseDal<TService> : IBaseDal
  {
    protected Dictionary<Database.DatabaseType, ConnTranWrapper> Transactions = new Dictionary<Database.DatabaseType, ConnTranWrapper>();
    
    protected string ReadQuery(string queryName)
    {
      return Resources.ReadQueryResource<TService>(queryName);
    }

    public ConnTranWrapper GetConnection(Database.DatabaseType databaseType)
    {
      if (Transactions.ContainsKey(databaseType))
      {
        return Transactions[databaseType];
      }
      else
      {
        return Database.OpenConnection(databaseType);
      }
    }

    public ConnTranWrapper OpenTransaction(Database.DatabaseType databaseType)
    {
      var wrapper = Database.OpenTransaction(databaseType);
      AddTransaction(databaseType, wrapper);
      return wrapper;
    }

    public void AddTransaction(Database.DatabaseType databaseType, ConnTranWrapper wrapper)
    {
      Transactions.Add(databaseType, wrapper);
    }

    public void RollBackTransaction(Database.DatabaseType databaseType)
    {
      Transactions[databaseType].RollBackTransaction();
      Transactions.Remove(databaseType);
    }

    public void CommitTransaction(Database.DatabaseType databaseType)
    {
      Transactions[databaseType].CommitTransaction();
      Transactions.Remove(databaseType);
    }

    public void RemoveTransaction(Database.DatabaseType databaseType)
    {
      Transactions.Remove(databaseType);
    }
  }
}