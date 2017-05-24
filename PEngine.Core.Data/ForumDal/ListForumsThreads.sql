SELECT
  ForumThread.*,
  Forum.Name as ForumName,
  Forum.UniqueName as ForumUniqueName,
  ForumUser.Name as ForumUserName
FROM
  ForumThread INNER JOIN
  Forum ON ForumThread.ForumGuid = Forum.Guid INNER JOIN
  ForumUser ON ForumThread.ForumUserGuid = ForumUser.Guid
WHERE
  (@forumGuid IS NULL OR ForumThread.ForumGuid = @forumGuid) AND
  (@forumUniqueName IS NULL OR Forum.UniqueName = @forumUniqueName);