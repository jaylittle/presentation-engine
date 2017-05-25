INSERT INTO ForumUser (
  Guid,
  UserID,
  Password,
  AdminFlag,
  BanFlag,
  Email,
  Website,
  Comment,
  LastIPAddress,
  LastLogon,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @UserID,
  @Password,
  @AdminFlag,
  @BanFlag,
  @Email,
  @Website,
  @Comment,
  @LastIPAddress,
  @LastLogon,
  @CreatedUTC,
  @ModifiedUTC
);