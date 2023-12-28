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
  public class PostServiceTests
  {
    private PostModel postData;
    private Mock<IPostDal> mockedPostDal;
    private PostService postService;
    private Guid validGuid = Guid.NewGuid();
    private Guid invalidGuid = Guid.NewGuid();
    private Guid visibleGuid = Guid.NewGuid();
    private Guid hiddenGuid = Guid.NewGuid();
    
    public PostServiceTests()
    {
      //Setup
      postData = new PostModel();
      mockedPostDal = new Mock<IPostDal>();
      mockedPostDal.Setup(pd => pd.InsertPost(postData, false)).Returns(Task.Delay(0));
      mockedPostDal.Setup(pd => pd.UpdatePost(postData)).Returns(Task.Delay(0));
      mockedPostDal.Setup(pd => pd.GetPostById(invalidGuid, null, null)).ReturnsAsync((PostModel)null);
      mockedPostDal.Setup(pd => pd.GetPostById(validGuid, null, null)).ReturnsAsync(new PostModel());
      mockedPostDal.Setup(ad => ad.GetPostById(visibleGuid, null, null)).ReturnsAsync(new PostModel() { VisibleFlag = true });
      mockedPostDal.Setup(ad => ad.GetPostById(hiddenGuid, null, null)).ReturnsAsync(new PostModel() { VisibleFlag = false });
      mockedPostDal.Setup(ad => ad.ListPosts()).ReturnsAsync(new List<PostModel> {
        new PostModel() { VisibleFlag = true },
        new PostModel() { VisibleFlag = false }
      });
      postService = new PostService(mockedPostDal.Object);
    }

    [Fact]
    public async Task PostService_List_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users only get visible records
      Assert.Single(await postService.ListPosts(false, false));

      //Verify that dmin users get all the records
      Assert.True((await postService.ListPosts(true, false)).Count() == 2);
    }

    [Fact]
    public async Task PostService_Get_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users can get visible records
      Assert.NotNull(await postService.GetPostById(visibleGuid, null, null, false, false));

      //Verify that non-admin users cannot get hidden records
      Assert.Null(await postService.GetPostById(hiddenGuid, null, null, false, false));

      //Verify that admin users can get visible records
      Assert.NotNull(await postService.GetPostById(visibleGuid, null, null, true, false));

      //Verify that admin users can get hidden records
      Assert.NotNull(await postService.GetPostById(hiddenGuid, null, null, true, false));
    }

    [Fact]
    public async Task PostService_Upsert_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(await TestHelpers.CallProducedError(async() => 
          await postService.UpsertPost(null)
        , PostService.POST_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(await TestHelpers.CallProducedError(async() => 
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public async Task PostService_Upsert_GuidIsValidated()
    {
      //Verify that record with invalid guid is rejected
      postData.Guid = invalidGuid;
      Assert.True(await TestHelpers.CallProducedError(async() => 
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_INVALID_RECORD));

      //Verify that record with valid Guid is accepted
      postData.Guid = validGuid;
      Assert.False(await TestHelpers.CallProducedError(async() => 
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_INVALID_RECORD));

      //Verify that record with no Guid is accepted
      postData.Guid = Guid.Empty;
      Assert.False(await TestHelpers.CallProducedError(async() => 
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_INVALID_RECORD));
    }

    [Fact]
    public async Task PostService_Upsert_NameIsValidated()
    {
      //Verify that record with no name/title is rejected
      Assert.True(await TestHelpers.CallProducedError(async () =>
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_TITLE_IS_REQUIRED));

      //Verify that record with name/title is accepted
      postData.Name = "Crap Title";
      Assert.False(await TestHelpers.CallProducedError(async () =>
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_TITLE_IS_REQUIRED));
    }

    [Fact]
    public async Task PostService_Upsert_DataIsValidated()
    {      
      //Verify that record without data/content is rejected
      Assert.True(await TestHelpers.CallProducedError(async() =>
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_CONTENT_IS_REQUIRED));

      //Verify that record with data/content is accepted
      postData.Data = "Crap Content";
      Assert.False(await TestHelpers.CallProducedError(async() =>
          await postService.UpsertPost(postData)
        , PostService.POST_ERROR_CONTENT_IS_REQUIRED));
    }
  }
}
