using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class ResumeDal : BaseDal<ResumeDal>, IResumeDal
  {
    public IEnumerable<ResumeObjectiveModel> ListResumeObjectives()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ResumeObjectiveModel>(ReadQuery("ListResumeObjectives", ct.ProviderName));
      }
    }

    public void InsertResumeObjective(ResumeObjectiveModel objective)
    {
      objective.UpdateGuid();
      objective.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertResumeObjective", ct.ProviderName), objective);
      }
    }

    public void UpdateResumeObjective(ResumeObjectiveModel objective)
    {
      objective.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateResumeObjective", ct.ProviderName), objective);
      }
    }

    public void DeleteResumeObjective(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteResumeObjective", ct.ProviderName), new {
          guid
        });
      }
    }

    public IEnumerable<ResumePersonalModel> ListResumePersonals()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ResumePersonalModel>(ReadQuery("ListResumePersonals", ct.ProviderName));
      }
    }

    public void InsertResumePersonal(ResumePersonalModel personal)
    {
      personal.UpdateGuid();
      personal.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertResumePersonal", ct.ProviderName), personal);
      }
    }
    
    public void UpdateResumePersonal(ResumePersonalModel personal)
    {
      personal.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateResumePersonal", ct.ProviderName), personal);
      }
    }

    public void DeleteResumePersonal(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteResumePersonal", ct.ProviderName), new {
          guid
        });
      }
    }

    public IEnumerable<ResumeSkillModel> ListResumeSkills()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ResumeSkillModel>(ReadQuery("ListResumeSkills", ct.ProviderName));
      }
    }

    public void InsertResumeSkill(ResumeSkillModel skill)
    {
      skill.UpdateGuid();
      skill.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertResumeSkill", ct.ProviderName), skill);
      }
    }

    public void UpdateResumeSkill(ResumeSkillModel skill)
    {
      skill.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateResumeSkill", ct.ProviderName), skill);
      }
    }

    public void DeleteResumeSkill(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteResumeSkill", ct.ProviderName), new {
          guid
        });
      }
    }

    public IEnumerable<ResumeEducationModel> ListResumeEducations()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ResumeEducationModel>(ReadQuery("ListResumeEducations", ct.ProviderName));
      }
    }

    public void InsertResumeEducation(ResumeEducationModel education)
    {
      education.UpdateGuid();
      education.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertResumeEducation", ct.ProviderName), education);
      }
    }

    public void UpdateResumeEducation(ResumeEducationModel education)
    {
      education.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateResumeEducation", ct.ProviderName), education);
      }
    }

    public void DeleteResumeEducation(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteResumeEducation", ct.ProviderName), new {
          guid
        });
      }
    }

    public IEnumerable<ResumeWorkHistoryModel> ListResumeWorkHistories()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ResumeWorkHistoryModel>(ReadQuery("ListResumeWorkHistories", ct.ProviderName));
      }
    }

    public void InsertResumeWorkHistory(ResumeWorkHistoryModel workHistory)
    {
      workHistory.UpdateGuid();
      workHistory.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertResumeWorkHistory", ct.ProviderName), workHistory);
      }
    }

    public void UpdateResumeWorkHistory(ResumeWorkHistoryModel workHistory)
    {
      workHistory.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateResumeWorkHistory", ct.ProviderName), workHistory);
      }
    }

    public void DeleteResumeWorkHistory(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteResumeWorkHistory", ct.ProviderName), new {
          guid
        });
      }
    }
  }
}
