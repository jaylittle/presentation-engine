using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft​.AspNetCore​.Mvc​.ApplicationModels;

//Note: The majority of this code was shamelessly copied and pasted from:
//https://www.strathweb.com/2016/09/required-query-string-parameters-in-asp-net-core-mvc/
namespace PEngine.Core.Web.Constraints
{
  public class RequiredFromQueryActionConstraint : IActionConstraint
  {
    private readonly string _parameter;

    public RequiredFromQueryActionConstraint(string parameter)
    {
      _parameter = parameter;
    }

    public int Order => 999;

    public bool Accept(ActionConstraintContext context)
    {
      if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
      {
        return false;
      }

      return true;
    }
  }

  public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
  {
    public void Apply(ParameterModel parameter)
    {
      if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
      {
        parameter.Action.Selectors.Last().ActionConstraints.Add(new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
      }
    }
  }
}