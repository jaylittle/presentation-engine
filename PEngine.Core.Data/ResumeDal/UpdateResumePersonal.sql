UPDATE
  ResumePersonal
SET
  LegacyID = @LegacyID,
  FullName = @FullName,
  Address1 = @Address1,
  Address2 = @Address2,
  City = @City,
  State = @State,
  Zip = @Zip,
  Phone = @Phone,
  Fax = @Fax,
  Email = @Email,
  WebsiteURL = @WebsiteURL,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;