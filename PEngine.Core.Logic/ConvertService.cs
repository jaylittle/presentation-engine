using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Logic
{
  public class ConvertService : IConvertService
  { 
    private IServiceProvider _serviceProvider;
    public ConvertService(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public bool ImportData(string contentRootPath, ref List<string> messages)
    {
      bool retvalue = true;
      string importFolder = System.IO.Path.Combine(contentRootPath, $"data{System.IO.Path.DirectorySeparatorChar}import");
      if (!System.IO.Directory.Exists(importFolder))
      {
        messages.Add($"Expected import diretory does not exist: {importFolder}");
        retvalue = false;
      }

      var articlePath = System.IO.Path.Combine(importFolder, "Article.xml");
      var articleSectionPath = System.IO.Path.Combine(importFolder, "ArticleSection.xml");
      if (System.IO.File.Exists(articlePath) && (System.IO.File.Exists(articleSectionPath)))
      {
        messages.Add("Found both Article and Article Section exports. Processing...");
        ImportArticles(articlePath, articleSectionPath, ref retvalue, ref messages);
      }
      else
      {
        messages.Add("Couldn't find both Article and Article Section exports. Skipping.");
      }

      var postPath = System.IO.Path.Combine(importFolder, "Post.xml");
      if (System.IO.File.Exists(articlePath) && (System.IO.File.Exists(articleSectionPath)))
      {
        messages.Add("Found Post exports. Processing...");
        ImportPosts(postPath, ref retvalue, ref messages);
      }
      else
      {
        messages.Add("Couldn't find Post exports. Skipping.");
      }

      var resumePersonalPath = System.IO.Path.Combine(importFolder, "ResumePersonal.xml");
      var resumeObjectivePath = System.IO.Path.Combine(importFolder, "ResumeObjective.xml");
      var resumeSkillPath = System.IO.Path.Combine(importFolder, "ResumeSkill.xml");
      var resumeEducationPath = System.IO.Path.Combine(importFolder, "ResumeEducation.xml");
      var resumeWorkHistoryPath = System.IO.Path.Combine(importFolder, "ResumeWorkHistory.xml");
      if (System.IO.File.Exists(resumePersonalPath) && System.IO.File.Exists(resumeObjectivePath))
      {
        messages.Add("Found both Resume Personal and Objective exports. Processing...");
        ImportResume(resumePersonalPath, resumeObjectivePath, resumeSkillPath
          , resumeEducationPath, resumeWorkHistoryPath, ref retvalue, ref messages);
      }
      else
      {
        messages.Add("Couldn't find both Resume Personal and Objective exports. Skipping.");
      }
      return retvalue;
    }

    private void ImportArticles(string articlePath, string articleSectionPath, ref bool retvalue, ref List<string> messages)
    {
      XDocument articleDoc = XDocument.Parse(System.IO.File.ReadAllText(articlePath));
      var articleRecords = articleDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      var newArticleRecords = new Dictionary<Guid, ArticleModel>();
      if (articleRecords.Any())
      {
        newArticleRecords = articleRecords.Select(articleRecord => {
          return new ArticleModel()
          {
            Guid = Guid.Parse(articleRecord.GetChildElementValue("Guid")),
            LegacyID = ParseNInt(articleRecord.GetChildElementValue("LegacyID")),
            Name = articleRecord.GetChildElementValue("Name"),
            Description = articleRecord.GetChildElementValue("Description"),
            Category = articleRecord.GetChildElementValue("Category"),
            ContentURL = articleRecord.GetChildElementValue("ContentURL"),
            DefaultSection = articleRecord.GetChildElementValue("DefaultSection"),
            VisibleFlag = bool.Parse(articleRecord.GetChildElementValue("VisibleFlag")),
            UniqueName = articleRecord.GetChildElementValue("UniqueName"),
            HideButtonsFlag = bool.Parse(articleRecord.GetChildElementValue("HideButtonsFlag")),
            HideDropDownFlag = bool.Parse(articleRecord.GetChildElementValue("HideDropDownFlag")),
            CreatedUTC = ParseNDateTime(articleRecord.GetChildElementValue("CreatedUTC")),
            ModifiedUTC = ParseNDateTime(articleRecord.GetChildElementValue("ModifiedUTC"))
          };
        }).ToDictionary(a => a.Guid, a => a);

        XDocument sectionDoc = XDocument.Parse(System.IO.File.ReadAllText(articleSectionPath));
        var sectionRecords = sectionDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        foreach (var sectionRecord in sectionRecords)
        {
          var newSection = new ArticleSectionModel()
          {
            Guid = Guid.Parse(sectionRecord.GetChildElementValue("Guid")),
            ArticleGuid = Guid.Parse(sectionRecord.GetChildElementValue("ArticleGuid")),
            Name = sectionRecord.GetChildElementValue("Name"),
            Data = sectionRecord.GetChildElementValue("Data"),
            SortOrder = int.Parse(sectionRecord.GetChildElementValue("SortOrder")),
            UniqueName = sectionRecord.GetChildElementValue("UniqueName"),
            CreatedUTC = ParseNDateTime(sectionRecord.GetChildElementValue("CreatedUTC")),
            ModifiedUTC = ParseNDateTime(sectionRecord.GetChildElementValue("ModifiedUTC"))
          };
          if (newArticleRecords.ContainsKey(newSection.ArticleGuid))
          {
            newArticleRecords[newSection.ArticleGuid].Sections.Add(newSection);
          }
          else
          {
            messages.Add($"Article Section {newSection.Guid} refers to a non-existent article: {newSection.ArticleGuid}");
            retvalue = false;
          }
        }
      }
      if (retvalue)
      {
        var dal = _serviceProvider.GetRequiredService<IArticleDal>();
        dal.DeleteAllArticles();

        var service = _serviceProvider.GetRequiredService<IArticleService>();
        retvalue = retvalue && ProcessUpserts<ArticleModel,IArticleService>(service, newArticleRecords, ref messages, (s, m) => {
          var errors = new List<string>();
          s.UpsertArticle(m, ref errors, true);
          return errors;
        });
      }
    }

    private void ImportPosts(string postPath, ref bool retvalue, ref List<string> messages)
    {
      XDocument postDoc = XDocument.Parse(System.IO.File.ReadAllText(postPath));
      var postRecords = postDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      var newPostRecords = new Dictionary<Guid, PostModel>();
      if (postRecords.Any())
      {
        newPostRecords = postRecords.Select(postRecord => {
          return new PostModel()
          {
            Guid = Guid.Parse(postRecord.GetChildElementValue("Guid")),
            LegacyID = ParseNInt(postRecord.GetChildElementValue("LegacyID")),
            Name = postRecord.GetChildElementValue("Title"),
            Data = postRecord.GetChildElementValue("Data"),
            IconFileName = postRecord.GetChildElementValue("IconFileName"),
            VisibleFlag = bool.Parse(postRecord.GetChildElementValue("VisibleFlag")),
            UniqueName = postRecord.GetChildElementValue("UniqueName"),
            CreatedUTC = ParseNDateTime(postRecord.GetChildElementValue("CreatedUTC")),
            ModifiedUTC = ParseNDateTime(postRecord.GetChildElementValue("ModifiedUTC"))
          };
        }).ToDictionary(p => p.Guid, p => p);
      }
      if (retvalue)
      {
        var dal = _serviceProvider.GetRequiredService<IPostDal>();
        dal.DeleteAllPosts();

        var service = _serviceProvider.GetRequiredService<IPostService>();
        retvalue = retvalue && ProcessUpserts<PostModel,IPostService>(service, newPostRecords, ref messages, (s, m) => {
          var errors = new List<string>();
          s.UpsertPost(m, ref errors, true);
          return errors;
        });
      }
    }

    private void ImportResume(string personalPath, string objectivePath, string skillPath, string educationPath, string workHistoryPath, ref bool retvalue, ref List<string> messages)
    {
      var newResumeRecord = new ResumeModel();
      
      XDocument personalDoc = XDocument.Parse(System.IO.File.ReadAllText(personalPath));
      var personalRecords = personalDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      newResumeRecord.Personals = personalRecords.Select(personalRecord => new ResumePersonalModel(){
        Guid = Guid.Parse(personalRecord.GetChildElementValue("Guid")),
        LegacyID = ParseNInt(personalRecord.GetChildElementValue("LegacyID")),
        FullName = personalRecord.GetChildElementValue("FullName"),
        Address1 = personalRecord.GetChildElementValue("Address1"),
        Address2 = personalRecord.GetChildElementValue("Address2"),
        City = personalRecord.GetChildElementValue("City"),
        State = personalRecord.GetChildElementValue("State"),
        Zip = personalRecord.GetChildElementValue("Zip"),
        Phone = personalRecord.GetChildElementValue("Phone"),
        Fax = personalRecord.GetChildElementValue("Fax"),
        Email = personalRecord.GetChildElementValue("Email"),
        WebsiteURL = personalRecord.GetChildElementValue("WebsiteURL"),
        CreatedUTC = ParseNDateTime(personalRecord.GetChildElementValue("CreatedUTC")),
        ModifiedUTC = ParseNDateTime(personalRecord.GetChildElementValue("ModifiedUTC"))
      }).ToList();

      XDocument objectiveDoc = XDocument.Parse(System.IO.File.ReadAllText(objectivePath));
      var objectiveRecords = objectiveDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      newResumeRecord.Objectives = objectiveRecords.Select(objectiveRecord => new ResumeObjectiveModel(){
        Guid = Guid.Parse(objectiveRecord.GetChildElementValue("Guid")),
        LegacyID = ParseNInt(objectiveRecord.GetChildElementValue("LegacyID")),
        Data = objectiveRecord.GetChildElementValue("Data"),
        CreatedUTC = ParseNDateTime(objectiveRecord.GetChildElementValue("CreatedUTC")),
        ModifiedUTC = ParseNDateTime(objectiveRecord.GetChildElementValue("ModifiedUTC"))
      }).ToList();

      if (System.IO.File.Exists(skillPath))
      {
        XDocument skillDoc = XDocument.Parse(System.IO.File.ReadAllText(skillPath));
        var skillRecords = skillDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        newResumeRecord.Skills = skillRecords.Select(skillRecord => new ResumeSkillModel(){
          Guid = Guid.Parse(skillRecord.GetChildElementValue("Guid")),
          LegacyID = ParseNInt(skillRecord.GetChildElementValue("LegacyID")),
          Type = skillRecord.GetChildElementValue("Type"),
          Name = skillRecord.GetChildElementValue("Name"),
          Hint = skillRecord.GetChildElementValue("Hint"),
          CreatedUTC = ParseNDateTime(skillRecord.GetChildElementValue("CreatedUTC")),
          ModifiedUTC = ParseNDateTime(skillRecord.GetChildElementValue("ModifiedUTC"))
        }).GroupBy(r => r.Type, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
      }

      if (System.IO.File.Exists(educationPath))
      {
        XDocument educationDoc = XDocument.Parse(System.IO.File.ReadAllText(educationPath));
        var educationRecords = educationDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        newResumeRecord.Educations = educationRecords.Select(educationRecord => new ResumeEducationModel(){
          Guid = Guid.Parse(educationRecord.GetChildElementValue("Guid")),
          LegacyID = ParseNInt(educationRecord.GetChildElementValue("LegacyID")),
          Institute = educationRecord.GetChildElementValue("Institute"),
          InstituteURL = educationRecord.GetChildElementValue("InstituteURL"),
          Program = educationRecord.GetChildElementValue("Program"),
          Started = ParseNDateTime(educationRecord.GetChildElementValue("Started")),
          Completed = ParseNDateTime(educationRecord.GetChildElementValue("Completed")),
          CreatedUTC = ParseNDateTime(educationRecord.GetChildElementValue("CreatedUTC")),
          ModifiedUTC = ParseNDateTime(educationRecord.GetChildElementValue("ModifiedUTC"))
        }).ToList();
      }

      if (System.IO.File.Exists(workHistoryPath))
      {
        XDocument workHistoryDoc = XDocument.Parse(System.IO.File.ReadAllText(workHistoryPath));
        var workHistoryRecords = workHistoryDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        newResumeRecord.WorkHistories = workHistoryRecords.Select(workHistoryRecord => new ResumeWorkHistoryModel(){
          Guid = Guid.Parse(workHistoryRecord.GetChildElementValue("Guid")),
          LegacyID = ParseNInt(workHistoryRecord.GetChildElementValue("LegacyID")),
          Employer = workHistoryRecord.GetChildElementValue("Employer"),
          EmployerURL = workHistoryRecord.GetChildElementValue("EmployerURL"),
          JobTitle = workHistoryRecord.GetChildElementValue("JobTitle"),
          JobDescription = workHistoryRecord.GetChildElementValue("JobDescription"),
          Started = ParseNDateTime(workHistoryRecord.GetChildElementValue("Started")),
          Completed = ParseNDateTime(workHistoryRecord.GetChildElementValue("Completed")),
          CreatedUTC = ParseNDateTime(workHistoryRecord.GetChildElementValue("CreatedUTC")),
          ModifiedUTC = ParseNDateTime(workHistoryRecord.GetChildElementValue("ModifiedUTC"))
        }).ToList();
      }
      
      if (retvalue)
      {
        var dal = _serviceProvider.GetRequiredService<IResumeDal>();
        dal.DeleteAllResumes();

        var service = _serviceProvider.GetRequiredService<IResumeService>();
        retvalue = retvalue && ProcessUpserts<ResumeModel, IResumeService>(service
          , new Dictionary<Guid, ResumeModel> { { Guid.Empty, newResumeRecord } }, ref messages, (s, m)=> {
            var errors = new List<string>();
            s.UpsertResume(m, ref errors, true);
            return errors;
          }
        );
      }
    }

    private bool ProcessUpserts<TModel,TService>(TService service, Dictionary<Guid, TModel> records, ref List<string> messages, Func<TService, TModel, List<string>> upsert)
    {
      bool retvalue = true;
      var succeededCounter = 0;
      var failedCounter = 0;
      foreach (var record in records)
      {
        var errors = upsert(service, record.Value);
        var myResult = !errors.Any();
        retvalue = retvalue && myResult;
        if (myResult)
        {
          messages.Add($"{typeof(TModel).Name} Upsert Succeeded for {record.Key}");
          succeededCounter++;
        }
        else
        {
          messages.Add($"{typeof(TModel).Name} Upsert Failed for {record.Key}");
          failedCounter++;
        }
        foreach (var error in errors)
        {
          messages.Add($"{typeof(TModel).Name} Upsert Error for {record.Key}: {error}");
        }
      }
      messages.Add($"{typeof(TModel).Name} Import completed with {succeededCounter} successes, {failedCounter} failures.");
      return retvalue;
    }

    private long? ParseNInt(string val)
    {
      long i;
      return long.TryParse(val, out i) ? (long?) i : (long?)null;
    }

    private DateTime? ParseNDateTime(string val)
    {
      DateTime i;
      return DateTime.TryParse (val, out i) ? (DateTime?) i : null;
    }
  }
}