using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class ResumeDal : BaseDal<ResumeDal>, IResumeDal
  {
    public async Task<IEnumerable<ResumeObjectiveModel>> ListResumeObjectives()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ResumeObjectiveModel>(ReadQuery("ListResumeObjectives", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task InsertResumeObjective(ResumeObjectiveModel objective, bool importFlag = false)
    {
      objective.UpdateGuid();
      objective.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertResumeObjective", ct.ProviderName), objective, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateResumeObjective(ResumeObjectiveModel objective)
    {
      objective.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateResumeObjective", ct.ProviderName), objective, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteResumeObjective(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteResumeObjective", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ResumePersonalModel>> ListResumePersonals()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ResumePersonalModel>(ReadQuery("ListResumePersonals", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task InsertResumePersonal(ResumePersonalModel personal, bool importFlag = false)
    {
      personal.UpdateGuid();
      personal.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertResumePersonal", ct.ProviderName), personal, transaction: ct.DbTransaction);
      }
    }
    
    public async Task UpdateResumePersonal(ResumePersonalModel personal)
    {
      personal.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateResumePersonal", ct.ProviderName), personal, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteResumePersonal(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteResumePersonal", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ResumeSkillModel>> ListResumeSkills()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ResumeSkillModel>(ReadQuery("ListResumeSkills", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task InsertResumeSkill(ResumeSkillModel skill, bool importFlag = false)
    {
      skill.UpdateGuid();
      skill.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertResumeSkill", ct.ProviderName), skill, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateResumeSkill(ResumeSkillModel skill)
    {
      skill.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateResumeSkill", ct.ProviderName), skill, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteResumeSkill(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteResumeSkill", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ResumeEducationModel>> ListResumeEducations()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ResumeEducationModel>(ReadQuery("ListResumeEducations", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task InsertResumeEducation(ResumeEducationModel education, bool importFlag = false)
    {
      education.UpdateGuid();
      education.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertResumeEducation", ct.ProviderName), education, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateResumeEducation(ResumeEducationModel education)
    {
      education.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateResumeEducation", ct.ProviderName), education, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteResumeEducation(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteResumeEducation", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ResumeWorkHistoryModel>> ListResumeWorkHistories()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ResumeWorkHistoryModel>(ReadQuery("ListResumeWorkHistories", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task InsertResumeWorkHistory(ResumeWorkHistoryModel workHistory, bool importFlag = false)
    {
      workHistory.UpdateGuid();
      workHistory.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertResumeWorkHistory", ct.ProviderName), workHistory, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateResumeWorkHistory(ResumeWorkHistoryModel workHistory)
    {
      workHistory.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateResumeWorkHistory", ct.ProviderName), workHistory, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteResumeWorkHistory(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteResumeWorkHistory", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteAllResumes()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteAllResumes", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }
  }
}
