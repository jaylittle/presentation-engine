SELECT
  *
FROM
  Post
WHERE
  (@guid IS NULL OR Guid = @guid) AND
  (@legacyId IS NULL OR LegacyId = @legacyId) AND
  (@uniqueName IS NULL OR UniqueName = @uniqueName);