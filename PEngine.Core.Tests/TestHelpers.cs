using System;
using System.Linq;
using System.Threading.Tasks;
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

    public static async Task<bool> CallProducedError(Func<Task<OpResult>> func, string errorMessage)
    {
      return (await func()).LogMessages.Any(e => string.Equals(errorMessage, e.Text, StringComparison.OrdinalIgnoreCase));
    }
  }
}