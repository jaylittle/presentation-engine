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
        XDocument articleDoc = XDocument.Parse(System.IO.File.ReadAllText(articlePath));
        var articleRecords = articleDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
        var newArticleRecords = new Dictionary<Guid, ArticleModel>();
        if (articleRecords.Any())
        {
          newArticleRecords = articleRecords.Select(articleRecord => {
            return new
            {
              OldGuid = Guid.Parse(articleRecord.GetChildElementValue("Guid")),
              Data = new ArticleModel()
              {
                Guid = Guid.Empty,
                LegacyID = ParseNInt(articleRecord.GetChildElementValue("LegacyID")),
                Name = articleRecord.GetChildElementValue("Name"),
                Description = articleRecord.GetChildElementValue("Description"),
                Category = articleRecord.GetChildElementValue("Category"),
                ContentURL = articleRecord.GetChildElementValue("ContentURL"),
                DefaultSection = articleRecord.GetChildElementValue("DefaultSection"),
                VisibleFlag = bool.Parse(articleRecord.GetChildElementValue("VisibleFlag")),
                UniqueName = articleRecord.GetChildElementValue("UniqueName"),
                HideButtonsFlag = bool.Parse(articleRecord.GetChildElementValue("HideButtonsFlag")),
                HideDropDownFlag = bool.Parse(articleRecord.GetChildElementValue("HideDropDownFlag"))
              }
            };
          }).ToDictionary(a => a.OldGuid, a => a.Data);

          XDocument sectionDoc = XDocument.Parse(System.IO.File.ReadAllText(articleSectionPath));
          var sectionRecords = sectionDoc.Descendants().Where(d => d.Name.LocalName.Equals("data"));
          foreach (var sectionRecord in sectionRecords)
          {
            var articleGuid = Guid.Parse(sectionRecord.GetChildElementValue("ArticleGuid"));
            var sectionGuid = Guid.Parse(sectionRecord.GetChildElementValue("Guid"));
            var newSection = new ArticleSectionModel()
            {
              Guid = Guid.Empty,
              ArticleGuid = Guid.Empty,
              Name = sectionRecord.GetChildElementValue("Name"),
              Data = sectionRecord.GetChildElementValue("Data"),
              SortOrder = int.Parse(sectionRecord.GetChildElementValue("SortOrder")),
              UniqueName = sectionRecord.GetChildElementValue("UniqueName")
            };
            if (newArticleRecords.ContainsKey(articleGuid))
            {
              newArticleRecords[articleGuid].Sections.Add(newSection);
            }
            else
            {
              messages.Add($"Article Section {sectionGuid} refers to a non-existent article: {articleGuid}");
              retvalue = false;
            }
          }
        }
        if (retvalue)
        {
          var articleService = _serviceProvider.GetRequiredService<IArticleService>();
          var succeededCounter = 0;
          var failedCounter = 0;
          foreach (var newArticleRecord in newArticleRecords)
          {
            newArticleRecord.Value.Guid = Guid.Empty;
            var errors = new List<string>();
            var myResult = articleService.UpsertArticle(newArticleRecord.Value, ref errors);
            retvalue = retvalue && myResult;
            if (myResult)
            {
              messages.Add($"Article Upsert Succeeded for {newArticleRecord.Value.Guid}");
              succeededCounter++;
            }
            else
            {
              messages.Add($"Article Upsert Failed for {newArticleRecord.Value.Guid}");
              failedCounter++;
            }
            foreach (var error in errors)
            {
              messages.Add($"Article Upsert Error for {newArticleRecord.Value.Guid}: {error}");
            }
          }
          messages.Add($"Article Import completed with {succeededCounter} successes, {failedCounter} failures.");
        }
      }
      else
      {
        messages.Add("Couldn't find both Article and Article Section exports. Skipping.");
      }
      return retvalue;
    }

    private int? ParseNInt(string val)
    {
      int i;
      return int.TryParse (val, out i) ? (int?) i : null;
    }

    private DateTime? ParseNDateTime(string val)
    {
      DateTime i;
      return DateTime.TryParse (val, out i) ? (DateTime?) i : null;
    }
  }
}