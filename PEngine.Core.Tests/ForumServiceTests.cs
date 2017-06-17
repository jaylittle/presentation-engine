using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using Xunit;
using Moq;
using Newtonsoft.Json;

namespace PEngine.Core.Tests
{
  public class ForumServiceTests
  {
    private ForumModel forumData;
    private ForumThreadModel forumThreadData;
    private ForumThreadPostModel forumThreadPostData;
    private ForumUserModel forumUserData;
    private Mock<IForumDal> mockedForumDal;
    private Mock<ISettingsProvider> mockedSettingsProvider;
    private ForumService forumService;
    private Guid validGuid = Guid.NewGuid();
    private Guid semiValidGuid1 = Guid.NewGuid();
    private Guid semiValidGuid2 = Guid.NewGuid();
    private Guid invalidGuid = Guid.NewGuid();
    private Guid visibleGuid = Guid.NewGuid();
    private Guid hiddenGuid = Guid.NewGuid();
    private SettingsData settingsData;
    
    public ForumServiceTests()
    {
      //Setup
      forumData = new ForumModel();
      forumThreadData = new ForumThreadModel();
      forumThreadPostData = new ForumThreadPostModel();
      forumUserData = new ForumUserModel();

      settingsData = new SettingsData();
      mockedSettingsProvider = new Mock<ISettingsProvider>();
      mockedSettingsProvider.SetupGet<SettingsData>(sd => sd.Current).Returns(settingsData);

      mockedForumDal = new Mock<IForumDal>();
      mockedForumDal.Setup(fd => fd.InsertForum(forumData, false)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.UpdateForum(forumData)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.GetForumById(invalidGuid, null)).ReturnsAsync((ForumModel)null);
      mockedForumDal.Setup(fd => fd.GetForumById(validGuid, null)).ReturnsAsync(new ForumModel());
      mockedForumDal.Setup(ad => ad.GetForumById(visibleGuid, null)).ReturnsAsync(new ForumModel() { VisibleFlag = true });
      mockedForumDal.Setup(ad => ad.GetForumById(hiddenGuid, null)).ReturnsAsync(new ForumModel() { VisibleFlag = false });
      mockedForumDal.Setup(ad => ad.ListForums()).ReturnsAsync(new List<ForumModel> {
        new ForumModel() { VisibleFlag = true },
        new ForumModel() { VisibleFlag = false }
      });
      mockedForumDal.Setup(fd => fd.InsertForumThread(forumThreadData, false)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.UpdateForumThread(forumThreadData)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.GetForumThreadById(invalidGuid, null)).ReturnsAsync((ForumThreadModel)null);
      mockedForumDal.Setup(fd => fd.GetForumThreadById(validGuid, null)).ReturnsAsync(new ForumThreadModel() { ForumUserGuid = validGuid, VisibleFlag = true, LockFlag = false, CreatedUTC = DateTime.UtcNow.AddMinutes(-1) });
      mockedForumDal.Setup(fd => fd.GetForumThreadById(semiValidGuid1, null)).ReturnsAsync(new ForumThreadModel() { ForumUserGuid = validGuid, VisibleFlag = false, LockFlag = false, CreatedUTC = DateTime.UtcNow.AddMinutes((-1 * settingsData.TimeLimitForumPostEdit) - 1) });
      mockedForumDal.Setup(fd => fd.GetForumThreadById(semiValidGuid2, null)).ReturnsAsync(new ForumThreadModel() { ForumUserGuid = validGuid, VisibleFlag = true, LockFlag = true, CreatedUTC = DateTime.UtcNow.AddMinutes((-1 * settingsData.TimeLimitForumPostEdit) - 1) });
      mockedForumDal.Setup(ad => ad.GetForumThreadById(visibleGuid, null)).ReturnsAsync(new ForumThreadModel() { VisibleFlag = true });
      mockedForumDal.Setup(ad => ad.GetForumThreadById(hiddenGuid, null)).ReturnsAsync(new ForumThreadModel() { VisibleFlag = false });
      mockedForumDal.Setup(ad => ad.ListForumThreads(null, null)).ReturnsAsync(new List<ForumThreadModel> {
        new ForumThreadModel() { VisibleFlag = true },
        new ForumThreadModel() { VisibleFlag = false }
      });
      mockedForumDal.Setup(fd => fd.InsertForumThreadPost(forumThreadPostData, false)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.UpdateForumThreadPost(forumThreadPostData)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.GetForumThreadPostById(invalidGuid)).ReturnsAsync((ForumThreadPostModel)null);
      mockedForumDal.Setup(fd => fd.GetForumThreadPostById(validGuid)).ReturnsAsync(new ForumThreadPostModel() { ForumUserGuid = validGuid, VisibleFlag = true, LockFlag = false, CreatedUTC = DateTime.UtcNow.AddMinutes(-1) });
      mockedForumDal.Setup(fd => fd.GetForumThreadPostById(semiValidGuid1)).ReturnsAsync(new ForumThreadPostModel() { ForumUserGuid = validGuid, VisibleFlag = false, LockFlag = false, CreatedUTC = DateTime.UtcNow.AddMinutes((-1 * settingsData.TimeLimitForumPostEdit) - 1) });
      mockedForumDal.Setup(fd => fd.GetForumThreadPostById(semiValidGuid2)).ReturnsAsync(new ForumThreadPostModel() { ForumUserGuid = validGuid, VisibleFlag = true, LockFlag = true, CreatedUTC = DateTime.UtcNow.AddMinutes((-1 * settingsData.TimeLimitForumPostEdit) - 1) });
      mockedForumDal.Setup(ad => ad.GetForumThreadPostById(visibleGuid)).ReturnsAsync(new ForumThreadPostModel() { VisibleFlag = true });
      mockedForumDal.Setup(ad => ad.GetForumThreadPostById(hiddenGuid)).ReturnsAsync(new ForumThreadPostModel() { VisibleFlag = false });
      mockedForumDal.Setup(ad => ad.ListForumThreadPosts(null, null, null, null)).ReturnsAsync(new List<ForumThreadPostModel> {
        new ForumThreadPostModel() { VisibleFlag = true },
        new ForumThreadPostModel() { VisibleFlag = false }
      });
      mockedForumDal.Setup(fd => fd.InsertForumUser(forumUserData, false)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.UpdateForumUser(forumUserData)).Returns(Task.Delay(0));
      mockedForumDal.Setup(fd => fd.GetForumUserById(invalidGuid, null)).ReturnsAsync((ForumUserModel)null);
      mockedForumDal.Setup(fd => fd.GetForumUserById(validGuid, null)).ReturnsAsync(new ForumUserModel());

      forumService = new ForumService(mockedForumDal.Object, mockedSettingsProvider.Object);
    }

