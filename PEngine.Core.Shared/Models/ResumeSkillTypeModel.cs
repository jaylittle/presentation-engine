using System;
using PEngine.Core.Shared.Interfaces;
using System.Collections.Generic;

namespace PEngine.Core.Shared.Models
{
  public class ResumeSkillTypeModel
  {
    public string Type { get; set; } = string.Empty;
    public List<ResumeSkillModel> Skills { get; set; } = new List<ResumeSkillModel>();

    public ResumeSkillTypeModel(string type, List<ResumeSkillModel> skills)
    {
      Type = type;
      Skills = skills;
    }
  }
}
