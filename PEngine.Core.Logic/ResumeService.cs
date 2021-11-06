using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Logic
{
  public class ResumeService : IResumeService
  {
    public const string RESUME_ERROR_DATA_MUST_BE_PROVIDED = "Resume data must be provided";
    public const string PERSONAL_ERROR_INVALID_RECORD = "Personal #{0}: Guid refers to an invalid record";
    public const string PERSONAL_ERROR_FULL_NAME_IS_REQUIRED = "Personal #{0}: Full Name is required";
    public const string PERSONAL_ERROR_EMAIL_IS_REQUIRED = "Personal #{0}: Email is required";
    public const string PERSONAL_ERROR_DATA_MUST_BE_PROVIDED = "At least one Personal record is required";
    public const string OBJECTIVE_ERROR_INVALID_RECORD = "Objective #{0}: Guid refers to an invalid record";
    public const string OBJECTIVE_ERROR_CONTENT_IS_REQUIRED = "Objective #{0}: Content is required";
    public const string OBJECTIVE_ERROR_DATA_MUST_BE_PROVIDED = "At least one Objective record is required";
    public const string SKILL_ERROR_INVALID_RECORD = "Skill #{0}: Guid refers to an invalid record";
    public const string SKILL_ERROR_TYPE_IS_REQUIRED = "Skill #{0}: Type is required";
    public const string SKILL_ERROR_NAME_IS_REQUIRED = "Skill #{0}: Name is required";
    public const string EDUCATION_ERROR_INVALID_RECORD = "Education #{0}: Guid refers to an invalid record";
    public const string EDUCATION_ERROR_INSTITUTE_IS_REQUIRED = "Education #{0}: Institute is required";
    public const string EDUCATION_ERROR_PROGRAM_IS_REQUIRED = "Education #{0}: Program is required";
    public const string EDUCATION_ERROR_STARTED_IS_REQUIRED = "Education #{0}: Started is required";
    public const string WORK_ERROR_INVALID_RECORD = "Work History #{0}: Guid refers to an invalid record";
    public const string WORK_ERROR_EMPLOYER_IS_REQUIRED = "Work History #{0}: Employer is required";
    public const string WORK_ERROR_JOB_TITLE_IS_REQUIRED = "Work History #{0}: Job Title is required";
    public const string WORK_ERROR_JOB_DESCRIPTION_IS_REQUIRED = "Work History #{0}: Job Description is required";
    public const string WORK_ERROR_STARTED_IS_REQUIRED = "Work History #{0}: Started is required";

    private IResumeDal _resumeDal;

    public ResumeService(IResumeDal resumeDal)
    {
      _resumeDal = resumeDal;
    }

    public async Task<ResumeModel> GetResume()
    {
      var retvalue = new ResumeModel() {
        Personals = (await _resumeDal.ListResumePersonals())
          .ToList(),
        Objectives = (await _resumeDal.ListResumeObjectives())
          .ToList(),
        SkillTypes = (await _resumeDal.ListResumeSkills())
          .GroupBy(s => s.Type, StringComparer.OrdinalIgnoreCase)
          .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
          .Select(g => new ResumeSkillTypeModel(g.Key, g.OrderBy(s => s.Order).ToList()))
          .ToList(),
        Educations = (await _resumeDal.ListResumeEducations())
          .OrderByDescending(ed => ed.Started)
          .ThenBy(ed => ed.Institute)
          .ThenBy(ed => ed.Program)
          .ToList(),
        WorkHistories = (await _resumeDal.ListResumeWorkHistories())
          .OrderByDescending(wh => wh.Started)
          .ThenBy(wh => wh.Employer)
          .ThenBy(wh => wh.JobTitle)
          .ToList()
      };
      return retvalue;
    }

    public async Task<OpResult> UpsertResume(ResumeModel resume, bool importFlag = false)
    {
      var retvalue = new OpResult();
      ResumeModel existingResume = await GetResume();
      var existingPersonalGuids = existingResume.Personals.Select(r => r.Guid).ToDictionary(g => g, g => true);
      var existingObjectiveGuids = existingResume.Objectives.Select(r => r.Guid).ToDictionary(g => g, g => true);
      var existingSkillGuids = existingResume.SkillTypes.SelectMany(t => t.Skills.Select(r => r.Guid)).ToDictionary(g => g, g => true);
      var existingEducationGuids = existingResume.Educations.Select(r => r.Guid).ToDictionary(g => g, g => true);
      var existingWorkHistoryGuids = existingResume.WorkHistories.Select(r => r.Guid).ToDictionary(g => g, g => true);
      
      if (resume == null)
      {
        retvalue.LogError(RESUME_ERROR_DATA_MUST_BE_PROVIDED);
        return retvalue;
      }

      if (resume.Personals != null  && resume.Personals.Count > 0)
      {
        var counter = 1;
        foreach (var personal in resume.Personals)
        {
          if (!importFlag && personal.Guid != Guid.Empty && !existingPersonalGuids.ContainsKey(personal.Guid))
          {
            retvalue.LogError(string.Format(PERSONAL_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(personal.FullName))
          {
            retvalue.LogError(string.Format(PERSONAL_ERROR_FULL_NAME_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(personal.Email))
          {
            retvalue.LogError(string.Format(PERSONAL_ERROR_EMAIL_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      else
      {
        retvalue.LogError(PERSONAL_ERROR_DATA_MUST_BE_PROVIDED);
      }
      if (resume.Objectives != null && resume.Objectives.Count > 0)
      {
        var counter = 1;
        foreach(var objective in resume.Objectives)
        {
          if (!importFlag && objective.Guid != Guid.Empty && !existingObjectiveGuids.ContainsKey((objective.Guid)))
          {
            retvalue.LogError(string.Format(OBJECTIVE_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(objective.Data))
          {
            retvalue.LogError(string.Format(OBJECTIVE_ERROR_CONTENT_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      else
      {
        retvalue.LogError(OBJECTIVE_ERROR_DATA_MUST_BE_PROVIDED);
      }
      if (resume.SkillTypes != null && resume.SkillTypes.Count > 0 && resume.SkillTypes.Any(s => s.Skills != null && s.Skills.Count > 0))
      {
        var counter = 1;
        foreach(var skillType in resume.SkillTypes)
        {
          if (skillType.Skills != null && skillType.Skills.Count > 0)
          {
            foreach (var skill in skillType.Skills)
            {
              if (!importFlag && skill.Guid != Guid.Empty && !existingSkillGuids.ContainsKey(skill.Guid))
              {
                retvalue.LogError(string.Format(SKILL_ERROR_INVALID_RECORD, counter));
              }
              if (string.IsNullOrWhiteSpace(skill.Type))
              {
                retvalue.LogError(string.Format(SKILL_ERROR_TYPE_IS_REQUIRED, counter));
              }
              if (string.IsNullOrWhiteSpace(skill.Name))
              {
                retvalue.LogError(string.Format(SKILL_ERROR_NAME_IS_REQUIRED, counter));
              }
              counter++;
            }
          }
        }
      }
      if (resume.Educations != null && resume.Educations.Count > 0)
      {
        var counter = 1;
        foreach (var education in resume.Educations)
        {
          if (!importFlag && education.Guid != Guid.Empty && !existingEducationGuids.ContainsKey((education.Guid)))
          {
            retvalue.LogError(string.Format(EDUCATION_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(education.Institute))
          {
            retvalue.LogError(string.Format(EDUCATION_ERROR_INSTITUTE_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(education.Program))
          {
            retvalue.LogError(string.Format(EDUCATION_ERROR_PROGRAM_IS_REQUIRED, counter));
          }
          if (!education.Started.HasValue)
          {
            retvalue.LogError(string.Format(EDUCATION_ERROR_STARTED_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      if (resume.WorkHistories != null && resume.WorkHistories.Count > 0)
      {
        var counter = 1;
        foreach (var workHistory in resume.WorkHistories)
        {
          if (!importFlag && workHistory.Guid != Guid.Empty && !existingWorkHistoryGuids.ContainsKey((workHistory.Guid)))
          {
            retvalue.LogError(string.Format(WORK_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(workHistory.Employer))
          {
            retvalue.LogError(string.Format(WORK_ERROR_EMPLOYER_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(workHistory.JobTitle))
          {
            retvalue.LogError(string.Format(WORK_ERROR_JOB_TITLE_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(workHistory.JobDescription))
          {
            retvalue.LogError(string.Format(WORK_ERROR_JOB_DESCRIPTION_IS_REQUIRED, counter));
          }
          if (!workHistory.Started.HasValue)
          {
            retvalue.LogError(string.Format(WORK_ERROR_STARTED_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      if (retvalue.Successful)
      {
        _resumeDal.AddTransaction(DatabaseType.PEngine, Database.OpenTransaction(DatabaseType.PEngine, false));
        
        try
        {
          if (resume.Personals != null)
          {
            foreach (var personal in resume.Personals)
            {
              if (importFlag || personal.Guid == Guid.Empty || !existingPersonalGuids.ContainsKey(personal.Guid))
              {
                await _resumeDal.InsertResumePersonal(personal, importFlag);
              }
              else
              {
                await _resumeDal.UpdateResumePersonal(personal);
              }
              existingPersonalGuids.Remove(personal.Guid);
            }
          }
          foreach (var guidToDelete in existingPersonalGuids)
          {
            await _resumeDal.DeleteResumePersonal(guidToDelete.Key);
          }

          if (resume.Objectives != null)
          {
            foreach (var objective in resume.Objectives)
            {
              if (importFlag || objective.Guid == Guid.Empty || !existingObjectiveGuids.ContainsKey(objective.Guid))
              {
                await _resumeDal.InsertResumeObjective(objective, importFlag);
              }
              else
              {
                await _resumeDal.UpdateResumeObjective(objective);
              }
              existingObjectiveGuids.Remove(objective.Guid);
            }
          }
          foreach (var guidToDelete in existingObjectiveGuids)
          {
            await _resumeDal.DeleteResumeObjective(guidToDelete.Key);
          }

          if (resume.SkillTypes != null)
          {
            foreach (var skillType in resume.SkillTypes)
            {
              int skillOrder = -1;
              if (skillType.Skills != null && skillType.Skills.Count > 0)
              {
                foreach (var skill in skillType.Skills)
                {
                  skillOrder +=1 ;
                  skill.Order = skillOrder;
                  if (importFlag || skill.Guid == Guid.Empty || !existingSkillGuids.ContainsKey(skill.Guid))
                  {
                    await _resumeDal.InsertResumeSkill(skill, importFlag);
                  }
                  else
                  {
                    await _resumeDal.UpdateResumeSkill(skill);
                  }
                  existingSkillGuids.Remove(skill.Guid);
                }
              }
            }
          }
          foreach (var guidToDelete in existingSkillGuids)
          {
            await _resumeDal.DeleteResumeSkill(guidToDelete.Key);
          }

          if (resume.Educations != null)
          {
            foreach (var education in resume.Educations)
            {
              if (importFlag || education.Guid == Guid.Empty || !existingEducationGuids.ContainsKey(education.Guid))
              {
                await _resumeDal.InsertResumeEducation(education, importFlag);
              }
              else
              {
                await _resumeDal.UpdateResumeEducation(education);
              }
              existingEducationGuids.Remove(education.Guid);
            }
          }
          foreach (var guidToDelete in existingEducationGuids)
          {
            await _resumeDal.DeleteResumeEducation(guidToDelete.Key);
          }

          if (resume.WorkHistories != null)
          {
            foreach (var workHistory in resume.WorkHistories)
            {
              if (importFlag || workHistory.Guid == Guid.Empty || !existingWorkHistoryGuids.ContainsKey(workHistory.Guid))
              {
                await _resumeDal.InsertResumeWorkHistory(workHistory, importFlag);
              }
              else
              {
                await _resumeDal.UpdateResumeWorkHistory(workHistory);
              }
              existingWorkHistoryGuids.Remove(workHistory.Guid);
            }
          }
          foreach (var guidToDelete in existingWorkHistoryGuids)
          {
            await _resumeDal.DeleteResumeWorkHistory(guidToDelete.Key);
          }
          _resumeDal.CommitTransaction(DatabaseType.PEngine);
        }
        catch (Exception ex)
        {
          _resumeDal.RollBackTransaction(DatabaseType.PEngine);
          throw new Exception("Resume Transaction Failed", ex);
        }
      }
      return retvalue;
    }
  }
}