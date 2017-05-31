using System;
using System.Linq;
using System.Collections.Generic;

namespace PEngine.Core.Tests
{
  public class TestHelpers
  {
    public static bool CallProducedError(Func<List<string>, List<string>> func, string errorMessage)
    {
      return func(new List<string>()).Contains(errorMessage, StringComparer.OrdinalIgnoreCase);
    }
  }
}