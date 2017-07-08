SELECT
  *
FROM
  Article INNER JOIN
  ArticleSection ON Article.Guid = ArticleSection.ArticleGuid
WHERE
  (@category IS NULL OR Category = @category);