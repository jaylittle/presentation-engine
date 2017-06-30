using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
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

    public async Task<OpResult> ImportData(string contentRootPath)
    {
      var retvalue = new OpResult(true);
      string importFolder = System.IO.Path.Combine(contentRootPath, $"data{System.IO.Path.DirectorySeparatorChar}import");
      if (!System.IO.Directory.Exists(importFolder))
      {
        retvalue.LogError($"Expected import diretory does not exist: {importFolder}");
      }

      var articlePath = System.IO.Path.Combine(importFolder, "Article.xml");
      var articleSectionPath = System.IO.Path.Combine(importFolder, "ArticleSection.xml");
      if (System.IO.File.Exists(articlePath) && (System.IO.File.Exists(articleSectionPath)))
      {
        retvalue.LogInfo("Found both Article and Article Section exports. Processing...");
        retvalue.Inhale(await ImportArticles(articlePath, articleSectionPath));
      }
      else
      {
        retvalue.LogInfo("Couldn't find both Article and Article Section exports. Skipping.");
      }

      var postPath = System.IO.Path.Combine(importFolder, "Post.xml");
      if (System.IO.File.Exists(articlePath) && (System.IO.File.Exists(articleSectionPath)))
      {
        retvalue.LogInfo("Found Post exports. Processing...");
        retvalue.Inhale(await ImportPosts(postPath));
      }
      else
      {
        retvalue.LogInfo("Couldn't find Post exports. Skipping.");
      }

      var resumePersonalPath = System.IO.Path.Combine(importFolder, "ResumePersonal.xml");
      var resumeObjectivePath = System.IO.Path.Combine(importFolder, "ResumeObjective.xml");
      var resumeSkillPath = System.IO.Path.Combine(importFolder, "ResumeSkill.xml");
      var resumeEducationPath = System.IO.Path.Combine(importFolder, "ResumeEducation.xml");
      var resumeWorkHistoryPath = System.IO.Path.Combine(importFolder, "ResumeWorkHistory.xml");
      if (System.IO.File.Exists(resumePersonalPath) && System.IO.File.Exists(resumeObjectivePath))
      {
        retvalue.LogInfo("Found both Resume Personal and Objective exports. Processing...");
        retvalue.Inhale(await ImportResume(resumePersonalPath, resumeObjectivePath
          , resumeSkillPath, resumeEducationPath, resumeWorkHistoryPath));
      }
      else
      {
        retvalue.LogInfo("Couldn't find both Resume Personal and Objective exports. Skipping.");
      }

      var forumPath = System.IO.Path.Combine(importFolder, "Forum.xml");
      var forumUserPath = System.IO.Path.Combine(importFolder, "ForumUser.xml");
      var forumThreadPath = System.IO.Path.Combine(importFolder, "ForumThread.xml");
      var forumThreadPostPath = System.IO.Path.Combine(importFolder, "ForumThreadPost.xml");
      if (System.IO.File.Exists(forumPath))
      {
        retvalue.LogInfo("Found Forum export. Processing...");
        retvalue.Inhale(await ImportForums(forumPath, forumUserPath, forumThreadPath, forumThreadPostPath));
      }
      else
      {
        retvalue.LogInfo("Couldn't find Forum export. Skipping.");
      }

      return retvalue;
    }

    private async Task<OpResult> ImportArticles(string articlePath, string articleSectionPath)
    {
      var retvalue = new OpResult();
      XDocument articleDoc = XDocument.Parse(System.IO.File.ReadAllText(articlePath));
      var articleRecords = articleDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      var newArticleRecords = new Dictionary<Guid, ArticleModel>();
      if (articleRecords.Any())
      {
        newArticleRecords = articleRecords.Select(articleRecord =>
          new ArticleModel()
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
          })
          .ToDictionary(a => a.Guid, a => a);

        XDocument sectionDoc = XDocument.Parse(System.IO.File.ReadAllText(articleSectionPath));
        var sectionRecords = sectionDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        foreach (var sectionRecord in sectionRecords)
        {
          var newSection = new ArticleSectionModel()
          {
            Guid = Guid.Parse(sectionRecord.GetChildElementValue("Guid")),
            ArticleGuid = Guid.Parse(sectionRecord.GetChildElementValue("ArticleGuid")),
            Name = sectionRecord.GetChildElementValue("Name"),
            Data = ConvertDataToMarkDown(sectionRecord.GetChildElementValue("Data"), false),
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
            retvalue.LogError($"Article Section {newSection.Guid} refers to a non-existent article: {newSection.ArticleGuid}");
          }
        }
      }
      if (retvalue.Successful)
      {
        var dal = _serviceProvider.GetRequiredService<IArticleDal>();
        await dal.DeleteAllArticles();

        var service = _serviceProvider.GetRequiredService<IArticleService>();
        retvalue.Inhale(await ProcessUpserts<ArticleModel,IArticleService>(service, newArticleRecords, (s, m) =>
          s.UpsertArticle(m, true).Result
        ));
      }
      return retvalue;
    }

    private async Task<OpResult> ImportPosts(string postPath)
    {
      var retvalue = new OpResult();
      XDocument postDoc = XDocument.Parse(System.IO.File.ReadAllText(postPath));
      var postRecords = postDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      var newPostRecords = new Dictionary<Guid, PostModel>();
      if (postRecords.Any())
      {
        newPostRecords = postRecords.Select(postRecord =>
          new PostModel()
          {
            Guid = Guid.Parse(postRecord.GetChildElementValue("Guid")),
            LegacyID = ParseNInt(postRecord.GetChildElementValue("LegacyID")),
            Name = postRecord.GetChildElementValue("Title"),
            Data = ConvertDataToMarkDown(postRecord.GetChildElementValue("Data"), false),
            IconFileName = postRecord.GetChildElementValue("IconFileName"),
            VisibleFlag = bool.Parse(postRecord.GetChildElementValue("VisibleFlag")),
            UniqueName = postRecord.GetChildElementValue("UniqueName"),
            CreatedUTC = ParseNDateTime(postRecord.GetChildElementValue("CreatedUTC")),
            ModifiedUTC = ParseNDateTime(postRecord.GetChildElementValue("ModifiedUTC"))
          })
          .ToDictionary(p => p.Guid, p => p);
      }
      if (retvalue.Successful)
      {
        var dal = _serviceProvider.GetRequiredService<IPostDal>();
        await dal.DeleteAllPosts();

        var service = _serviceProvider.GetRequiredService<IPostService>();
        retvalue.Inhale(await ProcessUpserts<PostModel,IPostService>(service, newPostRecords, (s, m) =>
          s.UpsertPost(m, true).Result
        ));
      }
      return retvalue;
    }

    private async Task<OpResult> ImportResume(string personalPath, string objectivePath, string skillPath, string educationPath, string workHistoryPath)
    {
      var retvalue = new OpResult();
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
      
      if (retvalue.Successful)
      {
        var dal = _serviceProvider.GetRequiredService<IResumeDal>();
        await dal.DeleteAllResumes();

        var service = _serviceProvider.GetRequiredService<IResumeService>();
        retvalue.Inhale(await ProcessUpserts<ResumeModel, IResumeService>(service
          , new Dictionary<Guid, ResumeModel> { { Guid.Empty, newResumeRecord } }, (s, m)=>
            s.UpsertResume(m, true).Result
        ));
      }
      return retvalue;
    }

    private async Task<OpResult> ImportForums(string forumPath, string userPath, string threadPath, string threadPostPath)
    {
      var retvalue = new OpResult();
      var service = _serviceProvider.GetRequiredService<IForumService>();
      
      XDocument forumDoc = XDocument.Parse(System.IO.File.ReadAllText(forumPath));
      var forumRecords = forumDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
      var newForumRecords = new Dictionary<Guid, ForumModel>();
      if (forumRecords.Any())
      {
        newForumRecords = forumRecords.Select(forumRecord =>
          new ForumModel()
          {
            Guid = Guid.Parse(forumRecord.GetChildElementValue("Guid")),
            Name = forumRecord.GetChildElementValue("Name"),
            Description = forumRecord.GetChildElementValue("Description"),
            VisibleFlag = bool.Parse(forumRecord.GetChildElementValue("VisibleFlag")),
            UniqueName = forumRecord.GetChildElementValue("UniqueName"),
            CreatedUTC = ParseNDateTime(forumRecord.GetChildElementValue("CreatedUTC")),
            ModifiedUTC = ParseNDateTime(forumRecord.GetChildElementValue("ModifiedUTC"))
          })
          .ToDictionary(p => p.Guid, p => p);
      }
      if (retvalue.Successful)
      {
        var dal = _serviceProvider.GetRequiredService<IForumDal>();
        await dal.DeleteAllForums();

        retvalue.Inhale(await ProcessUpserts<ForumModel,IForumService>(service, newForumRecords, (s, m) =>
          s.UpsertForum(m, true).Result
        ));
      }
      if (System.IO.File.Exists(userPath))
      {
        XDocument userDoc = XDocument.Parse(System.IO.File.ReadAllText(userPath));
        var userRecords = userDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        var newUserRecords = new Dictionary<Guid, ForumUserModel>();
        if (userRecords.Any())
        {
          newUserRecords = userRecords.Select(userRecord =>
            new ForumUserModel()
            {
              Guid = Guid.Parse(userRecord.GetChildElementValue("Guid")),
              UserId = userRecord.GetChildElementValue("UserID"),
              Password = userRecord.GetChildElementValue("Password"),
              AdminFlag = bool.Parse(userRecord.GetChildElementValue("AdminFlag")),
              BanFlag = bool.Parse(userRecord.GetChildElementValue("BanFlag")),
              Email = userRecord.GetChildElementValue("Email"),
              Website = userRecord.GetChildElementValue("Website"),
              Comment = userRecord.GetChildElementValue("Comment"),
              LastIPAddress = userRecord.GetChildElementValue("LastIPAddress"),
              LastLogon = ParseNDateTime(userRecord.GetChildElementValue("LastLogon")),
              CreatedUTC = ParseNDateTime(userRecord.GetChildElementValue("CreatedUTC")),
              ModifiedUTC = ParseNDateTime(userRecord.GetChildElementValue("ModifiedUTC"))
            })
            .ToDictionary(p => p.Guid, p => p);
        }
        if (retvalue.Successful)
        {
          retvalue .Inhale(await ProcessUpserts<ForumUserModel,IForumService>(service, newUserRecords, (s, m) =>
            s.UpsertForumUser(m, m.Guid, true, true).Result
          ));
        }
      }
      if (System.IO.File.Exists(threadPath))
      {
        XDocument threadDoc = XDocument.Parse(System.IO.File.ReadAllText(threadPath));
        var threadRecords = threadDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        var newThreadRecords = new Dictionary<Guid, ForumThreadModel>();
        if (threadRecords.Any())
        {
          newThreadRecords = threadRecords.Select(threadRecord =>
            new ForumThreadModel()
            {
              Guid = Guid.Parse(threadRecord.GetChildElementValue("Guid")),
              ForumGuid = Guid.Parse(threadRecord.GetChildElementValue("ForumGuid")),
              ForumUserGuid = Guid.Parse(threadRecord.GetChildElementValue("ForumUserGuid")),
              VisibleFlag = bool.Parse(threadRecord.GetChildElementValue("VisibleFlag")),
              LockFlag = bool.Parse(threadRecord.GetChildElementValue("LockFlag")),
              Name = threadRecord.GetChildElementValue("Title"),
              UniqueName = threadRecord.GetChildElementValue("UniqueName"),
              CreatedUTC = ParseNDateTime(threadRecord.GetChildElementValue("CreatedUTC")),
              ModifiedUTC = ParseNDateTime(threadRecord.GetChildElementValue("ModifiedUTC"))
            })
            .ToDictionary(p => p.Guid, p => p);
        }
        if (retvalue.Successful)
        {
          retvalue.Inhale(await ProcessUpserts<ForumThreadModel,IForumService>(service, newThreadRecords, (s, m) =>
            s.UpsertForumThread(m, m.Guid, true, true).Result
          ));
          if (System.IO.File.Exists(threadPostPath))
          {
            XDocument threadPostDoc = XDocument.Parse(System.IO.File.ReadAllText(threadPostPath));
            var threadPostRecords = threadPostDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
            var newThreadPostRecords = new Dictionary<Guid, ForumThreadPostModel>();
            if (threadPostRecords.Any())
            {
              newThreadPostRecords = threadPostRecords.Select(threadPostRecord =>
                new ForumThreadPostModel()
                {
                  Guid = Guid.Parse(threadPostRecord.GetChildElementValue("Guid")),
                  ForumThreadGuid = Guid.Parse(threadPostRecord.GetChildElementValue("ForumThreadGuid")),
                  ForumUserGuid = Guid.Parse(threadPostRecord.GetChildElementValue("ForumUserGuid")),
                  VisibleFlag = bool.Parse(threadPostRecord.GetChildElementValue("VisibleFlag")),
                  LockFlag = bool.Parse(threadPostRecord.GetChildElementValue("LockFlag")),
                  Data = ConvertDataToMarkDown(threadPostRecord.GetChildElementValue("Data"), true),
                  IPAddress = threadPostRecord.GetChildElementValue("IPAddress"),
                  CreatedUTC = ParseNDateTime(threadPostRecord.GetChildElementValue("CreatedUTC")),
                  ModifiedUTC = ParseNDateTime(threadPostRecord.GetChildElementValue("ModifiedUTC"))
                })
                .ToDictionary(p => p.Guid, p => p);
              
              if (retvalue.Successful)
              {
                retvalue.Inhale(await ProcessUpserts<ForumThreadPostModel,IForumService>(service, newThreadPostRecords, (s, m) => 
                  s.UpsertForumThreadPost(m, m.Guid, true, true).Result
                ));
              }
            }
          }
        }
      }
      return retvalue;
    }

    private async Task<OpResult> ProcessUpserts<TModel,TService>(TService service, Dictionary<Guid, TModel> records, Func<TService, TModel, OpResult> upsert)
    {
      return await Task.Run<OpResult>(() =>
      {
        var retvalue = new OpResult();
        var succeededCounter = 0;
        var failedCounter = 0;
        foreach (var record in records)
        {
          var myResult = upsert(service, record.Value);
          if (myResult.Successful)
          {
            retvalue.LogInfo($"{typeof(TModel).Name} Upsert Succeeded for {record.Key}");
            succeededCounter++;
          }
          else
          {
            retvalue.LogError($"{typeof(TModel).Name} Upsert Failed for {record.Key}");
            failedCounter++;
          }
          retvalue.Inhale(myResult);
        }
        retvalue.LogInfo($"{typeof(TModel).Name} Import completed with {succeededCounter} successes, {failedCounter} failures.");
        return retvalue;
      });
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

    public static string ConvertDataToMarkDown(string secdata, bool forum)
    {
      int lpos = 0;
      string tag = string.Empty;
      string tagname = string.Empty;
      string tagdata = string.Empty;
      int tagspace = 0;
      string[] tagelements = { };
      bool rawhtmlflag = false;
      int rawhtmlstart = 0;
      int rawhtmlend = 0;
      string outdata = string.Empty;
      string[] resforumtags = {"SCRIPT", "/SCRIPT", "IFRAME", "/IFRAME", "EMBED", "BLINK"
      , "TR", "TD", "TABLE", "/TR", "/TD", "/TABLE", "FRAMESET", "/FRAMESET"};
      bool restagflag = false;
      StringBuilder outputhtml = new StringBuilder();
      //Filter for obfusacated tags if forum flag is true
      //Remove HTML Content if Forum Flag is true
      if (forum)
      {
        while (secdata.IndexOf("[" + Environment.NewLine) >= 0)
        {
          secdata.Replace("[" + Environment.NewLine, "[ ");
        }
        while (secdata.IndexOf("<" + Environment.NewLine) >= 0)
        {
          secdata.Replace("<" + Environment.NewLine, "< ");
        }
        while (secdata.IndexOf("[ ") >= 0)
        {
          secdata.Replace("[ ", "[");
        }
        while (secdata.IndexOf("< ") >= 0)
        {
          secdata.Replace("< ", "<");
        }
        for (int cpos = secdata.IndexOf("<"); cpos >= 0; cpos = secdata.IndexOf("<", cpos + 1))
        {
          lpos = secdata.IndexOf(">", cpos + 1);
          if (lpos >= 0)
          {
            secdata = secdata.Substring(0, cpos) + secdata.Substring(lpos, secdata.Length - lpos);
          }
        }
      }
      lpos = -1;
      for (int cpos = secdata.IndexOf("["); cpos >= 0; cpos = secdata.IndexOf("[", lpos + 1))
      {
        if (!rawhtmlflag)
        {
          outdata = secdata.Substring(lpos + 1, cpos - (lpos + 1));
          outputhtml.Append(outdata);
        }
        lpos = secdata.IndexOf("]", cpos + 1);
        if (lpos > cpos)
        {
          tag = secdata.Substring(cpos + 1, lpos - (cpos + 1));
          tagspace = tag.IndexOf(" ");
          if (tagspace >= 0)
          {
            tagname = tag.Substring(0, tagspace).ToUpper();
            tagdata = tag.Substring(tagspace + 1, tag.Length - (tagspace + 1));
          }
          else
          {
            tagname = tag.ToUpper();
          }
          if ((!rawhtmlflag) || (tagname == "/RAWHTML"))
          {
            switch (tagname)
            {
              case "CENTER":
                outputhtml.Append("\n::::centered-text\n");
                break;
              case "/CENTER":
                outputhtml.Append("\n::::\n");
                break;
              case "IMAGE":
                if ((tagdata.ToUpper().IndexOf("HTTP") >= 0)
                  || (tagdata.Substring(0, 2) == "./") || (tagdata.Substring(0, 1) == "/"))
                {
                  outputhtml.Append($"\n![Image]({tagdata}){{.outside-image}}\n");
                }
                else
                {
                  outputhtml.Append($"\n![Image](/images/articles/{tagdata}){{.article-image}}\n");
                }
                break;
              case "SUBHEADER":
                if (!forum)
                {
                  outputhtml.Append($"\n::::sub-header\n");
                  outputhtml.Append($"{tagdata}\n");
                  outputhtml.Append($"::::\n");
                }
                break;
              case "LINK":
                var linkData = tagdata.Split(' ');
                var url = linkData.First();
                outputhtml.Append($"\n[{string.Join(" ", linkData.Skip(1))}]({url})\n");
                break;
              case "ICON":
                outputhtml.Append($"\n![Image](images/icons/{tagdata}){{.post-icon}}\n");
                break;
              case "SYSTEMIMAGE":
                outputhtml.Append($"\n![Image](/images/system/{tagdata}){{.system-image}}\n");
                break;
              case "RAWHTML":
                rawhtmlflag = true;
                rawhtmlstart = cpos + 9;
                rawhtmlend = rawhtmlstart;
                break;
              case "/RAWHTML":
                rawhtmlflag = false;
                rawhtmlend = cpos;
                outputhtml.Append(secdata.Substring(rawhtmlstart, rawhtmlend - rawhtmlstart));
                break;
              case "BLOCKQUOTE":
              case "QUOTE":
                outputhtml.Append("\n::::quoted-text\n");
                break;
              case "/BLOCKQUOTE":
              case "/QUOTE":
                outputhtml.Append("\n::::\n");
                break;
              case "SECTION":
              case "/SECTION":
                break;
              default:
                restagflag = false;
                if (forum)
                {
                  for (int fptr = 0; fptr < resforumtags.Length; fptr++)
                  {
                    if (resforumtags[fptr].ToUpper() == tagname)
                    {
                      restagflag = true;
                    }
                  }
                }
                if (!restagflag)
                {
                  outputhtml.Append($"<{tag}>");
                }
                break;
            }
          }
        }
      }
      if (lpos >= -1)
      {
        outdata = secdata.Substring(lpos + 1, secdata.Length - (lpos + 1));
        if (!rawhtmlflag)
        {
          outputhtml.Append(outdata);
          outputhtml.Append("\n");
        }
        else
        {
          outputhtml.Append(outdata);
        }
      }
      if (outputhtml.Length <= 0)
      {
        outputhtml.Append("There was no data to convert.");
      }
      return outputhtml.ToString();
    }
  }
}