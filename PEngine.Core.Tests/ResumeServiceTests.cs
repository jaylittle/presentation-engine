using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Shared.Models;
using Xunit;
using Moq;
using Newtonsoft.Json;

namespace PEngine.Core.Tests
{
  public class ResumeServiceTests
  {
    private ResumeModel resumeData;
    private Mock<IResumeDal> mockedResumeDal;
    private ResumeService resumeService;
    private Guid validGuid = Guid.NewGuid();
    private Guid invalidGuid = Guid.NewGuid();
    
    public ResumeServiceTests()
    {
      //Setup
      resumeData = new ResumeModel();
      mockedResumeDal = new Mock<IResumeDal>();
      mockedResumeDal.Setup(rd => rd.ListResumeEducations()).ReturnsAsync(new List<ResumeEducationModel>(){
        new ResumeEducationModel() { Guid = validGuid }
      });
      mockedResumeDal.Setup(rd => rd.ListResumeWorkHistories()).ReturnsAsync(new List<ResumeWorkHistoryModel>(){
        new ResumeWorkHistoryModel { Guid = validGuid }
      });
      mockedResumeDal.Setup(rd => rd.ListResumeSkills()).ReturnsAsync(new List<ResumeSkillModel>(){
        new ResumeSkillModel { Guid = validGuid }
      });
      mockedResumeDal.Setup(rd => rd.ListResumePersonals()).ReturnsAsync(new List<ResumePersonalModel>(){
        new ResumePersonalModel { Guid = validGuid }
      });
      mockedResumeDal.Setup(rd => rd.ListResumeObjectives()).ReturnsAsync(new List<ResumeObjectiveModel>(){
        new ResumeObjectiveModel { Guid = validGuid }
      });
      resumeService = new ResumeService(mockedResumeDal.Object);
    }

    [Fact]
    public void ResumeService_Upsert_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(TestHelpers.CallProducedError(() => 
          resumeService.UpsertResume(null).Result
        , ResumeService.RESUME_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , ResumeService.RESUME_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ResumeService_Upsert_Personal_GuidIsValidated()
    {
      //Verify that personal record with invalid guid is rejected
      resumeData.Personals.Add(new ResumePersonalModel() {
        Guid = invalidGuid
      });
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_INVALID_RECORD, 1)));

