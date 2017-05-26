SELECT
  ForumThreadPost.*,
  ForumThread.Name as ForumThreadName,
  ForumThread.UniqueName as ForumThreadUniqueName,
  Forum.Name as ForumName,
  Forum.UniqueName as ForumUniqueName,
  ForumUser.UserId as ForumUserId
FROM
  ForumThreadPost INNER JOIN
  ForumThread ON ForumThreadPost.ForumThreadGuid = ForumThread.Guid INNER JOIN
  Forum ON ForumThread.ForumGuid = Forum.Guid INNER JOIN
  ForumUser ON ForumThreadPost.ForumUserGuid = ForumUser.Guid
WHERE
  (@forumGuid IS NULL OR ForumThread.ForumGuid = @forumGuid) AND
  (@forumUniqueName IS NULL OR Forum.UniqueName = @forumUniqueName) AND
  (@forumThreadGuid IS NULL OR ForumThreadPost.ForumThreadGuid = @forumThreadGuid) AND
  (@forumThreadUniqueName IS NULL OR ForumThread.UniqueName = @forumThreadUniqueName);