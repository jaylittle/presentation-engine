using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IResumeDal
    {
      IEnumerable<ResumeObjectiveModel> ListResumeObjectives();
      void InsertResumeObjective(ResumeObjectiveModel objective);
      void UpdateResumeObjective(ResumeObjectiveModel objective);
      void DeleteResumeObjective(Guid guid);
      IEnumerable<ResumePersonalModel> ListResumePersonals();
      void InsertResumePersonal(ResumePersonalModel personal);
      void UpdateResumePersonal(ResumePersonalModel personal);
      void DeleteResumePersonal(Guid guid);
      IEnumerable<ResumeSkillModel> ListResumeSkills();
      void InsertResumeSkill(ResumeSkillModel skill);
      void UpdateResumeSkill(ResumeSkillModel skill);
      void DeleteResumeSkill(Guid guid);
      IEnumerable<ResumeEducationModel> ListResumeEducations();
      void InsertResumeEducation(ResumeEducationModel education);
      void UpdateResumeEducation(ResumeEducationModel education);
      void DeleteResumeEducation(Guid guid);
      IEnumerable<ResumeWorkHistoryModel> ListResumeWorkHistories();
      void InsertResumeWorkHistory(ResumeWorkHistoryModel workHistory);
      void UpdateResumeWorkHistory(ResumeWorkHistoryModel workHistory);
      void DeleteResumeWorkHistory(Guid guid);
    }
}
