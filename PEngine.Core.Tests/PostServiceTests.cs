using System;
using System.Collections.Generic;
using System.Linq;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Shared.Models;
using Xunit;
using Moq;

namespace pengine.tests
{
  public class PostServiceTests
  {
    //This test is just a sample - its terribad
    [Fact]
    public void PostService_NameValidation()
    {
      var postData = new PostModel()
      {
        Data = "Blah Blah Blah"
      };

      var mockedPostDal = new Mock<IPostDal>();

      mockedPostDal.Setup(pd => pd.InsertPost(postData));
      mockedPostDal.Setup(pd => pd.UpdatePost(postData));
      var postService = new PostService(mockedPostDal.Object);
      
      //Verify that record with no name/title is rejected
      var errors = new List<string>();
      Assert.True(
        !postService.UpsertPost(postData, ref errors) &&
        errors.Any(e => string.Equals(PostService.POST_ERROR_TITLE_IS_REQUIRED, e))
      );

      //Verify that record with name/title is accepted
      errors.Clear();
      postData.Name = "Crap Title";
      Assert.True(postService.UpsertPost(postData, ref errors));
    }
  }
}
