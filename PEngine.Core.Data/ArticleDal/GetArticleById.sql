SELECT
  *
FROM
  Article LEFT JOIN
  ArticleSection ON Article.Guid = ArticleSection.ArticleGuid
WHERE
  (@guid IS NULL OR Article.Guid = @guid) AND
  (@legacyId IS NULL OR Article.LegacyId = @legacyId) AND
  (@uniqueName IS NULL OR Article.UniqueName = @uniqueName);