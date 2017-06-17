using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IResumeDal : IBaseDal
    {
      Task<IEnumerable<ResumeObjectiveModel>> ListResumeObjectives();
      Task InsertResumeObjective(ResumeObjectiveModel objective, bool importFlag = false);
      Task UpdateResumeObjective(ResumeObjectiveModel objective);
      Task DeleteResumeObjective(Guid guid);
      Task<IEnumerable<ResumePersonalModel>> ListResumePersonals();
      Task InsertResumePersonal(ResumePersonalModel personal, bool importFlag = false);
      Task UpdateResumePersonal(ResumePersonalModel personal);
      Task DeleteResumePersonal(Guid guid);
      Task<IEnumerable<ResumeSkillModel>> ListResumeSkills();
      Task InsertResumeSkill(ResumeSkillModel skill, bool importFlag = false);
      Task UpdateResumeSkill(ResumeSkillModel skill);
      Task DeleteResumeSkill(Guid guid);
      Task<IEnumerable<ResumeEducationModel>> ListResumeEducations();
      Task InsertResumeEducation(ResumeEducationModel education, bool importFlag = false);
      Task UpdateResumeEducation(ResumeEducationModel education);
      Task DeleteResumeEducation(Guid guid);
      Task<IEnumerable<ResumeWorkHistoryModel>> ListResumeWorkHistories();
      Task InsertResumeWorkHistory(ResumeWorkHistoryModel workHistory, bool importFlag = false);
      Task UpdateResumeWorkHistory(ResumeWorkHistoryModel workHistory);
      Task DeleteResumeWorkHistory(Guid guid);
      Task DeleteAllResumes();
    }
}
