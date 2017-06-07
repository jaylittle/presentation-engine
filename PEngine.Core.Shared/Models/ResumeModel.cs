using System;
using System.Collections.Generic;

namespace PEngine.Core.Shared.Models
{
  public class ResumeModel : ISubTitleModel
  {
    public List<ResumePersonalModel> Personals { get; set; } = new List<ResumePersonalModel>();
    public List<ResumeObjectiveModel> Objectives { get; set; } = new List<ResumeObjectiveModel>();
    public Dictionary<string, List<ResumeSkillModel>> Skills { get; set; } = new Dictionary<string, List<ResumeSkillModel>>(StringComparer.OrdinalIgnoreCase);
    public List<ResumeEducationModel> Educations { get; set; } = new List<ResumeEducationModel>();
    public List<ResumeWorkHistoryModel> WorkHistories { get; set; } = new List<ResumeWorkHistoryModel>();

    public string GetSubTitle(bool inList, string currentSection, int? currentPage)
    {
      return $"{Settings.Current.LabelResumeButton}";
    }
  }
}