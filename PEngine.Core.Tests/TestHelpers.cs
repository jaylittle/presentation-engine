using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Tests
{
  public class TestHelpers
  {
    public static bool CallProducedError(Func<OpResult> func, string errorMessage)
    {
      return func().LogMessages.Any(e => string.Equals(errorMessage, e.Text, StringComparison.OrdinalIgnoreCase));
    }
  }
}