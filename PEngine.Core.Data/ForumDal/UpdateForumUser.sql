UPDATE
  ForumUser
SET
  UserId = @UserId,
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