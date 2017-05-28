using System;
using System.Collections.Generic;
using System.Linq;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Shared.Models;
using Xunit;
using Moq;
using Newtonsoft.Json;

namespace pengine.tests
{
  public class PostServiceTests
  {
    [Fact]
    public void PostService_NullValidation()
    {
      //Setup
      var postData = new PostModel();
      var mockedPostDal = new Mock<IPostDal>();
      mockedPostDal.Setup(pd => pd.InsertPost(postData));
      mockedPostDal.Setup(pd => pd.UpdatePost(postData));
      var postService = new PostService(mockedPostDal.Object);
      
      //Verify that upsert will null object is rejected
      var errors = new List<string>();
      postService.UpsertPost(null, ref errors);
      Assert.True(errors.Any(e => string.Equals(PostService.POST_ERROR_DATA_MUST_BE_PROVIDED, e)));

      //Verify that record with data/content is accepted
      errors.Clear();
      postService.UpsertPost(postData, ref errors);
      Assert.True(!errors.Any(e => string.Equals(PostService.POST_ERROR_DATA_MUST_BE_PROVIDED, e)));
    }

    [Fact]
    public void PostService_GuidValidation()
    {
      //Setup
      Guid validGuid = Guid.NewGuid();
      Guid invalidGuid = Guid.NewGuid();
      var postData = new PostModel();
      var mockedPostDal = new Mock<IPostDal>();
      mockedPostDal.Setup(pd => pd.InsertPost(postData));
      mockedPostDal.Setup(pd => pd.UpdatePost(postData));
      mockedPostDal.Setup(pd => pd.GetPostById(invalidGuid, null, null)).Returns((PostModel)null);
      mockedPostDal.Setup(pd => pd.GetPostById(validGuid, null, null)).Returns(new PostModel());
      var postService = new PostService(mockedPostDal.Object);
      
      //Verify that record with invalid guid is rejected
      var errors = new List<string>();
      postData.Guid = invalidGuid;
      postService.UpsertPost(postData, ref errors);
      Assert.True(errors.Any(e => string.Equals(PostService.POST_ERROR_INVALID_RECORD, e)));

      //Verify that record with valid Guid is accepted
      errors.Clear();
      postData.Guid = validGuid;
      postService.UpsertPost(postData, ref errors);
      Assert.True(!errors.Any(e => string.Equals(PostService.POST_ERROR_INVALID_RECORD, e)));

      //Verify that record with no Guid is accepted
      errors.Clear();
      postData.Guid = Guid.Empty;
      postService.UpsertPost(postData, ref errors);
      Assert.True(!errors.Any(e => string.Equals(PostService.POST_ERROR_INVALID_RECORD, e)));
    }

    [Fact]
    public void PostService_NameValidation()
    {
      //Setup
      var postData = new PostModel();
      var mockedPostDal = new Mock<IPostDal>();
      mockedPostDal.Setup(pd => pd.InsertPost(postData));
      mockedPostDal.Setup(pd => pd.UpdatePost(postData));
      var postService = new PostService(mockedPostDal.Object);
      
      //Verify that record with no name/title is rejected
      var errors = new List<string>();
      postService.UpsertPost(postData, ref errors);
      Assert.True(errors.Any(e => string.Equals(PostService.POST_ERROR_TITLE_IS_REQUIRED, e)));

      //Verify that record with name/title is accepted
      errors.Clear();
      postData.Name = "Crap Title";
      postService.UpsertPost(postData, ref errors);
      Assert.True(!errors.Any(e => string.Equals(PostService.POST_ERROR_TITLE_IS_REQUIRED, e)));
    }

    [Fact]
    public void PostService_DataValidation()
    {
      //Setup
      var postData = new PostModel();
      var mockedPostDal = new Mock<IPostDal>();
      mockedPostDal.Setup(pd => pd.InsertPost(postData));
      mockedPostDal.Setup(pd => pd.UpdatePost(postData));
      var postService = new PostService(mockedPostDal.Object);
      
      //Verify that record with no data/content is rejected
      var errors = new List<string>();
      postService.UpsertPost(postData, ref errors);
      Assert.True(errors.Any(e => string.Equals(PostService.POST_ERROR_CONTENT_IS_REQUIRED, e)));

      //Verify that record with data/content is accepted
      errors.Clear();
      postData.Data = "Crap Content";
      postService.UpsertPost(postData, ref errors);
      Assert.True(!errors.Any(e => string.Equals(PostService.POST_ERROR_CONTENT_IS_REQUIRED, e)));
    }
  }
}
