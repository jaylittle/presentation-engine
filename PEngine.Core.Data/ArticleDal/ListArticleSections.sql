SELECT
  *
FROM
  ArticleSection
WHERE
  @articleGuid IS NULL OR ArticleGuid = @articleGuid;