    [Fact]
    public void ForumService_List_Forum_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users only get visible records
      Assert.Equal(1, forumService.ListForums(false).Result.Count());

      //Verify that dmin users get all the records
      Assert.Equal(2, forumService.ListForums(true).Result.Count());
    }

    [Fact]
    public void ForumService_Get_Forum_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users can get visible records
      Assert.NotNull(forumService.GetForumById(visibleGuid, null, false).Result);

      //Verify that non-admin users cannot get hidden records
      Assert.Null(forumService.GetForumById(hiddenGuid, null, false).Result  );

      //Verify that admin users can get visible records
      Assert.NotNull(forumService.GetForumById(visibleGuid, null, true).Result);

      //Verify that admin users can get hidden records
      Assert.NotNull(forumService.GetForumById(hiddenGuid, null, true).Result);
    }

    [Fact]
    public void ForumService_Upsert_Forum_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForum(null).Result
        , ForumService.FORUM_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ForumService_Upsert_Forum_GuidIsValidated()
    {
      //Verify that record with invalid guid is rejected
      forumData.Guid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_INVALID_RECORD));

      //Verify that record with valid Guid is accepted
      forumData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_INVALID_RECORD));

      //Verify that record with no Guid is accepted
      forumData.Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_INVALID_RECORD));
    }

    [Fact]
    public void ForumService_Upsert_Forum_NameIsValidated()
    {
      //Verify that record with no name/title is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_TITLE_IS_REQUIRED));

      //Verify that record with name/title is accepted
      forumData.Name = "Crap Title";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_TITLE_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_Forum_DescriptionIsValidated()
    {
      //Verify that record with no description is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_DESCRIPTION_IS_REQUIRED));

      //Verify that record with description is accepted
      forumData.Description = "Crap Description";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForum(forumData).Result
        , ForumService.FORUM_ERROR_DESCRIPTION_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_List_Thread_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users only get visible records
      Assert.Equal(1, forumService.ListForumThreads(null, null, false).Result.Count());

      //Verify that dmin users get all the records
      Assert.Equal(2, forumService.ListForumThreads(null, null, true).Result.Count());
    }

    [Fact]
    public void ForumService_Get_Thread_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users can get visible records
      Assert.NotNull(forumService.GetForumThreadById(visibleGuid, null, validGuid, false).Result);

      //Verify that non-admin users cannot get hidden records
      Assert.Null(forumService.GetForumThreadById(hiddenGuid, null, validGuid, false).Result);

      //Verify that admin users can get visible records
      Assert.NotNull(forumService.GetForumThreadById(visibleGuid, null, validGuid, true).Result);

      //Verify that admin users can get hidden records
      Assert.NotNull(forumService.GetForumThreadById(hiddenGuid, null, validGuid, true).Result);
    }

    [Fact]
    public void ForumService_Upsert_Thread_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThread(null, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ForumService_Upsert_Thread_GuidIsValidated()
    {
      //Verify that record with invalid guid is rejected
      forumThreadData.Guid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result      
        , ForumService.THREAD_ERROR_INVALID_RECORD));

      //Verify that record with valid Guid is accepted
      forumThreadData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_INVALID_RECORD));

      //Verify that record with no Guid is accepted
      forumThreadData.Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_INVALID_RECORD));
    }

    [Fact]
    public void ForumService_Upsert_Thread_NameIsValidated()
    {
      //Verify that record with no name/title is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_TITLE_IS_REQUIRED));

      //Verify that record with name/title is accepted
      forumThreadData.Name = "Crap Title";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_TITLE_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_Thread_InitialPostIsValidated()
    {
      //Verify that new record with no initial post is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_INITIAL_POST_IS_REQUIRED));

      //Verify that new record with initial post is accepted
      forumThreadData.InitialPost = new ForumThreadPostModel();
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_INITIAL_POST_IS_REQUIRED));

      //Verify that old record with no initial post is accepted
      forumThreadData.InitialPost = null;
      forumThreadData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, Guid.NewGuid(), true).Result
        , ForumService.THREAD_ERROR_INITIAL_POST_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_Thread_UserIsValidated()
    {
      //Verify that record with different user guid and no admin is rejected
      forumThreadData.Guid = validGuid;
      forumThreadData.ForumUserGuid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, invalidGuid, false).Result
        , ForumService.THREAD_ERROR_NOT_AUTHORIZED));

      //Verify that record with same user guid and no admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, validGuid, false).Result
        , ForumService.THREAD_ERROR_NOT_AUTHORIZED));

      //Verify that record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, invalidGuid, true).Result
        , ForumService.THREAD_ERROR_NOT_AUTHORIZED));

      //Verify that non visible record with same user guid and no admin is rejected
      forumThreadData.Guid = semiValidGuid1;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, validGuid, false).Result
        , ForumService.THREAD_ERROR_NOT_AUTHORIZED));

      //Verify that non-visible record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
         forumService.UpsertForumThread(forumThreadData, invalidGuid, true).Result
       , ForumService.THREAD_ERROR_NOT_AUTHORIZED));

      //Verify that locked record with same user guid and no admin is rejected
      forumThreadData.Guid = semiValidGuid2;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, validGuid, false).Result
        , ForumService.THREAD_ERROR_NOT_AUTHORIZED));

      //Verify that locked record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, invalidGuid, true).Result
        , ForumService.THREAD_ERROR_NOT_AUTHORIZED));
    }

    [Fact]
    public void ForumService_Upsert_Thread_TimingIsValidated()
    {
      //Verify that existing record within time limit and no admin is accepted
      forumThreadData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, validGuid, false).Result
        , ForumService.THREAD_ERROR_TOO_LATE_TO_UPDATE));

      //Verify that existing record outside of time limit and no admin is rejected
      forumThreadData.Guid = semiValidGuid1;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, validGuid, false).Result   
        , ForumService.THREAD_ERROR_TOO_LATE_TO_UPDATE));

      //Verify that existing record outside of time limit and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThread(forumThreadData, validGuid, true).Result
        , ForumService.THREAD_ERROR_TOO_LATE_TO_UPDATE));
    }

    [Fact]
    public void ForumService_List_Post_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users only get visible records
      Assert.Equal(1, forumService.ListForumThreadPosts(null, null, null, null, false).Result.Count());

      //Verify that dmin users get all the records
      Assert.Equal(2, forumService.ListForumThreadPosts(null, null, null, null, true).Result.Count());
    }

    [Fact]
    public void ForumService_Get_Post_NonVisibleRecordsAreFiltered()
    {
      //Verify that non-admin users can get visible records
      Assert.NotNull(forumService.GetForumThreadPostById(visibleGuid, validGuid, false).Result);

      //Verify that non-admin users cannot get hidden records
      Assert.Null(forumService.GetForumThreadPostById(hiddenGuid, validGuid, false).Result);

      //Verify that admin users can get visible records
      Assert.NotNull(forumService.GetForumThreadPostById(visibleGuid, validGuid, true).Result);

      //Verify that admin users can get hidden records
      Assert.NotNull(forumService.GetForumThreadPostById(hiddenGuid, validGuid, true).Result);
    }

    [Fact]
    public void ForumService_Upsert_Post_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThreadPost(null, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThreadPost(forumThreadPostData, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ForumService_Upsert_Post_GuidIsValidated()
    {
      //Verify that record with invalid guid is rejected
      forumThreadPostData.Guid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThreadPost(forumThreadPostData, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_INVALID_RECORD));

      //Verify that record with valid Guid is accepted
      forumThreadPostData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThreadPost(forumThreadPostData, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_INVALID_RECORD));

      //Verify that record with no Guid is accepted
      forumThreadPostData.Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumThreadPost(forumThreadPostData, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_INVALID_RECORD));
    }

    [Fact]
    public void ForumService_Upsert_Post_ContentIsValidated()
    {
      //Verify that record with no content is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_CONTENT_IS_REQUIRED));

      //Verify that record with content is accepted
      forumThreadPostData.Data = "Crap Content";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, Guid.NewGuid(), true).Result
        , ForumService.POST_ERROR_CONTENT_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_Post_UserIsValidated()
    {
      //Verify that record with different user guid and no admin is rejected
      forumThreadPostData.Guid = validGuid;
      forumThreadPostData.ForumUserGuid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, invalidGuid, false).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));

      //Verify that record with same user guid and no admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, validGuid, false).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));

      //Verify that record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, invalidGuid, true).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));

      //Verify that non visible record with same user guid and no admin is rejected
      forumThreadPostData.Guid = semiValidGuid1;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, validGuid, false).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));

      //Verify that non-visible record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, invalidGuid, true).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));

      //Verify that locked record with same user guid and no admin is rejected
      forumThreadPostData.Guid = semiValidGuid2;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, validGuid, false).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));

      //Verify that locked record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, invalidGuid, true).Result
        , ForumService.POST_ERROR_NOT_AUTHORIZED));
    }

    [Fact]
    public void ForumService_Upsert_Post_TimingIsValidated()
    {
      //Verify that existing record within time limit and no admin is accepted
      forumThreadPostData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, validGuid, false).Result
        , ForumService.POST_ERROR_TOO_LATE_TO_UPDATE));

      //Verify that existing record outside of time limit and no admin is rejected
      forumThreadPostData.Guid = semiValidGuid1;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, validGuid, false).Result
        , ForumService.POST_ERROR_TOO_LATE_TO_UPDATE));

      //Verify that existing record outside of time limit and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumThreadPost(forumThreadPostData, validGuid, true).Result
        , ForumService.POST_ERROR_TOO_LATE_TO_UPDATE));
    }

    [Fact]
    public void ForumService_Upsert_User_ObjectIsValidated()
    {
      //Verify that record with null object is rejected
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumUser(null, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_DATA_MUST_BE_PROVIDED));

      //Verify that record with non-null object is accepted
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_DATA_MUST_BE_PROVIDED));
    }

    [Fact]
    public void ForumService_Upsert_User_GuidIsValidated()
    {
      //Verify that record with invalid guid is rejected
      forumUserData.Guid = invalidGuid;
      Assert.True(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_INVALID_RECORD));

      //Verify that record with valid Guid is accepted
      forumUserData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_INVALID_RECORD));

      //Verify that record with no Guid is accepted
      forumUserData.Guid = Guid.Empty;
      Assert.False(TestHelpers.CallProducedError(() => 
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_INVALID_RECORD));
    }

    [Fact]
    public void ForumService_Upsert_User_UserIdIsValidated()
    {
      //Verify that record with no userId is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_USER_ID_IS_REQUIRED));

      //Verify that record with userId is accepted
      forumUserData.UserId = "crapUserId";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_USER_ID_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_User_EmailIsValidated()
    {
      //Verify that record with no email is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_EMAIL_IS_REQUIRED));

      //Verify that record with email is accepted
      forumUserData.Email = "crap@crap.com";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_EMAIL_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_User_CommentIsValidated()
    {
      //Verify that record with no comment is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_COMMENT_IS_REQUIRED));

      //Verify that record with comment is accepted
      forumUserData.Comment = "Crap Comment";
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_COMMENT_IS_REQUIRED));
    }

    [Fact]
    public void ForumService_Upsert_User_PasswordIsValidated()
    {
      //Verify that new record with no password is rejected
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_PASSWORD_IS_REQUIRED_FOR_NEW_USERS));

      //Verify that new record with password is accepted
      forumUserData.NewPassword = new NewPasswordModel() { Value = "CrappyPassword" };
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_PASSWORD_IS_REQUIRED_FOR_NEW_USERS));

      //Verify that existing record with password is accepted
      forumUserData.Guid = validGuid;
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_PASSWORD_IS_REQUIRED_FOR_NEW_USERS));

      //Verify that existing record with no password is accepted
      forumUserData.NewPassword.Value = string.Empty;
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, Guid.NewGuid(), true).Result
        , ForumService.USER_ERROR_PASSWORD_IS_REQUIRED_FOR_NEW_USERS));
    }

    [Fact]
    public void ForumService_Upsert_User_UserIsValidated()
    {
      //Verify that record with different user guid and no admin is rejected
      forumUserData.Guid = validGuid;
      Assert.True(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, invalidGuid, false).Result
        , ForumService.USER_ERROR_NOT_AUTHORIZED));

      //Verify that record with same user guid and no admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, validGuid, false).Result
        , ForumService.USER_ERROR_NOT_AUTHORIZED));

      //Verify that record with different user guid and admin is accepted
      Assert.False(TestHelpers.CallProducedError(() =>
          forumService.UpsertForumUser(forumUserData, invalidGuid, true).Result
        , ForumService.USER_ERROR_NOT_AUTHORIZED));
    }
  }
}
