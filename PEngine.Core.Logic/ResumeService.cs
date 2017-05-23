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
      if (resume.Personals != null  && resume.Personals.Count > 0)
      {
        var counter = 1;
        foreach (var personal in resume.Personals)
        {
          if (string.IsNullOrWhiteSpace(personal.FullName))
          {
            errors.Add($"Personal #{counter}: Full Name is required");
          }
          if (string.IsNullOrWhiteSpace(personal.Address1))
          {
            errors.Add($"Personal #{counter}: Address 1 is required");
          }
          if (string.IsNullOrWhiteSpace(personal.City))
          {
            errors.Add($"Personal #{counter}: City is required");
          }
          if (string.IsNullOrWhiteSpace(personal.State))
          {
            errors.Add($"Personal #{counter}: State is required");
          }
          if (string.IsNullOrWhiteSpace(personal.Zip))
          {
            errors.Add($"Personal #{counter}: Zip is required");
          }
          if (string.IsNullOrWhiteSpace(personal.Email))
          {
            errors.Add($"Personal #{counter}: Email is required");
          }
          counter++;
        }
      }
      else
      {
        errors.Add("At least one Personal record is required");
      }
      if (resume.Objectives != null && resume.Objectives.Count > 0)
      {
        var counter = 1;
        foreach(var objective in resume.Objectives)
        {
          if (string.IsNullOrWhiteSpace(objective.Data))
          {
            errors.Add($"Objective #{counter}: Text is required");
          }
          counter++;
        }
      }
      else
      {
        errors.Add("At least one Objective record is required");
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
              if (string.IsNullOrWhiteSpace(skill.Type))
              {
                errors.Add($"Skill #{counter}: Type is required");
              }
              if (string.IsNullOrWhiteSpace(skill.Name))
              {
                errors.Add($"Skill #{counter}: Name is required");
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
          if (string.IsNullOrWhiteSpace(education.Institute))
          {
            errors.Add($"Education #{counter}: Institute is required");
          }
          if (string.IsNullOrWhiteSpace(education.Program))
          {
            errors.Add($"Education #{counter}: Program is required");
          }
          if (!education.Started.HasValue)
          {
            errors.Add($"Education #{counter}: Started is required");
          }
          counter++;
        }
      }
      if (resume.WorkHistories != null && resume.WorkHistories.Count > 0)
      {
        var counter = 1;
        foreach (var workHistory in resume.WorkHistories)
        {
          if (string.IsNullOrWhiteSpace(workHistory.Employer))
          {
            errors.Add($"Work History #{counter}: Employer is required");
          }
          if (string.IsNullOrWhiteSpace(workHistory.JobTitle))
          {
            errors.Add($"Work History #{counter}: Job Title is required");
          }
          if (string.IsNullOrWhiteSpace(workHistory.JobDescription))
          {
            errors.Add($"Work History #{counter}: Job Description is required");
          }
          if (!workHistory.Started.HasValue)
          {
            errors.Add($"Work History #{counter}: Started is required");
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