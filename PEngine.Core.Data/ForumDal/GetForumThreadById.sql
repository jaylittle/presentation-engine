SELECT
  *
FROM
  ForumThread
WHERE
  (@guid IS NULL OR Guid = @guid) AND
  (@uniqueName IS NULL OR UniqueName = @uniqueName);