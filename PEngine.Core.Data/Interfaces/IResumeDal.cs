using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IResumeDal : IBaseDal
    {
      IEnumerable<ResumeObjectiveModel> ListResumeObjectives();
      void InsertResumeObjective(ResumeObjectiveModel objective, bool importFlag = false);
      void UpdateResumeObjective(ResumeObjectiveModel objective);
      void DeleteResumeObjective(Guid guid);
      IEnumerable<ResumePersonalModel> ListResumePersonals();
      void InsertResumePersonal(ResumePersonalModel personal, bool importFlag = false);
      void UpdateResumePersonal(ResumePersonalModel personal);
      void DeleteResumePersonal(Guid guid);
      IEnumerable<ResumeSkillModel> ListResumeSkills();
      void InsertResumeSkill(ResumeSkillModel skill, bool importFlag = false);
      void UpdateResumeSkill(ResumeSkillModel skill);
      void DeleteResumeSkill(Guid guid);
      IEnumerable<ResumeEducationModel> ListResumeEducations();
      void InsertResumeEducation(ResumeEducationModel education, bool importFlag = false);
      void UpdateResumeEducation(ResumeEducationModel education);
      void DeleteResumeEducation(Guid guid);
      IEnumerable<ResumeWorkHistoryModel> ListResumeWorkHistories();
      void InsertResumeWorkHistory(ResumeWorkHistoryModel workHistory, bool importFlag = false);
      void UpdateResumeWorkHistory(ResumeWorkHistoryModel workHistory);
      void DeleteResumeWorkHistory(Guid guid);
      void DeleteAllResumes();
    }
}
