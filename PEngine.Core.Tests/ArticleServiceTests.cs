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
  public class ArticleServiceTests
  {
    private ArticleModel articleData;
    private Mock<IArticleDal> mockedArticleDal;
    private Guid validGuid = Guid.NewGuid();
    private Guid invalidGuid = Guid.NewGuid();
    private Guid visibleGuid = Guid.NewGuid();
    private Guid hiddenGuid = Guid.NewGuid();
    private ArticleService articleService;
    
    public ArticleServiceTests()
    {
      //Setup
      articleData = new ArticleModel();
      mockedArticleDal = new Mock<IArticleDal>();
      mockedArticleDal.Setup(ad => ad.InsertArticle(articleData, false)).Returns(Task.Delay(0));
      mockedArticleDal.Setup(ad => ad.UpdateArticle(articleData)).Returns(Task.Delay(0));
      mockedArticleDal.Setup(ad => ad.GetArticleById(invalidGuid, null, null)).ReturnsAsync((ArticleModel)null);
      mockedArticleDal.Setup(ad => ad.GetArticleById(validGuid, null, null)).ReturnsAsync(new ArticleModel());
      mockedArticleDal.Setup(ad => ad.GetArticleById(visibleGuid, null, null)).ReturnsAsync(new ArticleModel() { VisibleFlag = true });
      mockedArticleDal.Setup(ad => ad.GetArticleById(hiddenGuid, null, null)).ReturnsAsync(new ArticleModel() { VisibleFlag = false });
      mockedArticleDal.Setup(ad => ad.ListArticles(null)).ReturnsAsync(new List<ArticleModel> {
        new ArticleModel() { VisibleFlag = true },
        new ArticleModel() { VisibleFlag = false }
      });
      articleService = new ArticleService(mockedArticleDal.Object);
    }

    [Fact]
    public void ArticleService_List_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users only get visible records
      Assert.Equal(1, articleService.ListArticles(null, false).Result.Count());

      //Verify that dmin users get all the records
      Assert.Equal(2, articleService.ListArticles(null, true).Result.Count());
    }

    [Fact]
    public void ArticleService_Get_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users can get visible records
      Assert.NotNull(articleService.GetArticleById(visibleGuid, null, null, false).Result);

      //Verify that non-admin users cannot get hidden records
      Assert.Null(articleService.GetArticleById(hiddenGuid, null, null, false).Result);

      //Verify that admin users can get visible records
      Assert.NotNull(articleService.GetArticleById(visibleGuid, null, null, true).Result);

      //Verify that admin users can get hidden records
      Assert.NotNull(articleService.GetArticleById(hiddenGuid, null, null, true).Result);
    }

    [Fact]
    public void ArticleService_Upsert_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(null).Result
        , ArticleService.ARTICLE_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(TestHelpers.CallProducedError(() => 
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ArticleService_Upsert_GuidIsValidated()
    {
      //Verify that record with invalid guid is rejected
      articleData.Guid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() => 
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_INVALID_RECORD));

      //Verify that record with valid Guid is accepted
      articleData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() => 
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_INVALID_RECORD));

      //Verify that record with no Guid is accepted
      articleData.Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() => 
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_INVALID_RECORD));
    }

    [Fact]
    public void ArticleService_Upsert_NameIsValidated()
    { 
      //Verify that record with no name/title is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_TITLE_IS_REQUIRED));

      //Verify that record with name/title is accepted
      articleData.Name = "Crap Title";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_TITLE_IS_REQUIRED));
    }

    [Fact]
    public void ArticleService_Upsert_DescriptionIsValidated()
    { 
      //Verify that record without description is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DESCRIPTION_IS_REQUIRED));

      //Verify that record with description is accepted
      articleData.Description = "Crap Description";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DESCRIPTION_IS_REQUIRED));
    }

    [Fact]
    public void ArticleService_Upsert_CategoryIsValidated()
    { 
      //Verify that record without category is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_CATEGORY_IS_REQUIRED));

      //Verify that record with category is accepted
      articleData.Category = "Crap Description";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_CATEGORY_IS_REQUIRED));
    }

    [Fact]
    public void ArticleService_Upsert_DefaultSectionIsValidated()
    { 
      //Verify that record with no sections but default section not set is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DEFAULT_SECTION_INVALID));

      //Verify that record with no sections but default section set is rejected
      articleData.DefaultSection = "Section B";
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DEFAULT_SECTION_INVALID));

      //Verify that record with some sections but default section set incorrectly is rejected
      articleData.Sections.Add(new ArticleSectionModel() {
        Name = "Section A"
      });
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DEFAULT_SECTION_INVALID));

      //Verify that record with some sections and default section set correctly is accepted
      articleData.Sections.Add(new ArticleSectionModel() {
        Name = "Section B"
      });
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_DEFAULT_SECTION_INVALID));
    }

    [Fact]
    public void ArticleService_Upsert_SectionNameIsValidated()
    { 
      //Verify that record with section lacking name is rejected
      articleData.Sections.Add(new ArticleSectionModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , string.Format(ArticleService.SECTION_ERROR_NAME_IS_REQUIRED, 1)));

      //Verify that record with section with name is accepted
      articleData.Sections[0].Name = "Crap Name";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , string.Format(ArticleService.SECTION_ERROR_NAME_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ArticleService_Upsert_SectionContentIsValidated()
    { 
      //Verify that record with section lacking description is rejected
      articleData.Sections.Add(new ArticleSectionModel());
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , string.Format(ArticleService.SECTION_ERROR_CONTENT_IS_REQUIRED, 1)));

      //Verify that record with section with description is accepted
      articleData.Sections[0].Data = "Crap Content";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , string.Format(ArticleService.SECTION_ERROR_CONTENT_IS_REQUIRED, 1)));
    }

    [Fact]
    public void ArticleService_Upsert_URLOrSectionIsValidated()
    { 
      //Verify that record without either section or content URL is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED));

      //Verify that record with content URL is accepted
      articleData.ContentURL = "http://blah.com";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED));

      //Verify that record with section is accepted
      articleData.ContentURL = null;
      articleData.Sections.Add(new ArticleSectionModel());
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED));

      //Verify that record with section and content URL is accepted
      articleData.ContentURL = "http://blah2.com";
      Assert.False(TestHelpers.CallProducedError(() =>
          articleService.UpsertArticle(articleData).Result
        , ArticleService.ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED));
    }
  }
}