      //Verify that record with valid Guid is accepted
      resumeData.Personals[0].Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_INVALID_RECORD, 1)));

      //Verify that record with no Guid is accepted
      resumeData.Personals[0].Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_INVALID_RECORD, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Personal_FullNameIsValidated()
    {
      //Verify that personal record without full name is rejected
      resumeData.Personals.Add(new ResumePersonalModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_FULL_NAME_IS_REQUIRED, 1)));

      //Verify that personal record with full name is accepted
      resumeData.Personals[0].FullName = "Crap Full Name";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_FULL_NAME_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Personal_EmailIsValidated()
    {
      //Verify that personal record without email is rejected
      resumeData.Personals.Add(new ResumePersonalModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_EMAIL_IS_REQUIRED, 1)));

      //Verify that personal record with email is accepted
      resumeData.Personals[0].Email = "crap@domain.com";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.PERSONAL_ERROR_EMAIL_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Personal_RecordRequired()
    {
      //Verify that object without personal record is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , ResumeService.PERSONAL_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that object with personal record ias accepted
      resumeData.Personals.Add(new ResumePersonalModel());
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , ResumeService.PERSONAL_ERROR_DATA_MUST_BE_PROVIDED));
    }
    
    [Fact]
    public void ResumeService_Upsert_Objective_GuidIsValidated()
    {
      //Verify that objective record with invalid guid is rejected
      resumeData.Objectives.Add(new ResumeObjectiveModel() {
        Guid = invalidGuid
      });
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.OBJECTIVE_ERROR_INVALID_RECORD, 1)));

      //Verify that objective record with valid guid is accepted
      resumeData.Objectives[0].Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.OBJECTIVE_ERROR_INVALID_RECORD, 1)));

      //Verify that record with no Guid is accepted
      resumeData.Objectives[0].Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.OBJECTIVE_ERROR_INVALID_RECORD, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Objective_ContentIsValidated()
    {
      //Verify that objective record without content is rejected
      resumeData.Objectives.Add(new ResumeObjectiveModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.OBJECTIVE_ERROR_CONTENT_IS_REQUIRED, 1)));

      //Verify that objective record with content is accepted
      resumeData.Objectives[0].Data = "Crap Content";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result      
        , string.Format(ResumeService.OBJECTIVE_ERROR_CONTENT_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Objective_RecordRequired()
    {
      //Verify that object without objective record is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , ResumeService.OBJECTIVE_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that object with objective record ias accepted
      resumeData.Objectives.Add(new ResumeObjectiveModel());
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , ResumeService.OBJECTIVE_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ResumeService_Upsert_Skill_GuidIsValidated()
    {
      //Verify that skill record with invalid guid is rejected
      resumeData.SkillTypes.Add(new ResumeSkillTypeModel("Type", new List<ResumeSkillModel> {
        new ResumeSkillModel() { Guid = invalidGuid }
      }));
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.SKILL_ERROR_INVALID_RECORD, 1)));

      //Verify that skill record with valid guid is accepted
      resumeData.SkillTypes[0].Skills[0].Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.SKILL_ERROR_INVALID_RECORD, 1)));

      //Verify that skill record with no guid is accepted
      resumeData.SkillTypes[0].Skills[0].Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.SKILL_ERROR_INVALID_RECORD, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Skill_TypeIsValidated()
    {
      //Verify that skill record without type is rejected
      resumeData.SkillTypes.Add(new ResumeSkillTypeModel("Type", new List<ResumeSkillModel> {
        new ResumeSkillModel() { Guid = invalidGuid }
      }));
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.SKILL_ERROR_TYPE_IS_REQUIRED, 1)));

      //Verify that skill record with type is accepted
      resumeData.SkillTypes[0].Skills[0].Type = "Type";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.SKILL_ERROR_TYPE_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Skill_NameIsValidated()
    {
      //Verify that skill record without name is rejected
      resumeData.SkillTypes.Add(new ResumeSkillTypeModel("Type", new List<ResumeSkillModel> {
        new ResumeSkillModel() { Guid = invalidGuid }
      }));
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.SKILL_ERROR_NAME_IS_REQUIRED, 1)));

      //Verify that skill record with name is accepted
      resumeData.SkillTypes[0].Skills[0].Name = "Name";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result 
        , string.Format(ResumeService.SKILL_ERROR_NAME_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Education_GuidIsValidated()
    {
      //Verify that education record with invalid guid is rejected
      resumeData.Educations.Add(new ResumeEducationModel() {
        Guid = invalidGuid
      });
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_INVALID_RECORD, 1)));

      //Verify that education record with valid guid is accepted
      resumeData.Educations[0].Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_INVALID_RECORD, 1)));

      //Verify that education record with no guid is accepted
      resumeData.Educations[0].Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_INVALID_RECORD, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Education_InstituteIsValidated()
    {
      //Verify that education record without institute is rejected
      resumeData.Educations.Add(new ResumeEducationModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_INSTITUTE_IS_REQUIRED, 1)));

      //Verify that educaiton record with institute is accepted
      resumeData.Educations[0].Institute = "Crap Institute";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_INSTITUTE_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Education_ProgramIsValidated()
    {
      //Verify that education record without program is rejected
      resumeData.Educations.Add(new ResumeEducationModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_PROGRAM_IS_REQUIRED, 1)));

      //Verify that educaiton record with program is accepted
      resumeData.Educations[0].Program = "Crap Program";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_PROGRAM_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_Education_StartedIsValidated()
    {
      //Verify that education record without started is rejected
      resumeData.Educations.Add(new ResumeEducationModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_STARTED_IS_REQUIRED, 1)));

      //Verify that education record with started is accepted
      resumeData.Educations[0].Started = DateTime.UtcNow.Date;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.EDUCATION_ERROR_STARTED_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_WorkHistory_GuidIsValidated()
    {
      //Verify that work history record with invalid guid is rejected
      resumeData.WorkHistories.Add(new ResumeWorkHistoryModel() {
        Guid = invalidGuid
      });
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_INVALID_RECORD, 1)));

      //Verify that work history record with valid guid is accepted
      resumeData.WorkHistories[0].Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_INVALID_RECORD, 1)));

      //Verify that work history record with no guid is accepted
      resumeData.WorkHistories[0].Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_INVALID_RECORD, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_WorkHistory_EmployerIsValidated()
    {
      //Verify that work history record without employer is rejected
      resumeData.WorkHistories.Add(new ResumeWorkHistoryModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_EMPLOYER_IS_REQUIRED, 1)));

      //Verify that work history record with employer is accepted
      resumeData.WorkHistories[0].Employer = "Crap Employer";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_EMPLOYER_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_WorkHistory_JobTitleIsValidated()
    {
      //Verify that work history record without job title is rejected
      resumeData.WorkHistories.Add(new ResumeWorkHistoryModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_JOB_TITLE_IS_REQUIRED, 1)));

      //Verify that work history record with job title is accepted
      resumeData.WorkHistories[0].JobTitle = "Crap Job Title";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_JOB_TITLE_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_WorkHistory_JobDescriptionIsValidated()
    {
      //Verify that work history record without job description is rejected
      resumeData.WorkHistories.Add(new ResumeWorkHistoryModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_JOB_DESCRIPTION_IS_REQUIRED, 1)));

      //Verify that work history record with employer is accepted
      resumeData.WorkHistories[0].JobDescription = "Crap Job Description";
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_JOB_DESCRIPTION_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ResumeService_Upsert_WorkHistory_StartedIsValidated()
    {
      //Verify that work history record without started is rejected
      resumeData.WorkHistories.Add(new ResumeWorkHistoryModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_STARTED_IS_REQUIRED, 1)));

      //Verify that work history record with employer is accepted
      resumeData.WorkHistories[0].Started = DateTime.UtcNow.Date;
      Assert.False(TestHelpers.CallProducedError(() =>
          resumeService.UpsertResume(resumeData).Result
        , string.Format(ResumeService.WORK_ERROR_STARTED_IS_REQUIRED, 1)));
    }
  }
}
