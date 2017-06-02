SELECT
  *
FROM
  Article
WHERE
  (@category IS NULL OR Category = @category);