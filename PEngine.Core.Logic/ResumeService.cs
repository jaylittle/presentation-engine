using System;
using System.Linq;
using System.Collections.Generic;
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

    public ResumeModel GetResume()
    {
      var retvalue = new ResumeModel() {
        Personals = _resumeDal.ListResumePersonals().ToList(),
        Objectives = _resumeDal.ListResumeObjectives().ToList(),
        Skills = _resumeDal.ListResumeSkills().GroupBy(s => s.Type, StringComparer.OrdinalIgnoreCase).ToDictionary(s => s.Key, s => s.ToList()),
        Educations = _resumeDal.ListResumeEducations().ToList(),
        WorkHistories = _resumeDal.ListResumeWorkHistories().ToList()
      };
      return retvalue;
    }

    public bool UpsertResume(ResumeModel resume, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      ResumeModel existingResume = GetResume();
      if (resume == null)
      {
        errors.Add(RESUME_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }

      if (resume.Personals != null  && resume.Personals.Count > 0)
      {
        var counter = 1;
        foreach (var personal in resume.Personals)
        {
          if (personal.Guid != Guid.Empty && !existingResume.Personals.Any(p => p.Guid == personal.Guid))
          {
            errors.Add(string.Format(PERSONAL_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(personal.FullName))
          {
            errors.Add(string.Format(PERSONAL_ERROR_FULL_NAME_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(personal.Email))
          {
            errors.Add(string.Format(PERSONAL_ERROR_EMAIL_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      else
      {
        errors.Add(PERSONAL_ERROR_DATA_MUST_BE_PROVIDED);
      }
      if (resume.Objectives != null && resume.Objectives.Count > 0)
      {
        var counter = 1;
        foreach(var objective in resume.Objectives)
        {
          if (objective.Guid != Guid.Empty && !existingResume.Objectives.Any(o => o.Guid == objective.Guid))
          {
            errors.Add(string.Format(OBJECTIVE_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(objective.Data))
          {
            errors.Add(string.Format(OBJECTIVE_ERROR_CONTENT_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      else
      {
        errors.Add(OBJECTIVE_ERROR_DATA_MUST_BE_PROVIDED);
      }
      if (resume.Skills != null && resume.Skills.Count > 0 && resume.Skills.Any(s => s.Value != null && s.Value.Count > 0))
      {
        var counter = 1;
        foreach(var skillType in resume.Skills)
        {
          if (skillType.Value != null && skillType.Value.Count > 0)
          {
            foreach (var skill in skillType.Value)
            {
              if (skill.Guid != Guid.Empty && !existingResume.Skills.Any(t => t.Value.Any(s => s.Guid == skill.Guid)))
              {
                errors.Add(string.Format(SKILL_ERROR_INVALID_RECORD, counter));
              }
              if (string.IsNullOrWhiteSpace(skill.Type))
              {
                errors.Add(string.Format(SKILL_ERROR_TYPE_IS_REQUIRED, counter));
              }
              if (string.IsNullOrWhiteSpace(skill.Name))
              {
                errors.Add(string.Format(SKILL_ERROR_NAME_IS_REQUIRED, counter));
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
          if (education.Guid != Guid.Empty && !existingResume.Educations.Any(e => e.Guid == education.Guid))
          {
            errors.Add(string.Format(EDUCATION_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(education.Institute))
          {
            errors.Add(string.Format(EDUCATION_ERROR_INSTITUTE_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(education.Program))
          {
            errors.Add(string.Format(EDUCATION_ERROR_PROGRAM_IS_REQUIRED, counter));
          }
          if (!education.Started.HasValue)
          {
            errors.Add(string.Format(EDUCATION_ERROR_STARTED_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      if (resume.WorkHistories != null && resume.WorkHistories.Count > 0)
      {
        var counter = 1;
        foreach (var workHistory in resume.WorkHistories)
        {
          if (workHistory.Guid != Guid.Empty && !existingResume.WorkHistories.Any(wh => wh.Guid == workHistory.Guid))
          {
            errors.Add(string.Format(WORK_ERROR_INVALID_RECORD, counter));
          }
          if (string.IsNullOrWhiteSpace(workHistory.Employer))
          {
            errors.Add(string.Format(WORK_ERROR_EMPLOYER_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(workHistory.JobTitle))
          {
            errors.Add(string.Format(WORK_ERROR_JOB_TITLE_IS_REQUIRED, counter));
          }
          if (string.IsNullOrWhiteSpace(workHistory.JobDescription))
          {
            errors.Add(string.Format(WORK_ERROR_JOB_DESCRIPTION_IS_REQUIRED, counter));
          }
          if (!workHistory.Started.HasValue)
          {
            errors.Add(string.Format(WORK_ERROR_STARTED_IS_REQUIRED, counter));
          }
          counter++;
        }
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        var existingPersonalGuids = _resumeDal.ListResumePersonals().Select(r => r.Guid).ToList();
        var existingObjectiveGuids = _resumeDal.ListResumeObjectives().Select(r => r.Guid).ToList();
        var existingSkillGuids = _resumeDal.ListResumeSkills().Select(r => r.Guid).ToList();
        var existingEducationGuids = _resumeDal.ListResumeEducations().Select(r => r.Guid).ToList();
        var existingWorkHistoryGuids = _resumeDal.ListResumeWorkHistories().Select(r => r.Guid).ToList();

        _resumeDal.AddTransaction(DatabaseType.PEngine, Database.OpenTransaction(DatabaseType.PEngine, false));
        
        try
        {
          if (resume.Personals != null)
          {
            foreach (var personal in resume.Personals)
            {
              if (personal.Guid == Guid.Empty || !existingPersonalGuids.Contains(personal.Guid))
              {
                _resumeDal.InsertResumePersonal(personal);
              }
              else
              {
                _resumeDal.UpdateResumePersonal(personal);
              }
              existingPersonalGuids.Remove(personal.Guid);
            }
          }
          foreach (var guidToDelete in existingPersonalGuids)
          {
            _resumeDal.DeleteResumePersonal(guidToDelete);
          }

          if (resume.Objectives != null)
          {
            foreach (var objective in resume.Objectives)
            {
              if (objective.Guid == Guid.Empty || !existingObjectiveGuids.Contains(objective.Guid))
              {
                _resumeDal.InsertResumeObjective(objective);
              }
              else
              {
                _resumeDal.UpdateResumeObjective(objective);
              }
              existingObjectiveGuids.Remove(objective.Guid);
            }
          }
          foreach (var guidToDelete in existingObjectiveGuids)
          {
            _resumeDal.DeleteResumeObjective(guidToDelete);
          }

          if (resume.Skills != null)
          {
            foreach (var skillType in resume.Skills)
            {
              if (skillType.Value != null && skillType.Value.Count > 0)
              {
                foreach (var skill in skillType.Value)
                {
                  if (skill.Guid == Guid.Empty || !existingSkillGuids.Contains(skill.Guid))
                  {
                    _resumeDal.InsertResumeSkill(skill);
                  }
                  else
                  {
                    _resumeDal.UpdateResumeSkill(skill);
                  }
                  existingSkillGuids.Remove(skill.Guid);
                }
              }
            }
          }
          foreach (var guidToDelete in existingSkillGuids)
          {
            _resumeDal.DeleteResumeSkill(guidToDelete);
          }

          if (resume.Educations != null)
          {
            foreach (var education in resume.Educations)
            {
              if (education.Guid == Guid.Empty || !existingEducationGuids.Contains(education.Guid))
              {
                _resumeDal.InsertResumeEducation(education);
              }
              else
              {
                _resumeDal.UpdateResumeEducation(education);
              }
              existingEducationGuids.Remove(education.Guid);
            }
          }
          foreach (var guidToDelete in existingEducationGuids)
          {
            _resumeDal.DeleteResumeEducation(guidToDelete);
          }

          if (resume.WorkHistories != null)
          {
            foreach (var workHistory in resume.WorkHistories)
            {
              if (workHistory.Guid == Guid.Empty || !existingWorkHistoryGuids.Contains(workHistory.Guid))
              {
                _resumeDal.InsertResumeWorkHistory(workHistory);
              }
              else
              {
                _resumeDal.UpdateResumeWorkHistory(workHistory);
              }
              existingWorkHistoryGuids.Remove(workHistory.Guid);
            }
          }
          foreach (var guidToDelete in existingWorkHistoryGuids)
          {
            _resumeDal.DeleteResumeWorkHistory(guidToDelete);
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