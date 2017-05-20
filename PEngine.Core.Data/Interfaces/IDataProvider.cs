using System;
using System.Collections.Generic;
using System.Data.Common;
using PEngine.Core.Data;

namespace PEngine.Core.Data.Interfaces
{
  public interface IDataProvider
  {
    string Name { get; }
    bool RequiresFolder { get; }
    bool SingleWrite { get; }
    void Init(string contentRootPath);
    DbConnection GetConnection(DatabaseType type, bool readOnly = true);
    DbTransaction GetTransaction(DatabaseType type, bool readOnly = false);
  }
}