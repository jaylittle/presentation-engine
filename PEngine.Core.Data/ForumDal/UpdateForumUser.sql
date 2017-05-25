UPDATE
  ForumUser
SET
  UserID = @UserID,
  Password = @Password,
  AdminFlag = @AdminFlag,
  BanFlag = @BanFlag,
  Email = @Email,
  Website = @Website,
  Comment = @Comment,
  LastIPAddress = @LastIPAddress,
  LastLogon = @LastLogon,
  ModifiedUTC =@ModifiedUTC
WHERE
  Guid = @Guid;