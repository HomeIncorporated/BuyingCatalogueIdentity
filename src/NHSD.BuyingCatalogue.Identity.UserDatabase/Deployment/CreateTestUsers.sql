﻿DECLARE @aliceEmail AS nvarchar(50) = N'AliceSmith@email.com';
DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';

IF '$(INSERT_TEST_DATA)' = 'True'
AND NOT EXISTS (
  SELECT *
  FROM dbo.AspNetUsers
  WHERE UserName IN (@aliceEmail, @bobEmail))
BEGIN
    DECLARE @ccgRoleId AS nchar(4) = 'RO98';
    DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';

    DECLARE @aliceOrganisationId AS uniqueidentifier = (SELECT TOP (1) OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId ORDER BY OdsCode);
    DECLARE @aliceOrganisationName AS nvarchar(255) =  (SELECT TOP (1) Name FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId ORDER BY OdsCode);

    DECLARE @bobOrganisationId AS uniqueidentifier = (SELECT OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);
    DECLARE @bobOrganisationName AS nvarchar(255) =  (SELECT TOP (1) Name FROM dbo.Organisations WHERE PrimaryRoleId = @ccgRoleId ORDER BY OdsCode);

	DECLARE @address AS nchar(108) = N'{ "street_address": "One Hacker Way", "locality": "Heidelberg", "postal_code": 69118, "country": "Germany" }';

	DECLARE @aliceId AS nchar(36) = CAST(NEWID() AS nchar(36));
	DECLARE @bobId AS nchar(36) = CAST(NEWID() AS nchar(36));

	DECLARE @aliceNormalizedEmail AS nvarchar(50) = UPPER(@aliceEmail);
    DECLARE @bobNormalizedEmail AS nvarchar(50) = UPPER(@bobEmail);

    DECLARE @phoneNumber AS nvarchar(max) = '01234567890';

	-- 'Pass123$'
	DECLARE @alicePassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEFSsEthAqGVBLj1P1gF9puxtXm18lKHlmuh9J/Ib0KKBO3GjQvxymJbzpSqy0zuOHg==';

	-- 'Pass123$'
	DECLARE @bobPassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEOzr1Zwpoo1pKsTa+S65mBZVG4GIy6IYH/IAED6TvBA+FIMg8u/xb0b/cfexV7SHNw==';

	INSERT INTO dbo.AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, AccessFailedCount, ConcurrencyStamp, PhoneNumber,
        EmailConfirmed, LockoutEnabled, PasswordHash, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, 
        FirstName, LastName, PrimaryOrganisationId, OrganisationFunction, Disabled, CatalogueAgreementSigned
    )
	VALUES
	(@aliceId, @aliceEmail, @aliceNormalizedEmail, @aliceEmail, @aliceNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @alicePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'Alice', 'Smith', @aliceOrganisationId, 'Buyer', 0, 1),
	(@bobId, @bobEmail, @bobNormalizedEmail, @bobEmail, @bobNormalizedEmail, 0, NEWID(), @phoneNumber, 1, 1, @bobPassword, 0, 'OBDOPOU5YQ5WQXCR3DITKL6L5IDPYHHJ', 0, 'Bob', 'Smith', @bobOrganisationId, 'Authority', 0, 1);

	INSERT INTO dbo.AspNetUserClaims (ClaimType, ClaimValue, UserId)
	VALUES
	(N'email_verified', N'true', @aliceId),
	(N'website', N'http://alice.com/', @aliceId),
	(N'address', @address, @aliceId),
    (N'primaryOrganisationName', @aliceOrganisationName, @aliceId),
	(N'email_verified', N'true', @bobId),
	(N'location', N'somewhere', @bobId),
	(N'website', N'http://bob.com/', @bobId),
	(N'address', @address, @bobId),
    (N'primaryOrganisationName', @bobOrganisationName, @bobId);
END;
