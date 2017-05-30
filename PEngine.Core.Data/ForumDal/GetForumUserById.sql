SELECT
  *
FROM
  ForumUser
WHERE
  (@guid IS NULL OR ForumUser.Guid = @guid) AND
  (@userId IS NULL OR ForumUser.UserID = @userId);