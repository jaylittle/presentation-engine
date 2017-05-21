DELETE FROM
  ArticleSection
WHERE
  ArticleGuid = @guid;

DELETE FROM
  Article
WHERE
  Guid = @guid;