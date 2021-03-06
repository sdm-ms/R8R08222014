USE [master]
GO
/****** Object:  Database [RD0001]    Script Date: 7/26/2014 7:24:55 AM ******/
CREATE DATABASE [RD0001]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RD0001', FILENAME = N'F:\RD0001.mdf' , SIZE = 72704KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'RD0001_log', FILENAME = N'F:\RD0001_log.ldf' , SIZE = 63424KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [RD0001] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RD0001].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RD0001] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RD0001] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RD0001] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RD0001] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RD0001] SET ARITHABORT OFF 
GO
ALTER DATABASE [RD0001] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RD0001] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [RD0001] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RD0001] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RD0001] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RD0001] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RD0001] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RD0001] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RD0001] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RD0001] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RD0001] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RD0001] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RD0001] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RD0001] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RD0001] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RD0001] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RD0001] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RD0001] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RD0001] SET RECOVERY FULL 
GO
ALTER DATABASE [RD0001] SET  MULTI_USER 
GO
ALTER DATABASE [RD0001] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RD0001] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RD0001] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RD0001] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'RD0001', N'ON'
GO
USE [RD0001]
GO
/****** Object:  DatabaseRole [aspnet_WebEvent_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_WebEvent_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Roles_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Roles_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Roles_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Profile_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Profile_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Profile_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Personalization_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Personalization_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Personalization_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Membership_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Membership_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE ROLE [aspnet_Membership_BasicAccess]
GO
ALTER ROLE [aspnet_Roles_BasicAccess] ADD MEMBER [aspnet_Roles_FullAccess]
GO
ALTER ROLE [aspnet_Roles_ReportingAccess] ADD MEMBER [aspnet_Roles_FullAccess]
GO
ALTER ROLE [aspnet_Profile_BasicAccess] ADD MEMBER [aspnet_Profile_FullAccess]
GO
ALTER ROLE [aspnet_Profile_ReportingAccess] ADD MEMBER [aspnet_Profile_FullAccess]
GO
ALTER ROLE [aspnet_Personalization_BasicAccess] ADD MEMBER [aspnet_Personalization_FullAccess]
GO
ALTER ROLE [aspnet_Personalization_ReportingAccess] ADD MEMBER [aspnet_Personalization_FullAccess]
GO
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [aspnet_Membership_FullAccess]
GO
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [aspnet_Membership_FullAccess]
GO
/****** Object:  Schema [aspnet_Membership_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Membership_BasicAccess]
GO
/****** Object:  Schema [aspnet_Membership_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Membership_FullAccess]
GO
/****** Object:  Schema [aspnet_Membership_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Membership_ReportingAccess]
GO
/****** Object:  Schema [aspnet_Personalization_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Personalization_BasicAccess]
GO
/****** Object:  Schema [aspnet_Personalization_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Personalization_FullAccess]
GO
/****** Object:  Schema [aspnet_Personalization_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Personalization_ReportingAccess]
GO
/****** Object:  Schema [aspnet_Profile_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Profile_BasicAccess]
GO
/****** Object:  Schema [aspnet_Profile_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Profile_FullAccess]
GO
/****** Object:  Schema [aspnet_Profile_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Profile_ReportingAccess]
GO
/****** Object:  Schema [aspnet_Roles_BasicAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Roles_BasicAccess]
GO
/****** Object:  Schema [aspnet_Roles_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Roles_FullAccess]
GO
/****** Object:  Schema [aspnet_Roles_ReportingAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_Roles_ReportingAccess]
GO
/****** Object:  Schema [aspnet_WebEvent_FullAccess]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE SCHEMA [aspnet_WebEvent_FullAccess]
GO
/****** Object:  StoredProcedure [dbo].[aspnet_AnyDataInTables]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_AnyDataInTables]
    @TablesToCheck int
AS
BEGIN
    -- Check Membership table if (@TablesToCheck & 1) is set
    IF ((@TablesToCheck & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Membership))
        BEGIN
            SELECT N'aspnet_Membership'
            RETURN
        END
    END

    -- Check aspnet_Roles table if (@TablesToCheck & 2) is set
    IF ((@TablesToCheck & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Roles') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 RoleId FROM dbo.aspnet_Roles))
        BEGIN
            SELECT N'aspnet_Roles'
            RETURN
        END
    END

    -- Check aspnet_Profile table if (@TablesToCheck & 4) is set
    IF ((@TablesToCheck & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Profile))
        BEGIN
            SELECT N'aspnet_Profile'
            RETURN
        END
    END

    -- Check aspnet_PersonalizationPerUser table if (@TablesToCheck & 8) is set
    IF ((@TablesToCheck & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_PersonalizationPerUser))
        BEGIN
            SELECT N'aspnet_PersonalizationPerUser'
            RETURN
        END
    END

    -- Check aspnet_PersonalizationPerUser table if (@TablesToCheck & 16) is set
    IF ((@TablesToCheck & 16) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_WebEvent_LogEvent') AND (type = 'P'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 * FROM dbo.aspnet_WebEvent_Events))
        BEGIN
            SELECT N'aspnet_WebEvent_Events'
            RETURN
        END
    END

    -- Check aspnet_Users table if (@TablesToCheck & 1,2,4 & 8) are all set
    IF ((@TablesToCheck & 1) <> 0 AND
        (@TablesToCheck & 2) <> 0 AND
        (@TablesToCheck & 4) <> 0 AND
        (@TablesToCheck & 8) <> 0 AND
        (@TablesToCheck & 32) <> 0 AND
        (@TablesToCheck & 128) <> 0 AND
        (@TablesToCheck & 256) <> 0 AND
        (@TablesToCheck & 512) <> 0 AND
        (@TablesToCheck & 1024) <> 0)
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Users))
        BEGIN
            SELECT N'aspnet_Users'
            RETURN
        END
        IF (EXISTS(SELECT TOP 1 ApplicationId FROM dbo.aspnet_Applications))
        BEGIN
            SELECT N'aspnet_Applications'
            RETURN
        END
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Applications_CreateApplication]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Applications_CreateApplication]
    @ApplicationName      nvarchar(256),
    @ApplicationId        uniqueidentifier OUTPUT
AS
BEGIN
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName

    IF(@ApplicationId IS NULL)
    BEGIN
        DECLARE @TranStarted   bit
        SET @TranStarted = 0

        IF( @@TRANCOUNT = 0 )
        BEGIN
	        BEGIN TRANSACTION
	        SET @TranStarted = 1
        END
        ELSE
    	    SET @TranStarted = 0

        SELECT  @ApplicationId = ApplicationId
        FROM dbo.aspnet_Applications WITH (UPDLOCK, HOLDLOCK)
        WHERE LOWER(@ApplicationName) = LoweredApplicationName

        IF(@ApplicationId IS NULL)
        BEGIN
            SELECT  @ApplicationId = NEWID()
            INSERT  dbo.aspnet_Applications (ApplicationId, ApplicationName, LoweredApplicationName)
            VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName))
        END


        IF( @TranStarted = 1 )
        BEGIN
            IF(@@ERROR = 0)
            BEGIN
	        SET @TranStarted = 0
	        COMMIT TRANSACTION
            END
            ELSE
            BEGIN
                SET @TranStarted = 0
                ROLLBACK TRANSACTION
            END
        END
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_CheckSchemaVersion]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_CheckSchemaVersion]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128)
AS
BEGIN
    IF (EXISTS( SELECT  *
                FROM    dbo.aspnet_SchemaVersions
                WHERE   Feature = LOWER( @Feature ) AND
                        CompatibleSchemaVersion = @CompatibleSchemaVersion ))
        RETURN 0

    RETURN 1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer]
    @ApplicationName       nvarchar(256),
    @UserName              nvarchar(256),
    @NewPasswordQuestion   nvarchar(256),
    @NewPasswordAnswer     nvarchar(128)
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Membership m, dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId
    IF (@UserId IS NULL)
    BEGIN
        RETURN(1)
    END

    UPDATE dbo.aspnet_Membership
    SET    PasswordQuestion = @NewPasswordQuestion, PasswordAnswer = @NewPasswordAnswer
    WHERE  UserId=@UserId
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_CreateUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_CreateUser]
    @ApplicationName                        nvarchar(256),
    @UserName                               nvarchar(256),
    @Password                               nvarchar(128),
    @PasswordSalt                           nvarchar(128),
    @Email                                  nvarchar(256),
    @PasswordQuestion                       nvarchar(256),
    @PasswordAnswer                         nvarchar(128),
    @IsApproved                             bit,
    @CurrentTimeUtc                         datetime,
    @CreateDate                             datetime = NULL,
    @UniqueEmail                            int      = 0,
    @PasswordFormat                         int      = 0,
    @UserId                                 uniqueidentifier OUTPUT
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @NewUserId uniqueidentifier
    SELECT @NewUserId = NULL

    DECLARE @IsLockedOut bit
    SET @IsLockedOut = 0

    DECLARE @LastLockoutDate  datetime
    SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )

    DECLARE @FailedPasswordAttemptCount int
    SET @FailedPasswordAttemptCount = 0

    DECLARE @FailedPasswordAttemptWindowStart  datetime
    SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

    DECLARE @FailedPasswordAnswerAttemptCount int
    SET @FailedPasswordAnswerAttemptCount = 0

    DECLARE @FailedPasswordAnswerAttemptWindowStart  datetime
    SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

    DECLARE @NewUserCreated bit
    DECLARE @ReturnValue   int
    SET @ReturnValue = 0

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    SET @CreateDate = @CurrentTimeUtc

    SELECT  @NewUserId = UserId FROM dbo.aspnet_Users WHERE LOWER(@UserName) = LoweredUserName AND @ApplicationId = ApplicationId
    IF ( @NewUserId IS NULL )
    BEGIN
        SET @NewUserId = @UserId
        EXEC @ReturnValue = dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CreateDate, @NewUserId OUTPUT
        SET @NewUserCreated = 1
    END
    ELSE
    BEGIN
        SET @NewUserCreated = 0
        IF( @NewUserId <> @UserId AND @UserId IS NOT NULL )
        BEGIN
            SET @ErrorCode = 6
            GOTO Cleanup
        END
    END

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @ReturnValue = -1 )
    BEGIN
        SET @ErrorCode = 10
        GOTO Cleanup
    END

    IF ( EXISTS ( SELECT UserId
                  FROM   dbo.aspnet_Membership
                  WHERE  @NewUserId = UserId ) )
    BEGIN
        SET @ErrorCode = 6
        GOTO Cleanup
    END

    SET @UserId = @NewUserId

    IF (@UniqueEmail = 1)
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership m WITH ( UPDLOCK, HOLDLOCK )
                    WHERE ApplicationId = @ApplicationId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            SET @ErrorCode = 7
            GOTO Cleanup
        END
    END

    IF (@NewUserCreated = 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate = @CreateDate
        WHERE  @UserId = UserId
        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    INSERT INTO dbo.aspnet_Membership
                ( ApplicationId,
                  UserId,
                  Password,
                  PasswordSalt,
                  Email,
                  LoweredEmail,
                  PasswordQuestion,
                  PasswordAnswer,
                  PasswordFormat,
                  IsApproved,
                  IsLockedOut,
                  CreateDate,
                  LastLoginDate,
                  LastPasswordChangedDate,
                  LastLockoutDate,
                  FailedPasswordAttemptCount,
                  FailedPasswordAttemptWindowStart,
                  FailedPasswordAnswerAttemptCount,
                  FailedPasswordAnswerAttemptWindowStart )
         VALUES ( @ApplicationId,
                  @UserId,
                  @Password,
                  @PasswordSalt,
                  @Email,
                  LOWER(@Email),
                  @PasswordQuestion,
                  @PasswordAnswer,
                  @PasswordFormat,
                  @IsApproved,
                  @IsLockedOut,
                  @CreateDate,
                  @CreateDate,
                  @CreateDate,
                  @LastLockoutDate,
                  @FailedPasswordAttemptCount,
                  @FailedPasswordAttemptWindowStart,
                  @FailedPasswordAnswerAttemptCount,
                  @FailedPasswordAnswerAttemptWindowStart )

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_FindUsersByEmail]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_FindUsersByEmail]
    @ApplicationName       nvarchar(256),
    @EmailToMatch          nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    IF( @EmailToMatch IS NULL )
        INSERT INTO #PageIndexForUsers (UserId)
            SELECT u.UserId
            FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
            WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND m.Email IS NULL
            ORDER BY m.LoweredEmail
    ELSE
        INSERT INTO #PageIndexForUsers (UserId)
            SELECT u.UserId
            FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
            WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND m.LoweredEmail LIKE LOWER(@EmailToMatch)
            ORDER BY m.LoweredEmail

    SELECT  u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY m.LoweredEmail

    SELECT  @TotalRecords = COUNT(*)
    FROM    #PageIndexForUsers
    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_FindUsersByName]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_FindUsersByName]
    @ApplicationName       nvarchar(256),
    @UserNameToMatch       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT u.UserId
        FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND u.LoweredUserName LIKE LOWER(@UserNameToMatch)
        ORDER BY u.UserName


    SELECT  u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY u.UserName

    SELECT  @TotalRecords = COUNT(*)
    FROM    #PageIndexForUsers
    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetAllUsers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetAllUsers]
    @ApplicationName       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0


    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
    SELECT u.UserId
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u
    WHERE  u.ApplicationId = @ApplicationId AND u.UserId = m.UserId
    ORDER BY u.UserName

    SELECT @TotalRecords = @@ROWCOUNT

    SELECT u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY u.UserName
    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetNumberOfUsersOnline]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetNumberOfUsersOnline]
    @ApplicationName            nvarchar(256),
    @MinutesSinceLastInActive   int,
    @CurrentTimeUtc             datetime
AS
BEGIN
    DECLARE @DateActive datetime
    SELECT  @DateActive = DATEADD(minute,  -(@MinutesSinceLastInActive), @CurrentTimeUtc)

    DECLARE @NumOnline int
    SELECT  @NumOnline = COUNT(*)
    FROM    dbo.aspnet_Users u(NOLOCK),
            dbo.aspnet_Applications a(NOLOCK),
            dbo.aspnet_Membership m(NOLOCK)
    WHERE   u.ApplicationId = a.ApplicationId                  AND
            LastActivityDate > @DateActive                     AND
            a.LoweredApplicationName = LOWER(@ApplicationName) AND
            u.UserId = m.UserId
    RETURN(@NumOnline)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetPassword]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetPassword]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @MaxInvalidPasswordAttempts     int,
    @PasswordAttemptWindow          int,
    @CurrentTimeUtc                 datetime,
    @PasswordAnswer                 nvarchar(128) = NULL
AS
BEGIN
    DECLARE @UserId                                 uniqueidentifier
    DECLARE @PasswordFormat                         int
    DECLARE @Password                               nvarchar(128)
    DECLARE @passAns                                nvarchar(128)
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId,
            @Password = m.Password,
            @passAns = m.PasswordAnswer,
            @PasswordFormat = m.PasswordFormat,
            @IsLockedOut = m.IsLockedOut,
            @LastLockoutDate = m.LastLockoutDate,
            @FailedPasswordAttemptCount = m.FailedPasswordAttemptCount,
            @FailedPasswordAttemptWindowStart = m.FailedPasswordAttemptWindowStart,
            @FailedPasswordAnswerAttemptCount = m.FailedPasswordAnswerAttemptCount,
            @FailedPasswordAnswerAttemptWindowStart = m.FailedPasswordAnswerAttemptWindowStart
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m WITH ( UPDLOCK )
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF ( @@rowcount = 0 )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    IF( @IsLockedOut = 1 )
    BEGIN
        SET @ErrorCode = 99
        GOTO Cleanup
    END

    IF ( NOT( @PasswordAnswer IS NULL ) )
    BEGIN
        IF( ( @passAns IS NULL ) OR ( LOWER( @passAns ) <> LOWER( @PasswordAnswer ) ) )
        BEGIN
            IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAnswerAttemptWindowStart ) )
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = 1
            END
            ELSE
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount + 1
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
            END

            BEGIN
                IF( @FailedPasswordAnswerAttemptCount >= @MaxInvalidPasswordAttempts )
                BEGIN
                    SET @IsLockedOut = 1
                    SET @LastLockoutDate = @CurrentTimeUtc
                END
            END

            SET @ErrorCode = 3
        END
        ELSE
        BEGIN
            IF( @FailedPasswordAnswerAttemptCount > 0 )
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = 0
                SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            END
        END

        UPDATE dbo.aspnet_Membership
        SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
            FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
            FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
            FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
            FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
        WHERE @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    IF( @ErrorCode = 0 )
        SELECT @Password, @PasswordFormat

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetPasswordWithFormat]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetPasswordWithFormat]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @UpdateLastLoginActivityDate    bit,
    @CurrentTimeUtc                 datetime
AS
BEGIN
    DECLARE @IsLockedOut                        bit
    DECLARE @UserId                             uniqueidentifier
    DECLARE @Password                           nvarchar(128)
    DECLARE @PasswordSalt                       nvarchar(128)
    DECLARE @PasswordFormat                     int
    DECLARE @FailedPasswordAttemptCount         int
    DECLARE @FailedPasswordAnswerAttemptCount   int
    DECLARE @IsApproved                         bit
    DECLARE @LastActivityDate                   datetime
    DECLARE @LastLoginDate                      datetime

    SELECT  @UserId          = NULL

    SELECT  @UserId = u.UserId, @IsLockedOut = m.IsLockedOut, @Password=Password, @PasswordFormat=PasswordFormat,
            @PasswordSalt=PasswordSalt, @FailedPasswordAttemptCount=FailedPasswordAttemptCount,
		    @FailedPasswordAnswerAttemptCount=FailedPasswordAnswerAttemptCount, @IsApproved=IsApproved,
            @LastActivityDate = LastActivityDate, @LastLoginDate = LastLoginDate
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF (@UserId IS NULL)
        RETURN 1

    IF (@IsLockedOut = 1)
        RETURN 99

    SELECT   @Password, @PasswordFormat, @PasswordSalt, @FailedPasswordAttemptCount,
             @FailedPasswordAnswerAttemptCount, @IsApproved, @LastLoginDate, @LastActivityDate

    IF (@UpdateLastLoginActivityDate = 1 AND @IsApproved = 1)
    BEGIN
        UPDATE  dbo.aspnet_Membership
        SET     LastLoginDate = @CurrentTimeUtc
        WHERE   UserId = @UserId

        UPDATE  dbo.aspnet_Users
        SET     LastActivityDate = @CurrentTimeUtc
        WHERE   @UserId = UserId
    END


    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByEmail]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByEmail]
    @ApplicationName  nvarchar(256),
    @Email            nvarchar(256)
AS
BEGIN
    IF( @Email IS NULL )
        SELECT  u.UserName
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                u.UserId = m.UserId AND
                m.LoweredEmail IS NULL
    ELSE
        SELECT  u.UserName
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                u.UserId = m.UserId AND
                LOWER(@Email) = m.LoweredEmail

    IF (@@rowcount = 0)
        RETURN(1)
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByName]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByName]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    DECLARE @UserId uniqueidentifier

    IF (@UpdateLastActivity = 1)
    BEGIN
        -- select user ID from aspnet_users table
        SELECT TOP 1 @UserId = u.UserId
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1

        UPDATE   dbo.aspnet_Users
        SET      LastActivityDate = @CurrentTimeUtc
        WHERE    @UserId = UserId

        SELECT m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut, m.LastLockoutDate
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE  @UserId = u.UserId AND u.UserId = m.UserId 
    END
    ELSE
    BEGIN
        SELECT TOP 1 m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut,m.LastLockoutDate
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1
    END

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByUserId]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByUserId]
    @UserId               uniqueidentifier,
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    IF ( @UpdateLastActivity = 1 )
    BEGIN
        UPDATE   dbo.aspnet_Users
        SET      LastActivityDate = @CurrentTimeUtc
        FROM     dbo.aspnet_Users
        WHERE    @UserId = UserId

        IF ( @@ROWCOUNT = 0 ) -- User ID not found
            RETURN -1
    END

    SELECT  m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate, m.LastLoginDate, u.LastActivityDate,
            m.LastPasswordChangedDate, u.UserName, m.IsLockedOut,
            m.LastLockoutDate
    FROM    dbo.aspnet_Users u, dbo.aspnet_Membership m
    WHERE   @UserId = u.UserId AND u.UserId = m.UserId

    IF ( @@ROWCOUNT = 0 ) -- User ID not found
       RETURN -1

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_ResetPassword]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_ResetPassword]
    @ApplicationName             nvarchar(256),
    @UserName                    nvarchar(256),
    @NewPassword                 nvarchar(128),
    @MaxInvalidPasswordAttempts  int,
    @PasswordAttemptWindow       int,
    @PasswordSalt                nvarchar(128),
    @CurrentTimeUtc              datetime,
    @PasswordFormat              int = 0,
    @PasswordAnswer              nvarchar(128) = NULL
AS
BEGIN
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @UserId                                 uniqueidentifier
    SET     @UserId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    SELECT @IsLockedOut = IsLockedOut,
           @LastLockoutDate = LastLockoutDate,
           @FailedPasswordAttemptCount = FailedPasswordAttemptCount,
           @FailedPasswordAttemptWindowStart = FailedPasswordAttemptWindowStart,
           @FailedPasswordAnswerAttemptCount = FailedPasswordAnswerAttemptCount,
           @FailedPasswordAnswerAttemptWindowStart = FailedPasswordAnswerAttemptWindowStart
    FROM dbo.aspnet_Membership WITH ( UPDLOCK )
    WHERE @UserId = UserId

    IF( @IsLockedOut = 1 )
    BEGIN
        SET @ErrorCode = 99
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Membership
    SET    Password = @NewPassword,
           LastPasswordChangedDate = @CurrentTimeUtc,
           PasswordFormat = @PasswordFormat,
           PasswordSalt = @PasswordSalt
    WHERE  @UserId = UserId AND
           ( ( @PasswordAnswer IS NULL ) OR ( LOWER( PasswordAnswer ) = LOWER( @PasswordAnswer ) ) )

    IF ( @@ROWCOUNT = 0 )
        BEGIN
            IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAnswerAttemptWindowStart ) )
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = 1
            END
            ELSE
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount + 1
            END

            BEGIN
                IF( @FailedPasswordAnswerAttemptCount >= @MaxInvalidPasswordAttempts )
                BEGIN
                    SET @IsLockedOut = 1
                    SET @LastLockoutDate = @CurrentTimeUtc
                END
            END

            SET @ErrorCode = 3
        END
    ELSE
        BEGIN
            IF( @FailedPasswordAnswerAttemptCount > 0 )
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = 0
                SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            END
        END

    IF( NOT ( @PasswordAnswer IS NULL ) )
    BEGIN
        UPDATE dbo.aspnet_Membership
        SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
            FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
            FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
            FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
            FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
        WHERE @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_SetPassword]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_SetPassword]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @NewPassword      nvarchar(128),
    @PasswordSalt     nvarchar(128),
    @CurrentTimeUtc   datetime,
    @PasswordFormat   int = 0
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)

    UPDATE dbo.aspnet_Membership
    SET Password = @NewPassword, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt,
        LastPasswordChangedDate = @CurrentTimeUtc
    WHERE @UserId = UserId
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UnlockUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UnlockUser]
    @ApplicationName                         nvarchar(256),
    @UserName                                nvarchar(256)
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
        RETURN 1

    UPDATE dbo.aspnet_Membership
    SET IsLockedOut = 0,
        FailedPasswordAttemptCount = 0,
        FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 ),
        FailedPasswordAnswerAttemptCount = 0,
        FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 ),
        LastLockoutDate = CONVERT( datetime, '17540101', 112 )
    WHERE @UserId = UserId

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UpdateUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UpdateUser]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @Email                nvarchar(256),
    @Comment              ntext,
    @IsApproved           bit,
    @LastLoginDate        datetime,
    @LastActivityDate     datetime,
    @UniqueEmail          int,
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId, @ApplicationId = a.ApplicationId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)

    IF (@UniqueEmail = 1)
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership WITH (UPDLOCK, HOLDLOCK)
                    WHERE ApplicationId = @ApplicationId  AND @UserId <> UserId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            RETURN(7)
        END
    END

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    UPDATE dbo.aspnet_Users WITH (ROWLOCK)
    SET
         LastActivityDate = @LastActivityDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    UPDATE dbo.aspnet_Membership WITH (ROWLOCK)
    SET
         Email            = @Email,
         LoweredEmail     = LOWER(@Email),
         Comment          = @Comment,
         IsApproved       = @IsApproved,
         LastLoginDate    = @LastLoginDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN -1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UpdateUserInfo]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UpdateUserInfo]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @IsPasswordCorrect              bit,
    @UpdateLastLoginActivityDate    bit,
    @MaxInvalidPasswordAttempts     int,
    @PasswordAttemptWindow          int,
    @CurrentTimeUtc                 datetime,
    @LastLoginDate                  datetime,
    @LastActivityDate               datetime
AS
BEGIN
    DECLARE @UserId                                 uniqueidentifier
    DECLARE @IsApproved                             bit
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId,
            @IsApproved = m.IsApproved,
            @IsLockedOut = m.IsLockedOut,
            @LastLockoutDate = m.LastLockoutDate,
            @FailedPasswordAttemptCount = m.FailedPasswordAttemptCount,
            @FailedPasswordAttemptWindowStart = m.FailedPasswordAttemptWindowStart,
            @FailedPasswordAnswerAttemptCount = m.FailedPasswordAnswerAttemptCount,
            @FailedPasswordAnswerAttemptWindowStart = m.FailedPasswordAnswerAttemptWindowStart
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m WITH ( UPDLOCK )
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF ( @@rowcount = 0 )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    IF( @IsLockedOut = 1 )
    BEGIN
        GOTO Cleanup
    END

    IF( @IsPasswordCorrect = 0 )
    BEGIN
        IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAttemptWindowStart ) )
        BEGIN
            SET @FailedPasswordAttemptWindowStart = @CurrentTimeUtc
            SET @FailedPasswordAttemptCount = 1
        END
        ELSE
        BEGIN
            SET @FailedPasswordAttemptWindowStart = @CurrentTimeUtc
            SET @FailedPasswordAttemptCount = @FailedPasswordAttemptCount + 1
        END

        BEGIN
            IF( @FailedPasswordAttemptCount >= @MaxInvalidPasswordAttempts )
            BEGIN
                SET @IsLockedOut = 1
                SET @LastLockoutDate = @CurrentTimeUtc
            END
        END
    END
    ELSE
    BEGIN
        IF( @FailedPasswordAttemptCount > 0 OR @FailedPasswordAnswerAttemptCount > 0 )
        BEGIN
            SET @FailedPasswordAttemptCount = 0
            SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            SET @FailedPasswordAnswerAttemptCount = 0
            SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )
        END
    END

    IF( @UpdateLastLoginActivityDate = 1 )
    BEGIN
        UPDATE  dbo.aspnet_Users
        SET     LastActivityDate = @LastActivityDate
        WHERE   @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END

        UPDATE  dbo.aspnet_Membership
        SET     LastLoginDate = @LastLoginDate
        WHERE   UserId = @UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END


    UPDATE dbo.aspnet_Membership
    SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
        FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
        FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
        FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
        FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
    WHERE @UserId = UserId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Paths_CreatePath]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Paths_CreatePath]
    @ApplicationId UNIQUEIDENTIFIER,
    @Path           NVARCHAR(256),
    @PathId         UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    BEGIN TRANSACTION
    IF (NOT EXISTS(SELECT * FROM dbo.aspnet_Paths WHERE LoweredPath = LOWER(@Path) AND ApplicationId = @ApplicationId))
    BEGIN
        INSERT dbo.aspnet_Paths (ApplicationId, Path, LoweredPath) VALUES (@ApplicationId, @Path, LOWER(@Path))
    END
    COMMIT TRANSACTION
    SELECT @PathId = PathId FROM dbo.aspnet_Paths WHERE LOWER(@Path) = LoweredPath AND ApplicationId = @ApplicationId
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Personalization_GetApplicationId]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Personalization_GetApplicationId] (
    @ApplicationName NVARCHAR(256),
    @ApplicationId UNIQUEIDENTIFIER OUT)
AS
BEGIN
    SELECT @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_DeleteAllState]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_DeleteAllState] (
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @Count int OUT)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        IF (@AllUsersScope = 1)
            DELETE FROM aspnet_PersonalizationAllUsers
            WHERE PathId IN
               (SELECT Paths.PathId
                FROM dbo.aspnet_Paths Paths
                WHERE Paths.ApplicationId = @ApplicationId)
        ELSE
            DELETE FROM aspnet_PersonalizationPerUser
            WHERE PathId IN
               (SELECT Paths.PathId
                FROM dbo.aspnet_Paths Paths
                WHERE Paths.ApplicationId = @ApplicationId)

        SELECT @Count = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_FindState]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_FindState] (
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @PageIndex              INT,
    @PageSize               INT,
    @Path NVARCHAR(256) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @InactiveSinceDate DATETIME = NULL)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        RETURN

    -- Set the page bounds
    DECLARE @PageLowerBound INT
    DECLARE @PageUpperBound INT
    DECLARE @TotalRecords   INT
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table to store the selected results
    CREATE TABLE #PageIndex (
        IndexId int IDENTITY (0, 1) NOT NULL,
        ItemId UNIQUEIDENTIFIER
    )

    IF (@AllUsersScope = 1)
    BEGIN
        -- Insert into our temp table
        INSERT INTO #PageIndex (ItemId)
        SELECT Paths.PathId
        FROM dbo.aspnet_Paths Paths,
             ((SELECT Paths.PathId
               FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
               WHERE Paths.ApplicationId = @ApplicationId
                      AND AllUsers.PathId = Paths.PathId
                      AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              ) AS SharedDataPerPath
              FULL OUTER JOIN
              (SELECT DISTINCT Paths.PathId
               FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Paths Paths
               WHERE Paths.ApplicationId = @ApplicationId
                      AND PerUser.PathId = Paths.PathId
                      AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              ) AS UserDataPerPath
              ON SharedDataPerPath.PathId = UserDataPerPath.PathId
             )
        WHERE Paths.PathId = SharedDataPerPath.PathId OR Paths.PathId = UserDataPerPath.PathId
        ORDER BY Paths.Path ASC

        SELECT @TotalRecords = @@ROWCOUNT

        SELECT Paths.Path,
               SharedDataPerPath.LastUpdatedDate,
               SharedDataPerPath.SharedDataLength,
               UserDataPerPath.UserDataLength,
               UserDataPerPath.UserCount
        FROM dbo.aspnet_Paths Paths,
             ((SELECT PageIndex.ItemId AS PathId,
                      AllUsers.LastUpdatedDate AS LastUpdatedDate,
                      DATALENGTH(AllUsers.PageSettings) AS SharedDataLength
               FROM dbo.aspnet_PersonalizationAllUsers AllUsers, #PageIndex PageIndex
               WHERE AllUsers.PathId = PageIndex.ItemId
                     AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
              ) AS SharedDataPerPath
              FULL OUTER JOIN
              (SELECT PageIndex.ItemId AS PathId,
                      SUM(DATALENGTH(PerUser.PageSettings)) AS UserDataLength,
                      COUNT(*) AS UserCount
               FROM aspnet_PersonalizationPerUser PerUser, #PageIndex PageIndex
               WHERE PerUser.PathId = PageIndex.ItemId
                     AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
               GROUP BY PageIndex.ItemId
              ) AS UserDataPerPath
              ON SharedDataPerPath.PathId = UserDataPerPath.PathId
             )
        WHERE Paths.PathId = SharedDataPerPath.PathId OR Paths.PathId = UserDataPerPath.PathId
        ORDER BY Paths.Path ASC
    END
    ELSE
    BEGIN
        -- Insert into our temp table
        INSERT INTO #PageIndex (ItemId)
        SELECT PerUser.Id
        FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
        WHERE Paths.ApplicationId = @ApplicationId
              AND PerUser.UserId = Users.UserId
              AND PerUser.PathId = Paths.PathId
              AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              AND (@UserName IS NULL OR Users.LoweredUserName LIKE LOWER(@UserName))
              AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
        ORDER BY Paths.Path ASC, Users.UserName ASC

        SELECT @TotalRecords = @@ROWCOUNT

        SELECT Paths.Path, PerUser.LastUpdatedDate, DATALENGTH(PerUser.PageSettings), Users.UserName, Users.LastActivityDate
        FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths, #PageIndex PageIndex
        WHERE PerUser.Id = PageIndex.ItemId
              AND PerUser.UserId = Users.UserId
              AND PerUser.PathId = Paths.PathId
              AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
        ORDER BY Paths.Path ASC, Users.UserName ASC
    END

    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_GetCountOfState]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_GetCountOfState] (
    @Count int OUT,
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @Path NVARCHAR(256) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @InactiveSinceDate DATETIME = NULL)
AS
BEGIN

    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
        IF (@AllUsersScope = 1)
            SELECT @Count = COUNT(*)
            FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
            WHERE Paths.ApplicationId = @ApplicationId
                  AND AllUsers.PathId = Paths.PathId
                  AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
        ELSE
            SELECT @Count = COUNT(*)
            FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
            WHERE Paths.ApplicationId = @ApplicationId
                  AND PerUser.UserId = Users.UserId
                  AND PerUser.PathId = Paths.PathId
                  AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
                  AND (@UserName IS NULL OR Users.LoweredUserName LIKE LOWER(@UserName))
                  AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_ResetSharedState]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_ResetSharedState] (
    @Count int OUT,
    @ApplicationName NVARCHAR(256),
    @Path NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationAllUsers
        WHERE PathId IN
            (SELECT AllUsers.PathId
             FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
             WHERE Paths.ApplicationId = @ApplicationId
                   AND AllUsers.PathId = Paths.PathId
                   AND Paths.LoweredPath = LOWER(@Path))

        SELECT @Count = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_ResetUserState]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_ResetUserState] (
    @Count                  int                 OUT,
    @ApplicationName        NVARCHAR(256),
    @InactiveSinceDate      DATETIME            = NULL,
    @UserName               NVARCHAR(256)       = NULL,
    @Path                   NVARCHAR(256)       = NULL)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser
        WHERE Id IN (SELECT PerUser.Id
                     FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
                     WHERE Paths.ApplicationId = @ApplicationId
                           AND PerUser.UserId = Users.UserId
                           AND PerUser.PathId = Paths.PathId
                           AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
                           AND (@UserName IS NULL OR Users.LoweredUserName = LOWER(@UserName))
                           AND (@Path IS NULL OR Paths.LoweredPath = LOWER(@Path)))

        SELECT @Count = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @Path              NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT p.PageSettings FROM dbo.aspnet_PersonalizationAllUsers p WHERE p.PathId = @PathId
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @Path              NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    DELETE FROM dbo.aspnet_PersonalizationAllUsers WHERE PathId = @PathId
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @Path             NVARCHAR(256),
    @PageSettings     IMAGE,
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Paths_CreatePath @ApplicationId, @Path, @PathId OUTPUT
    END

    IF (EXISTS(SELECT PathId FROM dbo.aspnet_PersonalizationAllUsers WHERE PathId = @PathId))
        UPDATE dbo.aspnet_PersonalizationAllUsers SET PageSettings = @PageSettings, LastUpdatedDate = @CurrentTimeUtc WHERE PathId = @PathId
    ELSE
        INSERT INTO dbo.aspnet_PersonalizationAllUsers(PathId, PageSettings, LastUpdatedDate) VALUES (@PathId, @PageSettings, @CurrentTimeUtc)
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_GetPageSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_GetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        RETURN
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    SELECT p.PageSettings FROM dbo.aspnet_PersonalizationPerUser p WHERE p.PathId = @PathId AND p.UserId = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        RETURN
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE PathId = @PathId AND UserId = @UserId
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_SetPageSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_SetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @PageSettings     IMAGE,
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Paths_CreatePath @ApplicationId, @Path, @PathId OUTPUT
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CurrentTimeUtc, @UserId OUTPUT
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    IF (EXISTS(SELECT PathId FROM dbo.aspnet_PersonalizationPerUser WHERE UserId = @UserId AND PathId = @PathId))
        UPDATE dbo.aspnet_PersonalizationPerUser SET PageSettings = @PageSettings, LastUpdatedDate = @CurrentTimeUtc WHERE UserId = @UserId AND PathId = @PathId
    ELSE
        INSERT INTO dbo.aspnet_PersonalizationPerUser(UserId, PathId, PageSettings, LastUpdatedDate) VALUES (@UserId, @PathId, @PageSettings, @CurrentTimeUtc)
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_DeleteInactiveProfiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_DeleteInactiveProfiles]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @InactiveSinceDate      datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT  0
        RETURN
    END

    DELETE
    FROM    dbo.aspnet_Profile
    WHERE   UserId IN
            (   SELECT  UserId
                FROM    dbo.aspnet_Users u
                WHERE   ApplicationId = @ApplicationId
                        AND (LastActivityDate <= @InactiveSinceDate)
                        AND (
                                (@ProfileAuthOptions = 2)
                             OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                             OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                            )
            )

    SELECT  @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_DeleteProfiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_DeleteProfiles]
    @ApplicationName        nvarchar(256),
    @UserNames              nvarchar(4000)
AS
BEGIN
    DECLARE @UserName     nvarchar(256)
    DECLARE @CurrentPos   int
    DECLARE @NextPos      int
    DECLARE @NumDeleted   int
    DECLARE @DeletedUser  int
    DECLARE @TranStarted  bit
    DECLARE @ErrorCode    int

    SET @ErrorCode = 0
    SET @CurrentPos = 1
    SET @NumDeleted = 0
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    WHILE (@CurrentPos <= LEN(@UserNames))
    BEGIN
        SELECT @NextPos = CHARINDEX(N',', @UserNames,  @CurrentPos)
        IF (@NextPos = 0 OR @NextPos IS NULL)
            SELECT @NextPos = LEN(@UserNames) + 1

        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPos, @NextPos - @CurrentPos)
        SELECT @CurrentPos = @NextPos+1

        IF (LEN(@UserName) > 0)
        BEGIN
            SELECT @DeletedUser = 0
            EXEC dbo.aspnet_Users_DeleteUser @ApplicationName, @UserName, 4, @DeletedUser OUTPUT
            IF( @@ERROR <> 0 )
            BEGIN
                SET @ErrorCode = -1
                GOTO Cleanup
            END
            IF (@DeletedUser <> 0)
                SELECT @NumDeleted = @NumDeleted + 1
        END
    END
    SELECT @NumDeleted
    IF (@TranStarted = 1)
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END
    SET @TranStarted = 0

    RETURN 0

Cleanup:
    IF (@TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END
    RETURN @ErrorCode
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @InactiveSinceDate      datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT 0
        RETURN
    END

    SELECT  COUNT(*)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
    WHERE   ApplicationId = @ApplicationId
        AND u.UserId = p.UserId
        AND (LastActivityDate <= @InactiveSinceDate)
        AND (
                (@ProfileAuthOptions = 2)
                OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
            )
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetProfiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_GetProfiles]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @PageIndex              int,
    @PageSize               int,
    @UserNameToMatch        nvarchar(256) = NULL,
    @InactiveSinceDate      datetime      = NULL
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT  u.UserId
        FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
        WHERE   ApplicationId = @ApplicationId
            AND u.UserId = p.UserId
            AND (@InactiveSinceDate IS NULL OR LastActivityDate <= @InactiveSinceDate)
            AND (     (@ProfileAuthOptions = 2)
                   OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                   OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                 )
            AND (@UserNameToMatch IS NULL OR LoweredUserName LIKE LOWER(@UserNameToMatch))
        ORDER BY UserName

    SELECT  u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate,
            DATALENGTH(p.PropertyNames) + DATALENGTH(p.PropertyValuesString) + DATALENGTH(p.PropertyValuesBinary)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p, #PageIndexForUsers i
    WHERE   u.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= @PageLowerBound AND i.IndexId <= @PageUpperBound

    SELECT COUNT(*)
    FROM   #PageIndexForUsers

    DROP TABLE #PageIndexForUsers
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetProperties]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_GetProperties]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)

    IF (@UserId IS NULL)
        RETURN
    SELECT TOP 1 PropertyNames, PropertyValuesString, PropertyValuesBinary
    FROM         dbo.aspnet_Profile
    WHERE        UserId = @UserId

    IF (@@ROWCOUNT > 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate=@CurrentTimeUtc
        WHERE  UserId = @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_SetProperties]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_SetProperties]
    @ApplicationName        nvarchar(256),
    @PropertyNames          ntext,
    @PropertyValuesString   ntext,
    @PropertyValuesBinary   image,
    @UserName               nvarchar(256),
    @IsUserAnonymous        bit,
    @CurrentTimeUtc         datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
       BEGIN TRANSACTION
       SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DECLARE @UserId uniqueidentifier
    DECLARE @LastActivityDate datetime
    SELECT  @UserId = NULL
    SELECT  @LastActivityDate = @CurrentTimeUtc

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
        EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, @IsUserAnonymous, @LastActivityDate, @UserId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Users
    SET    LastActivityDate=@CurrentTimeUtc
    WHERE  UserId = @UserId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS( SELECT *
               FROM   dbo.aspnet_Profile
               WHERE  UserId = @UserId))
        UPDATE dbo.aspnet_Profile
        SET    PropertyNames=@PropertyNames, PropertyValuesString = @PropertyValuesString,
               PropertyValuesBinary = @PropertyValuesBinary, LastUpdatedDate=@CurrentTimeUtc
        WHERE  UserId = @UserId
    ELSE
        INSERT INTO dbo.aspnet_Profile(UserId, PropertyNames, PropertyValuesString, PropertyValuesBinary, LastUpdatedDate)
             VALUES (@UserId, @PropertyNames, @PropertyValuesString, @PropertyValuesBinary, @CurrentTimeUtc)

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_RegisterSchemaVersion]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_RegisterSchemaVersion]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128),
    @IsCurrentVersion          bit,
    @RemoveIncompatibleSchema  bit
AS
BEGIN
    IF( @RemoveIncompatibleSchema = 1 )
    BEGIN
        DELETE FROM dbo.aspnet_SchemaVersions WHERE Feature = LOWER( @Feature )
    END
    ELSE
    BEGIN
        IF( @IsCurrentVersion = 1 )
        BEGIN
            UPDATE dbo.aspnet_SchemaVersions
            SET IsCurrentVersion = 0
            WHERE Feature = LOWER( @Feature )
        END
    END

    INSERT  dbo.aspnet_SchemaVersions( Feature, CompatibleSchemaVersion, IsCurrentVersion )
    VALUES( LOWER( @Feature ), @CompatibleSchemaVersion, @IsCurrentVersion )
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_CreateRole]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Roles_CreateRole]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS(SELECT RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId))
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    INSERT INTO dbo.aspnet_Roles
                (ApplicationId, RoleName, LoweredRoleName)
         VALUES (@ApplicationId, @RoleName, LOWER(@RoleName))

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_DeleteRole]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Roles_DeleteRole]
    @ApplicationName            nvarchar(256),
    @RoleName                   nvarchar(256),
    @DeleteOnlyIfRoleIsEmpty    bit
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    DECLARE @RoleId   uniqueidentifier
    SELECT  @RoleId = NULL
    SELECT  @RoleId = RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
    BEGIN
        SELECT @ErrorCode = 1
        GOTO Cleanup
    END
    IF (@DeleteOnlyIfRoleIsEmpty <> 0)
    BEGIN
        IF (EXISTS (SELECT RoleId FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId))
        BEGIN
            SELECT @ErrorCode = 2
            GOTO Cleanup
        END
    END


    DELETE FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DELETE FROM dbo.aspnet_Roles WHERE @RoleId = RoleId  AND ApplicationId = @ApplicationId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_GetAllRoles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Roles_GetAllRoles] (
    @ApplicationName           nvarchar(256))
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN
    SELECT RoleName
    FROM   dbo.aspnet_Roles WHERE ApplicationId = @ApplicationId
    ORDER BY RoleName
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_RoleExists]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Roles_RoleExists]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(0)
    IF (EXISTS (SELECT RoleName FROM dbo.aspnet_Roles WHERE LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId ))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Setup_RemoveAllRoleMembers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Setup_RemoveAllRoleMembers]
    @name   sysname
AS
BEGIN
    CREATE TABLE #aspnet_RoleMembers
    (
        Group_name      sysname,
        Group_id        smallint,
        Users_in_group  sysname,
        User_id         smallint
    )

    INSERT INTO #aspnet_RoleMembers
    EXEC sp_helpuser @name

    DECLARE @user_id smallint
    DECLARE @cmd nvarchar(500)
    DECLARE c1 cursor FORWARD_ONLY FOR
        SELECT User_id FROM #aspnet_RoleMembers

    OPEN c1

    FETCH c1 INTO @user_id
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = 'EXEC sp_droprolemember ' + '''' + @name + ''', ''' + USER_NAME(@user_id) + ''''
        EXEC (@cmd)
        FETCH c1 INTO @user_id
    END

    CLOSE c1
    DEALLOCATE c1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Setup_RestorePermissions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Setup_RestorePermissions]
    @name   sysname
AS
BEGIN
    DECLARE @object sysname
    DECLARE @protectType char(10)
    DECLARE @action varchar(60)
    DECLARE @grantee sysname
    DECLARE @cmd nvarchar(500)
    DECLARE c1 cursor FORWARD_ONLY FOR
        SELECT Object, ProtectType, [Action], Grantee FROM #aspnet_Permissions where Object = @name

    OPEN c1

    FETCH c1 INTO @object, @protectType, @action, @grantee
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = @protectType + ' ' + @action + ' on ' + @object + ' TO [' + @grantee + ']'
        EXEC (@cmd)
        FETCH c1 INTO @object, @protectType, @action, @grantee
    END

    CLOSE c1
    DEALLOCATE c1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UnRegisterSchemaVersion]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UnRegisterSchemaVersion]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128)
AS
BEGIN
    DELETE FROM dbo.aspnet_SchemaVersions
        WHERE   Feature = LOWER(@Feature) AND @CompatibleSchemaVersion = CompatibleSchemaVersion
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Users_CreateUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Users_CreateUser]
    @ApplicationId    uniqueidentifier,
    @UserName         nvarchar(256),
    @IsUserAnonymous  bit,
    @LastActivityDate DATETIME,
    @UserId           uniqueidentifier OUTPUT
AS
BEGIN
    IF( @UserId IS NULL )
        SELECT @UserId = NEWID()
    ELSE
    BEGIN
        IF( EXISTS( SELECT UserId FROM dbo.aspnet_Users
                    WHERE @UserId = UserId ) )
            RETURN -1
    END

    INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
    VALUES (@ApplicationId, @UserId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate)

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Users_DeleteUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Users_DeleteUser]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @TablesToDeleteFrom int,
    @NumTablesDeletedFrom int OUTPUT
AS
BEGIN
    DECLARE @UserId               uniqueidentifier
    SELECT  @UserId               = NULL
    SELECT  @NumTablesDeletedFrom = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    DECLARE @ErrorCode   int
    DECLARE @RowCount    int

    SET @ErrorCode = 0
    SET @RowCount  = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   u.LoweredUserName       = LOWER(@UserName)
        AND u.ApplicationId         = a.ApplicationId
        AND LOWER(@ApplicationName) = a.LoweredApplicationName

    IF (@UserId IS NULL)
    BEGIN
        GOTO Cleanup
    END

    -- Delete from Membership table if (@TablesToDeleteFrom & 1) is set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
    BEGIN
        DELETE FROM dbo.aspnet_Membership WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
               @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
    IF ((@TablesToDeleteFrom & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_UsersInRoles') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
    IF ((@TablesToDeleteFrom & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_Profile WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
    IF ((@TablesToDeleteFrom & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (@TablesToDeleteFrom & 2) <> 0 AND
        (@TablesToDeleteFrom & 4) <> 0 AND
        (@TablesToDeleteFrom & 8) <> 0 AND
        (EXISTS (SELECT UserId FROM dbo.aspnet_Users WHERE @UserId = UserId)))
    BEGIN
        DELETE FROM dbo.aspnet_Users WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:
    SET @NumTablesDeletedFrom = 0

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
	    ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_AddUsersToRoles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_AddUsersToRoles]
	@ApplicationName  nvarchar(256),
	@UserNames		  nvarchar(4000),
	@RoleNames		  nvarchar(4000),
	@CurrentTimeUtc   datetime
AS
BEGIN
	DECLARE @AppId uniqueidentifier
	SELECT  @AppId = NULL
	SELECT  @AppId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
	IF (@AppId IS NULL)
		RETURN(2)
	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	DECLARE @tbNames	table(Name nvarchar(256) NOT NULL PRIMARY KEY)
	DECLARE @tbRoles	table(RoleId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @tbUsers	table(UserId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @Num		int
	DECLARE @Pos		int
	DECLARE @NextPos	int
	DECLARE @Name		nvarchar(256)

	SET @Num = 0
	SET @Pos = 1
	WHILE(@Pos <= LEN(@RoleNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @RoleNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@RoleNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@RoleNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbRoles
	  SELECT RoleId
	  FROM   dbo.aspnet_Roles ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredRoleName AND ar.ApplicationId = @AppId

	IF (@@ROWCOUNT <> @Num)
	BEGIN
		SELECT TOP 1 Name
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT ar.LoweredRoleName FROM dbo.aspnet_Roles ar,  @tbRoles r WHERE r.RoleId = ar.RoleId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(2)
	END

	DELETE FROM @tbNames WHERE 1=1
	SET @Num = 0
	SET @Pos = 1

	WHILE(@Pos <= LEN(@UserNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @UserNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@UserNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@UserNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbUsers
	  SELECT UserId
	  FROM   dbo.aspnet_Users ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredUserName AND ar.ApplicationId = @AppId

	IF (@@ROWCOUNT <> @Num)
	BEGIN
		DELETE FROM @tbNames
		WHERE LOWER(Name) IN (SELECT LoweredUserName FROM dbo.aspnet_Users au,  @tbUsers u WHERE au.UserId = u.UserId)

		INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
		  SELECT @AppId, NEWID(), Name, LOWER(Name), 0, @CurrentTimeUtc
		  FROM   @tbNames

		INSERT INTO @tbUsers
		  SELECT  UserId
		  FROM	dbo.aspnet_Users au, @tbNames t
		  WHERE   LOWER(t.Name) = au.LoweredUserName AND au.ApplicationId = @AppId
	END

	IF (EXISTS (SELECT * FROM dbo.aspnet_UsersInRoles ur, @tbUsers tu, @tbRoles tr WHERE tu.UserId = ur.UserId AND tr.RoleId = ur.RoleId))
	BEGIN
		SELECT TOP 1 UserName, RoleName
		FROM		 dbo.aspnet_UsersInRoles ur, @tbUsers tu, @tbRoles tr, aspnet_Users u, aspnet_Roles r
		WHERE		u.UserId = tu.UserId AND r.RoleId = tr.RoleId AND tu.UserId = ur.UserId AND tr.RoleId = ur.RoleId

		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(3)
	END

	INSERT INTO dbo.aspnet_UsersInRoles (UserId, RoleId)
	SELECT UserId, RoleId
	FROM @tbUsers, @tbRoles

	IF( @TranStarted = 1 )
		COMMIT TRANSACTION
	RETURN(0)
END                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_FindUsersInRole]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_FindUsersInRole]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256),
    @UserNameToMatch  nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId uniqueidentifier
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId AND LoweredUserName LIKE LOWER(@UserNameToMatch)
    ORDER BY u.UserName
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_GetRolesForUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_GetRolesForUser]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(1)

    SELECT r.RoleName
    FROM   dbo.aspnet_Roles r, dbo.aspnet_UsersInRoles ur
    WHERE  r.RoleId = ur.RoleId AND r.ApplicationId = @ApplicationId AND ur.UserId = @UserId
    ORDER BY r.RoleName
    RETURN (0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_GetUsersInRoles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_GetUsersInRoles]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId uniqueidentifier
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId
    ORDER BY u.UserName
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_IsUserInRole]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_IsUserInRole]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(2)
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    DECLARE @RoleId uniqueidentifier
    SELECT  @RoleId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(2)

    SELECT  @RoleId = RoleId
    FROM    dbo.aspnet_Roles
    WHERE   LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
        RETURN(3)

    IF (EXISTS( SELECT * FROM dbo.aspnet_UsersInRoles WHERE  UserId = @UserId AND RoleId = @RoleId))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles]
	@ApplicationName  nvarchar(256),
	@UserNames		  nvarchar(4000),
	@RoleNames		  nvarchar(4000)
AS
BEGIN
	DECLARE @AppId uniqueidentifier
	SELECT  @AppId = NULL
	SELECT  @AppId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
	IF (@AppId IS NULL)
		RETURN(2)


	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	DECLARE @tbNames  table(Name nvarchar(256) NOT NULL PRIMARY KEY)
	DECLARE @tbRoles  table(RoleId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @tbUsers  table(UserId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @Num	  int
	DECLARE @Pos	  int
	DECLARE @NextPos  int
	DECLARE @Name	  nvarchar(256)
	DECLARE @CountAll int
	DECLARE @CountU	  int
	DECLARE @CountR	  int


	SET @Num = 0
	SET @Pos = 1
	WHILE(@Pos <= LEN(@RoleNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @RoleNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@RoleNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@RoleNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbRoles
	  SELECT RoleId
	  FROM   dbo.aspnet_Roles ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredRoleName AND ar.ApplicationId = @AppId
	SELECT @CountR = @@ROWCOUNT

	IF (@CountR <> @Num)
	BEGIN
		SELECT TOP 1 N'', Name
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT ar.LoweredRoleName FROM dbo.aspnet_Roles ar,  @tbRoles r WHERE r.RoleId = ar.RoleId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(2)
	END


	DELETE FROM @tbNames WHERE 1=1
	SET @Num = 0
	SET @Pos = 1


	WHILE(@Pos <= LEN(@UserNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @UserNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@UserNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@UserNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbUsers
	  SELECT UserId
	  FROM   dbo.aspnet_Users ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredUserName AND ar.ApplicationId = @AppId

	SELECT @CountU = @@ROWCOUNT
	IF (@CountU <> @Num)
	BEGIN
		SELECT TOP 1 Name, N''
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT au.LoweredUserName FROM dbo.aspnet_Users au,  @tbUsers u WHERE u.UserId = au.UserId)

		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(1)
	END

	SELECT  @CountAll = COUNT(*)
	FROM	dbo.aspnet_UsersInRoles ur, @tbUsers u, @tbRoles r
	WHERE   ur.UserId = u.UserId AND ur.RoleId = r.RoleId

	IF (@CountAll <> @CountU * @CountR)
	BEGIN
		SELECT TOP 1 UserName, RoleName
		FROM		 @tbUsers tu, @tbRoles tr, dbo.aspnet_Users u, dbo.aspnet_Roles r
		WHERE		 u.UserId = tu.UserId AND r.RoleId = tr.RoleId AND
					 tu.UserId NOT IN (SELECT ur.UserId FROM dbo.aspnet_UsersInRoles ur WHERE ur.RoleId = tr.RoleId) AND
					 tr.RoleId NOT IN (SELECT ur.RoleId FROM dbo.aspnet_UsersInRoles ur WHERE ur.UserId = tu.UserId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(3)
	END

	DELETE FROM dbo.aspnet_UsersInRoles
	WHERE UserId IN (SELECT UserId FROM @tbUsers)
	  AND RoleId IN (SELECT RoleId FROM @tbRoles)
	IF( @TranStarted = 1 )
		COMMIT TRANSACTION
	RETURN(0)
END
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
GO
/****** Object:  StoredProcedure [dbo].[aspnet_WebEvent_LogEvent]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_WebEvent_LogEvent]
        @EventId         char(32),
        @EventTimeUtc    datetime,
        @EventTime       datetime,
        @EventType       nvarchar(256),
        @EventSequence   decimal(19,0),
        @EventOccurrence decimal(19,0),
        @EventCode       int,
        @EventDetailCode int,
        @Message         nvarchar(1024),
        @ApplicationPath nvarchar(256),
        @ApplicationVirtualPath nvarchar(256),
        @MachineName    nvarchar(256),
        @RequestUrl      nvarchar(1024),
        @ExceptionType   nvarchar(256),
        @Details         ntext
AS
BEGIN
    INSERT
        dbo.aspnet_WebEvent_Events
        (
            EventId,
            EventTimeUtc,
            EventTime,
            EventType,
            EventSequence,
            EventOccurrence,
            EventCode,
            EventDetailCode,
            Message,
            ApplicationPath,
            ApplicationVirtualPath,
            MachineName,
            RequestUrl,
            ExceptionType,
            Details
        )
    VALUES
    (
        @EventId,
        @EventTimeUtc,
        @EventTime,
        @EventType,
        @EventSequence,
        @EventOccurrence,
        @EventCode,
        @EventDetailCode,
        @Message,
        @ApplicationPath,
        @ApplicationVirtualPath,
        @MachineName,
        @RequestUrl,
        @ExceptionType,
        @Details
    )
END
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AddressFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddressFields](
	[AddressFieldID] [uniqueidentifier] NOT NULL,
	[FieldID] [uniqueidentifier] NOT NULL,
	[AddressString] [nvarchar](max) NULL,
	[Latitude] [decimal](18, 8) NULL,
	[Longitude] [decimal](18, 8) NULL,
	[LastGeocode] [datetime2](7) NULL,
	[Status] [tinyint] NOT NULL,
	[Geo] [geography] NULL,
 CONSTRAINT [PK_dbo.AddressFields] PRIMARY KEY CLUSTERED 
(
	[AddressFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdministrationRights]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdministrationRights](
	[AdministrationRightID] [uniqueidentifier] NOT NULL,
	[AdministrationRightsGroupID] [uniqueidentifier] NOT NULL,
	[UserActionID] [uniqueidentifier] NULL,
	[AllowUserToMakeImmediateChanges] [bit] NOT NULL,
	[AllowUserToMakeProposals] [bit] NOT NULL,
	[AllowUserToSeekRewards] [bit] NOT NULL,
	[AllowUserNotToSeekRewards] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.AdministrationRights] PRIMARY KEY CLUSTERED 
(
	[AdministrationRightID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdministrationRightsGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdministrationRightsGroups](
	[AdministrationRightsGroupID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.AdministrationRightsGroups] PRIMARY KEY CLUSTERED 
(
	[AdministrationRightsGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Applications]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Applications](
	[ApplicationName] [nvarchar](256) NOT NULL,
	[LoweredApplicationName] [nvarchar](256) NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Description] [nvarchar](256) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[LoweredApplicationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ApplicationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Membership]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Membership](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordFormat] [int] NOT NULL DEFAULT ((0)),
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[MobilePIN] [nvarchar](16) NULL,
	[Email] [nvarchar](256) NULL,
	[LoweredEmail] [nvarchar](256) NULL,
	[PasswordQuestion] [nvarchar](256) NULL,
	[PasswordAnswer] [nvarchar](128) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[LastPasswordChangedDate] [datetime] NOT NULL,
	[LastLockoutDate] [datetime] NOT NULL,
	[FailedPasswordAttemptCount] [int] NOT NULL,
	[FailedPasswordAttemptWindowStart] [datetime] NOT NULL,
	[FailedPasswordAnswerAttemptCount] [int] NOT NULL,
	[FailedPasswordAnswerAttemptWindowStart] [datetime] NOT NULL,
	[Comment] [ntext] NULL,
PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Paths]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Paths](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[PathId] [uniqueidentifier] NOT NULL,
	[Path] [nvarchar](256) NOT NULL,
	[LoweredPath] [nvarchar](256) NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[PathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_PersonalizationAllUsers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_PersonalizationAllUsers](
	[PathId] [uniqueidentifier] NOT NULL,
	[PageSettings] [image] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_PersonalizationPerUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_PersonalizationPerUser](
	[Id] [uniqueidentifier] NOT NULL,
	[PathId] [uniqueidentifier] NULL,
	[UserId] [uniqueidentifier] NULL,
	[PageSettings] [image] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Profile]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Profile](
	[UserId] [uniqueidentifier] NOT NULL,
	[PropertyNames] [ntext] NOT NULL,
	[PropertyValuesString] [ntext] NOT NULL,
	[PropertyValuesBinary] [image] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Roles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Roles](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[LoweredRoleName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](256) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_SchemaVersions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_SchemaVersions](
	[Feature] [nvarchar](128) NOT NULL,
	[CompatibleSchemaVersion] [nvarchar](128) NOT NULL,
	[IsCurrentVersion] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Feature] ASC,
	[CompatibleSchemaVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Users]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Users](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[UserName] [nvarchar](256) NOT NULL,
	[LoweredUserName] [nvarchar](256) NOT NULL,
	[MobileAlias] [nvarchar](16) NULL DEFAULT (NULL),
	[IsAnonymous] [bit] NOT NULL DEFAULT ((0)),
	[LastActivityDate] [datetime] NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_UsersInRoles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_UsersInRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_WebEvent_Events]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[aspnet_WebEvent_Events](
	[EventId] [char](32) NOT NULL,
	[EventTimeUtc] [datetime] NOT NULL,
	[EventTime] [datetime] NOT NULL,
	[EventType] [nvarchar](256) NOT NULL,
	[EventSequence] [decimal](19, 0) NOT NULL,
	[EventOccurrence] [decimal](19, 0) NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventDetailCode] [int] NOT NULL,
	[Message] [nvarchar](1024) NULL,
	[ApplicationPath] [nvarchar](256) NULL,
	[ApplicationVirtualPath] [nvarchar](256) NULL,
	[MachineName] [nvarchar](256) NOT NULL,
	[RequestUrl] [nvarchar](1024) NULL,
	[ExceptionType] [nvarchar](256) NULL,
	[Details] [ntext] NULL,
PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ChangesGroup]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChangesGroup](
	[ChangesGroupID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[TblID] [uniqueidentifier] NULL,
	[Creator] [uniqueidentifier] NULL,
	[MakeChangeRatingID] [uniqueidentifier] NULL,
	[RewardRatingID] [uniqueidentifier] NULL,
	[StatusOfChanges] [tinyint] NOT NULL,
	[ScheduleApprovalOrRejection] [datetime2](7) NULL,
	[ScheduleImplementation] [datetime2](7) NULL,
 CONSTRAINT [PK_dbo.ChangesGroup] PRIMARY KEY CLUSTERED 
(
	[ChangesGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChangesStatusOfObject]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChangesStatusOfObject](
	[ChangesStatusOfObjectID] [uniqueidentifier] NOT NULL,
	[ChangesGroupID] [uniqueidentifier] NOT NULL,
	[ObjectType] [tinyint] NOT NULL,
	[AddObject] [bit] NOT NULL,
	[DeleteObject] [bit] NOT NULL,
	[ReplaceObject] [bit] NOT NULL,
	[ChangeName] [bit] NOT NULL,
	[ChangeOther] [bit] NOT NULL,
	[ChangeSetting1] [bit] NOT NULL,
	[ChangeSetting2] [bit] NOT NULL,
	[MayAffectRunningRating] [bit] NOT NULL,
	[NewName] [nvarchar](50) NULL,
	[NewObject] [uniqueidentifier] NULL,
	[ExistingObject] [uniqueidentifier] NULL,
	[NewValueBoolean] [bit] NULL,
	[NewValueInteger] [int] NULL,
	[NewValueGuid] [uniqueidentifier] NULL,
	[NewValueDecimal] [decimal](18, 4) NULL,
	[NewValueText] [nvarchar](max) NULL,
	[NewValueDateTime] [datetime2](7) NULL,
	[ChangeDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ChangesStatusOfObject] PRIMARY KEY CLUSTERED 
(
	[ChangesStatusOfObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceFields](
	[ChoiceFieldID] [uniqueidentifier] NOT NULL,
	[FieldID] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceFields] PRIMARY KEY CLUSTERED 
(
	[ChoiceFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceGroupFieldDefinitions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceGroupFieldDefinitions](
	[ChoiceGroupFieldDefinitionID] [uniqueidentifier] NOT NULL,
	[ChoiceGroupID] [uniqueidentifier] NOT NULL,
	[FieldDefinitionID] [uniqueidentifier] NOT NULL,
	[DependentOnChoiceGroupFieldDefinitionID] [uniqueidentifier] NULL,
	[TrackTrustBasedOnChoices] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceGroupFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[ChoiceGroupFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceGroups](
	[ChoiceGroupID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NOT NULL,
	[AllowMultipleSelections] [bit] NOT NULL,
	[Alphabetize] [bit] NOT NULL,
	[InvisibleWhenEmpty] [bit] NOT NULL,
	[ShowTagCloud] [bit] NOT NULL,
	[PickViaAutoComplete] [bit] NOT NULL,
	[DependentOnChoiceGroupID] [uniqueidentifier] NULL,
	[ShowAllPossibilitiesIfNoDependentChoice] [bit] NOT NULL,
	[AlphabetizeWhenShowingAllPossibilities] [bit] NOT NULL,
	[AllowAutoAddWhenAddingFields] [bit] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceGroups] PRIMARY KEY CLUSTERED 
(
	[ChoiceGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceInFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceInFields](
	[ChoiceInFieldID] [uniqueidentifier] NOT NULL,
	[ChoiceFieldID] [uniqueidentifier] NOT NULL,
	[ChoiceInGroupID] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceInFields] PRIMARY KEY CLUSTERED 
(
	[ChoiceInFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceInGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceInGroups](
	[ChoiceInGroupID] [uniqueidentifier] NOT NULL,
	[ChoiceGroupID] [uniqueidentifier] NOT NULL,
	[ChoiceNum] [int] NOT NULL,
	[ChoiceText] [nvarchar](50) NOT NULL,
	[ActiveOnDeterminingGroupChoiceInGroupID] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceInGroups] PRIMARY KEY CLUSTERED 
(
	[ChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Comments]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[CommentID] [uniqueidentifier] NOT NULL,
	[TblRowID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CommentTitle] [nvarchar](max) NOT NULL,
	[CommentText] [nvarchar](max) NOT NULL,
	[DateTime] [datetime2](7) NOT NULL,
	[LastDeletedDate] [datetime2](7) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Comments] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DatabaseStatus]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseStatus](
	[DatabaseStatusID] [uniqueidentifier] NOT NULL,
	[PreventChanges] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DatabaseStatus] PRIMARY KEY CLUSTERED 
(
	[DatabaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DateTimeFieldDefinitions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateTimeFieldDefinitions](
	[DateTimeFieldDefinitionID] [uniqueidentifier] NOT NULL,
	[FieldDefinitionID] [uniqueidentifier] NOT NULL,
	[IncludeDate] [bit] NOT NULL,
	[IncludeTime] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.DateTimeFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[DateTimeFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DateTimeFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateTimeFields](
	[DateTimeFieldID] [uniqueidentifier] NOT NULL,
	[FieldID] [uniqueidentifier] NOT NULL,
	[DateTime] [datetime2](7) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.DateTimeFields] PRIMARY KEY CLUSTERED 
(
	[DateTimeFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Domains]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Domains](
	[DomainID] [uniqueidentifier] NOT NULL,
	[ActiveRatingWebsite] [bit] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[TblDimensionsID] [uniqueidentifier] NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Domains] PRIMARY KEY CLUSTERED 
(
	[DomainID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FieldDefinitions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldDefinitions](
	[FieldDefinitionID] [uniqueidentifier] NOT NULL,
	[TblID] [uniqueidentifier] NOT NULL,
	[FieldNum] [int] NOT NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[FieldType] [int] NOT NULL,
	[UseAsFilter] [bit] NOT NULL,
	[AddToSearchWords] [bit] NOT NULL,
	[DisplayInTableSettings] [int] NOT NULL,
	[DisplayInPopupSettings] [int] NOT NULL,
	[DisplayInTblRowPageSettings] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[NumNonNull] [int] NOT NULL,
	[ProportionNonNull] [float] NOT NULL,
	[UsingNonSparseColumn] [bit] NOT NULL,
	[ShouldUseNonSparseColumn] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.FieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fields](
	[FieldID] [uniqueidentifier] NOT NULL,
	[TblRowID] [uniqueidentifier] NOT NULL,
	[FieldDefinitionID] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[NotYetAddedToDatabase] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Fields] PRIMARY KEY CLUSTERED 
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumAdministrators]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumAdministrators](
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumAdministrators] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumComplaints]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumComplaints](
	[UserID] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	[ComplainText] [ntext] NULL,
 CONSTRAINT [PK_dbo.ForumComplaints] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumGroupPermissions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumGroupPermissions](
	[ForumID] [int] NOT NULL,
	[GroupID] [int] NOT NULL,
	[AllowReading] [bit] NOT NULL,
	[AllowPosting] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ForumGroupPermissions] PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumGroups](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.ForumGroups] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumMessageRating]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumMessageRating](
	[MessageID] [int] NOT NULL,
	[VoterUserID] [int] NOT NULL,
	[Score] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumMessageRating] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC,
	[VoterUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumMessages]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ForumMessages](
	[MessageID] [int] IDENTITY(1,1) NOT NULL,
	[TopicID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Body] [ntext] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[Visible] [bit] NOT NULL,
	[IPAddress] [varchar](50) NULL,
	[Rating] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumMessages] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ForumModerators]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumModerators](
	[UserID] [int] NOT NULL,
	[ForumID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumModerators] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumNewTopicSubscriptions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumNewTopicSubscriptions](
	[UserID] [int] NOT NULL,
	[ForumID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumNewTopicSubscriptions] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPersonalMessages]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPersonalMessages](
	[MessageID] [int] IDENTITY(1,1) NOT NULL,
	[FromUserID] [int] NOT NULL,
	[ToUserID] [int] NOT NULL,
	[Body] [ntext] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[New] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ForumPersonalMessages] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPollAnswers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPollAnswers](
	[UserID] [int] NOT NULL,
	[OptionID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumPollAnswers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPollOptions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPollOptions](
	[OptionID] [int] IDENTITY(1,1) NOT NULL,
	[PollID] [int] NOT NULL,
	[OptionText] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.ForumPollOptions] PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPolls]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPolls](
	[PollID] [int] IDENTITY(1,1) NOT NULL,
	[TopicID] [int] NOT NULL,
	[Question] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_dbo.ForumPolls] PRIMARY KEY CLUSTERED 
(
	[PollID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Forums]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Forums](
	[ForumID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Premoderated] [bit] NOT NULL,
	[GroupID] [int] NOT NULL,
	[MembersOnly] [bit] NOT NULL,
	[OrderByNumber] [int] NOT NULL,
	[RestrictTopicCreation] [bit] NOT NULL,
	[IconFile] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Forums] PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumSubforums]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumSubforums](
	[ParentForumID] [int] NOT NULL,
	[SubForumID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumSubforums] PRIMARY KEY CLUSTERED 
(
	[ParentForumID] ASC,
	[SubForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumSubscriptions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumSubscriptions](
	[UserID] [int] NOT NULL,
	[TopicID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumSubscriptions] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[TopicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumTopics]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumTopics](
	[TopicID] [int] IDENTITY(1,1) NOT NULL,
	[ForumID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[Visible] [bit] NOT NULL,
	[LastMessageID] [int] NOT NULL,
	[IsSticky] [int] NOT NULL,
	[IsClosed] [bit] NOT NULL,
	[ViewsCount] [int] NOT NULL,
	[RepliesCount] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumTopics] PRIMARY KEY CLUSTERED 
(
	[TopicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUploadedFiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUploadedFiles](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[MessageID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumUploadedFiles] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUploadedPersonalFiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUploadedPersonalFiles](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[MessageID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumUploadedPersonalFiles] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUserGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUserGroups](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.ForumUserGroups] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUsers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUsers](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Homepage] [nvarchar](50) NULL,
	[Interests] [nvarchar](255) NULL,
	[PostsCount] [int] NOT NULL,
	[RegistrationDate] [datetime2](7) NOT NULL,
	[Disabled] [bit] NOT NULL,
	[ActivationCode] [nvarchar](50) NOT NULL,
	[AvatarFileName] [nvarchar](50) NULL,
	[Signature] [nvarchar](1000) NULL,
	[LastLogonDate] [datetime2](7) NULL,
	[ReputationCache] [int] NOT NULL,
	[OpenIdUserName] [nvarchar](255) NULL,
 CONSTRAINT [PK_dbo.ForumUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUsersInGroup]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUsersInGroup](
	[GroupID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ForumUsersInGroup] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HierarchyItems]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HierarchyItems](
	[HierarchyItemID] [uniqueidentifier] NOT NULL,
	[ParentHierarchyItemID] [uniqueidentifier] NULL,
	[TblID] [uniqueidentifier] NULL,
	[HierarchyItemName] [nvarchar](max) NULL,
	[FullHierarchyWithHtml] [nvarchar](max) NULL,
	[FullHierarchyNoHtml] [nvarchar](max) NULL,
	[RouteToHere] [nvarchar](max) NULL,
	[IncludeInMenu] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.HierarchyItems] PRIMARY KEY CLUSTERED 
(
	[HierarchyItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InsertableContents]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InsertableContents](
	[InsertableContentID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[DomainID] [uniqueidentifier] NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[TblID] [uniqueidentifier] NULL,
	[Content] [nvarchar](max) NULL,
	[IsTextOnly] [bit] NOT NULL,
	[Overridable] [bit] NOT NULL,
	[Location] [smallint] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.InsertableContents] PRIMARY KEY CLUSTERED 
(
	[InsertableContentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InvitedUser]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvitedUser](
	[ActivationNumber] [uniqueidentifier] NOT NULL,
	[EmailId] [nvarchar](50) NULL,
	[MayView] [bit] NOT NULL,
	[MayPredict] [bit] NOT NULL,
	[MayAddTbls] [bit] NOT NULL,
	[MayResolveRatings] [bit] NOT NULL,
	[MayChangeTblRows] [bit] NOT NULL,
	[MayChangeChoiceGroups] [bit] NOT NULL,
	[MayChangeCharacteristics] [bit] NOT NULL,
	[MayChangeColumns] [bit] NOT NULL,
	[MayChangeUsersRights] [bit] NOT NULL,
	[MayAdjustPoints] [bit] NOT NULL,
	[MayChangeProposalSettings] [bit] NOT NULL,
	[IsRegistered] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.InvitedUser] PRIMARY KEY CLUSTERED 
(
	[ActivationNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LongProcesses]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LongProcesses](
	[LongProcessID] [uniqueidentifier] NOT NULL,
	[TypeOfProcess] [int] NOT NULL,
	[Object1ID] [uniqueidentifier] NULL,
	[Object2ID] [uniqueidentifier] NULL,
	[Priority] [int] NOT NULL,
	[AdditionalInfo] [varbinary](max) NULL,
	[ProgressInfo] [int] NULL,
	[Started] [bit] NOT NULL,
	[Complete] [bit] NOT NULL,
	[ResetWhenComplete] [bit] NOT NULL,
	[DelayBeforeRestart] [int] NULL,
	[EarliestRestart] [datetime2](7) NULL,
 CONSTRAINT [PK_dbo.LongProcesses] PRIMARY KEY CLUSTERED 
(
	[LongProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NumberFieldDefinitions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NumberFieldDefinitions](
	[NumberFieldDefinitionID] [uniqueidentifier] NOT NULL,
	[FieldDefinitionID] [uniqueidentifier] NOT NULL,
	[Minimum] [decimal](18, 4) NULL,
	[Maximum] [decimal](18, 4) NULL,
	[DecimalPlaces] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.NumberFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[NumberFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NumberFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NumberFields](
	[NumberFieldID] [uniqueidentifier] NOT NULL,
	[FieldID] [uniqueidentifier] NOT NULL,
	[Number] [decimal](18, 4) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.NumberFields] PRIMARY KEY CLUSTERED 
(
	[NumberFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OverrideCharacteristics]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OverrideCharacteristics](
	[OverrideCharacteristicsID] [uniqueidentifier] NOT NULL,
	[RatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[TblRowID] [uniqueidentifier] NOT NULL,
	[TblColumnID] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.OverrideCharacteristics] PRIMARY KEY CLUSTERED 
(
	[OverrideCharacteristicsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointsAdjustments]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointsAdjustments](
	[PointsAdjustmentID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NOT NULL,
	[Reason] [int] NOT NULL,
	[TotalAdjustment] [decimal](18, 4) NOT NULL,
	[CurrentAdjustment] [decimal](18, 4) NOT NULL,
	[CashValue] [decimal](18, 2) NULL,
	[WhenMade] [datetime2](7) NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.PointsAdjustments] PRIMARY KEY CLUSTERED 
(
	[PointsAdjustmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointsManagers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointsManagers](
	[PointsManagerID] [uniqueidentifier] NOT NULL,
	[DomainID] [uniqueidentifier] NOT NULL,
	[TrustTrackerUnitID] [uniqueidentifier] NULL,
	[CurrentPeriodDollarSubsidy] [decimal](18, 2) NOT NULL,
	[EndOfDollarSubsidyPeriod] [datetime2](7) NULL,
	[NextPeriodDollarSubsidy] [decimal](18, 2) NULL,
	[NextPeriodLength] [int] NULL,
	[NumPrizes] [smallint] NOT NULL,
	[MinimumPayment] [decimal](18, 2) NULL,
	[TotalUserPoints] [decimal](18, 4) NOT NULL,
	[CurrentUserPoints] [decimal](18, 4) NOT NULL,
	[CurrentUserPendingPoints] [decimal](18, 4) NOT NULL,
	[CurrentUserNotYetPendingPoints] [decimal](18, 4) NOT NULL,
	[CurrentPointsToCount] [decimal](18, 4) NOT NULL,
	[NumUsersMeetingUltimateStandard] [int] NOT NULL,
	[NumUsersMeetingCurrentStandard] [int] NOT NULL,
	[HighStakesProbability] [decimal](18, 4) NOT NULL,
	[HighStakesSecretMultiplier] [decimal](18, 4) NOT NULL,
	[HighStakesKnownMultiplier] [decimal](18, 4) NULL,
	[HighStakesNoviceOn] [bit] NOT NULL,
	[HighStakesNoviceNumAutomatic] [int] NOT NULL,
	[HighStakesNoviceNumOneThird] [int] NOT NULL,
	[HighStakesNoviceNumOneTenth] [int] NOT NULL,
	[DatabaseChangeSelectHighStakesNoviceNumPct] [decimal](18, 4) NOT NULL,
	[HighStakesNoviceNumActive] [int] NOT NULL,
	[HighStakesNoviceTargetNum] [int] NOT NULL,
	[DollarValuePerPoint] [decimal](18, 8) NOT NULL,
	[DiscountForGuarantees] [decimal](18, 4) NOT NULL,
	[MaximumTotalGuarantees] [decimal](18, 4) NOT NULL,
	[MaximumGuaranteePaymentPerHour] [decimal](18, 2) NOT NULL,
	[TotalUnconditionalGuaranteesEarnedEver] [decimal](18, 4) NOT NULL,
	[TotalConditionalGuaranteesEarnedEver] [decimal](18, 4) NOT NULL,
	[TotalConditionalGuaranteesPending] [decimal](18, 4) NOT NULL,
	[AllowApplicationsWhenNoConditionalGuaranteesAvailable] [bit] NOT NULL,
	[ConditionalGuaranteesAvailableForNewUsers] [bit] NOT NULL,
	[ConditionalGuaranteesAvailableForExistingUsers] [bit] NOT NULL,
	[ConditionalGuaranteeTimeBlockInHours] [int] NOT NULL,
	[ConditionalGuaranteeApplicationsReceived] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.PointsManagers] PRIMARY KEY CLUSTERED 
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointsTotals]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointsTotals](
	[PointsTotalID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NOT NULL,
	[CurrentPoints] [decimal](18, 4) NOT NULL,
	[TotalPoints] [decimal](18, 4) NOT NULL,
	[PotentialMaxLossOnNotYetPending] [decimal](18, 4) NOT NULL,
	[PendingPoints] [decimal](18, 4) NOT NULL,
	[NotYetPendingPoints] [decimal](18, 4) NOT NULL,
	[TrustPoints] [decimal](18, 4) NOT NULL,
	[TrustPointsRatio] [decimal](18, 4) NOT NULL,
	[NumPendingOrFinalizedRatings] [int] NOT NULL,
	[PointsPerRating] [decimal](18, 4) NOT NULL,
	[FirstUserRating] [datetime2](7) NULL,
	[LastCheckIn] [datetime2](7) NULL,
	[CurrentCheckInPeriodStart] [datetime2](7) NULL,
	[TotalTimeThisCheckInPeriod] [decimal](18, 4) NOT NULL,
	[TotalTimeThisRewardPeriod] [decimal](18, 4) NOT NULL,
	[TotalTimeEver] [decimal](18, 4) NOT NULL,
	[PointsPerHour] [decimal](18, 4) NULL,
	[ProjectedPointsPerHour] [decimal](18, 4) NULL,
	[GuaranteedPaymentsEarnedThisRewardPeriod] [decimal](18, 2) NOT NULL,
	[PendingConditionalGuaranteeApplication] [nvarchar](50) NULL,
	[PendingConditionalGuaranteePayment] [decimal](18, 2) NULL,
	[PendingConditionalGuaranteeTotalHoursAtStart] [decimal](18, 4) NULL,
	[PendingConditionalGuaranteeTotalHoursNeeded] [decimal](18, 4) NULL,
	[PendingConditionalGuaranteePaymentAlreadyMade] [decimal](18, 2) NULL,
	[RequestConditionalGuaranteeWhenAvailableTimeRequestMade] [datetime2](7) NULL,
	[TotalPointsOrPendingPointsLongTermUnweighted] [decimal](18, 4) NOT NULL,
	[PointsPerRatingLongTerm] [decimal](18, 4) NOT NULL,
	[PointsPumpingProportionAvg_Numer] [real] NOT NULL,
	[PointsPumpingProportionAvg_Denom] [real] NOT NULL,
	[PointsPumpingProportionAvg] [real] NOT NULL,
	[NumUserRatings] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PointsTotals] PRIMARY KEY CLUSTERED 
(
	[PointsTotalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProposalEvaluationRatingSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposalEvaluationRatingSettings](
	[ProposalEvaluationRatingSettingsID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[UserActionID] [uniqueidentifier] NULL,
	[RatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[MinValueToApprove] [decimal](18, 4) NOT NULL,
	[MaxValueToReject] [decimal](18, 4) NOT NULL,
	[TimeRequiredBeyondThreshold] [int] NOT NULL,
	[MinProportionOfThisTime] [decimal](18, 4) NOT NULL,
	[HalfLifeForResolvingAtFinalValue] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ProposalEvaluationRatingSettings] PRIMARY KEY CLUSTERED 
(
	[ProposalEvaluationRatingSettingsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProposalSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposalSettings](
	[ProposalSettingsID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[TblID] [uniqueidentifier] NULL,
	[UsersMayProposeAddingTbls] [bit] NOT NULL,
	[UsersMayProposeResolvingRatings] [bit] NOT NULL,
	[UsersMayProposeChangingTblRows] [bit] NOT NULL,
	[UsersMayProposeChangingChoiceGroups] [bit] NOT NULL,
	[UsersMayProposeChangingCharacteristics] [bit] NOT NULL,
	[UsersMayProposeChangingColumns] [bit] NOT NULL,
	[UsersMayProposeChangingUsersRights] [bit] NOT NULL,
	[UsersMayProposeAdjustingPoints] [bit] NOT NULL,
	[UsersMayProposeChangingProposalSettings] [bit] NOT NULL,
	[MinValueToApprove] [decimal](18, 4) NOT NULL,
	[MaxValueToReject] [decimal](18, 4) NOT NULL,
	[MinTimePastThreshold] [int] NOT NULL,
	[MinProportionOfThisTime] [decimal](18, 4) NOT NULL,
	[MinAdditionalTimeForRewardRating] [int] NOT NULL,
	[HalfLifeForRewardRating] [int] NOT NULL,
	[MaxBonusForProposal] [decimal](18, 4) NOT NULL,
	[MaxPenaltyForRejection] [decimal](18, 4) NOT NULL,
	[SubsidyForApprovalRating] [decimal](18, 4) NOT NULL,
	[SubsidyForRewardRating] [decimal](18, 4) NOT NULL,
	[HalfLifeForResolvingAtFinalValue] [int] NOT NULL,
	[RequiredPointsToMakeProposal] [decimal](18, 4) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ProposalSettings] PRIMARY KEY CLUSTERED 
(
	[ProposalSettingsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingCharacteristics]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingCharacteristics](
	[RatingCharacteristicsID] [uniqueidentifier] NOT NULL,
	[RatingPhaseGroupID] [uniqueidentifier] NOT NULL,
	[SubsidyDensityRangeGroupID] [uniqueidentifier] NULL,
	[MinimumUserRating] [decimal](18, 4) NOT NULL,
	[MaximumUserRating] [decimal](18, 4) NOT NULL,
	[DecimalPlaces] [tinyint] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingCharacteristics] PRIMARY KEY CLUSTERED 
(
	[RatingCharacteristicsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingConditions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingConditions](
	[RatingConditionID] [uniqueidentifier] NOT NULL,
	[ConditionRatingID] [uniqueidentifier] NULL,
	[GreaterThan] [decimal](18, 4) NULL,
	[LessThan] [decimal](18, 4) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingConditions] PRIMARY KEY CLUSTERED 
(
	[RatingConditionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupAttributes]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupAttributes](
	[RatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[RatingCharacteristicsID] [uniqueidentifier] NOT NULL,
	[RatingConditionID] [uniqueidentifier] NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[ConstrainedSum] [decimal](18, 4) NULL,
	[Name] [nvarchar](max) NULL,
	[TypeOfRatingGroup] [tinyint] NULL,
	[Description] [nvarchar](max) NULL,
	[RatingEndingTimeVaries] [bit] NOT NULL,
	[RatingsCanBeAutocalculated] [bit] NOT NULL,
	[LongTermPointsWeight] [decimal](18, 4) NOT NULL,
	[MinimumDaysToTrackLongTerm] [int] NOT NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingGroupAttributes] PRIMARY KEY CLUSTERED 
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupPhaseStatus]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupPhaseStatus](
	[RatingGroupPhaseStatusID] [uniqueidentifier] NOT NULL,
	[RatingPhaseGroupID] [uniqueidentifier] NOT NULL,
	[RatingPhaseID] [uniqueidentifier] NOT NULL,
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[RoundNum] [int] NOT NULL,
	[RoundNumThisPhase] [int] NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EarliestCompleteTime] [datetime2](7) NOT NULL,
	[ActualCompleteTime] [datetime2](7) NOT NULL,
	[ShortTermResolveTime] [datetime2](7) NOT NULL,
	[HighStakesSecret] [bit] NOT NULL,
	[HighStakesKnown] [bit] NOT NULL,
	[HighStakesReflected] [bit] NOT NULL,
	[HighStakesNoviceUser] [bit] NOT NULL,
	[HighStakesBecomeKnown] [datetime2](7) NULL,
	[HighStakesNoviceUserAfter] [datetime2](7) NULL,
	[DeletionTime] [datetime2](7) NULL,
	[WhenCreated] [datetime2](7) NOT NULL DEFAULT ('1900-01-01T00:00:00.000'),
 CONSTRAINT [PK_dbo.RatingGroupPhaseStatus] PRIMARY KEY CLUSTERED 
(
	[RatingGroupPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupResolutions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupResolutions](
	[RatingGroupResolutionID] [uniqueidentifier] NOT NULL,
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[CancelPreviousResolutions] [bit] NOT NULL,
	[ResolveByUnwinding] [bit] NOT NULL,
	[EffectiveTime] [datetime2](7) NOT NULL,
	[ExecutionTime] [datetime2](7) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
	[WhenCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_dbo.RatingGroupResolutions] PRIMARY KEY CLUSTERED 
(
	[RatingGroupResolutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroups](
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[RatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[TblRowID] [uniqueidentifier] NOT NULL,
	[TblColumnID] [uniqueidentifier] NOT NULL,
	[CurrentValueOfFirstRating] [decimal](18, 4) NULL,
	[ValueRecentlyChanged] [bit] NOT NULL,
	[ResolutionTime] [datetime2](7) NULL,
	[TypeOfRatingGroup] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[HighStakesKnown] [bit] NOT NULL,
	[WhenCreated] [datetime2](7) NOT NULL DEFAULT ('1900-01-01T00:00:00.000'),
 CONSTRAINT [PK_dbo.RatingGroups] PRIMARY KEY CLUSTERED 
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupStatusRecords]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupStatusRecords](
	[RatingGroupStatusRecordID] [uniqueidentifier] NOT NULL,
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[OldValueOfFirstRating] [numeric](18, 4) NULL,
	[NewValueTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_dbo.RatingGroupStatusRecords] PRIMARY KEY CLUSTERED 
(
	[RatingGroupStatusRecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPhaseGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPhaseGroups](
	[RatingPhaseGroupID] [uniqueidentifier] NOT NULL,
	[NumPhases] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingPhaseGroups] PRIMARY KEY CLUSTERED 
(
	[RatingPhaseGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPhases]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPhases](
	[RatingPhaseID] [uniqueidentifier] NOT NULL,
	[RatingPhaseGroupID] [uniqueidentifier] NOT NULL,
	[NumberInGroup] [int] NOT NULL,
	[SubsidyLevel] [decimal](18, 4) NOT NULL,
	[ScoringRule] [smallint] NOT NULL,
	[Timed] [bit] NOT NULL,
	[BaseTimingOnSpecificTime] [bit] NOT NULL,
	[EndTime] [datetime2](7) NULL,
	[RunTime] [int] NULL,
	[HalfLifeForResolution] [int] NOT NULL,
	[RepeatIndefinitely] [bit] NOT NULL,
	[RepeatNTimes] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingPhases] PRIMARY KEY CLUSTERED 
(
	[RatingPhaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPhaseStatus]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPhaseStatus](
	[RatingPhaseStatusID] [uniqueidentifier] NOT NULL,
	[RatingGroupPhaseStatusID] [uniqueidentifier] NOT NULL,
	[RatingID] [uniqueidentifier] NOT NULL,
	[ShortTermResolutionValue] [decimal](18, 4) NULL,
	[NumUserRatingsMadeDuringPhase] [int] NOT NULL,
	[TriggerUserRatingsUpdate] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.RatingPhaseStatus] PRIMARY KEY CLUSTERED 
(
	[RatingPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPlans]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPlans](
	[RatingPlanID] [uniqueidentifier] NOT NULL,
	[RatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[NumInGroup] [int] NOT NULL,
	[OwnedRatingGroupAttributesID] [uniqueidentifier] NULL,
	[DefaultUserRating] [decimal](18, 4) NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingPlans] PRIMARY KEY CLUSTERED 
(
	[RatingPlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Ratings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ratings](
	[RatingID] [uniqueidentifier] NOT NULL,
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[RatingCharacteristicsID] [uniqueidentifier] NOT NULL,
	[OwnedRatingGroupID] [uniqueidentifier] NULL,
	[TopmostRatingGroupID] [uniqueidentifier] NOT NULL,
	[MostRecentUserRatingID] [uniqueidentifier] NULL,
	[NumInGroup] [int] NOT NULL,
	[TotalUserRatings] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[CurrentValue] [decimal](18, 4) NULL,
	[LastTrustedValue] [decimal](18, 4) NULL,
	[LastModifiedResolutionTimeOrCurrentValue] [datetime2](7) NOT NULL,
	[ReviewRecentUserRatingsAfter] [datetime2](7) NULL,
 CONSTRAINT [PK_dbo.Ratings] PRIMARY KEY CLUSTERED 
(
	[RatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RewardPendingPointsTrackers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RewardPendingPointsTrackers](
	[RewardPendingPointsTrackerID] [uniqueidentifier] NOT NULL,
	[PendingRating] [decimal](18, 4) NULL,
	[TimeOfPendingRating] [datetime2](7) NULL,
	[RewardTblRowID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.RewardPendingPointsTrackers] PRIMARY KEY CLUSTERED 
(
	[RewardPendingPointsTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RewardRatingSettings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RewardRatingSettings](
	[RewardRatingSettingsID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[UserActionID] [uniqueidentifier] NULL,
	[RatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[ProbOfRewardEvaluation] [decimal](18, 4) NOT NULL,
	[Multiplier] [decimal](18, 4) NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RewardRatingSettings] PRIMARY KEY CLUSTERED 
(
	[RewardRatingSettingsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleStatus]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleStatus](
	[RoleStatusID] [uniqueidentifier] NOT NULL,
	[RoleID] [nvarchar](max) NULL,
	[LastCheckIn] [datetime2](7) NULL,
	[IsWorkerRole] [bit] NOT NULL,
	[IsBackgroundProcessing] [bit] NOT NULL,
	[WhenCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_dbo.RoleStatus] PRIMARY KEY CLUSTERED 
(
	[RoleStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordChoices]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordChoices](
	[SearchWordChoiceID] [uniqueidentifier] NOT NULL,
	[ChoiceInGroupID] [uniqueidentifier] NOT NULL,
	[SearchWordID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.SearchWordChoices] PRIMARY KEY CLUSTERED 
(
	[SearchWordChoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordHierarchyItems]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordHierarchyItems](
	[SearchWordHierarchyItemID] [uniqueidentifier] NOT NULL,
	[HierarchyItemID] [uniqueidentifier] NOT NULL,
	[SearchWordID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.SearchWordHierarchyItems] PRIMARY KEY CLUSTERED 
(
	[SearchWordHierarchyItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWords]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWords](
	[SearchWordID] [uniqueidentifier] NOT NULL,
	[TheWord] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.SearchWords] PRIMARY KEY CLUSTERED 
(
	[SearchWordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordTblRowNames]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordTblRowNames](
	[SearchWordTblRowNameID] [uniqueidentifier] NOT NULL,
	[TblRowID] [uniqueidentifier] NOT NULL,
	[SearchWordID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.SearchWordTblRowNames] PRIMARY KEY CLUSTERED 
(
	[SearchWordTblRowNameID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordTextFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordTextFields](
	[SearchWordTextFieldID] [uniqueidentifier] NOT NULL,
	[TextFieldID] [uniqueidentifier] NOT NULL,
	[SearchWordID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.SearchWordTextFields] PRIMARY KEY CLUSTERED 
(
	[SearchWordTextFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubsidyAdjustments]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubsidyAdjustments](
	[SubsidyAdjustmentID] [uniqueidentifier] NOT NULL,
	[RatingGroupPhaseStatusID] [uniqueidentifier] NOT NULL,
	[SubsidyAdjustmentFactor] [decimal](18, 4) NOT NULL,
	[EffectiveTime] [datetime2](7) NOT NULL,
	[EndingTime] [datetime2](7) NULL,
	[EndingTimeHalfLife] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.SubsidyAdjustments] PRIMARY KEY CLUSTERED 
(
	[SubsidyAdjustmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubsidyDensityRangeGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubsidyDensityRangeGroups](
	[SubsidyDensityRangeGroupID] [uniqueidentifier] NOT NULL,
	[UseLogarithmBase] [decimal](18, 4) NULL,
	[CumDensityTotal] [decimal](18, 4) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.SubsidyDensityRangeGroups] PRIMARY KEY CLUSTERED 
(
	[SubsidyDensityRangeGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubsidyDensityRanges]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubsidyDensityRanges](
	[SubsidyDensityRangeID] [uniqueidentifier] NOT NULL,
	[SubsidyDensityRangeGroupID] [uniqueidentifier] NOT NULL,
	[RangeBottom] [decimal](18, 4) NOT NULL,
	[RangeTop] [decimal](18, 4) NOT NULL,
	[LiquidityFactor] [decimal](18, 4) NOT NULL,
	[CumDensityBottom] [decimal](18, 4) NOT NULL,
	[CumDensityTop] [decimal](18, 4) NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.SubsidyDensityRanges] PRIMARY KEY CLUSTERED 
(
	[SubsidyDensityRangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblColumnFormatting]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblColumnFormatting](
	[TblColumnFormattingID] [uniqueidentifier] NOT NULL,
	[TblColumnID] [uniqueidentifier] NOT NULL,
	[Prefix] [nvarchar](10) NULL,
	[Suffix] [nvarchar](10) NULL,
	[OmitLeadingZero] [bit] NOT NULL,
	[ExtraDecimalPlaceAbove] [decimal](18, 4) NULL,
	[ExtraDecimalPlace2Above] [decimal](18, 4) NULL,
	[ExtraDecimalPlace3Above] [decimal](18, 4) NULL,
	[SuppStylesHeader] [nvarchar](max) NULL,
	[SuppStylesMain] [nvarchar](max) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TblColumnFormatting] PRIMARY KEY CLUSTERED 
(
	[TblColumnFormattingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblColumns]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblColumns](
	[TblColumnID] [uniqueidentifier] NOT NULL,
	[TblTabID] [uniqueidentifier] NOT NULL,
	[DefaultRatingGroupAttributesID] [uniqueidentifier] NOT NULL,
	[ConditionTblColumnID] [uniqueidentifier] NULL,
	[TrustTrackerUnitID] [uniqueidentifier] NULL,
	[ConditionGreaterThan] [decimal](18, 4) NULL,
	[ConditionLessThan] [decimal](18, 4) NULL,
	[CategoryNum] [int] NOT NULL,
	[Abbreviation] [nchar](20) NULL,
	[Name] [nvarchar](50) NULL,
	[Explanation] [nvarchar](max) NULL,
	[WidthStyle] [nvarchar](20) NULL,
	[NumNonNull] [int] NOT NULL,
	[ProportionNonNull] [float] NOT NULL,
	[UsingNonSparseColumn] [bit] NOT NULL,
	[ShouldUseNonSparseColumn] [bit] NOT NULL,
	[UseAsFilter] [bit] NOT NULL,
	[Sortable] [bit] NOT NULL,
	[DefaultSortOrderAscending] [bit] NOT NULL,
	[AutomaticallyCreateMissingRatings] [bit] NOT NULL,
	[NotYetAddedToDatabase] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[WhenCreated] [datetime2](7) NOT NULL DEFAULT ('1900-01-01T00:00:00.000'),
 CONSTRAINT [PK_dbo.TblColumns] PRIMARY KEY CLUSTERED 
(
	[TblColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblDimensions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblDimensions](
	[TblDimensionsID] [uniqueidentifier] NOT NULL,
	[MaxWidthOfImageInRowHeaderCell] [int] NOT NULL,
	[MaxHeightOfImageInRowHeaderCell] [int] NOT NULL,
	[MaxWidthOfImageInTblRowPopUpWindow] [int] NOT NULL,
	[MaxHeightOfImageInTblRowPopUpWindow] [int] NOT NULL,
	[WidthOfTblRowPopUpWindow] [int] NOT NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NULL,
 CONSTRAINT [PK_dbo.TblDimensions] PRIMARY KEY CLUSTERED 
(
	[TblDimensionsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblRowFieldDisplays]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblRowFieldDisplays](
	[TblRowFieldDisplayID] [uniqueidentifier] NOT NULL,
	[Row] [nvarchar](max) NULL,
	[PopUp] [nvarchar](max) NULL,
	[TblRowPage] [nvarchar](max) NULL,
	[ResetNeeded] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TblRowFieldDisplays] PRIMARY KEY CLUSTERED 
(
	[TblRowFieldDisplayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblRows]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TblRows](
	[TblRowID] [uniqueidentifier] NOT NULL,
	[TblID] [uniqueidentifier] NOT NULL,
	[TblRowFieldDisplayID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Status] [tinyint] NOT NULL,
	[StatusRecentlyChanged] [bit] NOT NULL,
	[CountHighStakesNow] [int] NOT NULL,
	[CountNonnullEntries] [int] NOT NULL,
	[CountUserPoints] [decimal](18, 4) NOT NULL,
	[ElevateOnMostNeedsRating] [bit] NOT NULL,
	[InitialFieldsDisplaySet] [bit] NOT NULL,
	[FastAccessInitialCopy] [bit] NOT NULL,
	[FastAccessDeleteThenRecopy] [bit] NOT NULL,
	[FastAccessUpdateFields] [bit] NOT NULL,
	[FastAccessUpdateRatings] [bit] NOT NULL,
	[FastAccessUpdateSpecified] [bit] NOT NULL,
	[NotYetAddedToDatabase] [bit] NOT NULL,
	[FastAccessUpdated] [varbinary](max) NULL,
	[WhenCreated] [datetime2](7) NOT NULL DEFAULT ('1900-01-01T00:00:00.000'),
 CONSTRAINT [PK_dbo.TblRows] PRIMARY KEY CLUSTERED 
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TblRowStatusRecord]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblRowStatusRecord](
	[TblRowStatusRecordID] [uniqueidentifier] NOT NULL,
	[TblRowId] [uniqueidentifier] NOT NULL,
	[TimeChanged] [datetime2](7) NOT NULL,
	[Adding] [bit] NOT NULL,
	[Deleting] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TblRowStatusRecord] PRIMARY KEY CLUSTERED 
(
	[TblRowStatusRecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbls]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbls](
	[TblID] [uniqueidentifier] NOT NULL,
	[PointsManagerID] [uniqueidentifier] NOT NULL,
	[DefaultRatingGroupAttributesID] [uniqueidentifier] NULL,
	[WordToDescribeGroupOfColumnsInThisTbl] [nvarchar](50) NULL,
	[Name] [nvarchar](max) NULL,
	[TypeOfTblRow] [nvarchar](50) NULL,
	[TblDimensionID] [uniqueidentifier] NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
	[AllowOverrideOfRatingGroupCharacterstics] [bit] NOT NULL,
	[AllowUsersToAddComments] [bit] NOT NULL,
	[LimitCommentsToUsersWhoCanMakeUserRatings] [bit] NOT NULL,
	[OneRatingPerRatingGroup] [bit] NOT NULL,
	[TblRowAdditionCriteria] [nvarchar](max) NULL,
	[SuppStylesHeader] [nvarchar](max) NULL,
	[SuppStylesMain] [nvarchar](max) NULL,
	[WidthStyleEntityCol] [nvarchar](20) NULL,
	[WidthStyleNumCol] [nvarchar](20) NULL,
	[FastTableSyncStatus] [tinyint] NOT NULL,
	[NumTblRowsActive] [int] NOT NULL,
	[NumTblRowsDeleted] [int] NOT NULL,
	[TblDimension_TblDimensionsID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.Tbls] PRIMARY KEY CLUSTERED 
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblTabs]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblTabs](
	[TblTabID] [uniqueidentifier] NOT NULL,
	[TblID] [uniqueidentifier] NOT NULL,
	[NumInTbl] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[DefaultSortTblColumnID] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TblTabs] PRIMARY KEY CLUSTERED 
(
	[TblTabID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TextFieldDefinitions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TextFieldDefinitions](
	[TextFieldDefinitionID] [uniqueidentifier] NOT NULL,
	[FieldDefinitionID] [uniqueidentifier] NOT NULL,
	[IncludeText] [bit] NOT NULL,
	[IncludeLink] [bit] NOT NULL,
	[Searchable] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TextFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[TextFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TextFields]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TextFields](
	[TextFieldID] [uniqueidentifier] NOT NULL,
	[FieldID] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[Link] [nvarchar](max) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TextFields] PRIMARY KEY CLUSTERED 
(
	[TextFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerForChoiceInGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerForChoiceInGroups](
	[TrustTrackerForChoiceInGroupID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[ChoiceInGroupID] [uniqueidentifier] NOT NULL,
	[TblID] [uniqueidentifier] NOT NULL,
	[SumAdjustmentPctTimesRatingMagnitude] [real] NOT NULL,
	[SumRatingMagnitudes] [real] NOT NULL,
	[TrustLevelForChoice] [real] NOT NULL,
 CONSTRAINT [PK_dbo.TrustTrackerForChoiceInGroups] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerForChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks](
	[TrustTrackerForChoiceInGroupsUserRatingLinkID] [uniqueidentifier] NOT NULL,
	[UserRatingID] [uniqueidentifier] NOT NULL,
	[TrustTrackerForChoiceInGroupID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.TrustTrackerForChoiceInGroupsUserRatingLinks] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerForChoiceInGroupsUserRatingLinkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackers](
	[TrustTrackerID] [uniqueidentifier] NOT NULL,
	[TrustTrackerUnitID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[OverallTrustLevel] [float] NOT NULL,
	[OverallTrustLevelAtLastReview] [float] NOT NULL,
	[DeltaOverallTrustLevel] [float] NOT NULL,
	[SkepticalTrustLevel] [float] NOT NULL,
	[SumUserInteractionWeights] [float] NOT NULL,
	[EgalitarianTrustLevel] [float] NOT NULL,
	[Egalitarian_Num] [float] NOT NULL,
	[Egalitarian_Denom] [float] NOT NULL,
	[EgalitarianTrustLevelOverride] [float] NULL,
	[MustUpdateUserInteractionEgalitarianTrustLevel] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TrustTrackers] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerStats]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerStats](
	[TrustTrackerStatID] [uniqueidentifier] NOT NULL,
	[TrustTrackerID] [uniqueidentifier] NOT NULL,
	[StatNum] [smallint] NOT NULL,
	[TrustValue] [float] NOT NULL,
	[Trust_Numer] [float] NOT NULL,
	[Trust_Denom] [float] NOT NULL,
	[SumUserInteractionStatWeights] [float] NOT NULL,
 CONSTRAINT [PK_dbo.TrustTrackerStats] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerStatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerUnits]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerUnits](
	[TrustTrackerUnitID] [uniqueidentifier] NOT NULL,
	[SkepticalTrustThreshhold] [smallint] NOT NULL,
	[LastSkepticalTrustThreshhold] [smallint] NOT NULL,
	[MinUpdateIntervalSeconds] [int] NOT NULL,
	[MaxUpdateIntervalSeconds] [int] NOT NULL,
	[ExtendIntervalWhenChangeIsLessThanThis] [decimal](18, 4) NOT NULL,
	[ExtendIntervalMultiplier] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_dbo.TrustTrackerUnits] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UniquenessLockReferences]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UniquenessLockReferences](
	[Id] [uniqueidentifier] NOT NULL,
	[UniquenessLockID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.UniquenessLockReferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UniquenessLocks]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UniquenessLocks](
	[Id] [uniqueidentifier] NOT NULL,
	[DeletionTime] [datetime2](7) NULL,
 CONSTRAINT [PK_dbo.UniquenessLocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserActions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActions](
	[UserActionID] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[SuperUser] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.UserActions] PRIMARY KEY CLUSTERED 
(
	[UserActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserCheckIns]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCheckIns](
	[UserCheckInID] [uniqueidentifier] NOT NULL,
	[CheckInTime] [datetime2](7) NOT NULL,
	[UserID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.UserCheckIns] PRIMARY KEY CLUSTERED 
(
	[UserCheckInID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserInfo](
	[UserInfoID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](250) NULL,
	[Address1] [nvarchar](200) NULL,
	[Address2] [nvarchar](200) NULL,
	[WorkPhone] [nvarchar](50) NULL,
	[HomePhone] [nvarchar](50) NULL,
	[MobilePhone] [nvarchar](50) NULL,
	[Password] [varchar](50) NULL,
	[ZipCode] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[IsVerified] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.UserInfo] PRIMARY KEY CLUSTERED 
(
	[UserInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserInteractions]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInteractions](
	[UserInteractionID] [uniqueidentifier] NOT NULL,
	[TrustTrackerUnitID] [uniqueidentifier] NOT NULL,
	[OrigRatingUserID] [uniqueidentifier] NOT NULL,
	[LatestRatingUserID] [uniqueidentifier] NOT NULL,
	[NumTransactions] [int] NOT NULL,
	[LatestUserEgalitarianTrust] [float] NOT NULL,
	[WeightInCalculatingTrustTotal] [float] NOT NULL,
	[LatestUserEgalitarianTrustAtLastWeightUpdate] [float] NULL,
 CONSTRAINT [PK_dbo.UserInteractions] PRIMARY KEY CLUSTERED 
(
	[UserInteractionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInteractionStats]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInteractionStats](
	[UserInteractionStatID] [uniqueidentifier] NOT NULL,
	[UserInteractionID] [uniqueidentifier] NOT NULL,
	[TrustTrackerStatID] [uniqueidentifier] NOT NULL,
	[StatNum] [smallint] NOT NULL,
	[SumAdjustPctTimesWeight] [float] NOT NULL,
	[SumWeights] [float] NOT NULL,
	[AvgAdjustmentPctWeighted] [float] NOT NULL,
 CONSTRAINT [PK_dbo.UserInteractionStats] PRIMARY KEY CLUSTERED 
(
	[UserInteractionStatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRatingGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRatingGroups](
	[UserRatingGroupID] [uniqueidentifier] NOT NULL,
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[RatingGroupPhaseStatusID] [uniqueidentifier] NOT NULL,
	[WhenCreated] [datetime2](7) NOT NULL DEFAULT ('1900-01-01T00:00:00.000'),
 CONSTRAINT [PK_dbo.UserRatingGroups] PRIMARY KEY CLUSTERED 
(
	[UserRatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRatings]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRatings](
	[UserRatingID] [uniqueidentifier] NOT NULL,
	[UserRatingGroupID] [uniqueidentifier] NOT NULL,
	[RatingID] [uniqueidentifier] NOT NULL,
	[RatingPhaseStatusID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[TrustTrackerUnitID] [uniqueidentifier] NULL,
	[RewardPendingPointsTrackerID] [uniqueidentifier] NULL,
	[MostRecentUserRatingID] [uniqueidentifier] NULL,
	[PreviousRatingOrVirtualRating] [decimal](18, 4) NOT NULL,
	[PreviousDisplayedRating] [decimal](18, 4) NULL,
	[EnteredUserRating] [decimal](18, 4) NOT NULL,
	[NewUserRating] [decimal](18, 4) NOT NULL,
	[OriginalAdjustmentPct] [decimal](7, 4) NOT NULL,
	[OriginalTrustLevel] [decimal](7, 4) NOT NULL,
	[MaxGain] [decimal](18, 4) NOT NULL,
	[MaxLoss] [decimal](18, 4) NOT NULL,
	[PotentialPointsShortTerm] [decimal](18, 4) NOT NULL,
	[PotentialPointsLongTerm] [decimal](18, 4) NOT NULL,
	[PotentialPointsLongTermUnweighted] [decimal](18, 4) NOT NULL,
	[LongTermPointsWeight] [decimal](18, 4) NOT NULL,
	[PointsPumpingProportion] [decimal](18, 4) NULL,
	[PastPointsPumpingProportion] [decimal](18, 4) NOT NULL,
	[PercentPreviousRatings] [decimal](18, 4) NOT NULL,
	[IsTrusted] [bit] NOT NULL,
	[MadeDirectly] [bit] NOT NULL,
	[LongTermResolutionReflected] [bit] NOT NULL,
	[ShortTermResolutionReflected] [bit] NOT NULL,
	[PointsHaveBecomePending] [bit] NOT NULL,
	[ForceRecalculate] [bit] NOT NULL,
	[HighStakesPreviouslySecret] [bit] NOT NULL,
	[HighStakesKnown] [bit] NOT NULL,
	[PreviouslyRated] [bit] NOT NULL,
	[SubsequentlyRated] [bit] NOT NULL,
	[IsMostRecent10Pct] [bit] NOT NULL,
	[IsMostRecent30Pct] [bit] NOT NULL,
	[IsMostRecent70Pct] [bit] NOT NULL,
	[IsMostRecent90Pct] [bit] NOT NULL,
	[IsUsersFirstWeek] [bit] NOT NULL,
	[LogarithmicBase] [decimal](18, 4) NULL,
	[HighStakesMultiplierOverride] [decimal](18, 4) NULL,
	[WhenPointsBecomePending] [datetime2](7) NULL,
	[LastModifiedTime] [datetime2](7) NOT NULL,
	[VolatilityTrackingNextTimeFrameToRemove] [tinyint] NOT NULL,
	[LastWeekDistanceFromStart] [decimal](18, 4) NOT NULL,
	[LastWeekPushback] [decimal](18, 4) NOT NULL,
	[LastYearPushback] [decimal](18, 4) NOT NULL,
	[UserRatingNumberForUser] [int] NOT NULL,
	[NextRecencyUpdateAtUserRatingNum] [int] NULL,
 CONSTRAINT [PK_dbo.UserRatings] PRIMARY KEY CLUSTERED 
(
	[UserRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRatingsToAdd]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRatingsToAdd](
	[UserRatingsToAddID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[TopRatingGroupID] [uniqueidentifier] NOT NULL,
	[UserRatingHierarchy] [varbinary](max) NULL,
	[WhenCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_dbo.UserRatingsToAdd] PRIMARY KEY CLUSTERED 
(
	[UserRatingsToAddID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](50) NULL,
	[SuperUser] [bit] NOT NULL,
	[TrustPointsRatioTotals] [decimal](18, 4) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[WhenCreated] [datetime2](7) NOT NULL DEFAULT ('1900-01-01T00:00:00.000'),
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersAdministrationRightsGroups]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersAdministrationRightsGroups](
	[UsersAdministrationRightsGroupID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NULL,
	[PointsManagerID] [uniqueidentifier] NOT NULL,
	[AdministrationRightsGroupID] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.UsersAdministrationRightsGroups] PRIMARY KEY CLUSTERED 
(
	[UsersAdministrationRightsGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersRights]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersRights](
	[UsersRightsID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NULL,
	[PointsManagerID] [uniqueidentifier] NULL,
	[MayView] [bit] NOT NULL,
	[MayPredict] [bit] NOT NULL,
	[MayAddTbls] [bit] NOT NULL,
	[MayResolveRatings] [bit] NOT NULL,
	[MayChangeTblRows] [bit] NOT NULL,
	[MayChangeChoiceGroups] [bit] NOT NULL,
	[MayChangeCharacteristics] [bit] NOT NULL,
	[MayChangeCategories] [bit] NOT NULL,
	[MayChangeUsersRights] [bit] NOT NULL,
	[MayAdjustPoints] [bit] NOT NULL,
	[MayChangeProposalSettings] [bit] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Creator] [uniqueidentifier] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.UsersRights] PRIMARY KEY CLUSTERED 
(
	[UsersRightsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VolatilityTblRowTrackers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolatilityTblRowTrackers](
	[VolatilityTblRowTrackerID] [uniqueidentifier] NOT NULL,
	[TblRowID] [uniqueidentifier] NOT NULL,
	[DurationType] [tinyint] NOT NULL,
	[TotalMovement] [decimal](18, 4) NOT NULL,
	[DistanceFromStart] [decimal](18, 4) NOT NULL,
	[Pushback] [decimal](18, 4) NOT NULL,
	[PushbackProportion] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_dbo.VolatilityTblRowTrackers] PRIMARY KEY CLUSTERED 
(
	[VolatilityTblRowTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VolatilityTrackers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolatilityTrackers](
	[VolatilityTrackerID] [uniqueidentifier] NOT NULL,
	[RatingGroupID] [uniqueidentifier] NOT NULL,
	[VolatilityTblRowTrackerID] [uniqueidentifier] NOT NULL,
	[DurationType] [tinyint] NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NOT NULL,
	[TotalMovement] [decimal](18, 4) NOT NULL,
	[DistanceFromStart] [decimal](18, 4) NOT NULL,
	[Pushback] [decimal](18, 4) NOT NULL,
	[PushbackProportion] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_dbo.VolatilityTrackers] PRIMARY KEY CLUSTERED 
(
	[VolatilityTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[vw_aspnet_Applications]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Applications]
  AS SELECT [dbo].[aspnet_Applications].[ApplicationName], [dbo].[aspnet_Applications].[LoweredApplicationName], [dbo].[aspnet_Applications].[ApplicationId], [dbo].[aspnet_Applications].[Description]
  FROM [dbo].[aspnet_Applications]
  
GO
/****** Object:  View [dbo].[vw_aspnet_MembershipUsers]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_MembershipUsers]
  AS SELECT [dbo].[aspnet_Membership].[UserId],
            [dbo].[aspnet_Membership].[PasswordFormat],
            [dbo].[aspnet_Membership].[MobilePIN],
            [dbo].[aspnet_Membership].[Email],
            [dbo].[aspnet_Membership].[LoweredEmail],
            [dbo].[aspnet_Membership].[PasswordQuestion],
            [dbo].[aspnet_Membership].[PasswordAnswer],
            [dbo].[aspnet_Membership].[IsApproved],
            [dbo].[aspnet_Membership].[IsLockedOut],
            [dbo].[aspnet_Membership].[CreateDate],
            [dbo].[aspnet_Membership].[LastLoginDate],
            [dbo].[aspnet_Membership].[LastPasswordChangedDate],
            [dbo].[aspnet_Membership].[LastLockoutDate],
            [dbo].[aspnet_Membership].[FailedPasswordAttemptCount],
            [dbo].[aspnet_Membership].[FailedPasswordAttemptWindowStart],
            [dbo].[aspnet_Membership].[FailedPasswordAnswerAttemptCount],
            [dbo].[aspnet_Membership].[FailedPasswordAnswerAttemptWindowStart],
            [dbo].[aspnet_Membership].[Comment],
            [dbo].[aspnet_Users].[ApplicationId],
            [dbo].[aspnet_Users].[UserName],
            [dbo].[aspnet_Users].[MobileAlias],
            [dbo].[aspnet_Users].[IsAnonymous],
            [dbo].[aspnet_Users].[LastActivityDate]
  FROM [dbo].[aspnet_Membership] INNER JOIN [dbo].[aspnet_Users]
      ON [dbo].[aspnet_Membership].[UserId] = [dbo].[aspnet_Users].[UserId]
  
GO
/****** Object:  View [dbo].[vw_aspnet_Profiles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Profiles]
  AS SELECT [dbo].[aspnet_Profile].[UserId], [dbo].[aspnet_Profile].[LastUpdatedDate],
      [DataSize]=  DATALENGTH([dbo].[aspnet_Profile].[PropertyNames])
                 + DATALENGTH([dbo].[aspnet_Profile].[PropertyValuesString])
                 + DATALENGTH([dbo].[aspnet_Profile].[PropertyValuesBinary])
  FROM [dbo].[aspnet_Profile]
  
GO
/****** Object:  View [dbo].[vw_aspnet_Roles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Roles]
  AS SELECT [dbo].[aspnet_Roles].[ApplicationId], [dbo].[aspnet_Roles].[RoleId], [dbo].[aspnet_Roles].[RoleName], [dbo].[aspnet_Roles].[LoweredRoleName], [dbo].[aspnet_Roles].[Description]
  FROM [dbo].[aspnet_Roles]
  
GO
/****** Object:  View [dbo].[vw_aspnet_Users]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Users]
  AS SELECT [dbo].[aspnet_Users].[ApplicationId], [dbo].[aspnet_Users].[UserId], [dbo].[aspnet_Users].[UserName], [dbo].[aspnet_Users].[LoweredUserName], [dbo].[aspnet_Users].[MobileAlias], [dbo].[aspnet_Users].[IsAnonymous], [dbo].[aspnet_Users].[LastActivityDate]
  FROM [dbo].[aspnet_Users]
  
GO
/****** Object:  View [dbo].[vw_aspnet_UsersInRoles]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_UsersInRoles]
  AS SELECT [dbo].[aspnet_UsersInRoles].[UserId], [dbo].[aspnet_UsersInRoles].[RoleId]
  FROM [dbo].[aspnet_UsersInRoles]
  
GO
/****** Object:  View [dbo].[vw_aspnet_WebPartState_Paths]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_WebPartState_Paths]
  AS SELECT [dbo].[aspnet_Paths].[ApplicationId], [dbo].[aspnet_Paths].[PathId], [dbo].[aspnet_Paths].[Path], [dbo].[aspnet_Paths].[LoweredPath]
  FROM [dbo].[aspnet_Paths]
  
GO
/****** Object:  View [dbo].[vw_aspnet_WebPartState_Shared]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_WebPartState_Shared]
  AS SELECT [dbo].[aspnet_PersonalizationAllUsers].[PathId], [DataSize]=DATALENGTH([dbo].[aspnet_PersonalizationAllUsers].[PageSettings]), [dbo].[aspnet_PersonalizationAllUsers].[LastUpdatedDate]
  FROM [dbo].[aspnet_PersonalizationAllUsers]
  
GO
/****** Object:  View [dbo].[vw_aspnet_WebPartState_User]    Script Date: 7/26/2014 7:24:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_WebPartState_User]
  AS SELECT [dbo].[aspnet_PersonalizationPerUser].[PathId], [dbo].[aspnet_PersonalizationPerUser].[UserId], [DataSize]=DATALENGTH([dbo].[aspnet_PersonalizationPerUser].[PageSettings]), [dbo].[aspnet_PersonalizationPerUser].[LastUpdatedDate]
  FROM [dbo].[aspnet_PersonalizationPerUser]
  
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Applications_Index]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE CLUSTERED INDEX [aspnet_Applications_Index] ON [dbo].[aspnet_Applications]
(
	[LoweredApplicationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Membership_index]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE CLUSTERED INDEX [aspnet_Membership_index] ON [dbo].[aspnet_Membership]
(
	[ApplicationId] ASC,
	[LoweredEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Paths_index]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_Paths_index] ON [dbo].[aspnet_Paths]
(
	[ApplicationId] ASC,
	[LoweredPath] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_PersonalizationPerUser_index1]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_PersonalizationPerUser_index1] ON [dbo].[aspnet_PersonalizationPerUser]
(
	[PathId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Roles_index1]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_Roles_index1] ON [dbo].[aspnet_Roles]
(
	[ApplicationId] ASC,
	[LoweredRoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Users_Index]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_Users_Index] ON [dbo].[aspnet_Users]
(
	[ApplicationId] ASC,
	[LoweredUserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldID] ON [dbo].[AddressFields]
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AdministrationRightsGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_AdministrationRightsGroupID] ON [dbo].[AdministrationRights]
(
	[AdministrationRightsGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserActionID] ON [dbo].[AdministrationRights]
(
	[UserActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[AdministrationRightsGroups]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_PersonalizationPerUser_ncindex2]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [aspnet_PersonalizationPerUser_ncindex2] ON [dbo].[aspnet_PersonalizationPerUser]
(
	[UserId] ASC,
	[PathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_Users_Index2]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [aspnet_Users_Index2] ON [dbo].[aspnet_Users]
(
	[ApplicationId] ASC,
	[LastActivityDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_UsersInRoles_index]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [aspnet_UsersInRoles_index] ON [dbo].[aspnet_UsersInRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[ChangesGroup]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[ChangesGroup]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RewardRatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RewardRatingID] ON [dbo].[ChangesGroup]
(
	[RewardRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[ChangesGroup]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChangesGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChangesGroupID] ON [dbo].[ChangesStatusOfObject]
(
	[ChangesGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldID] ON [dbo].[ChoiceFields]
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChoiceGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChoiceGroupID] ON [dbo].[ChoiceGroupFieldDefinitions]
(
	[ChoiceGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DependentOnChoiceGroupFieldDefinitionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_DependentOnChoiceGroupFieldDefinitionID] ON [dbo].[ChoiceGroupFieldDefinitions]
(
	[DependentOnChoiceGroupFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldDefinitionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldDefinitionID] ON [dbo].[ChoiceGroupFieldDefinitions]
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[ChoiceGroups]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DependentOnChoiceGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_DependentOnChoiceGroupID] ON [dbo].[ChoiceGroups]
(
	[DependentOnChoiceGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[ChoiceGroups]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChoiceFieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChoiceFieldID] ON [dbo].[ChoiceInFields]
(
	[ChoiceFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChoiceInGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChoiceInGroupID] ON [dbo].[ChoiceInFields]
(
	[ChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ActiveOnDeterminingGroupChoiceInGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ActiveOnDeterminingGroupChoiceInGroupID] ON [dbo].[ChoiceInGroups]
(
	[ActiveOnDeterminingGroupChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChoiceGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChoiceGroupID] ON [dbo].[ChoiceInGroups]
(
	[ChoiceGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowID] ON [dbo].[Comments]
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[Comments]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldDefinitionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldDefinitionID] ON [dbo].[DateTimeFieldDefinitions]
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldID] ON [dbo].[DateTimeFields]
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblDimensionsID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblDimensionsID] ON [dbo].[Domains]
(
	[TblDimensionsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[FieldDefinitions]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldDefinitionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldDefinitionID] ON [dbo].[Fields]
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowID] ON [dbo].[Fields]
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentHierarchyItemID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ParentHierarchyItemID] ON [dbo].[HierarchyItems]
(
	[ParentHierarchyItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[HierarchyItems]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DomainID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_DomainID] ON [dbo].[InsertableContents]
(
	[DomainID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[InsertableContents]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[InsertableContents]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldDefinitionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldDefinitionID] ON [dbo].[NumberFieldDefinitions]
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldID] ON [dbo].[NumberFields]
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupAttributesID] ON [dbo].[OverrideCharacteristics]
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblColumnID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblColumnID] ON [dbo].[OverrideCharacteristics]
(
	[TblColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowID] ON [dbo].[OverrideCharacteristics]
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[PointsAdjustments]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[PointsAdjustments]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DomainID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_DomainID] ON [dbo].[PointsManagers]
(
	[DomainID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerUnitID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerUnitID] ON [dbo].[PointsManagers]
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[PointsTotals]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[PointsTotals]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[ProposalEvaluationRatingSettings]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupAttributesID] ON [dbo].[ProposalEvaluationRatingSettings]
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserActionID] ON [dbo].[ProposalEvaluationRatingSettings]
(
	[UserActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[ProposalSettings]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[ProposalSettings]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[RatingCharacteristics]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingPhaseGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingPhaseGroupID] ON [dbo].[RatingCharacteristics]
(
	[RatingPhaseGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubsidyDensityRangeGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_SubsidyDensityRangeGroupID] ON [dbo].[RatingCharacteristics]
(
	[SubsidyDensityRangeGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConditionRatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ConditionRatingID] ON [dbo].[RatingConditions]
(
	[ConditionRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[RatingGroupAttributes]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingCharacteristicsID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingCharacteristicsID] ON [dbo].[RatingGroupAttributes]
(
	[RatingCharacteristicsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingConditionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingConditionID] ON [dbo].[RatingGroupAttributes]
(
	[RatingConditionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupID] ON [dbo].[RatingGroupPhaseStatus]
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingPhaseGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingPhaseGroupID] ON [dbo].[RatingGroupPhaseStatus]
(
	[RatingPhaseGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingPhaseID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingPhaseID] ON [dbo].[RatingGroupPhaseStatus]
(
	[RatingPhaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[RatingGroupResolutions]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupID] ON [dbo].[RatingGroupResolutions]
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupAttributesID] ON [dbo].[RatingGroups]
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblColumnID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblColumnID] ON [dbo].[RatingGroups]
(
	[TblColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowID] ON [dbo].[RatingGroups]
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupID] ON [dbo].[RatingGroupStatusRecords]
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[RatingPhaseGroups]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingPhaseGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingPhaseGroupID] ON [dbo].[RatingPhases]
(
	[RatingPhaseGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupPhaseStatusID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupPhaseStatusID] ON [dbo].[RatingPhaseStatus]
(
	[RatingGroupPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingID] ON [dbo].[RatingPhaseStatus]
(
	[RatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[RatingPlans]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OwnedRatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_OwnedRatingGroupAttributesID] ON [dbo].[RatingPlans]
(
	[OwnedRatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupAttributesID] ON [dbo].[RatingPlans]
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[Ratings]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MostRecentUserRatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_MostRecentUserRatingID] ON [dbo].[Ratings]
(
	[MostRecentUserRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OwnedRatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_OwnedRatingGroupID] ON [dbo].[Ratings]
(
	[OwnedRatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingCharacteristicsID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingCharacteristicsID] ON [dbo].[Ratings]
(
	[RatingCharacteristicsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupID] ON [dbo].[Ratings]
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TopmostRatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TopmostRatingGroupID] ON [dbo].[Ratings]
(
	[TopmostRatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RewardTblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RewardTblRowID] ON [dbo].[RewardPendingPointsTrackers]
(
	[RewardTblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[RewardPendingPointsTrackers]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[RewardRatingSettings]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupAttributesID] ON [dbo].[RewardRatingSettings]
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserActionID] ON [dbo].[RewardRatingSettings]
(
	[UserActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChoiceInGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChoiceInGroupID] ON [dbo].[SearchWordChoices]
(
	[ChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SearchWordID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_SearchWordID] ON [dbo].[SearchWordChoices]
(
	[SearchWordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HierarchyItemID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_HierarchyItemID] ON [dbo].[SearchWordHierarchyItems]
(
	[HierarchyItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SearchWordID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_SearchWordID] ON [dbo].[SearchWordHierarchyItems]
(
	[SearchWordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SearchWordID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_SearchWordID] ON [dbo].[SearchWordTblRowNames]
(
	[SearchWordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowID] ON [dbo].[SearchWordTblRowNames]
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SearchWordID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_SearchWordID] ON [dbo].[SearchWordTextFields]
(
	[SearchWordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TextFieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TextFieldID] ON [dbo].[SearchWordTextFields]
(
	[TextFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupPhaseStatusID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupPhaseStatusID] ON [dbo].[SubsidyAdjustments]
(
	[RatingGroupPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[SubsidyDensityRangeGroups]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubsidyDensityRangeGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_SubsidyDensityRangeGroupID] ON [dbo].[SubsidyDensityRanges]
(
	[SubsidyDensityRangeGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblColumnID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblColumnID] ON [dbo].[TblColumnFormatting]
(
	[TblColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConditionTblColumnID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ConditionTblColumnID] ON [dbo].[TblColumns]
(
	[ConditionTblColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DefaultRatingGroupAttributesID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_DefaultRatingGroupAttributesID] ON [dbo].[TblColumns]
(
	[DefaultRatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblTabID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblTabID] ON [dbo].[TblColumns]
(
	[TblTabID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerUnitID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerUnitID] ON [dbo].[TblColumns]
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[TblRows]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowFieldDisplayID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowFieldDisplayID] ON [dbo].[TblRows]
(
	[TblRowFieldDisplayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowId]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowId] ON [dbo].[TblRowStatusRecord]
(
	[TblRowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Creator]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_Creator] ON [dbo].[Tbls]
(
	[Creator] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[Tbls]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblDimension_TblDimensionsID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblDimension_TblDimensionsID] ON [dbo].[Tbls]
(
	[TblDimension_TblDimensionsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[TblTabs]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldDefinitionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldDefinitionID] ON [dbo].[TextFieldDefinitions]
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FieldID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_FieldID] ON [dbo].[TextFields]
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChoiceInGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChoiceInGroupID] ON [dbo].[TrustTrackerForChoiceInGroups]
(
	[ChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblID] ON [dbo].[TrustTrackerForChoiceInGroups]
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[TrustTrackerForChoiceInGroups]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerForChoiceInGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerForChoiceInGroupID] ON [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]
(
	[TrustTrackerForChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserRatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserRatingID] ON [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]
(
	[UserRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerUnitID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerUnitID] ON [dbo].[TrustTrackers]
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[TrustTrackers]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerID] ON [dbo].[TrustTrackerStats]
(
	[TrustTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UniquenessLockID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UniquenessLockID] ON [dbo].[UniquenessLockReferences]
(
	[UniquenessLockID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[UserCheckIns]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserInfoID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserInfoID] ON [dbo].[UserInfo]
(
	[UserInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_LatestRatingUserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_LatestRatingUserID] ON [dbo].[UserInteractions]
(
	[LatestRatingUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrigRatingUserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrigRatingUserID] ON [dbo].[UserInteractions]
(
	[OrigRatingUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerUnitID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerUnitID] ON [dbo].[UserInteractions]
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerStatID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerStatID] ON [dbo].[UserInteractionStats]
(
	[TrustTrackerStatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserInteractionID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserInteractionID] ON [dbo].[UserInteractionStats]
(
	[UserInteractionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupID] ON [dbo].[UserRatingGroups]
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupPhaseStatusID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupPhaseStatusID] ON [dbo].[UserRatingGroups]
(
	[RatingGroupPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MostRecentUserRatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_MostRecentUserRatingID] ON [dbo].[UserRatings]
(
	[MostRecentUserRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingID] ON [dbo].[UserRatings]
(
	[RatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingPhaseStatusID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingPhaseStatusID] ON [dbo].[UserRatings]
(
	[RatingPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RewardPendingPointsTrackerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RewardPendingPointsTrackerID] ON [dbo].[UserRatings]
(
	[RewardPendingPointsTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TrustTrackerUnitID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TrustTrackerUnitID] ON [dbo].[UserRatings]
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[UserRatings]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserRatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserRatingGroupID] ON [dbo].[UserRatings]
(
	[UserRatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TopRatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TopRatingGroupID] ON [dbo].[UserRatingsToAdd]
(
	[TopRatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[UserRatingsToAdd]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AdministrationRightsGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_AdministrationRightsGroupID] ON [dbo].[UsersAdministrationRightsGroups]
(
	[AdministrationRightsGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[UsersAdministrationRightsGroups]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[UsersAdministrationRightsGroups]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PointsManagerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_PointsManagerID] ON [dbo].[UsersRights]
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[UsersRights]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TblRowID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_TblRowID] ON [dbo].[VolatilityTblRowTrackers]
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RatingGroupID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_RatingGroupID] ON [dbo].[VolatilityTrackers]
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_VolatilityTblRowTrackerID]    Script Date: 7/26/2014 7:24:56 AM ******/
CREATE NONCLUSTERED INDEX [IX_VolatilityTblRowTrackerID] ON [dbo].[VolatilityTrackers]
(
	[VolatilityTblRowTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[aspnet_Paths] ADD  DEFAULT (newid()) FOR [PathId]
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[aspnet_Roles] ADD  DEFAULT (newid()) FOR [RoleId]
GO
ALTER TABLE [dbo].[RatingGroupResolutions] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [WhenCreated]
GO
ALTER TABLE [dbo].[RoleStatus] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [WhenCreated]
GO
ALTER TABLE [dbo].[UserRatingsToAdd] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [WhenCreated]
GO
ALTER TABLE [dbo].[AddressFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AddressFields_dbo.Fields_FieldID] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[AddressFields] CHECK CONSTRAINT [FK_dbo.AddressFields_dbo.Fields_FieldID]
GO
ALTER TABLE [dbo].[AdministrationRights]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministrationRights_dbo.AdministrationRightsGroups_AdministrationRightsGroupID] FOREIGN KEY([AdministrationRightsGroupID])
REFERENCES [dbo].[AdministrationRightsGroups] ([AdministrationRightsGroupID])
GO
ALTER TABLE [dbo].[AdministrationRights] CHECK CONSTRAINT [FK_dbo.AdministrationRights_dbo.AdministrationRightsGroups_AdministrationRightsGroupID]
GO
ALTER TABLE [dbo].[AdministrationRights]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministrationRights_dbo.UserActions_UserActionID] FOREIGN KEY([UserActionID])
REFERENCES [dbo].[UserActions] ([UserActionID])
GO
ALTER TABLE [dbo].[AdministrationRights] CHECK CONSTRAINT [FK_dbo.AdministrationRights_dbo.UserActions_UserActionID]
GO
ALTER TABLE [dbo].[AdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministrationRightsGroups_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[AdministrationRightsGroups] CHECK CONSTRAINT [FK_dbo.AdministrationRightsGroups_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[aspnet_Membership]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_Membership]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[aspnet_Paths]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_PersonalizationAllUsers]  WITH CHECK ADD FOREIGN KEY([PathId])
REFERENCES [dbo].[aspnet_Paths] ([PathId])
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser]  WITH CHECK ADD FOREIGN KEY([PathId])
REFERENCES [dbo].[aspnet_Paths] ([PathId])
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[aspnet_Profile]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[aspnet_Roles]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_Users]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_UsersInRoles]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[aspnet_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[aspnet_UsersInRoles]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChangesGroup_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [FK_dbo.ChangesGroup_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChangesGroup_dbo.Ratings_RewardRatingID] FOREIGN KEY([RewardRatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [FK_dbo.ChangesGroup_dbo.Ratings_RewardRatingID]
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChangesGroup_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [FK_dbo.ChangesGroup_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChangesGroup_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [FK_dbo.ChangesGroup_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[ChangesStatusOfObject]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChangesStatusOfObject_dbo.ChangesGroup_ChangesGroupID] FOREIGN KEY([ChangesGroupID])
REFERENCES [dbo].[ChangesGroup] ([ChangesGroupID])
GO
ALTER TABLE [dbo].[ChangesStatusOfObject] CHECK CONSTRAINT [FK_dbo.ChangesStatusOfObject_dbo.ChangesGroup_ChangesGroupID]
GO
ALTER TABLE [dbo].[ChoiceFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceFields_dbo.Fields_FieldID] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[ChoiceFields] CHECK CONSTRAINT [FK_dbo.ChoiceFields_dbo.Fields_FieldID]
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceGroupFieldDefinitions_dbo.ChoiceGroupFieldDefinitions_DependentOnChoiceGroupFieldDefinitionID] FOREIGN KEY([DependentOnChoiceGroupFieldDefinitionID])
REFERENCES [dbo].[ChoiceGroupFieldDefinitions] ([ChoiceGroupFieldDefinitionID])
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions] CHECK CONSTRAINT [FK_dbo.ChoiceGroupFieldDefinitions_dbo.ChoiceGroupFieldDefinitions_DependentOnChoiceGroupFieldDefinitionID]
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceGroupFieldDefinitions_dbo.ChoiceGroups_ChoiceGroupID] FOREIGN KEY([ChoiceGroupID])
REFERENCES [dbo].[ChoiceGroups] ([ChoiceGroupID])
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions] CHECK CONSTRAINT [FK_dbo.ChoiceGroupFieldDefinitions_dbo.ChoiceGroups_ChoiceGroupID]
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceGroupFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions] CHECK CONSTRAINT [FK_dbo.ChoiceGroupFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID]
GO
ALTER TABLE [dbo].[ChoiceGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceGroups_dbo.ChoiceGroups_DependentOnChoiceGroupID] FOREIGN KEY([DependentOnChoiceGroupID])
REFERENCES [dbo].[ChoiceGroups] ([ChoiceGroupID])
GO
ALTER TABLE [dbo].[ChoiceGroups] CHECK CONSTRAINT [FK_dbo.ChoiceGroups_dbo.ChoiceGroups_DependentOnChoiceGroupID]
GO
ALTER TABLE [dbo].[ChoiceGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceGroups_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ChoiceGroups] CHECK CONSTRAINT [FK_dbo.ChoiceGroups_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[ChoiceGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceGroups_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[ChoiceGroups] CHECK CONSTRAINT [FK_dbo.ChoiceGroups_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[ChoiceInFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceInFields_dbo.ChoiceFields_ChoiceFieldID] FOREIGN KEY([ChoiceFieldID])
REFERENCES [dbo].[ChoiceFields] ([ChoiceFieldID])
GO
ALTER TABLE [dbo].[ChoiceInFields] CHECK CONSTRAINT [FK_dbo.ChoiceInFields_dbo.ChoiceFields_ChoiceFieldID]
GO
ALTER TABLE [dbo].[ChoiceInFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceInFields_dbo.ChoiceInGroups_ChoiceInGroupID] FOREIGN KEY([ChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[ChoiceInFields] CHECK CONSTRAINT [FK_dbo.ChoiceInFields_dbo.ChoiceInGroups_ChoiceInGroupID]
GO
ALTER TABLE [dbo].[ChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceInGroups_dbo.ChoiceGroups_ChoiceGroupID] FOREIGN KEY([ChoiceGroupID])
REFERENCES [dbo].[ChoiceGroups] ([ChoiceGroupID])
GO
ALTER TABLE [dbo].[ChoiceInGroups] CHECK CONSTRAINT [FK_dbo.ChoiceInGroups_dbo.ChoiceGroups_ChoiceGroupID]
GO
ALTER TABLE [dbo].[ChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChoiceInGroups_dbo.ChoiceInGroups_ActiveOnDeterminingGroupChoiceInGroupID] FOREIGN KEY([ActiveOnDeterminingGroupChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[ChoiceInGroups] CHECK CONSTRAINT [FK_dbo.ChoiceInGroups_dbo.ChoiceInGroups_ActiveOnDeterminingGroupChoiceInGroupID]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Comments_dbo.TblRows_TblRowID] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_dbo.Comments_dbo.TblRows_TblRowID]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Comments_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_dbo.Comments_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[DateTimeFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DateTimeFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[DateTimeFieldDefinitions] CHECK CONSTRAINT [FK_dbo.DateTimeFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID]
GO
ALTER TABLE [dbo].[DateTimeFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DateTimeFields_dbo.Fields_FieldID] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[DateTimeFields] CHECK CONSTRAINT [FK_dbo.DateTimeFields_dbo.Fields_FieldID]
GO
ALTER TABLE [dbo].[Domains]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Domains_dbo.TblDimensions_TblDimensionsID] FOREIGN KEY([TblDimensionsID])
REFERENCES [dbo].[TblDimensions] ([TblDimensionsID])
GO
ALTER TABLE [dbo].[Domains] CHECK CONSTRAINT [FK_dbo.Domains_dbo.TblDimensions_TblDimensionsID]
GO
ALTER TABLE [dbo].[FieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FieldDefinitions_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[FieldDefinitions] CHECK CONSTRAINT [FK_dbo.FieldDefinitions_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Fields_dbo.FieldDefinitions_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_dbo.Fields_dbo.FieldDefinitions_FieldDefinitionID]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Fields_dbo.TblRows_TblRowID] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_dbo.Fields_dbo.TblRows_TblRowID]
GO
ALTER TABLE [dbo].[HierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HierarchyItems_dbo.HierarchyItems_HigherHierarchyItemID] FOREIGN KEY([ParentHierarchyItemID])
REFERENCES [dbo].[HierarchyItems] ([HierarchyItemID])
GO
ALTER TABLE [dbo].[HierarchyItems] CHECK CONSTRAINT [FK_dbo.HierarchyItems_dbo.HierarchyItems_HigherHierarchyItemID]
GO
ALTER TABLE [dbo].[HierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HierarchyItems_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[HierarchyItems] CHECK CONSTRAINT [FK_dbo.HierarchyItems_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[InsertableContents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InsertableContents_dbo.Domains_DomainID] FOREIGN KEY([DomainID])
REFERENCES [dbo].[Domains] ([DomainID])
GO
ALTER TABLE [dbo].[InsertableContents] CHECK CONSTRAINT [FK_dbo.InsertableContents_dbo.Domains_DomainID]
GO
ALTER TABLE [dbo].[InsertableContents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InsertableContents_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[InsertableContents] CHECK CONSTRAINT [FK_dbo.InsertableContents_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[InsertableContents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InsertableContents_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[InsertableContents] CHECK CONSTRAINT [FK_dbo.InsertableContents_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[NumberFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.NumberFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[NumberFieldDefinitions] CHECK CONSTRAINT [FK_dbo.NumberFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID]
GO
ALTER TABLE [dbo].[NumberFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.NumberFields_dbo.Fields_FieldID] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[NumberFields] CHECK CONSTRAINT [FK_dbo.NumberFields_dbo.Fields_FieldID]
GO
ALTER TABLE [dbo].[OverrideCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OverrideCharacteristics_dbo.RatingGroupAttributes_RatingGroupAttributesID] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[OverrideCharacteristics] CHECK CONSTRAINT [FK_dbo.OverrideCharacteristics_dbo.RatingGroupAttributes_RatingGroupAttributesID]
GO
ALTER TABLE [dbo].[OverrideCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OverrideCharacteristics_dbo.TblColumns_TblColumnID] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[OverrideCharacteristics] CHECK CONSTRAINT [FK_dbo.OverrideCharacteristics_dbo.TblColumns_TblColumnID]
GO
ALTER TABLE [dbo].[OverrideCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OverrideCharacteristics_dbo.TblRows_TblRowID] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[OverrideCharacteristics] CHECK CONSTRAINT [FK_dbo.OverrideCharacteristics_dbo.TblRows_TblRowID]
GO
ALTER TABLE [dbo].[PointsAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PointsAdjustments_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[PointsAdjustments] CHECK CONSTRAINT [FK_dbo.PointsAdjustments_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[PointsAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PointsAdjustments_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[PointsAdjustments] CHECK CONSTRAINT [FK_dbo.PointsAdjustments_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[PointsManagers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PointsManagers_dbo.Domains_DomainID] FOREIGN KEY([DomainID])
REFERENCES [dbo].[Domains] ([DomainID])
GO
ALTER TABLE [dbo].[PointsManagers] CHECK CONSTRAINT [FK_dbo.PointsManagers_dbo.Domains_DomainID]
GO
ALTER TABLE [dbo].[PointsManagers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PointsManagers_dbo.TrustTrackerUnits_TrustTrackerUnitID] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[PointsManagers] CHECK CONSTRAINT [FK_dbo.PointsManagers_dbo.TrustTrackerUnits_TrustTrackerUnitID]
GO
ALTER TABLE [dbo].[PointsTotals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PointsTotals_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[PointsTotals] CHECK CONSTRAINT [FK_dbo.PointsTotals_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[PointsTotals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PointsTotals_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[PointsTotals] CHECK CONSTRAINT [FK_dbo.PointsTotals_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProposalEvaluationRatingSettings_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings] CHECK CONSTRAINT [FK_dbo.ProposalEvaluationRatingSettings_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProposalEvaluationRatingSettings_dbo.RatingGroupAttributes_RatingGroupAttributesID] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings] CHECK CONSTRAINT [FK_dbo.ProposalEvaluationRatingSettings_dbo.RatingGroupAttributes_RatingGroupAttributesID]
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProposalEvaluationRatingSettings_dbo.UserActions_UserActionID] FOREIGN KEY([UserActionID])
REFERENCES [dbo].[UserActions] ([UserActionID])
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings] CHECK CONSTRAINT [FK_dbo.ProposalEvaluationRatingSettings_dbo.UserActions_UserActionID]
GO
ALTER TABLE [dbo].[ProposalSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProposalSettings_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ProposalSettings] CHECK CONSTRAINT [FK_dbo.ProposalSettings_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[ProposalSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProposalSettings_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[ProposalSettings] CHECK CONSTRAINT [FK_dbo.ProposalSettings_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[RatingCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingCharacteristics_dbo.RatingPhaseGroups_RatingPhaseGroupID] FOREIGN KEY([RatingPhaseGroupID])
REFERENCES [dbo].[RatingPhaseGroups] ([RatingPhaseGroupID])
GO
ALTER TABLE [dbo].[RatingCharacteristics] CHECK CONSTRAINT [FK_dbo.RatingCharacteristics_dbo.RatingPhaseGroups_RatingPhaseGroupID]
GO
ALTER TABLE [dbo].[RatingCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingCharacteristics_dbo.SubsidyDensityRangeGroups_SubsidyDensityRangeGroupID] FOREIGN KEY([SubsidyDensityRangeGroupID])
REFERENCES [dbo].[SubsidyDensityRangeGroups] ([SubsidyDensityRangeGroupID])
GO
ALTER TABLE [dbo].[RatingCharacteristics] CHECK CONSTRAINT [FK_dbo.RatingCharacteristics_dbo.SubsidyDensityRangeGroups_SubsidyDensityRangeGroupID]
GO
ALTER TABLE [dbo].[RatingCharacteristics]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingCharacteristics_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingCharacteristics] CHECK CONSTRAINT [FK_dbo.RatingCharacteristics_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[RatingConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingConditions_dbo.Ratings_ConditionRatingID] FOREIGN KEY([ConditionRatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[RatingConditions] CHECK CONSTRAINT [FK_dbo.RatingConditions_dbo.Ratings_ConditionRatingID]
GO
ALTER TABLE [dbo].[RatingGroupAttributes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupAttributes_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[RatingGroupAttributes] CHECK CONSTRAINT [FK_dbo.RatingGroupAttributes_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[RatingGroupAttributes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupAttributes_dbo.RatingCharacteristics_RatingCharacteristicsID] FOREIGN KEY([RatingCharacteristicsID])
REFERENCES [dbo].[RatingCharacteristics] ([RatingCharacteristicsID])
GO
ALTER TABLE [dbo].[RatingGroupAttributes] CHECK CONSTRAINT [FK_dbo.RatingGroupAttributes_dbo.RatingCharacteristics_RatingCharacteristicsID]
GO
ALTER TABLE [dbo].[RatingGroupAttributes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupAttributes_dbo.RatingConditions_RatingConditionID] FOREIGN KEY([RatingConditionID])
REFERENCES [dbo].[RatingConditions] ([RatingConditionID])
GO
ALTER TABLE [dbo].[RatingGroupAttributes] CHECK CONSTRAINT [FK_dbo.RatingGroupAttributes_dbo.RatingConditions_RatingConditionID]
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupPhaseStatus_dbo.RatingGroups_RatingGroupID] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus] CHECK CONSTRAINT [FK_dbo.RatingGroupPhaseStatus_dbo.RatingGroups_RatingGroupID]
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupPhaseStatus_dbo.RatingPhaseGroups_RatingPhaseGroupID] FOREIGN KEY([RatingPhaseGroupID])
REFERENCES [dbo].[RatingPhaseGroups] ([RatingPhaseGroupID])
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus] CHECK CONSTRAINT [FK_dbo.RatingGroupPhaseStatus_dbo.RatingPhaseGroups_RatingPhaseGroupID]
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupPhaseStatus_dbo.RatingPhases_RatingPhaseID] FOREIGN KEY([RatingPhaseID])
REFERENCES [dbo].[RatingPhases] ([RatingPhaseID])
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus] CHECK CONSTRAINT [FK_dbo.RatingGroupPhaseStatus_dbo.RatingPhases_RatingPhaseID]
GO
ALTER TABLE [dbo].[RatingGroupResolutions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupResolutions_dbo.RatingGroups_RatingGroupID] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[RatingGroupResolutions] CHECK CONSTRAINT [FK_dbo.RatingGroupResolutions_dbo.RatingGroups_RatingGroupID]
GO
ALTER TABLE [dbo].[RatingGroupResolutions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupResolutions_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingGroupResolutions] CHECK CONSTRAINT [FK_dbo.RatingGroupResolutions_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[RatingGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroups_dbo.RatingGroupAttributes_RatingGroupAttributesID] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RatingGroups] CHECK CONSTRAINT [FK_dbo.RatingGroups_dbo.RatingGroupAttributes_RatingGroupAttributesID]
GO
ALTER TABLE [dbo].[RatingGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroups_dbo.TblColumns_TblColumnID] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[RatingGroups] CHECK CONSTRAINT [FK_dbo.RatingGroups_dbo.TblColumns_TblColumnID]
GO
ALTER TABLE [dbo].[RatingGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroups_dbo.TblRows_TblRowID] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[RatingGroups] CHECK CONSTRAINT [FK_dbo.RatingGroups_dbo.TblRows_TblRowID]
GO
ALTER TABLE [dbo].[RatingGroupStatusRecords]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingGroupStatusRecords_dbo.RatingGroups_RatingGroupID] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[RatingGroupStatusRecords] CHECK CONSTRAINT [FK_dbo.RatingGroupStatusRecords_dbo.RatingGroups_RatingGroupID]
GO
ALTER TABLE [dbo].[RatingPhaseGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPhaseGroups_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingPhaseGroups] CHECK CONSTRAINT [FK_dbo.RatingPhaseGroups_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[RatingPhases]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPhases_dbo.RatingPhaseGroups_RatingPhaseGroupID] FOREIGN KEY([RatingPhaseGroupID])
REFERENCES [dbo].[RatingPhaseGroups] ([RatingPhaseGroupID])
GO
ALTER TABLE [dbo].[RatingPhases] CHECK CONSTRAINT [FK_dbo.RatingPhases_dbo.RatingPhaseGroups_RatingPhaseGroupID]
GO
ALTER TABLE [dbo].[RatingPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPhaseStatus_dbo.RatingGroupPhaseStatus_RatingGroupPhaseStatusID] FOREIGN KEY([RatingGroupPhaseStatusID])
REFERENCES [dbo].[RatingGroupPhaseStatus] ([RatingGroupPhaseStatusID])
GO
ALTER TABLE [dbo].[RatingPhaseStatus] CHECK CONSTRAINT [FK_dbo.RatingPhaseStatus_dbo.RatingGroupPhaseStatus_RatingGroupPhaseStatusID]
GO
ALTER TABLE [dbo].[RatingPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPhaseStatus_dbo.Ratings_RatingID] FOREIGN KEY([RatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[RatingPhaseStatus] CHECK CONSTRAINT [FK_dbo.RatingPhaseStatus_dbo.Ratings_RatingID]
GO
ALTER TABLE [dbo].[RatingPlans]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPlans_dbo.RatingGroupAttributes_OwnedRatingGroupAttributesID] FOREIGN KEY([OwnedRatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RatingPlans] CHECK CONSTRAINT [FK_dbo.RatingPlans_dbo.RatingGroupAttributes_OwnedRatingGroupAttributesID]
GO
ALTER TABLE [dbo].[RatingPlans]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPlans_dbo.RatingGroupAttributes_RatingGroupAttributesID] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RatingPlans] CHECK CONSTRAINT [FK_dbo.RatingPlans_dbo.RatingGroupAttributes_RatingGroupAttributesID]
GO
ALTER TABLE [dbo].[RatingPlans]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RatingPlans_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingPlans] CHECK CONSTRAINT [FK_dbo.RatingPlans_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ratings_dbo.RatingCharacteristics_RatingCharacteristicsID] FOREIGN KEY([RatingCharacteristicsID])
REFERENCES [dbo].[RatingCharacteristics] ([RatingCharacteristicsID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_dbo.Ratings_dbo.RatingCharacteristics_RatingCharacteristicsID]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ratings_dbo.RatingGroups_OwnedRatingGroupID] FOREIGN KEY([OwnedRatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_dbo.Ratings_dbo.RatingGroups_OwnedRatingGroupID]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ratings_dbo.RatingGroups_RatingGroupID] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_dbo.Ratings_dbo.RatingGroups_RatingGroupID]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ratings_dbo.RatingGroups_TopmostRatingGroupID] FOREIGN KEY([TopmostRatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_dbo.Ratings_dbo.RatingGroups_TopmostRatingGroupID]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ratings_dbo.UserRatings_MostRecentUserRatingID] FOREIGN KEY([MostRecentUserRatingID])
REFERENCES [dbo].[UserRatings] ([UserRatingID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_dbo.Ratings_dbo.UserRatings_MostRecentUserRatingID]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ratings_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_dbo.Ratings_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RewardPendingPointsTrackers_dbo.TblRows_RewardTblRowID] FOREIGN KEY([RewardTblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers] CHECK CONSTRAINT [FK_dbo.RewardPendingPointsTrackers_dbo.TblRows_RewardTblRowID]
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RewardPendingPointsTrackers_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers] CHECK CONSTRAINT [FK_dbo.RewardPendingPointsTrackers_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[RewardRatingSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RewardRatingSettings_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[RewardRatingSettings] CHECK CONSTRAINT [FK_dbo.RewardRatingSettings_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[RewardRatingSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RewardRatingSettings_dbo.RatingGroupAttributes_RatingGroupAttributesID] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RewardRatingSettings] CHECK CONSTRAINT [FK_dbo.RewardRatingSettings_dbo.RatingGroupAttributes_RatingGroupAttributesID]
GO
ALTER TABLE [dbo].[RewardRatingSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RewardRatingSettings_dbo.UserActions_UserActionID] FOREIGN KEY([UserActionID])
REFERENCES [dbo].[UserActions] ([UserActionID])
GO
ALTER TABLE [dbo].[RewardRatingSettings] CHECK CONSTRAINT [FK_dbo.RewardRatingSettings_dbo.UserActions_UserActionID]
GO
ALTER TABLE [dbo].[SearchWordChoices]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordChoices_dbo.ChoiceInGroups_ChoiceInGroupID] FOREIGN KEY([ChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[SearchWordChoices] CHECK CONSTRAINT [FK_dbo.SearchWordChoices_dbo.ChoiceInGroups_ChoiceInGroupID]
GO
ALTER TABLE [dbo].[SearchWordChoices]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordChoices_dbo.SearchWords_SearchWordID] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordChoices] CHECK CONSTRAINT [FK_dbo.SearchWordChoices_dbo.SearchWords_SearchWordID]
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordHierarchyItems_dbo.HierarchyItems_HierarchyItemID] FOREIGN KEY([HierarchyItemID])
REFERENCES [dbo].[HierarchyItems] ([HierarchyItemID])
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems] CHECK CONSTRAINT [FK_dbo.SearchWordHierarchyItems_dbo.HierarchyItems_HierarchyItemID]
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordHierarchyItems_dbo.SearchWords_SearchWordID] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems] CHECK CONSTRAINT [FK_dbo.SearchWordHierarchyItems_dbo.SearchWords_SearchWordID]
GO
ALTER TABLE [dbo].[SearchWordTblRowNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordTblRowNames_dbo.SearchWords_SearchWordID] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordTblRowNames] CHECK CONSTRAINT [FK_dbo.SearchWordTblRowNames_dbo.SearchWords_SearchWordID]
GO
ALTER TABLE [dbo].[SearchWordTblRowNames]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordTblRowNames_dbo.TblRows_TblRowID] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[SearchWordTblRowNames] CHECK CONSTRAINT [FK_dbo.SearchWordTblRowNames_dbo.TblRows_TblRowID]
GO
ALTER TABLE [dbo].[SearchWordTextFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordTextFields_dbo.SearchWords_SearchWordID] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordTextFields] CHECK CONSTRAINT [FK_dbo.SearchWordTextFields_dbo.SearchWords_SearchWordID]
GO
ALTER TABLE [dbo].[SearchWordTextFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchWordTextFields_dbo.TextFields_TextFieldID] FOREIGN KEY([TextFieldID])
REFERENCES [dbo].[TextFields] ([TextFieldID])
GO
ALTER TABLE [dbo].[SearchWordTextFields] CHECK CONSTRAINT [FK_dbo.SearchWordTextFields_dbo.TextFields_TextFieldID]
GO
ALTER TABLE [dbo].[SubsidyAdjustments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubsidyAdjustments_dbo.RatingGroupPhaseStatus_RatingGroupPhaseStatusID] FOREIGN KEY([RatingGroupPhaseStatusID])
REFERENCES [dbo].[RatingGroupPhaseStatus] ([RatingGroupPhaseStatusID])
GO
ALTER TABLE [dbo].[SubsidyAdjustments] CHECK CONSTRAINT [FK_dbo.SubsidyAdjustments_dbo.RatingGroupPhaseStatus_RatingGroupPhaseStatusID]
GO
ALTER TABLE [dbo].[SubsidyDensityRangeGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubsidyDensityRangeGroups_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[SubsidyDensityRangeGroups] CHECK CONSTRAINT [FK_dbo.SubsidyDensityRangeGroups_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[SubsidyDensityRanges]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubsidyDensityRanges_dbo.SubsidyDensityRangeGroups_SubsidyDensityRangeGroupID] FOREIGN KEY([SubsidyDensityRangeGroupID])
REFERENCES [dbo].[SubsidyDensityRangeGroups] ([SubsidyDensityRangeGroupID])
GO
ALTER TABLE [dbo].[SubsidyDensityRanges] CHECK CONSTRAINT [FK_dbo.SubsidyDensityRanges_dbo.SubsidyDensityRangeGroups_SubsidyDensityRangeGroupID]
GO
ALTER TABLE [dbo].[TblColumnFormatting]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblColumnFormatting_dbo.TblColumns_TblColumnID] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[TblColumnFormatting] CHECK CONSTRAINT [FK_dbo.TblColumnFormatting_dbo.TblColumns_TblColumnID]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblColumns_dbo.RatingGroupAttributes_DefaultRatingGroupAttributesID] FOREIGN KEY([DefaultRatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [FK_dbo.TblColumns_dbo.RatingGroupAttributes_DefaultRatingGroupAttributesID]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblColumns_dbo.TblColumns_ConditionTblColumnID] FOREIGN KEY([ConditionTblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [FK_dbo.TblColumns_dbo.TblColumns_ConditionTblColumnID]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblColumns_dbo.TblTabs_TblTabID] FOREIGN KEY([TblTabID])
REFERENCES [dbo].[TblTabs] ([TblTabID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [FK_dbo.TblColumns_dbo.TblTabs_TblTabID]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblColumns_dbo.TrustTrackerUnits_TrustTrackerUnitID] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [FK_dbo.TblColumns_dbo.TrustTrackerUnits_TrustTrackerUnitID]
GO
ALTER TABLE [dbo].[TblRows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblRows_dbo.TblRowFieldDisplays_TblRowFieldDisplayID] FOREIGN KEY([TblRowFieldDisplayID])
REFERENCES [dbo].[TblRowFieldDisplays] ([TblRowFieldDisplayID])
GO
ALTER TABLE [dbo].[TblRows] CHECK CONSTRAINT [FK_dbo.TblRows_dbo.TblRowFieldDisplays_TblRowFieldDisplayID]
GO
ALTER TABLE [dbo].[TblRows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblRows_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[TblRows] CHECK CONSTRAINT [FK_dbo.TblRows_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[TblRowStatusRecord]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblRowStatusRecord_dbo.TblRows_TblRowId] FOREIGN KEY([TblRowId])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[TblRowStatusRecord] CHECK CONSTRAINT [FK_dbo.TblRowStatusRecord_dbo.TblRows_TblRowId]
GO
ALTER TABLE [dbo].[Tbls]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tbls_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[Tbls] CHECK CONSTRAINT [FK_dbo.Tbls_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[Tbls]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tbls_dbo.TblDimensions_TblDimension_TblDimensionsID] FOREIGN KEY([TblDimension_TblDimensionsID])
REFERENCES [dbo].[TblDimensions] ([TblDimensionsID])
GO
ALTER TABLE [dbo].[Tbls] CHECK CONSTRAINT [FK_dbo.Tbls_dbo.TblDimensions_TblDimension_TblDimensionsID]
GO
ALTER TABLE [dbo].[Tbls]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tbls_dbo.Users_Creator] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Tbls] CHECK CONSTRAINT [FK_dbo.Tbls_dbo.Users_Creator]
GO
ALTER TABLE [dbo].[TblTabs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TblTabs_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TblTabs] CHECK CONSTRAINT [FK_dbo.TblTabs_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[TextFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TextFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[TextFieldDefinitions] CHECK CONSTRAINT [FK_dbo.TextFieldDefinitions_dbo.FieldDefinitions_FieldDefinitionID]
GO
ALTER TABLE [dbo].[TextFields]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TextFields_dbo.Fields_FieldID] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[TextFields] CHECK CONSTRAINT [FK_dbo.TextFields_dbo.Fields_FieldID]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroups_dbo.ChoiceInGroups_ChoiceInGroupID] FOREIGN KEY([ChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] CHECK CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroups_dbo.ChoiceInGroups_ChoiceInGroupID]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroups_dbo.Tbls_TblID] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] CHECK CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroups_dbo.Tbls_TblID]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroups_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] CHECK CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroups_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroupsUserRatingLinks_dbo.TrustTrackerForChoiceInGroups_TrustTrackerForChoiceInGroupID] FOREIGN KEY([TrustTrackerForChoiceInGroupID])
REFERENCES [dbo].[TrustTrackerForChoiceInGroups] ([TrustTrackerForChoiceInGroupID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks] CHECK CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroupsUserRatingLinks_dbo.TrustTrackerForChoiceInGroups_TrustTrackerForChoiceInGroupID]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroupsUserRatingLinks_dbo.UserRatings_UserRatingID] FOREIGN KEY([UserRatingID])
REFERENCES [dbo].[UserRatings] ([UserRatingID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks] CHECK CONSTRAINT [FK_dbo.TrustTrackerForChoiceInGroupsUserRatingLinks_dbo.UserRatings_UserRatingID]
GO
ALTER TABLE [dbo].[TrustTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackers_dbo.TrustTrackerUnits_TrustTrackerUnitID] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[TrustTrackers] CHECK CONSTRAINT [FK_dbo.TrustTrackers_dbo.TrustTrackerUnits_TrustTrackerUnitID]
GO
ALTER TABLE [dbo].[TrustTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackers_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[TrustTrackers] CHECK CONSTRAINT [FK_dbo.TrustTrackers_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[TrustTrackerStats]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrustTrackerStats_dbo.TrustTrackers_TrustTrackerID] FOREIGN KEY([TrustTrackerID])
REFERENCES [dbo].[TrustTrackers] ([TrustTrackerID])
GO
ALTER TABLE [dbo].[TrustTrackerStats] CHECK CONSTRAINT [FK_dbo.TrustTrackerStats_dbo.TrustTrackers_TrustTrackerID]
GO
ALTER TABLE [dbo].[UniquenessLockReferences]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UniquenessLockReferences_dbo.UniquenessLocks_UniquenessLockID] FOREIGN KEY([UniquenessLockID])
REFERENCES [dbo].[UniquenessLocks] ([Id])
GO
ALTER TABLE [dbo].[UniquenessLockReferences] CHECK CONSTRAINT [FK_dbo.UniquenessLockReferences_dbo.UniquenessLocks_UniquenessLockID]
GO
ALTER TABLE [dbo].[UserCheckIns]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserCheckIns_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserCheckIns] CHECK CONSTRAINT [FK_dbo.UserCheckIns_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[UserInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInfo_dbo.Users_UserInfoID] FOREIGN KEY([UserInfoID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserInfo] CHECK CONSTRAINT [FK_dbo.UserInfo_dbo.Users_UserInfoID]
GO
ALTER TABLE [dbo].[UserInteractions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInteractions_dbo.TrustTrackerUnits_TrustTrackerUnitID] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[UserInteractions] CHECK CONSTRAINT [FK_dbo.UserInteractions_dbo.TrustTrackerUnits_TrustTrackerUnitID]
GO
ALTER TABLE [dbo].[UserInteractions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInteractions_dbo.Users_LatestRatingUserID] FOREIGN KEY([LatestRatingUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserInteractions] CHECK CONSTRAINT [FK_dbo.UserInteractions_dbo.Users_LatestRatingUserID]
GO
ALTER TABLE [dbo].[UserInteractions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInteractions_dbo.Users_OrigRatingUserID] FOREIGN KEY([OrigRatingUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserInteractions] CHECK CONSTRAINT [FK_dbo.UserInteractions_dbo.Users_OrigRatingUserID]
GO
ALTER TABLE [dbo].[UserInteractionStats]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInteractionStats_dbo.TrustTrackerStats_TrustTrackerStatID] FOREIGN KEY([TrustTrackerStatID])
REFERENCES [dbo].[TrustTrackerStats] ([TrustTrackerStatID])
GO
ALTER TABLE [dbo].[UserInteractionStats] CHECK CONSTRAINT [FK_dbo.UserInteractionStats_dbo.TrustTrackerStats_TrustTrackerStatID]
GO
ALTER TABLE [dbo].[UserInteractionStats]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserInteractionStats_dbo.UserInteractions_UserInteractionID] FOREIGN KEY([UserInteractionID])
REFERENCES [dbo].[UserInteractions] ([UserInteractionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserInteractionStats] CHECK CONSTRAINT [FK_dbo.UserInteractionStats_dbo.UserInteractions_UserInteractionID]
GO
ALTER TABLE [dbo].[UserRatingGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatingGroups_dbo.RatingGroupPhaseStatus_RatingGroupPhaseStatusID] FOREIGN KEY([RatingGroupPhaseStatusID])
REFERENCES [dbo].[RatingGroupPhaseStatus] ([RatingGroupPhaseStatusID])
GO
ALTER TABLE [dbo].[UserRatingGroups] CHECK CONSTRAINT [FK_dbo.UserRatingGroups_dbo.RatingGroupPhaseStatus_RatingGroupPhaseStatusID]
GO
ALTER TABLE [dbo].[UserRatingGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatingGroups_dbo.RatingGroups_RatingGroupID] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[UserRatingGroups] CHECK CONSTRAINT [FK_dbo.UserRatingGroups_dbo.RatingGroups_RatingGroupID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.RatingPhaseStatus_RatingPhaseStatusID] FOREIGN KEY([RatingPhaseStatusID])
REFERENCES [dbo].[RatingPhaseStatus] ([RatingPhaseStatusID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.RatingPhaseStatus_RatingPhaseStatusID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.Ratings_RatingID] FOREIGN KEY([RatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.Ratings_RatingID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.RewardPendingPointsTrackers_RewardPendingPointsTrackerID] FOREIGN KEY([RewardPendingPointsTrackerID])
REFERENCES [dbo].[RewardPendingPointsTrackers] ([RewardPendingPointsTrackerID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.RewardPendingPointsTrackers_RewardPendingPointsTrackerID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.TrustTrackerUnits_TrustTrackerUnitID] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.TrustTrackerUnits_TrustTrackerUnitID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.UserRatingGroups_UserRatingGroupID] FOREIGN KEY([UserRatingGroupID])
REFERENCES [dbo].[UserRatingGroups] ([UserRatingGroupID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.UserRatingGroups_UserRatingGroupID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.UserRatings_MostRecentUserRatingID] FOREIGN KEY([MostRecentUserRatingID])
REFERENCES [dbo].[UserRatings] ([UserRatingID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.UserRatings_MostRecentUserRatingID]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatings_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_dbo.UserRatings_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[UserRatingsToAdd]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatingsToAdd_dbo.RatingGroups_TopRatingGroupID] FOREIGN KEY([TopRatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[UserRatingsToAdd] CHECK CONSTRAINT [FK_dbo.UserRatingsToAdd_dbo.RatingGroups_TopRatingGroupID]
GO
ALTER TABLE [dbo].[UserRatingsToAdd]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRatingsToAdd_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserRatingsToAdd] CHECK CONSTRAINT [FK_dbo.UserRatingsToAdd_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersAdministrationRightsGroups_dbo.AdministrationRightsGroups_AdministrationRightsGroupID] FOREIGN KEY([AdministrationRightsGroupID])
REFERENCES [dbo].[AdministrationRightsGroups] ([AdministrationRightsGroupID])
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups] CHECK CONSTRAINT [FK_dbo.UsersAdministrationRightsGroups_dbo.AdministrationRightsGroups_AdministrationRightsGroupID]
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersAdministrationRightsGroups_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups] CHECK CONSTRAINT [FK_dbo.UsersAdministrationRightsGroups_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersAdministrationRightsGroups_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups] CHECK CONSTRAINT [FK_dbo.UsersAdministrationRightsGroups_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[UsersRights]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersRights_dbo.PointsManagers_PointsManagerID] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[UsersRights] CHECK CONSTRAINT [FK_dbo.UsersRights_dbo.PointsManagers_PointsManagerID]
GO
ALTER TABLE [dbo].[UsersRights]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersRights_dbo.Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UsersRights] CHECK CONSTRAINT [FK_dbo.UsersRights_dbo.Users_UserID]
GO
ALTER TABLE [dbo].[VolatilityTblRowTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.VolatilityTblRowTrackers_dbo.TblRows_TblRowID] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[VolatilityTblRowTrackers] CHECK CONSTRAINT [FK_dbo.VolatilityTblRowTrackers_dbo.TblRows_TblRowID]
GO
ALTER TABLE [dbo].[VolatilityTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.VolatilityTrackers_dbo.RatingGroups_RatingGroupID] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[VolatilityTrackers] CHECK CONSTRAINT [FK_dbo.VolatilityTrackers_dbo.RatingGroups_RatingGroupID]
GO
ALTER TABLE [dbo].[VolatilityTrackers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.VolatilityTrackers_dbo.VolatilityTblRowTrackers_VolatilityTblRowTrackerID] FOREIGN KEY([VolatilityTblRowTrackerID])
REFERENCES [dbo].[VolatilityTblRowTrackers] ([VolatilityTblRowTrackerID])
GO
ALTER TABLE [dbo].[VolatilityTrackers] CHECK CONSTRAINT [FK_dbo.VolatilityTrackers_dbo.VolatilityTblRowTrackers_VolatilityTblRowTrackerID]
GO
USE [master]
GO
ALTER DATABASE [RD0001] SET  READ_WRITE 
GO
