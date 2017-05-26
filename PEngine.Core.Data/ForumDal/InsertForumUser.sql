INSERT INTO ForumUser (
  Guid,
  UserId,
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
  @UserId,
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