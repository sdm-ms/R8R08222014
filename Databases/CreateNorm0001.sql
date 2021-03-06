USE [master]
GO
/****** Object:  Database [Norm0001]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE DATABASE [Norm0001]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Norm0001', FILENAME = N'C:\SQLData\Norm0001.mdf' , SIZE = 113792KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Norm0001_log', FILENAME = N'C:\SQLData\Norm0001_log.ldf' , SIZE = 353216KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Norm0001] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Norm0001].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Norm0001] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Norm0001] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Norm0001] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Norm0001] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Norm0001] SET ARITHABORT OFF 
GO
ALTER DATABASE [Norm0001] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Norm0001] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Norm0001] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Norm0001] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Norm0001] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Norm0001] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Norm0001] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Norm0001] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Norm0001] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Norm0001] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Norm0001] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Norm0001] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Norm0001] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Norm0001] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Norm0001] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Norm0001] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Norm0001] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Norm0001] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Norm0001] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Norm0001] SET  MULTI_USER 
GO
ALTER DATABASE [Norm0001] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Norm0001] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Norm0001] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Norm0001] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Norm0001', N'ON'
GO
USE [Norm0001]
GO
/****** Object:  User [NT AUTHORITY\NETWORK SERVICE]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  DatabaseRole [aspnet_WebEvent_FullAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_WebEvent_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_ReportingAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Roles_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_FullAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Roles_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_BasicAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Roles_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_ReportingAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Profile_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_FullAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Profile_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_BasicAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Profile_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_ReportingAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Personalization_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_FullAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Personalization_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_BasicAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Personalization_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_ReportingAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Membership_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_FullAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Membership_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_BasicAccess]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE ROLE [aspnet_Membership_BasicAccess]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
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
/****** Object:  Schema [aspnet_Membership_BasicAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Membership_BasicAccess]
GO
/****** Object:  Schema [aspnet_Membership_FullAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Membership_FullAccess]
GO
/****** Object:  Schema [aspnet_Membership_ReportingAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Membership_ReportingAccess]
GO
/****** Object:  Schema [aspnet_Personalization_BasicAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Personalization_BasicAccess]
GO
/****** Object:  Schema [aspnet_Personalization_FullAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Personalization_FullAccess]
GO
/****** Object:  Schema [aspnet_Personalization_ReportingAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Personalization_ReportingAccess]
GO
/****** Object:  Schema [aspnet_Profile_BasicAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Profile_BasicAccess]
GO
/****** Object:  Schema [aspnet_Profile_FullAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Profile_FullAccess]
GO
/****** Object:  Schema [aspnet_Profile_ReportingAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Profile_ReportingAccess]
GO
/****** Object:  Schema [aspnet_Roles_BasicAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Roles_BasicAccess]
GO
/****** Object:  Schema [aspnet_Roles_FullAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Roles_FullAccess]
GO
/****** Object:  Schema [aspnet_Roles_ReportingAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_Roles_ReportingAccess]
GO
/****** Object:  Schema [aspnet_WebEvent_FullAccess]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE SCHEMA [aspnet_WebEvent_FullAccess]
GO
/****** Object:  UserDefinedDataType [dbo].[tAppName]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE TYPE [dbo].[tAppName] FROM [varchar](280) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[tSessionId]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE TYPE [dbo].[tSessionId] FROM [nvarchar](88) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[tSessionItemLong]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE TYPE [dbo].[tSessionItemLong] FROM [image] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[tSessionItemShort]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE TYPE [dbo].[tSessionItemShort] FROM [varbinary](7000) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[tTextPtr]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE TYPE [dbo].[tTextPtr] FROM [varbinary](16) NULL
GO
/****** Object:  StoredProcedure [dbo].[aspnet_AnyDataInTables]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Applications_CreateApplication]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_CheckSchemaVersion]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_CreateUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_FindUsersByEmail]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_FindUsersByName]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetAllUsers]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetNumberOfUsersOnline]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetPassword]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetPasswordWithFormat]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByEmail]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByName]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByUserId]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_ResetPassword]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_SetPassword]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UnlockUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UpdateUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UpdateUserInfo]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Paths_CreatePath]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Personalization_GetApplicationId]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_DeleteAllState]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_FindState]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_GetCountOfState]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_ResetSharedState]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_ResetUserState]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_GetPageSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_SetPageSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_DeleteInactiveProfiles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_DeleteProfiles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetProfiles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetProperties]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_SetProperties]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_RegisterSchemaVersion]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_CreateRole]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_DeleteRole]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_GetAllRoles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_RoleExists]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Setup_RemoveAllRoleMembers]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Setup_RestorePermissions]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UnRegisterSchemaVersion]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Users_CreateUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_Users_DeleteUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_AddUsersToRoles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_FindUsersInRole]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_GetRolesForUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_GetUsersInRoles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_IsUserInRole]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[aspnet_WebEvent_LogEvent]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  StoredProcedure [dbo].[GetHashCode]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

/*****************************************************************************/

CREATE PROCEDURE [dbo].[GetHashCode]
    @input tAppName,
    @hash int OUTPUT
AS
    /* 
       This sproc is based on this C# hash function:

        int GetHashCode(string s)
        {
            int     hash = 5381;
            int     len = s.Length;

            for (int i = 0; i < len; i++) {
                int     c = Convert.ToInt32(s[i]);
                hash = ((hash << 5) + hash) ^ c;
            }

            return hash;
        }

        However, SQL 7 doesn't provide a 32-bit integer
        type that allows rollover of bits, we have to
        divide our 32bit integer into the upper and lower
        16 bits to do our calculation.
    */
       
    DECLARE @hi_16bit   int
    DECLARE @lo_16bit   int
    DECLARE @hi_t       int
    DECLARE @lo_t       int
    DECLARE @len        int
    DECLARE @i          int
    DECLARE @c          int
    DECLARE @carry      int

    SET @hi_16bit = 0
    SET @lo_16bit = 5381
    
    SET @len = DATALENGTH(@input)
    SET @i = 1
    
    WHILE (@i <= @len)
    BEGIN
        SET @c = ASCII(SUBSTRING(@input, @i, 1))

        /* Formula:                        
           hash = ((hash << 5) + hash) ^ c */

        /* hash << 5 */
        SET @hi_t = @hi_16bit * 32 /* high 16bits << 5 */
        SET @hi_t = @hi_t & 0xFFFF /* zero out overflow */
        
        SET @lo_t = @lo_16bit * 32 /* low 16bits << 5 */
        
        SET @carry = @lo_16bit & 0x1F0000 /* move low 16bits carryover to hi 16bits */
        SET @carry = @carry / 0x10000 /* >> 16 */
        SET @hi_t = @hi_t + @carry
        SET @hi_t = @hi_t & 0xFFFF /* zero out overflow */

        /* + hash */
        SET @lo_16bit = @lo_16bit + @lo_t
        SET @hi_16bit = @hi_16bit + @hi_t + (@lo_16bit / 0x10000)
        /* delay clearing the overflow */

        /* ^c */
        SET @lo_16bit = @lo_16bit ^ @c

        /* Now clear the overflow bits */	
        SET @hi_16bit = @hi_16bit & 0xFFFF
        SET @lo_16bit = @lo_16bit & 0xFFFF

        SET @i = @i + 1
    END

    /* Do a sign extension of the hi-16bit if needed */
    IF (@hi_16bit & 0x8000 <> 0)
        SET @hi_16bit = 0xFFFF0000 | @hi_16bit

    /* Merge hi and lo 16bit back together */
    SET @hi_16bit = @hi_16bit * 0x10000 /* << 16 */
    SET @hash = @hi_16bit | @lo_16bit

    RETURN 0




GO
/****** Object:  StoredProcedure [dbo].[GetMajorVersion]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

/*****************************************************************************/

CREATE PROCEDURE [dbo].[GetMajorVersion]
    @@ver int OUTPUT
AS
BEGIN
	DECLARE @version        nchar(100)
	DECLARE @dot            int
	DECLARE @hyphen         int
	DECLARE @SqlToExec      nchar(4000)

	SELECT @@ver = 7
	SELECT @version = @@Version
	SELECT @hyphen  = CHARINDEX(N' - ', @version)
	IF (NOT(@hyphen IS NULL) AND @hyphen > 0)
	BEGIN
		SELECT @hyphen = @hyphen + 3
		SELECT @dot    = CHARINDEX(N'.', @version, @hyphen)
		IF (NOT(@dot IS NULL) AND @dot > @hyphen)
		BEGIN
			SELECT @version = SUBSTRING(@version, @hyphen, @dot - @hyphen)
			SELECT @@ver     = CONVERT(int, @version)
		END
	END
END




GO
/****** Object:  StoredProcedure [dbo].[TempGetVersion]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

/*****************************************************************************/

CREATE PROCEDURE [dbo].[TempGetVersion]
    @ver      char(10) OUTPUT
AS
    SELECT @ver = "2"
    RETURN 0




GO
/****** Object:  StoredProcedure [dbo].[USPDistanceWithin]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	
	CREATE PROCEDURE [dbo].[USPDistanceWithin] 
	(@lat as real,@long as real, @distance as float)
	AS
	BEGIN

		DECLARE @edi geography = geography::STPointFromText('POINT(' + CAST(@Long AS VARCHAR(20)) + ' ' + 
									CAST(@Lat AS VARCHAR(20)) + ')', 4326)

		SET @distance = @distance * 1609.344 -- convert distance into meter
		
		select 
			 AddressFieldID
			,FieldID
			,AddressString
			,Latitude
			,Longitude
			,LastGeocode
			,Status
			--,Geo
		from 
			AddressFields a WITH(INDEX(SIndx_AddressFields_geo))
		where 
			a.geo.STDistance(@edi) < = @Distance 
		
	END






GO
/****** Object:  StoredProcedure [dbo].[USPSetAllGeo]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

	CREATE PROC [dbo].[USPSetAllGeo]
	AS
	UPDATE AddressFields
	SET Geo = geography::STPointFromText('POINT(' + CAST([Longitude] AS VARCHAR(20)) + ' ' + 
						CAST([Latitude] AS VARCHAR(20)) + ')', 4326)




GO
/****** Object:  StoredProcedure [dbo].[USPSetGEOValue]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE PROC [dbo].[USPSetGEOValue] @latitude decimal(18,8), @longitude decimal(18,8)
	AS
		UPDATE AddressFields
		SET Geo = geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(20)) + ' ' + 
						CAST(@latitude AS VARCHAR(20)) + ')', 4326)
		WHERE [Longitude] =@longitude and [Latitude] = @latitude






GO
/****** Object:  UserDefinedFunction [dbo].[DistanceBetween]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[DistanceBetween] (@Lat1 as real,
                @Long1 as real, @Lat2 as real, @Long2 as real)
RETURNS real
AS
BEGIN

DECLARE @dLat1InRad as float(53);
SET @dLat1InRad = @Lat1 * (PI()/180.0);
DECLARE @dLong1InRad as float(53);
SET @dLong1InRad = @Long1 * (PI()/180.0);
DECLARE @dLat2InRad as float(53);
SET @dLat2InRad = @Lat2 * (PI()/180.0);
DECLARE @dLong2InRad as float(53);
SET @dLong2InRad = @Long2 * (PI()/180.0);

DECLARE @dLongitude as float(53);
SET @dLongitude = @dLong2InRad - @dLong1InRad;
DECLARE @dLatitude as float(53);
SET @dLatitude = @dLat2InRad - @dLat1InRad;
/* Intermediate result a. */
DECLARE @a as float(53);
SET @a = SQUARE (SIN (@dLatitude / 2.0)) + COS (@dLat1InRad)
                 * COS (@dLat2InRad)
                 * SQUARE(SIN (@dLongitude / 2.0));
/* Intermediate result c (great circle distance in Radians). */
DECLARE @c as real;
SET @c = 2.0 * ATN2 (SQRT (@a), SQRT (1.0 - @a));
DECLARE @kEarthRadius as real;
/* SET kEarthRadius = 3956.0 miles */
SET @kEarthRadius = 6376.5;        /* kms */

DECLARE @dDistance as real;
SET @dDistance = @kEarthRadius * @c;
return (@dDistance);
END




GO
/****** Object:  UserDefinedFunction [dbo].[DistanceBetween2]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
		
	CREATE FUNCTION [dbo].[DistanceBetween2] 
	(@AddressFieldID as int, @Lat1 as real,@Long1 as real)
	RETURNS real
	AS
	BEGIN

		DECLARE @KMperNM float = 1.0/1.852;

		DECLARE @nwi geography =(select geo from addressfields where AddressFieldID  = @AddressFieldID)

		DECLARE @edi geography = geography::STPointFromText('POINT(' + CAST(@Long1 AS VARCHAR(20)) + ' ' + 
									CAST(@Lat1 AS VARCHAR(20)) + ')', 4326)

		DECLARE @dDistance as real = (SELECT (@nwi.STDistance(@edi)/1000.0) * @KMperNM)

		return (@dDistance);  

	END






GO
/****** Object:  UserDefinedFunction [dbo].[DistanceWithin]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	
	CREATE FUNCTION [dbo].[DistanceWithin] 
	(@AddressFieldId as int, @Lat1 as real,@Long1 as real, @distance as real)
	RETURNS bit
	AS
	BEGIN
	
		/*
		
		SELECT AddressFieldID FROM addressfields 
		WHERE g.Filter(geography::Parse
				('POLYGON((-120.1 44.9, -119.9 44.9, -119.9 45.1, -120.1 45.1, -120.1 44.9))')) = 1;
		*/
	
		DECLARE @ret bit
		DECLARE @KMperNM float = 1.0/1.852;
		DECLARE @nwi geography =(select geo from addressfields where AddressFieldID  = @AddressFieldID)
		DECLARE @edi geography = geography::STPointFromText('POINT(' + CAST(@Lat1 AS VARCHAR(20)) + ' ' + 
									CAST(@Long1 AS VARCHAR(20)) + ')', 4326)
		DECLARE @dDistance as real = (SELECT (@nwi.STDistance(@edi)/1000.0) * @KMperNM)
		
		IF @dDistance <= @distance
		BEGIN
			SET @ret = 1
		END
		ELSE
		BEGIN
			SET @ret = 0
		END
	
		RETURN (@ret)
	
	END






GO
/****** Object:  UserDefinedFunction [dbo].[UDFDistanceWithin]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	
	CREATE FUNCTION [dbo].[UDFDistanceWithin] 
	(@lat as real,@long as real, @distance as real)
	RETURNS @AddressIdsToReturn TABLE 
		(
			 AddressFieldID INT
			,FieldID INT
		)
	AS
	BEGIN
	
		DECLARE @edi geography = geography::STPointFromText('POINT(' + CAST(@Long AS VARCHAR(20)) + ' ' + 
									CAST(@Lat AS VARCHAR(20)) + ')', 4326)

		SET @distance = @distance * 1609.344 -- convert distance into meter
		
		INSERT INTO @AddressIdsToReturn
		select 
			 AddressFieldID
			,FieldID
		from 
			AddressFields a WITH(INDEX(SIndx_AddressFields_geo))
		where 
			a.geo.STDistance(@edi) < = @Distance 
		
		RETURN 
		
	END






GO
/****** Object:  UserDefinedFunction [dbo].[UDFNearestNeighbors]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	
	CREATE FUNCTION [dbo].[UDFNearestNeighbors] 
	(@lat as real,@long as real, @neighbors as int)
	RETURNS @AddressIdsToReturn TABLE 
		(
			 AddressFieldID INT
			,FieldID INT
		)
	AS
	BEGIN
	
		DECLARE @edi geography = geography::STPointFromText('POINT(' + CAST(@Long AS VARCHAR(20)) + ' ' + 
									CAST(@Lat AS VARCHAR(20)) + ')', 4326)
		DECLARE @start FLOAT = 1000;
		
		WITH NearestPoints AS

		(

		  SELECT TOP(@neighbors) WITH TIES *,  AddressFields.geo.STDistance(@edi) AS dist

		  FROM Numbers JOIN AddressFields WITH(INDEX(SIndx_AddressFields_geo)) 

		  ON AddressFields.geo.STDistance(@edi) < @start*POWER(2,Numbers.n)

		  ORDER BY n

		)
		
			
		INSERT INTO @AddressIdsToReturn
		
		SELECT TOP(@neighbors)
			 AddressFieldID
			,FieldID
		FROM NearestPoints
		ORDER BY n DESC, dist
		
		RETURN 
		
	END






GO
/****** Object:  Table [dbo].[AddressFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddressFields](
	[AddressFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldID] [int] NOT NULL,
	[AddressString] [nvarchar](max) NOT NULL,
	[Latitude] [decimal](18, 8) NULL,
	[Longitude] [decimal](18, 8) NULL,
	[LastGeocode] [datetime] NULL,
	[Status] [tinyint] NOT NULL,
	[Geo] [geography] NULL,
 CONSTRAINT [PK_dbo.AddressFields] PRIMARY KEY CLUSTERED 
(
	[AddressFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdministrationRights]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdministrationRights](
	[AdministrationRightID] [int] IDENTITY(1,1) NOT NULL,
	[AdministrationRightsGroupID] [int] NOT NULL,
	[UserActionID] [int] NULL,
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
/****** Object:  Table [dbo].[AdministrationRightsGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdministrationRightsGroups](
	[AdministrationRightsGroupID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.AdministrationRightsGroups] PRIMARY KEY CLUSTERED 
(
	[AdministrationRightsGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[aspnet_Applications]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_Membership]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_Paths]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_PersonalizationAllUsers]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_PersonalizationPerUser]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_Profile]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_Roles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_SchemaVersions]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_Users]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_UsersInRoles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[aspnet_WebEvent_Events]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Table [dbo].[ChangesGroup]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChangesGroup](
	[ChangesGroupID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NULL,
	[TblID] [int] NULL,
	[Creator] [int] NULL,
	[MakeChangeRatingID] [int] NULL,
	[RewardRatingID] [int] NULL,
	[StatusOfChanges] [tinyint] NOT NULL,
	[ScheduleApprovalOrRejection] [datetime] NULL,
	[ScheduleImplementation] [datetime] NULL,
 CONSTRAINT [PK_dbo.ChangesGroup] PRIMARY KEY CLUSTERED 
(
	[ChangesGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChangesStatusOfObject]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChangesStatusOfObject](
	[ChangesStatusOfObjectID] [int] IDENTITY(1,1) NOT NULL,
	[ChangesGroupID] [int] NOT NULL,
	[ObjectType] [tinyint] NOT NULL,
	[AddObject] [bit] NOT NULL,
	[DeleteObject] [bit] NOT NULL,
	[ReplaceObject] [bit] NOT NULL,
	[ChangeName] [bit] NOT NULL,
	[ChangeOther] [bit] NOT NULL,
	[ChangeSetting1] [bit] NOT NULL,
	[ChangeSetting2] [bit] NOT NULL,
	[MayAffectRunningRating] [bit] NOT NULL,
	[NewName] [nvarchar](50) NOT NULL,
	[NewObject] [int] NULL,
	[ExistingObject] [int] NULL,
	[NewValueBoolean] [bit] NULL,
	[NewValueInteger] [int] NULL,
	[NewValueDecimal] [decimal](18, 4) NULL,
	[NewValueText] [nvarchar](max) NOT NULL,
	[NewValueDateTime] [datetime] NULL,
	[ChangeDescription] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.ChangesStatusOfObject] PRIMARY KEY CLUSTERED 
(
	[ChangesStatusOfObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceFields](
	[ChoiceFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldID] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceFields] PRIMARY KEY CLUSTERED 
(
	[ChoiceFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceGroupFieldDefinitions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceGroupFieldDefinitions](
	[ChoiceGroupFieldDefinitionID] [int] IDENTITY(1,1) NOT NULL,
	[ChoiceGroupID] [int] NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[DependentOnChoiceGroupFieldDefinitionID] [int] NULL,
	[TrackTrustBasedOnChoices] [bit] NOT NULL CONSTRAINT [DF_ChoiceGroupFieldDefinitions_TrackTrustBasedOnChoices]  DEFAULT ((0)),
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceGroupFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[ChoiceGroupFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceGroups](
	[ChoiceGroupID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NOT NULL,
	[AllowMultipleSelections] [bit] NOT NULL,
	[Alphabetize] [bit] NOT NULL,
	[InvisibleWhenEmpty] [bit] NOT NULL,
	[ShowTagCloud] [bit] NOT NULL,
	[PickViaAutoComplete] [bit] NOT NULL,
	[DependentOnChoiceGroupID] [int] NULL,
	[ShowAllPossibilitiesIfNoDependentChoice] [bit] NOT NULL,
	[AlphabetizeWhenShowingAllPossibilities] [bit] NOT NULL,
	[AllowAutoAddWhenAddingFields] [bit] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceGroups] PRIMARY KEY CLUSTERED 
(
	[ChoiceGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceInFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceInFields](
	[ChoiceInFieldID] [int] IDENTITY(1,1) NOT NULL,
	[ChoiceFieldID] [int] NOT NULL,
	[ChoiceInGroupID] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_ChoiceInFields] PRIMARY KEY CLUSTERED 
(
	[ChoiceInFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChoiceInGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChoiceInGroups](
	[ChoiceInGroupID] [int] IDENTITY(1,1) NOT NULL,
	[ChoiceGroupID] [int] NOT NULL,
	[ChoiceNum] [int] NOT NULL,
	[ChoiceText] [nvarchar](50) NOT NULL,
	[ActiveOnDeterminingGroupChoiceInGroupID] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ChoiceInGroups] PRIMARY KEY CLUSTERED 
(
	[ChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Comments]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Comments](
	[CommentsID] [int] IDENTITY(1,1) NOT NULL,
	[TblRowID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[CommentTitle] [varchar](max) NOT NULL,
	[CommentText] [varchar](max) NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[LastDeletedDate] [datetime] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Comments] PRIMARY KEY CLUSTERED 
(
	[CommentsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DatabaseStatus]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseStatus](
	[DatabaseStatusID] [int] IDENTITY(1,1) NOT NULL,
	[PreventChanges] [bit] NOT NULL,
 CONSTRAINT [PK_DatabaseStatus] PRIMARY KEY CLUSTERED 
(
	[DatabaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DateTimeFieldDefinitions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateTimeFieldDefinitions](
	[DateTimeFieldDefinitionID] [int] IDENTITY(1,1) NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[IncludeDate] [bit] NOT NULL,
	[IncludeTime] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.DateTimeFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[DateTimeFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DateTimeFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateTimeFields](
	[DateTimeFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldID] [int] NOT NULL,
	[DateTime] [datetime] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.DateTimeFields] PRIMARY KEY CLUSTERED 
(
	[DateTimeFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Domains]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Domains](
	[DomainID] [int] IDENTITY(1,1) NOT NULL,
	[ActiveRatingWebsite] [bit] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[TblDimensionsID] [int] NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Domains] PRIMARY KEY CLUSTERED 
(
	[DomainID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FieldDefinitions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldDefinitions](
	[FieldDefinitionID] [int] IDENTITY(1,1) NOT NULL,
	[TblID] [int] NOT NULL,
	[FieldNum] [int] NOT NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[FieldType] [int] NOT NULL,
	[UseAsFilter] [bit] NOT NULL,
	[AddToSearchWords] [bit] NOT NULL CONSTRAINT [DF_FieldDefinitions_AddToSearchWords]  DEFAULT ((0)),
	[DisplayInTableSettings] [int] NOT NULL,
	[DisplayInPopupSettings] [int] NOT NULL,
	[DisplayInTblRowPageSettings] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[NumNonNull] [int] NOT NULL DEFAULT ((0)),
	[ProportionNonNull] [float] NOT NULL DEFAULT ((0)),
	[UsingNonSparseColumn] [bit] NOT NULL DEFAULT ((1)),
	[ShouldUseNonSparseColumn] [bit] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_dbo.FieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[FieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fields](
	[FieldID] [int] IDENTITY(1,1) NOT NULL,
	[TblRowID] [int] NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Fields] PRIMARY KEY CLUSTERED 
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumAdministrators]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumAdministrators](
	[UserID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumComplaints]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumComplaints](
	[UserID] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	[ComplainText] [ntext] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumGroupPermissions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumGroupPermissions](
	[ForumID] [int] NOT NULL,
	[GroupID] [int] NOT NULL,
	[AllowReading] [bit] NOT NULL,
	[AllowPosting] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumGroups](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumMessageRating]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumMessageRating](
	[MessageID] [int] NOT NULL,
	[VoterUserID] [int] NOT NULL,
	[Score] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC,
	[VoterUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumMessages]    Script Date: 7/10/2014 2:45:47 PM ******/
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
	[CreationDate] [datetime] NOT NULL,
	[Visible] [bit] NOT NULL,
	[IPAddress] [varchar](50) NULL,
	[Rating] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ForumModerators]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumModerators](
	[UserID] [int] NOT NULL,
	[ForumID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumNewTopicSubscriptions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumNewTopicSubscriptions](
	[UserID] [int] NOT NULL,
	[ForumID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPersonalMessages]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPersonalMessages](
	[MessageID] [int] IDENTITY(1,1) NOT NULL,
	[FromUserID] [int] NOT NULL,
	[ToUserID] [int] NOT NULL,
	[Body] [ntext] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[New] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPollAnswers]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPollAnswers](
	[UserID] [int] NOT NULL,
	[OptionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPollOptions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPollOptions](
	[OptionID] [int] IDENTITY(1,1) NOT NULL,
	[PollID] [int] NOT NULL,
	[OptionText] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumPolls]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumPolls](
	[PollID] [int] IDENTITY(1,1) NOT NULL,
	[TopicID] [int] NOT NULL,
	[Question] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PollID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Forums]    Script Date: 7/10/2014 2:45:47 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumSubforums]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumSubforums](
	[ParentForumID] [int] NOT NULL,
	[SubForumID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ParentForumID] ASC,
	[SubForumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumSubscriptions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumSubscriptions](
	[UserID] [int] NOT NULL,
	[TopicID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[TopicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumTopics]    Script Date: 7/10/2014 2:45:47 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[TopicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUploadedFiles]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUploadedFiles](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[MessageID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUploadedPersonalFiles]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUploadedPersonalFiles](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[MessageID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUserGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUserGroups](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUsers]    Script Date: 7/10/2014 2:45:47 PM ******/
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
	[RegistrationDate] [datetime] NOT NULL,
	[Disabled] [bit] NOT NULL,
	[ActivationCode] [nvarchar](50) NOT NULL,
	[AvatarFileName] [nvarchar](50) NULL,
	[Signature] [nvarchar](1000) NULL,
	[LastLogonDate] [datetime] NULL,
	[ReputationCache] [int] NOT NULL,
	[OpenIdUserName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ForumUsersInGroup]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForumUsersInGroup](
	[GroupID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HierarchyItems]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HierarchyItems](
	[HierarchyItemID] [int] IDENTITY(1,1) NOT NULL,
	[HigherHierarchyItemID] [int] NULL,
	[HigherHierarchyItemForRoutingID] [int] NULL,
	[TblID] [int] NULL,
	[HierarchyItemName] [nvarchar](max) NOT NULL,
	[FullHierarchyWithHtml] [nvarchar](max) NOT NULL,
	[FullHierarchyNoHtml] [nvarchar](max) NOT NULL,
	[RouteToHere] [nvarchar](max) NOT NULL,
	[IncludeInMenu] [bit] NOT NULL CONSTRAINT [DF_HierarchyItems_IncludeInMenu]  DEFAULT ((1)),
 CONSTRAINT [PK_HierarchyItems] PRIMARY KEY CLUSTERED 
(
	[HierarchyItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InsertableContents]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InsertableContents](
	[InsertableContentID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[DomainID] [int] NULL,
	[PointsManagerID] [int] NULL,
	[TblID] [int] NULL,
	[Content] [varchar](max) NOT NULL,
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InvitedUser]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvitedUser](
	[ActivationNumber] [int] IDENTITY(1,1) NOT NULL,
	[EmailId] [nvarchar](50) NOT NULL,
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
/****** Object:  Table [dbo].[LongProcesses]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LongProcesses](
	[LongProcessID] [int] IDENTITY(1,1) NOT NULL,
	[TypeOfProcess] [int] NOT NULL,
	[Object1ID] [int] NULL,
	[Object2ID] [int] NULL,
	[Priority] [int] NOT NULL,
	[AdditionalInfo] [varbinary](max) NULL,
	[ProgressInfo] [int] NULL,
	[Started] [bit] NOT NULL,
	[Complete] [bit] NOT NULL,
	[ResetWhenComplete] [bit] NOT NULL,
	[DelayBeforeRestart] [int] NULL,
	[EarliestRestart] [datetime] NULL,
 CONSTRAINT [PK_LongProcesses] PRIMARY KEY CLUSTERED 
(
	[LongProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NumberFieldDefinitions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NumberFieldDefinitions](
	[NumberFieldDefinitionID] [int] IDENTITY(1,1) NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
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
/****** Object:  Table [dbo].[NumberFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NumberFields](
	[NumberFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldID] [int] NOT NULL,
	[Number] [decimal](18, 4) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.NumberFields] PRIMARY KEY CLUSTERED 
(
	[NumberFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OverrideCharacteristics]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OverrideCharacteristics](
	[OverrideCharacteristicsID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupAttributesID] [int] NOT NULL,
	[TblRowID] [int] NOT NULL,
	[TblColumnID] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.OverrideCharacteristics] PRIMARY KEY CLUSTERED 
(
	[OverrideCharacteristicsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointsAdjustments]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointsAdjustments](
	[PointsAdjustmentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[PointsManagerID] [int] NOT NULL,
	[Reason] [int] NOT NULL,
	[TotalAdjustment] [decimal](18, 4) NOT NULL,
	[CurrentAdjustment] [decimal](18, 4) NOT NULL,
	[CashValue] [decimal](18, 2) NULL,
	[WhenMade] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.PointsAdjustments] PRIMARY KEY CLUSTERED 
(
	[PointsAdjustmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointsManagers]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointsManagers](
	[PointsManagerID] [int] IDENTITY(1,1) NOT NULL,
	[DomainID] [int] NOT NULL,
	[TrustTrackerUnitID] [int] NULL,
	[CurrentPeriodDollarSubsidy] [decimal](18, 2) NOT NULL,
	[EndOfDollarSubsidyPeriod] [datetime] NULL,
	[NextPeriodDollarSubsidy] [decimal](18, 2) NULL,
	[NextPeriodLength] [int] NULL,
	[NumPrizes] [smallint] NOT NULL,
	[MinimumPayment] [decimal](18, 2) NULL,
	[TotalUserPoints] [decimal](18, 4) NOT NULL,
	[CurrentUserPoints] [decimal](18, 4) NOT NULL,
	[CurrentUserPendingPoints] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_CurrentUserPendingPoints]  DEFAULT ((0)),
	[CurrentUserNotYetPendingPoints] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_CurrentUserNotYetPendingPoints]  DEFAULT ((0)),
	[CurrentPointsToCount] [decimal](18, 4) NOT NULL,
	[NumUsersMeetingUltimateStandard] [int] NOT NULL,
	[NumUsersMeetingCurrentStandard] [int] NOT NULL,
	[HighStakesProbability] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesProbability]  DEFAULT ((0.01)),
	[HighStakesSecretMultiplier] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesPastMultiplier]  DEFAULT ((100)),
	[HighStakesKnownMultiplier] [decimal](18, 4) NULL CONSTRAINT [DF_PointsManagers_HighStakesCurrentMultiplier]  DEFAULT ((10)),
	[HighStakesNoviceOn] [bit] NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesNoviceOn]  DEFAULT ((1)),
	[HighStakesNoviceNumAutomatic] [int] NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesNoviceNumAutomatic]  DEFAULT ((10)),
	[HighStakesNoviceNumOneThird] [int] NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesNoviceNumOneThird]  DEFAULT ((20)),
	[HighStakesNoviceNumOneTenth] [int] NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesNoviceNumOneTenth]  DEFAULT ((50)),
	[DatabaseChangeSelectHighStakesNoviceNumPct] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_DatabaseChangeSelectHighStakesNoviceNumPct]  DEFAULT ((0.333)),
	[HighStakesNoviceNumActive] [int] NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesNoviceNumActive]  DEFAULT ((0)),
	[HighStakesNoviceTargetNum] [int] NOT NULL CONSTRAINT [DF_PointsManagers_HighStakesNoviceTargetNum]  DEFAULT ((20)),
	[DollarValuePerPoint] [decimal](18, 8) NOT NULL CONSTRAINT [DF_PointsManagers_DollarValuePerPoint]  DEFAULT ((0)),
	[DiscountForGuarantees] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_DiscountForGuarantee]  DEFAULT ((1)),
	[MaximumTotalGuarantees] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_MaximumTotalGuarantees]  DEFAULT ((0)),
	[MaximumGuaranteePaymentPerHour] [decimal](18, 2) NOT NULL CONSTRAINT [DF_PointsManagers_ConditionalGuaranteeCapOnPaymentPerHour]  DEFAULT ((0)),
	[TotalUnconditionalGuaranteesEarnedEver] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_TotalGuaranteesEarnedSoFar]  DEFAULT ((0)),
	[TotalConditionalGuaranteesEarnedEver] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_TotalConditionalGuaranteesEarnedSoFar]  DEFAULT ((0)),
	[TotalConditionalGuaranteesPending] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsManagers_TotalConditionalGuaranteesPending]  DEFAULT ((0)),
	[AllowApplicationsWhenNoConditionalGuaranteesAvailable] [bit] NOT NULL CONSTRAINT [DF_PointsManagers_AllowApplicationsWhenNoConditionalGuaranteesAvailable]  DEFAULT ((0)),
	[ConditionalGuaranteesAvailableForNewUsers] [bit] NOT NULL CONSTRAINT [DF_PointsManagers_ConditionalGuaranteesAvailableForNewUsers]  DEFAULT ((0)),
	[ConditionalGuaranteesAvailableForExistingUsers] [bit] NOT NULL CONSTRAINT [DF_PointsManagers_ConditionalGuaranteesAvailableForExistingUsers]  DEFAULT ((1)),
	[ConditionalGuaranteeTimeBlockInHours] [int] NOT NULL CONSTRAINT [DF_PointsManagers_ConditionalGuaranteeTimeBlockInHours]  DEFAULT ((1)),
	[ConditionalGuaranteeApplicationsReceived] [int] NOT NULL CONSTRAINT [DF_PointsManagers_ConditionalGuaranteeApplicationsReceived]  DEFAULT ((0)),
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.PointsManagers] PRIMARY KEY CLUSTERED 
(
	[PointsManagerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointsTotals]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointsTotals](
	[PointsTotalID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[PointsManagerID] [int] NOT NULL,
	[CurrentPoints] [decimal](18, 4) NOT NULL,
	[TotalPoints] [decimal](18, 4) NOT NULL,
	[PotentialMaxLossOnNotYetPending] [decimal](18, 4) NOT NULL,
	[PendingPoints] [decimal](18, 4) NOT NULL,
	[NotYetPendingPoints] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_NotYetPendingPoints]  DEFAULT ((0)),
	[TrustPoints] [decimal](18, 4) NOT NULL,
	[TrustPointsRatio] [decimal](18, 4) NOT NULL,
	[NumPendingOrFinalizedRatings] [int] NOT NULL CONSTRAINT [DF_PointsTotals_NumPendingOrFinalizedRatings]  DEFAULT ((0)),
	[PointsPerRating] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_PointsPerRating]  DEFAULT ((0)),
	[FirstUserRating] [datetime] NULL,
	[LastCheckIn] [datetime] NULL,
	[CurrentCheckInPeriodStart] [datetime] NULL,
	[TotalTimeThisCheckInPeriod] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_TotalTimeThisCheckInPeriod]  DEFAULT ((0)),
	[TotalTimeThisRewardPeriod] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_TotalTimeThisRewardPeriod]  DEFAULT ((0)),
	[TotalTimeEver] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_TotalTimeEver]  DEFAULT ((0)),
	[PointsPerHour] [decimal](18, 4) NULL,
	[ProjectedPointsPerHour] [decimal](18, 4) NULL,
	[GuaranteedPaymentsEarnedThisRewardPeriod] [decimal](18, 2) NOT NULL CONSTRAINT [DF_PointsTotals_GuaranteedPaymentsEarnedThisRewardPeriod]  DEFAULT ((0)),
	[PendingConditionalGuaranteeApplication] [nvarchar](50) NULL,
	[PendingConditionalGuaranteePayment] [decimal](18, 2) NULL,
	[PendingConditionalGuaranteeTotalHoursAtStart] [decimal](18, 4) NULL,
	[PendingConditionalGuaranteeTotalHoursNeeded] [decimal](18, 4) NULL,
	[PendingConditionalGuaranteePaymentAlreadyMade] [decimal](18, 2) NULL,
	[RequestConditionalGuaranteeWhenAvailableTimeRequestMade] [datetime] NULL,
	[TotalPointsOrPendingPointsLongTermUnweighted] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_TotalPointsOrPendingPointsLongTermUnweighted]  DEFAULT ((0)),
	[PointsPerRatingLongTerm] [decimal](18, 4) NOT NULL CONSTRAINT [DF_PointsTotals_PointsPerRatingLongTerm]  DEFAULT ((0)),
	[PointsPumpingProportionAvg_Numer] [real] NOT NULL DEFAULT ((0)),
	[PointsPumpingProportionAvg_Denom] [real] NOT NULL DEFAULT ((0)),
	[PointsPumpingProportionAvg] [real] NOT NULL DEFAULT ((1)),
	[NumUserRatings] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.PointsTotals] PRIMARY KEY CLUSTERED 
(
	[PointsTotalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProposalEvaluationRatingSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposalEvaluationRatingSettings](
	[ProposalEvaluationRatingSettingsID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NULL,
	[UserActionID] [int] NULL,
	[RatingGroupAttributesID] [int] NOT NULL,
	[MinValueToApprove] [decimal](18, 4) NOT NULL,
	[MaxValueToReject] [decimal](18, 4) NOT NULL,
	[TimeRequiredBeyondThreshold] [int] NOT NULL,
	[MinProportionOfThisTime] [decimal](18, 4) NOT NULL,
	[HalfLifeForResolvingAtFinalValue] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ProposalEvaluationRatingSettings] PRIMARY KEY CLUSTERED 
(
	[ProposalEvaluationRatingSettingsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProposalSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposalSettings](
	[ProposalSettingsID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NULL,
	[TblID] [int] NULL,
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
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ProposalSettings] PRIMARY KEY CLUSTERED 
(
	[ProposalSettingsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingCharacteristics]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingCharacteristics](
	[RatingCharacteristicsID] [int] IDENTITY(1,1) NOT NULL,
	[RatingPhaseGroupID] [int] NOT NULL,
	[SubsidyDensityRangeGroupID] [int] NULL,
	[MinimumUserRating] [decimal](18, 4) NOT NULL,
	[MaximumUserRating] [decimal](18, 4) NOT NULL,
	[DecimalPlaces] [tinyint] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingCharacteristics] PRIMARY KEY CLUSTERED 
(
	[RatingCharacteristicsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingConditions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingConditions](
	[RatingConditionID] [int] IDENTITY(1,1) NOT NULL,
	[ConditionRatingID] [int] NULL,
	[GreaterThan] [decimal](18, 4) NULL,
	[LessThan] [decimal](18, 4) NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingConditions] PRIMARY KEY CLUSTERED 
(
	[RatingConditionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupAttributes]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RatingGroupAttributes](
	[RatingGroupAttributesID] [int] IDENTITY(1,1) NOT NULL,
	[RatingCharacteristicsID] [int] NOT NULL,
	[RatingConditionID] [int] NULL,
	[PointsManagerID] [int] NULL,
	[ConstrainedSum] [decimal](18, 4) NULL,
	[Name] [nvarchar](max) NOT NULL,
	[TypeOfRatingGroup] [tinyint] NULL,
	[Description] [varchar](max) NULL,
	[RatingEndingTimeVaries] [bit] NOT NULL,
	[RatingsCanBeAutocalculated] [bit] NOT NULL CONSTRAINT [DF_RatingGroupAttributes_RatingsCanBeAutocalculated]  DEFAULT ((1)),
	[LongTermPointsWeight] [decimal](18, 4) NOT NULL,
	[MinimumDaysToTrackLongTerm] [int] NOT NULL CONSTRAINT [DF_RatingGroupAttributes_MinimumDaysToTrackLongTerm]  DEFAULT ((365)),
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingGroupAttributes] PRIMARY KEY CLUSTERED 
(
	[RatingGroupAttributesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RatingGroupPhaseStatus]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupPhaseStatus](
	[RatingGroupPhaseStatusID] [int] IDENTITY(1,1) NOT NULL,
	[RatingPhaseGroupID] [int] NOT NULL,
	[RatingPhaseID] [int] NOT NULL,
	[RatingGroupID] [int] NOT NULL,
	[RoundNum] [int] NOT NULL,
	[RoundNumThisPhase] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EarliestCompleteTime] [datetime] NOT NULL,
	[ActualCompleteTime] [datetime] NOT NULL,
	[ShortTermResolveTime] [datetime] NOT NULL,
	[HighStakesSecret] [bit] NOT NULL CONSTRAINT [DF_RatingGroupPhaseStatus_HighStakesSecret]  DEFAULT ((0)),
	[HighStakesKnown] [bit] NOT NULL CONSTRAINT [DF_RatingGroupPhaseStatus_HighStakesKnown]  DEFAULT ((0)),
	[HighStakesReflected] [bit] NOT NULL CONSTRAINT [DF_RatingGroupPhaseStatus_HighStakesReflected]  DEFAULT ((1)),
	[HighStakesNoviceUser] [bit] NOT NULL CONSTRAINT [DF_RatingGroupPhaseStatus_HighStakesNoviceUser]  DEFAULT ((0)),
	[HighStakesBecomeKnown] [datetime] NULL,
	[HighStakesNoviceUserAfter] [datetime] NULL,
	[DeletionTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.RatingGroupPhaseStatus] PRIMARY KEY CLUSTERED 
(
	[RatingGroupPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupResolutions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupResolutions](
	[RatingGroupResolutionID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupID] [int] NOT NULL,
	[CancelPreviousResolutions] [bit] NOT NULL,
	[ResolveByUnwinding] [bit] NOT NULL,
	[EffectiveTime] [datetime] NOT NULL,
	[ExecutionTime] [datetime] NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingGroupResolutions] PRIMARY KEY CLUSTERED 
(
	[RatingGroupResolutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroups](
	[RatingGroupID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupAttributesID] [int] NOT NULL,
	[TblRowID] [int] NOT NULL,
	[TblColumnID] [int] NOT NULL,
	[CurrentValueOfFirstRating] [decimal](18, 4) NULL,
	[ValueRecentlyChanged] [bit] NOT NULL CONSTRAINT [DF_RatingGroups_ValueRecentlyChanged]  DEFAULT ((0)),
	[ResolutionTime] [datetime] NULL,
	[TypeOfRatingGroup] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[HighStakesKnown] [bit] NOT NULL CONSTRAINT [DF_RatingGroups_HighStakesKnown]  DEFAULT ((0)),
 CONSTRAINT [PK_dbo.RatingGroups] PRIMARY KEY CLUSTERED 
(
	[RatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingGroupStatusRecords]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingGroupStatusRecords](
	[RatingGroupStatusRecordID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupID] [int] NOT NULL,
	[OldValueOfFirstRating] [numeric](18, 4) NULL,
	[NewValueTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RatingGroupStatusRecords] PRIMARY KEY CLUSTERED 
(
	[RatingGroupStatusRecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPhaseGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPhaseGroups](
	[RatingPhaseGroupID] [int] IDENTITY(1,1) NOT NULL,
	[NumPhases] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingPhaseGroups] PRIMARY KEY CLUSTERED 
(
	[RatingPhaseGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPhases]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPhases](
	[RatingPhaseID] [int] IDENTITY(1,1) NOT NULL,
	[RatingPhaseGroupID] [int] NOT NULL,
	[NumberInGroup] [int] NOT NULL,
	[SubsidyLevel] [decimal](18, 4) NOT NULL,
	[ScoringRule] [smallint] NOT NULL,
	[Timed] [bit] NOT NULL,
	[BaseTimingOnSpecificTime] [bit] NOT NULL,
	[EndTime] [datetime] NULL,
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
/****** Object:  Table [dbo].[RatingPhaseStatus]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingPhaseStatus](
	[RatingPhaseStatusID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupPhaseStatusID] [int] NOT NULL,
	[RatingID] [int] NOT NULL,
	[ShortTermResolutionValue] [decimal](18, 4) NULL,
	[NumUserRatingsMadeDuringPhase] [int] NOT NULL DEFAULT ((0)),
	[TriggerUserRatingsUpdate] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_RatingPhaseStatus] PRIMARY KEY CLUSTERED 
(
	[RatingPhaseStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RatingPlans]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RatingPlans](
	[RatingPlansID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupAttributesID] [int] NOT NULL,
	[NumInGroup] [int] NOT NULL,
	[OwnedRatingGroupAttributesID] [int] NULL,
	[DefaultUserRating] [decimal](18, 4) NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [varchar](max) NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RatingPlans] PRIMARY KEY CLUSTERED 
(
	[RatingPlansID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ratings]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ratings](
	[RatingID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupID] [int] NOT NULL,
	[RatingCharacteristicsID] [int] NOT NULL,
	[OwnedRatingGroupID] [int] NULL,
	[TopmostRatingGroupID] [int] NOT NULL,
	[MostRecentUserRatingID] [int] NULL,
	[NumInGroup] [int] NOT NULL,
	[TotalUserRatings] [int] NOT NULL CONSTRAINT [DF_Ratings_TotalUserRatings]  DEFAULT ((0)),
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[CurrentValue] [decimal](18, 4) NULL,
	[LastTrustedValue] [decimal](18, 4) NULL,
	[LastModifiedResolutionTimeOrCurrentValue] [datetime] NOT NULL,
	[ReviewRecentUserRatingsAfter] [datetime] NULL,
 CONSTRAINT [PK_dbo.Ratings] PRIMARY KEY CLUSTERED 
(
	[RatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RewardPendingPointsTrackers]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RewardPendingPointsTrackers](
	[RewardPendingPointsTrackerID] [int] IDENTITY(1,1) NOT NULL,
	[PendingRating] [decimal](18, 4) NULL,
	[TimeOfPendingRating] [datetime] NULL,
	[RewardTblRowID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_RewardPendingPointsTrackers] PRIMARY KEY CLUSTERED 
(
	[RewardPendingPointsTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RewardRatingSettings]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RewardRatingSettings](
	[RewardRatingSettingsID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NULL,
	[UserActionID] [int] NULL,
	[RatingGroupAttributesID] [int] NOT NULL,
	[ProbOfRewardEvaluation] [decimal](18, 4) NOT NULL,
	[Multiplier] [decimal](18, 4) NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RewardRatingSettings] PRIMARY KEY CLUSTERED 
(
	[RewardRatingSettingsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleStatus]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleStatus](
	[RoleStatusID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [nvarchar](max) NOT NULL,
	[LastCheckIn] [datetime] NULL,
	[IsWorkerRole] [bit] NOT NULL,
	[IsBackgroundProcessing] [bit] NOT NULL,
 CONSTRAINT [PK_RoleStatus] PRIMARY KEY CLUSTERED 
(
	[RoleStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordChoices]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordChoices](
	[SearchWordChoiceID] [int] IDENTITY(1,1) NOT NULL,
	[ChoiceInGroupID] [int] NOT NULL,
	[SearchWordID] [int] NOT NULL,
 CONSTRAINT [PK_SearchWordChoices_1] PRIMARY KEY CLUSTERED 
(
	[SearchWordChoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordHierarchyItems]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordHierarchyItems](
	[SearchWordHierarchyItemID] [int] IDENTITY(1,1) NOT NULL,
	[HierarchyItemID] [int] NOT NULL,
	[SearchWordID] [int] NOT NULL,
 CONSTRAINT [PK_SearchWordHierarchyItems] PRIMARY KEY CLUSTERED 
(
	[SearchWordHierarchyItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWords]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWords](
	[SearchWordID] [int] IDENTITY(1,1) NOT NULL,
	[TheWord] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SearchWords] PRIMARY KEY CLUSTERED 
(
	[SearchWordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordTblRowNames]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordTblRowNames](
	[SearchWordTblRowNameID] [int] IDENTITY(1,1) NOT NULL,
	[TblRowID] [int] NOT NULL,
	[SearchWordID] [int] NOT NULL,
 CONSTRAINT [PK_SearchWordTblRowNames] PRIMARY KEY CLUSTERED 
(
	[SearchWordTblRowNameID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchWordTextFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchWordTextFields](
	[SearchWordTextFieldID] [int] IDENTITY(1,1) NOT NULL,
	[TextFieldID] [int] NOT NULL,
	[SearchWordID] [int] NOT NULL,
 CONSTRAINT [PK_SearchWordTextFields] PRIMARY KEY CLUSTERED 
(
	[SearchWordTextFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubsidyAdjustments]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubsidyAdjustments](
	[SubsidyAdjustmentID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupPhaseStatusID] [int] NOT NULL,
	[SubsidyAdjustmentFactor] [decimal](18, 4) NOT NULL,
	[EffectiveTime] [datetime] NOT NULL,
	[EndingTime] [datetime] NULL,
	[EndingTimeHalfLife] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.SubsidyAdjustments] PRIMARY KEY CLUSTERED 
(
	[SubsidyAdjustmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubsidyDensityRangeGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubsidyDensityRangeGroups](
	[SubsidyDensityRangeGroupID] [int] IDENTITY(1,1) NOT NULL,
	[UseLogarithmBase] [decimal](18, 4) NULL,
	[CumDensityTotal] [decimal](18, 4) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.SubsidyDensityRangeGroups] PRIMARY KEY CLUSTERED 
(
	[SubsidyDensityRangeGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubsidyDensityRanges]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubsidyDensityRanges](
	[SubsidyDensityRangeID] [int] IDENTITY(1,1) NOT NULL,
	[SubsidyDensityRangeGroupID] [int] NOT NULL,
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
/****** Object:  Table [dbo].[TblColumnFormatting]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblColumnFormatting](
	[TblColumnFormattingID] [int] IDENTITY(1,1) NOT NULL,
	[TblColumnID] [int] NOT NULL,
	[Prefix] [nvarchar](10) NOT NULL,
	[Suffix] [nvarchar](10) NOT NULL,
	[OmitLeadingZero] [bit] NOT NULL,
	[ExtraDecimalPlaceAbove] [decimal](18, 4) NULL,
	[ExtraDecimalPlace2Above] [decimal](18, 4) NULL,
	[ExtraDecimalPlace3Above] [decimal](18, 4) NULL,
	[SuppStylesHeader] [nvarchar](max) NOT NULL,
	[SuppStylesMain] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_TblColumnFormatting] PRIMARY KEY CLUSTERED 
(
	[TblColumnFormattingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblColumns]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblColumns](
	[TblColumnID] [int] IDENTITY(1,1) NOT NULL,
	[TblTabID] [int] NOT NULL,
	[DefaultRatingGroupAttributesID] [int] NOT NULL,
	[ConditionTblColumnID] [int] NULL,
	[TrustTrackerUnitID] [int] NULL,
	[ConditionGreaterThan] [decimal](18, 4) NULL,
	[ConditionLessThan] [decimal](18, 4) NULL,
	[CategoryNum] [int] NOT NULL,
	[Abbreviation] [nchar](20) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Explanation] [nvarchar](max) NOT NULL,
	[WidthStyle] [nvarchar](20) NOT NULL CONSTRAINT [DF_TblColumns_WidthStyle]  DEFAULT (''),
	[NumNonNull] [int] NOT NULL DEFAULT ((0)),
	[ProportionNonNull] [float] NOT NULL DEFAULT ((0)),
	[UsingNonSparseColumn] [bit] NOT NULL DEFAULT ((1)),
	[ShouldUseNonSparseColumn] [bit] NOT NULL DEFAULT ((1)),
	[UseAsFilter] [bit] NOT NULL,
	[Sortable] [bit] NOT NULL,
	[DefaultSortOrderAscending] [bit] NOT NULL,
	[AutomaticallyCreateMissingRatings] [bit] NOT NULL DEFAULT ((0)),
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TblColumns] PRIMARY KEY CLUSTERED 
(
	[TblColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblDimensions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblDimensions](
	[TblDimensionsID] [int] IDENTITY(1,1) NOT NULL,
	[MaxWidthOfImageInRowHeaderCell] [int] NOT NULL,
	[MaxHeightOfImageInRowHeaderCell] [int] NOT NULL,
	[MaxWidthOfImageInTblRowPopUpWindow] [int] NOT NULL,
	[MaxHeightOfImageInTblRowPopUpWindow] [int] NOT NULL,
	[WidthOfTblRowPopUpWindow] [int] NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NULL,
 CONSTRAINT [PK_dbo.TblDimensions] PRIMARY KEY CLUSTERED 
(
	[TblDimensionsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblRowFieldDisplays]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblRowFieldDisplays](
	[TblRowFieldDisplayID] [int] IDENTITY(1,1) NOT NULL,
	[Row] [nvarchar](max) NULL,
	[PopUp] [nvarchar](max) NULL,
	[TblRowPage] [nvarchar](max) NULL,
	[ResetNeeded] [bit] NOT NULL,
 CONSTRAINT [PK_TblRowFieldDisplays] PRIMARY KEY CLUSTERED 
(
	[TblRowFieldDisplayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TblRows]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TblRows](
	[TblRowID] [int] IDENTITY(1,1) NOT NULL,
	[TblID] [int] NOT NULL,
	[TblRowFieldDisplayID] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[StatusRecentlyChanged] [bit] NOT NULL CONSTRAINT [DF_TblRows_StatusRecentlyChanged]  DEFAULT ((0)),
	[CountHighStakesNow] [int] NOT NULL CONSTRAINT [DF_TblRows_CountHighStakesNow]  DEFAULT ((0)),
	[CountNonnullEntries] [int] NOT NULL CONSTRAINT [DF_TblRows_CountNullEntries]  DEFAULT ((0)),
	[CountUserPoints] [decimal](18, 4) NOT NULL CONSTRAINT [DF_TblRows_CountUserPoints]  DEFAULT ((0)),
	[ElevateOnMostNeedsRating] [bit] NOT NULL CONSTRAINT [DF_TblRows_ElevateOnMostNeedsRating]  DEFAULT ((0)),
	[InitialFieldsDisplaySet] [bit] NOT NULL DEFAULT ((0)),
	[FastAccessInitialCopy] [bit] NOT NULL DEFAULT ((0)),
	[FastAccessDeleteThenRecopy] [bit] NOT NULL DEFAULT ((0)),
	[FastAccessUpdateFields] [bit] NOT NULL DEFAULT ((0)),
	[FastAccessUpdateRatings] [bit] NOT NULL DEFAULT ((0)),
	[FastAccessUpdateSpecified] [bit] NOT NULL DEFAULT ((0)),
	[FastAccessUpdated] [varbinary](max) NULL,
 CONSTRAINT [PK_dbo.TblRows] PRIMARY KEY CLUSTERED 
(
	[TblRowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TblRowStatusRecord]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblRowStatusRecord](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[TblRowId] [int] NOT NULL,
	[TimeChanged] [datetime] NOT NULL,
	[Adding] [bit] NOT NULL,
	[Deleting] [bit] NOT NULL,
 CONSTRAINT [PK_TblRowStatusRecord] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbls]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tbls](
	[TblID] [int] IDENTITY(1,1) NOT NULL,
	[PointsManagerID] [int] NOT NULL,
	[DefaultRatingGroupAttributesID] [int] NULL,
	[TblTabWord] [varchar](50) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[TypeOfTblRow] [varchar](50) NULL,
	[TblDimensionsID] [int] NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
	[AllowOverrideOfRatingGroupCharacterstics] [bit] NOT NULL,
	[AllowUsersToAddComments] [bit] NOT NULL,
	[LimitCommentsToUsersWhoCanMakeUserRatings] [bit] NOT NULL,
	[OneRatingPerRatingGroup] [bit] NOT NULL,
	[TblRowAdditionCriteria] [nvarchar](max) NOT NULL,
	[SuppStylesHeader] [nvarchar](max) NOT NULL,
	[SuppStylesMain] [nvarchar](max) NOT NULL,
	[WidthStyleEntityCol] [nvarchar](20) NOT NULL,
	[WidthStyleNumCol] [nvarchar](20) NOT NULL,
	[FastTableSyncStatus] [tinyint] NOT NULL CONSTRAINT [DF_Tbls_FastTableSyncStatus]  DEFAULT ((0)),
	[NumTblRowsActive] [int] NOT NULL DEFAULT ((0)),
	[NumTblRowsDeleted] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.Tbls] PRIMARY KEY CLUSTERED 
(
	[TblID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TblTabs]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblTabs](
	[TblTabID] [int] IDENTITY(1,1) NOT NULL,
	[TblID] [int] NOT NULL,
	[NumInTbl] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DefaultSortTblColumnID] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TblTabs] PRIMARY KEY CLUSTERED 
(
	[TblTabID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TextFieldDefinitions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TextFieldDefinitions](
	[TextFieldDefinitionID] [int] IDENTITY(1,1) NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[IncludeText] [bit] NOT NULL,
	[IncludeLink] [bit] NOT NULL,
	[Searchable] [bit] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_TextFieldDefinitions] PRIMARY KEY CLUSTERED 
(
	[TextFieldDefinitionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TextFields]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TextFields](
	[TextFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldID] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[Link] [nvarchar](max) NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TextFields] PRIMARY KEY CLUSTERED 
(
	[TextFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerForChoiceInGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerForChoiceInGroups](
	[TrustTrackerForChoiceInGroupID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ChoiceInGroupID] [int] NOT NULL,
	[TblID] [int] NOT NULL,
	[SumAdjustmentPctTimesRatingMagnitude] [real] NOT NULL,
	[SumRatingMagnitudes] [real] NOT NULL,
	[TrustLevelForChoice] [real] NOT NULL,
 CONSTRAINT [PK_TrustTrackerForChoiceInGroups] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerForChoiceInGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks](
	[TrustTrackerForChoiceInGroupsUserRatingLinkID] [int] IDENTITY(1,1) NOT NULL,
	[UserRatingID] [int] NOT NULL,
	[TrustTrackerForChoiceInGroupID] [int] NOT NULL,
 CONSTRAINT [PK_TrustTrackerForChoiceInGroupsUserRatingLinks] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerForChoiceInGroupsUserRatingLinkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackers]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackers](
	[TrustTrackerID] [int] IDENTITY(1,1) NOT NULL,
	[TrustTrackerUnitID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[OverallTrustLevel] [float] NOT NULL CONSTRAINT [DF_TrustTrackers_TrustLevel]  DEFAULT ((0)),
	[OverallTrustLevelAtLastReview] [float] NOT NULL CONSTRAINT [DF_TrustTrackers_LastTrustLevel]  DEFAULT ((0)),
	[DeltaOverallTrustLevel] [float] NOT NULL CONSTRAINT [DF_TrustTrackers_DeltaTrustLevel]  DEFAULT ((0)),
	[SkepticalTrustLevel] [float] NOT NULL CONSTRAINT [DF_TrustTrackers_SkepticalTrustLevel]  DEFAULT ((0)),
	[SumUserInteractionWeights] [float] NOT NULL CONSTRAINT [DF_TrustTrackers_SumUserInteractionWeights]  DEFAULT ((0)),
	[EgalitarianTrustLevel] [float] NOT NULL DEFAULT ((0)),
	[Egalitarian_Num] [float] NOT NULL DEFAULT ((0)),
	[Egalitarian_Denom] [float] NOT NULL DEFAULT ((0)),
	[EgalitarianTrustLevelOverride] [float] NULL,
	[MustUpdateUserInteractionEgalitarianTrustLevel] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_TrustTrackers] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerStats]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerStats](
	[TrustTrackerStatID] [int] IDENTITY(1,1) NOT NULL,
	[TrustTrackerID] [int] NOT NULL,
	[StatNum] [smallint] NOT NULL,
	[TrustValue] [float] NOT NULL CONSTRAINT [DF_TrustTrackerStats_SumWeightedAdjPcts]  DEFAULT ((0)),
	[Trust_Numer] [float] NOT NULL DEFAULT ((0)),
	[Trust_Denom] [float] NOT NULL DEFAULT ((0)),
	[SumUserInteractionStatWeights] [float] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_TrustTrackerStats] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerStatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TrustTrackerUnits]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrustTrackerUnits](
	[TrustTrackerUnitID] [int] IDENTITY(1,1) NOT NULL,
	[SkepticalTrustThreshhold] [smallint] NOT NULL CONSTRAINT [DF_TrustTrackerUnits_SkepticalTrustThreshhold]  DEFAULT ((0)),
	[LastSkepticalTrustThreshhold] [smallint] NOT NULL CONSTRAINT [DF_TrustTrackerUnits_LastSkepticalTrustThreshhold]  DEFAULT ((0)),
	[MinUpdateIntervalSeconds] [int] NOT NULL DEFAULT ((0)),
	[MaxUpdateIntervalSeconds] [int] NOT NULL DEFAULT ((0)),
	[ExtendIntervalWhenChangeIsLessThanThis] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[ExtendIntervalMultiplier] [decimal](18, 4) NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_TrustTrackerUnits] PRIMARY KEY CLUSTERED 
(
	[TrustTrackerUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UniquenessLockReferences]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UniquenessLockReferences](
	[Id] [uniqueidentifier] NOT NULL,
	[UniquenessLockID] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UniquenessLocks]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UniquenessLocks](
	[Id] [uniqueidentifier] NOT NULL,
	[DeletionTime] [datetime] NULL,
PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserActions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserActions](
	[UserActionID] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](max) NOT NULL,
	[SuperUser] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.UserActions] PRIMARY KEY CLUSTERED 
(
	[UserActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserCheckIns]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCheckIns](
	[UserCheckInID] [int] IDENTITY(1,1) NOT NULL,
	[CheckInTime] [datetime] NOT NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_UserCheckIns] PRIMARY KEY CLUSTERED 
(
	[UserCheckInID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserInfo](
	[UserInfoID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Email] [varchar](250) NOT NULL,
	[Address1] [varchar](200) NOT NULL,
	[Address2] [varchar](200) NOT NULL,
	[WorkPhone] [varchar](50) NOT NULL,
	[HomePhone] [varchar](50) NOT NULL,
	[MobilePhone] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[ZipCode] [varchar](50) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](50) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[IsVerified] [bit] NOT NULL,
 CONSTRAINT [PK_UserInfo] PRIMARY KEY CLUSTERED 
(
	[UserInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserInteractions]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInteractions](
	[UserInteractionID] [int] IDENTITY(1,1) NOT NULL,
	[TrustTrackerUnitID] [int] NOT NULL,
	[OrigRatingUserID] [int] NOT NULL,
	[LatestRatingUserID] [int] NOT NULL,
	[NumTransactions] [int] NOT NULL,
	[LatestUserEgalitarianTrust] [float] NOT NULL,
	[WeightInCalculatingTrustTotal] [float] NOT NULL,
	[LatestUserEgalitarianTrustAtLastWeightUpdate] [float] NULL,
 CONSTRAINT [PK_UserInteractions] PRIMARY KEY CLUSTERED 
(
	[UserInteractionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInteractionStats]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInteractionStats](
	[UserInteractionStatID] [int] IDENTITY(1,1) NOT NULL,
	[UserInteractionID] [int] NOT NULL,
	[TrustTrackerStatID] [int] NOT NULL,
	[StatNum] [smallint] NOT NULL,
	[SumAdjustPctTimesWeight] [float] NOT NULL,
	[SumWeights] [float] NOT NULL,
	[AvgAdjustmentPctWeighted] [float] NOT NULL,
 CONSTRAINT [PK_UserInteractionStats] PRIMARY KEY CLUSTERED 
(
	[UserInteractionStatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRatingGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRatingGroups](
	[UserRatingGroupID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupID] [int] NOT NULL,
	[RatingGroupPhaseStatusID] [int] NOT NULL,
	[WhenMade] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.UserRatingGroups] PRIMARY KEY CLUSTERED 
(
	[UserRatingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRatings]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRatings](
	[UserRatingID] [int] IDENTITY(1,1) NOT NULL,
	[UserRatingGroupID] [int] NOT NULL,
	[RatingID] [int] NOT NULL,
	[RatingPhaseStatusID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[TrustTrackerUnitID] [int] NULL,
	[RewardPendingPointsTrackerID] [int] NULL,
	[MostRecentUserRatingID] [int] NULL,
	[PreviousRatingOrVirtualRating] [decimal](18, 4) NOT NULL,
	[PreviousDisplayedRating] [decimal](18, 4) NULL,
	[EnteredUserRating] [decimal](18, 4) NOT NULL,
	[NewUserRating] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[OriginalAdjustmentPct] [decimal](7, 4) NOT NULL DEFAULT ((0)),
	[OriginalTrustLevel] [decimal](7, 4) NOT NULL DEFAULT ((0)),
	[MaxGain] [decimal](18, 4) NOT NULL CONSTRAINT [DF_UserRatings_MaxGain]  DEFAULT ((0)),
	[MaxLoss] [decimal](18, 4) NOT NULL CONSTRAINT [DF_UserRatings_MaxLoss]  DEFAULT ((0)),
	[PotentialPointsShortTerm] [decimal](18, 4) NOT NULL,
	[PotentialPointsLongTerm] [decimal](18, 4) NOT NULL,
	[PotentialPointsLongTermUnweighted] [decimal](18, 4) NOT NULL CONSTRAINT [DF_UserRatings_PotentialPointsLongTermUnweighted]  DEFAULT ((0)),
	[LongTermPointsWeight] [decimal](18, 4) NOT NULL CONSTRAINT [DF_UserRatings_LongTermPointsWeight]  DEFAULT ((0)),
	[PointsPumpingProportion] [decimal](18, 4) NULL DEFAULT ((0)),
	[PastPointsPumpingProportion] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[PercentPreviousRatings] [decimal](18, 4) NOT NULL CONSTRAINT [DF_UserRatings_PercentPreviousRatings]  DEFAULT ((0)),
	[IsTrusted] [bit] NOT NULL CONSTRAINT [DF_UserRatings_IsTrusted]  DEFAULT ((0)),
	[MadeDirectly] [bit] NOT NULL,
	[LongTermResolutionReflected] [bit] NOT NULL CONSTRAINT [DF_UserRatings_LongTermResolutionReflected]  DEFAULT ((0)),
	[ShortTermResolutionReflected] [bit] NOT NULL,
	[PointsHaveBecomePending] [bit] NOT NULL,
	[ForceRecalculate] [bit] NOT NULL CONSTRAINT [DF_UserRatings_ForceRecalculate]  DEFAULT ((0)),
	[HighStakesPreviouslySecret] [bit] NOT NULL CONSTRAINT [DF_UserRatings_HighStakesPreviouslySecret]  DEFAULT ((0)),
	[HighStakesKnown] [bit] NOT NULL CONSTRAINT [DF_UserRatings_HighStakesKnown]  DEFAULT ((0)),
	[PreviouslyRated] [bit] NOT NULL DEFAULT ((0)),
	[SubsequentlyRated] [bit] NOT NULL CONSTRAINT [DF_UserRatings_SubsequentlyRated]  DEFAULT ((0)),
	[IsMostRecent10Pct] [bit] NOT NULL DEFAULT ((1)),
	[IsMostRecent30Pct] [bit] NOT NULL DEFAULT ((1)),
	[IsMostRecent70Pct] [bit] NOT NULL DEFAULT ((1)),
	[IsMostRecent90Pct] [bit] NOT NULL DEFAULT ((1)),
	[IsUsersFirstWeek] [bit] NOT NULL DEFAULT ((0)),
	[LogarithmicBase] [decimal](18, 4) NULL,
	[HighStakesMultiplierOverride] [decimal](18, 4) NULL,
	[WhenPointsBecomePending] [datetime] NULL,
	[LastModifiedTime] [datetime] NOT NULL,
	[VolatilityTrackingNextTimeFrameToRemove] [tinyint] NOT NULL CONSTRAINT [DF_UserRatings_VolatilityTrackingTimeFrameComplete]  DEFAULT ((0)),
	[LastWeekDistanceFromStart] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[LastWeekPushback] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[LastYearPushback] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[UserRatingNumberForUser] [int] NOT NULL DEFAULT ((0)),
	[NextRecencyUpdateAtUserRatingNum] [int] NULL,
 CONSTRAINT [PK_dbo.UserRatings] PRIMARY KEY CLUSTERED 
(
	[UserRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRatingsToAdd]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRatingsToAdd](
	[UserRatingsToAddID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[TopRatingGroupID] [int] NOT NULL,
	[UserRatingHierarchy] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_UserRatingsToAdd] PRIMARY KEY CLUSTERED 
(
	[UserRatingsToAddID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[SuperUser] [bit] NOT NULL,
	[TrustPointsRatioTotals] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Users_TrustPointsRatioTotals]  DEFAULT ((0)),
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersAdministrationRightsGroups]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersAdministrationRightsGroups](
	[UsersAdministrationRightsGroupID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[PointsManagerID] [int] NOT NULL,
	[AdministrationRightsGroupID] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.UsersAdministrationRightsGroups] PRIMARY KEY CLUSTERED 
(
	[UsersAdministrationRightsGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersRights]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersRights](
	[UsersRightsID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[PointsManagerID] [int] NULL,
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
	[Name] [nvarchar](max) NOT NULL,
	[Creator] [int] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.UsersRights] PRIMARY KEY CLUSTERED 
(
	[UsersRightsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VolatilityTblRowTrackers]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolatilityTblRowTrackers](
	[VolatilityTblRowTrackerID] [int] IDENTITY(1,1) NOT NULL,
	[TblRowID] [int] NOT NULL,
	[DurationType] [tinyint] NOT NULL,
	[TotalMovement] [decimal](18, 4) NOT NULL,
	[DistanceFromStart] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[Pushback] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[PushbackProportion] [decimal](18, 4) NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_VolatilityTblRowTrackers] PRIMARY KEY CLUSTERED 
(
	[VolatilityTblRowTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VolatilityTrackers]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolatilityTrackers](
	[VolatilityTrackerID] [int] IDENTITY(1,1) NOT NULL,
	[RatingGroupID] [int] NOT NULL,
	[VolatilityTblRowTrackerID] [int] NOT NULL,
	[DurationType] [tinyint] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[TotalMovement] [decimal](18, 4) NOT NULL,
	[DistanceFromStart] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[Pushback] [decimal](18, 4) NOT NULL DEFAULT ((0)),
	[PushbackProportion] [decimal](18, 4) NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_VolatilityTrackers] PRIMARY KEY CLUSTERED 
(
	[VolatilityTrackerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[vw_aspnet_Applications]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Applications]
  AS SELECT [dbo].[aspnet_Applications].[ApplicationName], [dbo].[aspnet_Applications].[LoweredApplicationName], [dbo].[aspnet_Applications].[ApplicationId], [dbo].[aspnet_Applications].[Description]
  FROM [dbo].[aspnet_Applications]
  
GO
/****** Object:  View [dbo].[vw_aspnet_MembershipUsers]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  View [dbo].[vw_aspnet_Profiles]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  View [dbo].[vw_aspnet_Roles]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Roles]
  AS SELECT [dbo].[aspnet_Roles].[ApplicationId], [dbo].[aspnet_Roles].[RoleId], [dbo].[aspnet_Roles].[RoleName], [dbo].[aspnet_Roles].[LoweredRoleName], [dbo].[aspnet_Roles].[Description]
  FROM [dbo].[aspnet_Roles]
  
GO
/****** Object:  View [dbo].[vw_aspnet_Users]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_Users]
  AS SELECT [dbo].[aspnet_Users].[ApplicationId], [dbo].[aspnet_Users].[UserId], [dbo].[aspnet_Users].[UserName], [dbo].[aspnet_Users].[LoweredUserName], [dbo].[aspnet_Users].[MobileAlias], [dbo].[aspnet_Users].[IsAnonymous], [dbo].[aspnet_Users].[LastActivityDate]
  FROM [dbo].[aspnet_Users]
  
GO
/****** Object:  View [dbo].[vw_aspnet_UsersInRoles]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_UsersInRoles]
  AS SELECT [dbo].[aspnet_UsersInRoles].[UserId], [dbo].[aspnet_UsersInRoles].[RoleId]
  FROM [dbo].[aspnet_UsersInRoles]
  
GO
/****** Object:  View [dbo].[vw_aspnet_WebPartState_Paths]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_WebPartState_Paths]
  AS SELECT [dbo].[aspnet_Paths].[ApplicationId], [dbo].[aspnet_Paths].[PathId], [dbo].[aspnet_Paths].[Path], [dbo].[aspnet_Paths].[LoweredPath]
  FROM [dbo].[aspnet_Paths]
  
GO
/****** Object:  View [dbo].[vw_aspnet_WebPartState_Shared]    Script Date: 7/10/2014 2:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

  CREATE VIEW [dbo].[vw_aspnet_WebPartState_Shared]
  AS SELECT [dbo].[aspnet_PersonalizationAllUsers].[PathId], [DataSize]=DATALENGTH([dbo].[aspnet_PersonalizationAllUsers].[PageSettings]), [dbo].[aspnet_PersonalizationAllUsers].[LastUpdatedDate]
  FROM [dbo].[aspnet_PersonalizationAllUsers]
  
GO
/****** Object:  View [dbo].[vw_aspnet_WebPartState_User]    Script Date: 7/10/2014 2:45:47 PM ******/
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
/****** Object:  Index [aspnet_Applications_Index]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE CLUSTERED INDEX [aspnet_Applications_Index] ON [dbo].[aspnet_Applications]
(
	[LoweredApplicationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Membership_index]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE CLUSTERED INDEX [aspnet_Membership_index] ON [dbo].[aspnet_Membership]
(
	[ApplicationId] ASC,
	[LoweredEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Paths_index]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_Paths_index] ON [dbo].[aspnet_Paths]
(
	[ApplicationId] ASC,
	[LoweredPath] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_PersonalizationPerUser_index1]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_PersonalizationPerUser_index1] ON [dbo].[aspnet_PersonalizationPerUser]
(
	[PathId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Roles_index1]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_Roles_index1] ON [dbo].[aspnet_Roles]
(
	[ApplicationId] ASC,
	[LoweredRoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [aspnet_Users_Index]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE UNIQUE CLUSTERED INDEX [aspnet_Users_Index] ON [dbo].[aspnet_Users]
(
	[ApplicationId] ASC,
	[LoweredUserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UniquenessLocks_DeletionTime]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE CLUSTERED INDEX [IX_UniquenessLocks_DeletionTime] ON [dbo].[UniquenessLocks]
(
	[DeletionTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_PersonalizationPerUser_ncindex2]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [aspnet_PersonalizationPerUser_ncindex2] ON [dbo].[aspnet_PersonalizationPerUser]
(
	[UserId] ASC,
	[PathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_Users_Index2]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE NONCLUSTERED INDEX [aspnet_Users_Index2] ON [dbo].[aspnet_Users]
(
	[ApplicationId] ASC,
	[LastActivityDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aspnet_UsersInRoles_index]    Script Date: 7/10/2014 2:45:47 PM ******/
CREATE NONCLUSTERED INDEX [aspnet_UsersInRoles_index] ON [dbo].[aspnet_UsersInRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[aspnet_Paths] ADD  DEFAULT (newid()) FOR [PathId]
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[aspnet_Roles] ADD  DEFAULT (newid()) FOR [RoleId]
GO
ALTER TABLE [dbo].[ForumMessages] ADD  DEFAULT ((1)) FOR [Visible]
GO
ALTER TABLE [dbo].[ForumMessages] ADD  DEFAULT ((0)) FOR [Rating]
GO
ALTER TABLE [dbo].[ForumPersonalMessages] ADD  DEFAULT ((1)) FOR [New]
GO
ALTER TABLE [dbo].[Forums] ADD  DEFAULT ((0)) FOR [Premoderated]
GO
ALTER TABLE [dbo].[Forums] ADD  DEFAULT ((0)) FOR [MembersOnly]
GO
ALTER TABLE [dbo].[Forums] ADD  DEFAULT ((0)) FOR [OrderByNumber]
GO
ALTER TABLE [dbo].[Forums] ADD  DEFAULT ((0)) FOR [RestrictTopicCreation]
GO
ALTER TABLE [dbo].[ForumTopics] ADD  DEFAULT ((0)) FOR [LastMessageID]
GO
ALTER TABLE [dbo].[ForumTopics] ADD  DEFAULT ((0)) FOR [IsSticky]
GO
ALTER TABLE [dbo].[ForumTopics] ADD  DEFAULT ((0)) FOR [IsClosed]
GO
ALTER TABLE [dbo].[ForumTopics] ADD  DEFAULT ((0)) FOR [ViewsCount]
GO
ALTER TABLE [dbo].[ForumTopics] ADD  DEFAULT ((0)) FOR [RepliesCount]
GO
ALTER TABLE [dbo].[ForumUsers] ADD  DEFAULT ((0)) FOR [PostsCount]
GO
ALTER TABLE [dbo].[ForumUsers] ADD  DEFAULT (getdate()) FOR [RegistrationDate]
GO
ALTER TABLE [dbo].[ForumUsers] ADD  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[ForumUsers] ADD  DEFAULT ('') FOR [ActivationCode]
GO
ALTER TABLE [dbo].[ForumUsers] ADD  DEFAULT ((0)) FOR [ReputationCache]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] ADD  DEFAULT ((1)) FOR [TrustLevelForChoice]
GO
ALTER TABLE [dbo].[UserInteractionStats] ADD  CONSTRAINT [DF_UserInteractionStats_SumAdjustPctTimesWeight]  DEFAULT ((0)) FOR [SumAdjustPctTimesWeight]
GO
ALTER TABLE [dbo].[UserInteractionStats] ADD  CONSTRAINT [DF_UserInteractionStats_SumWeights]  DEFAULT ((0)) FOR [SumWeights]
GO
ALTER TABLE [dbo].[UserInteractionStats] ADD  DEFAULT ((0)) FOR [AvgAdjustmentPctWeighted]
GO
ALTER TABLE [dbo].[AddressFields]  WITH CHECK ADD  CONSTRAINT [Field_AddressField] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[AddressFields] CHECK CONSTRAINT [Field_AddressField]
GO
ALTER TABLE [dbo].[AdministrationRights]  WITH CHECK ADD  CONSTRAINT [AdministrationRightsGroup_AdministrationRight] FOREIGN KEY([AdministrationRightsGroupID])
REFERENCES [dbo].[AdministrationRightsGroups] ([AdministrationRightsGroupID])
GO
ALTER TABLE [dbo].[AdministrationRights] CHECK CONSTRAINT [AdministrationRightsGroup_AdministrationRight]
GO
ALTER TABLE [dbo].[AdministrationRights]  WITH CHECK ADD  CONSTRAINT [UserAction_AdministrationRight] FOREIGN KEY([UserActionID])
REFERENCES [dbo].[UserActions] ([UserActionID])
GO
ALTER TABLE [dbo].[AdministrationRights] CHECK CONSTRAINT [UserAction_AdministrationRight]
GO
ALTER TABLE [dbo].[AdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [PointsManager_AdministrationRightsGroup] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[AdministrationRightsGroups] CHECK CONSTRAINT [PointsManager_AdministrationRightsGroup]
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
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [FK_ChangesGroup_Ratings] FOREIGN KEY([RewardRatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [FK_ChangesGroup_Ratings]
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [PointsManager_ChangesGroup] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [PointsManager_ChangesGroup]
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [Tbl_ChangesGroup] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [Tbl_ChangesGroup]
GO
ALTER TABLE [dbo].[ChangesGroup]  WITH CHECK ADD  CONSTRAINT [User_ChangesGroup] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[ChangesGroup] CHECK CONSTRAINT [User_ChangesGroup]
GO
ALTER TABLE [dbo].[ChangesStatusOfObject]  WITH CHECK ADD  CONSTRAINT [ChangesGroup_ChangesStatusOfObject] FOREIGN KEY([ChangesGroupID])
REFERENCES [dbo].[ChangesGroup] ([ChangesGroupID])
GO
ALTER TABLE [dbo].[ChangesStatusOfObject] CHECK CONSTRAINT [ChangesGroup_ChangesStatusOfObject]
GO
ALTER TABLE [dbo].[ChoiceFields]  WITH CHECK ADD  CONSTRAINT [Field_ChoiceField] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[ChoiceFields] CHECK CONSTRAINT [Field_ChoiceField]
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [ChoiceGroup_ChoiceGroupFieldDefinition] FOREIGN KEY([ChoiceGroupID])
REFERENCES [dbo].[ChoiceGroups] ([ChoiceGroupID])
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions] CHECK CONSTRAINT [ChoiceGroup_ChoiceGroupFieldDefinition]
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FieldDefinition_ChoiceGroupFieldDefinition] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions] CHECK CONSTRAINT [FieldDefinition_ChoiceGroupFieldDefinition]
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_ChoiceGroupFieldDefinitions_ChoiceGroupFieldDefinitions] FOREIGN KEY([DependentOnChoiceGroupFieldDefinitionID])
REFERENCES [dbo].[ChoiceGroupFieldDefinitions] ([ChoiceGroupFieldDefinitionID])
GO
ALTER TABLE [dbo].[ChoiceGroupFieldDefinitions] CHECK CONSTRAINT [FK_ChoiceGroupFieldDefinitions_ChoiceGroupFieldDefinitions]
GO
ALTER TABLE [dbo].[ChoiceGroups]  WITH CHECK ADD  CONSTRAINT [FK_ChoiceGroups_ChoiceGroups] FOREIGN KEY([DependentOnChoiceGroupID])
REFERENCES [dbo].[ChoiceGroups] ([ChoiceGroupID])
GO
ALTER TABLE [dbo].[ChoiceGroups] CHECK CONSTRAINT [FK_ChoiceGroups_ChoiceGroups]
GO
ALTER TABLE [dbo].[ChoiceGroups]  WITH CHECK ADD  CONSTRAINT [PointsManager_ChoiceGroup] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ChoiceGroups] CHECK CONSTRAINT [PointsManager_ChoiceGroup]
GO
ALTER TABLE [dbo].[ChoiceGroups]  WITH CHECK ADD  CONSTRAINT [User_ChoiceGroup] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[ChoiceGroups] CHECK CONSTRAINT [User_ChoiceGroup]
GO
ALTER TABLE [dbo].[ChoiceInFields]  WITH CHECK ADD  CONSTRAINT [FK_ChoiceInFields_ChoiceFields] FOREIGN KEY([ChoiceFieldID])
REFERENCES [dbo].[ChoiceFields] ([ChoiceFieldID])
GO
ALTER TABLE [dbo].[ChoiceInFields] CHECK CONSTRAINT [FK_ChoiceInFields_ChoiceFields]
GO
ALTER TABLE [dbo].[ChoiceInFields]  WITH CHECK ADD  CONSTRAINT [FK_ChoiceInFields_ChoiceInGroups] FOREIGN KEY([ChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[ChoiceInFields] CHECK CONSTRAINT [FK_ChoiceInFields_ChoiceInGroups]
GO
ALTER TABLE [dbo].[ChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [ChoiceGroup_ChoiceInGroup] FOREIGN KEY([ChoiceGroupID])
REFERENCES [dbo].[ChoiceGroups] ([ChoiceGroupID])
GO
ALTER TABLE [dbo].[ChoiceInGroups] CHECK CONSTRAINT [ChoiceGroup_ChoiceInGroup]
GO
ALTER TABLE [dbo].[ChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_ChoiceInGroups_ChoiceInGroups] FOREIGN KEY([ActiveOnDeterminingGroupChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[ChoiceInGroups] CHECK CONSTRAINT [FK_ChoiceInGroups_ChoiceInGroups]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [TblRow_Comment] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [TblRow_Comment]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [User_Comment] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [User_Comment]
GO
ALTER TABLE [dbo].[DateTimeFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FieldDefinition_DateTimeFieldDefinition] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[DateTimeFieldDefinitions] CHECK CONSTRAINT [FieldDefinition_DateTimeFieldDefinition]
GO
ALTER TABLE [dbo].[DateTimeFields]  WITH CHECK ADD  CONSTRAINT [Field_DateTimeField] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[DateTimeFields] CHECK CONSTRAINT [Field_DateTimeField]
GO
ALTER TABLE [dbo].[Domains]  WITH CHECK ADD  CONSTRAINT [TblDimension_Domain] FOREIGN KEY([TblDimensionsID])
REFERENCES [dbo].[TblDimensions] ([TblDimensionsID])
GO
ALTER TABLE [dbo].[Domains] CHECK CONSTRAINT [TblDimension_Domain]
GO
ALTER TABLE [dbo].[FieldDefinitions]  WITH CHECK ADD  CONSTRAINT [Tbl_FieldDefinition] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[FieldDefinitions] CHECK CONSTRAINT [Tbl_FieldDefinition]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FieldDefinition_Field] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FieldDefinition_Field]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [TblRow_Field] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [TblRow_Field]
GO
ALTER TABLE [dbo].[HierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyItems_HierarchyItems] FOREIGN KEY([HigherHierarchyItemForRoutingID])
REFERENCES [dbo].[HierarchyItems] ([HierarchyItemID])
GO
ALTER TABLE [dbo].[HierarchyItems] CHECK CONSTRAINT [FK_HierarchyItems_HierarchyItems]
GO
ALTER TABLE [dbo].[HierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyItems_HierarchyItems1] FOREIGN KEY([HigherHierarchyItemID])
REFERENCES [dbo].[HierarchyItems] ([HierarchyItemID])
GO
ALTER TABLE [dbo].[HierarchyItems] CHECK CONSTRAINT [FK_HierarchyItems_HierarchyItems1]
GO
ALTER TABLE [dbo].[HierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyItems_Tbls] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[HierarchyItems] CHECK CONSTRAINT [FK_HierarchyItems_Tbls]
GO
ALTER TABLE [dbo].[InsertableContents]  WITH CHECK ADD  CONSTRAINT [Domain_InsertableContents] FOREIGN KEY([DomainID])
REFERENCES [dbo].[Domains] ([DomainID])
GO
ALTER TABLE [dbo].[InsertableContents] CHECK CONSTRAINT [Domain_InsertableContents]
GO
ALTER TABLE [dbo].[InsertableContents]  WITH CHECK ADD  CONSTRAINT [PointsManager_InsertableContents] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[InsertableContents] CHECK CONSTRAINT [PointsManager_InsertableContents]
GO
ALTER TABLE [dbo].[InsertableContents]  WITH CHECK ADD  CONSTRAINT [Tbl_InsertableContents] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[InsertableContents] CHECK CONSTRAINT [Tbl_InsertableContents]
GO
ALTER TABLE [dbo].[NumberFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FieldDefinition_NumberFieldDefinition] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[NumberFieldDefinitions] CHECK CONSTRAINT [FieldDefinition_NumberFieldDefinition]
GO
ALTER TABLE [dbo].[NumberFields]  WITH CHECK ADD  CONSTRAINT [Field_NumberField] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[NumberFields] CHECK CONSTRAINT [Field_NumberField]
GO
ALTER TABLE [dbo].[OverrideCharacteristics]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_OverrideCharacteristic] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[OverrideCharacteristics] CHECK CONSTRAINT [RatingGroupAttribute_OverrideCharacteristic]
GO
ALTER TABLE [dbo].[OverrideCharacteristics]  WITH CHECK ADD  CONSTRAINT [TblColumn_OverrideCharacteristic] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[OverrideCharacteristics] CHECK CONSTRAINT [TblColumn_OverrideCharacteristic]
GO
ALTER TABLE [dbo].[OverrideCharacteristics]  WITH CHECK ADD  CONSTRAINT [TblRow_OverrideCharacteristic] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[OverrideCharacteristics] CHECK CONSTRAINT [TblRow_OverrideCharacteristic]
GO
ALTER TABLE [dbo].[PointsAdjustments]  WITH CHECK ADD  CONSTRAINT [PointsManager_PointsAdjustment] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[PointsAdjustments] CHECK CONSTRAINT [PointsManager_PointsAdjustment]
GO
ALTER TABLE [dbo].[PointsAdjustments]  WITH CHECK ADD  CONSTRAINT [User_PointsAdjustment] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[PointsAdjustments] CHECK CONSTRAINT [User_PointsAdjustment]
GO
ALTER TABLE [dbo].[PointsManagers]  WITH CHECK ADD  CONSTRAINT [FK_PointsManagers_TrustTrackerUnits] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[PointsManagers] CHECK CONSTRAINT [FK_PointsManagers_TrustTrackerUnits]
GO
ALTER TABLE [dbo].[PointsManagers]  WITH CHECK ADD  CONSTRAINT [PointsManagers_Domains] FOREIGN KEY([DomainID])
REFERENCES [dbo].[Domains] ([DomainID])
GO
ALTER TABLE [dbo].[PointsManagers] CHECK CONSTRAINT [PointsManagers_Domains]
GO
ALTER TABLE [dbo].[PointsTotals]  WITH CHECK ADD  CONSTRAINT [PointsManager_PointsTotal] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[PointsTotals] CHECK CONSTRAINT [PointsManager_PointsTotal]
GO
ALTER TABLE [dbo].[PointsTotals]  WITH CHECK ADD  CONSTRAINT [User_PointsTotal] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[PointsTotals] CHECK CONSTRAINT [User_PointsTotal]
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings]  WITH CHECK ADD  CONSTRAINT [PointsManager_ProposalEvaluationRatingSetting] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings] CHECK CONSTRAINT [PointsManager_ProposalEvaluationRatingSetting]
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_ProposalEvaluationRatingSetting] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings] CHECK CONSTRAINT [RatingGroupAttribute_ProposalEvaluationRatingSetting]
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings]  WITH CHECK ADD  CONSTRAINT [UserAction_ProposalEvaluationRatingSetting] FOREIGN KEY([UserActionID])
REFERENCES [dbo].[UserActions] ([UserActionID])
GO
ALTER TABLE [dbo].[ProposalEvaluationRatingSettings] CHECK CONSTRAINT [UserAction_ProposalEvaluationRatingSetting]
GO
ALTER TABLE [dbo].[ProposalSettings]  WITH CHECK ADD  CONSTRAINT [PointsManager_ProposalSetting] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[ProposalSettings] CHECK CONSTRAINT [PointsManager_ProposalSetting]
GO
ALTER TABLE [dbo].[ProposalSettings]  WITH CHECK ADD  CONSTRAINT [Tbl_ProposalSetting] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[ProposalSettings] CHECK CONSTRAINT [Tbl_ProposalSetting]
GO
ALTER TABLE [dbo].[RatingCharacteristics]  WITH CHECK ADD  CONSTRAINT [RatingPhaseGroup_RatingCharacteristic] FOREIGN KEY([RatingPhaseGroupID])
REFERENCES [dbo].[RatingPhaseGroups] ([RatingPhaseGroupID])
GO
ALTER TABLE [dbo].[RatingCharacteristics] CHECK CONSTRAINT [RatingPhaseGroup_RatingCharacteristic]
GO
ALTER TABLE [dbo].[RatingCharacteristics]  WITH CHECK ADD  CONSTRAINT [SubsidyDensityRangeGroup_RatingCharacteristic] FOREIGN KEY([SubsidyDensityRangeGroupID])
REFERENCES [dbo].[SubsidyDensityRangeGroups] ([SubsidyDensityRangeGroupID])
GO
ALTER TABLE [dbo].[RatingCharacteristics] CHECK CONSTRAINT [SubsidyDensityRangeGroup_RatingCharacteristic]
GO
ALTER TABLE [dbo].[RatingCharacteristics]  WITH CHECK ADD  CONSTRAINT [User_RatingCharacteristic] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingCharacteristics] CHECK CONSTRAINT [User_RatingCharacteristic]
GO
ALTER TABLE [dbo].[RatingConditions]  WITH CHECK ADD  CONSTRAINT [FK_RatingConditions_Ratings] FOREIGN KEY([ConditionRatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[RatingConditions] CHECK CONSTRAINT [FK_RatingConditions_Ratings]
GO
ALTER TABLE [dbo].[RatingGroupAttributes]  WITH CHECK ADD  CONSTRAINT [PointsManager_RatingGroupAttribute] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[RatingGroupAttributes] CHECK CONSTRAINT [PointsManager_RatingGroupAttribute]
GO
ALTER TABLE [dbo].[RatingGroupAttributes]  WITH CHECK ADD  CONSTRAINT [RatingCharacteristic_RatingGroupAttribute] FOREIGN KEY([RatingCharacteristicsID])
REFERENCES [dbo].[RatingCharacteristics] ([RatingCharacteristicsID])
GO
ALTER TABLE [dbo].[RatingGroupAttributes] CHECK CONSTRAINT [RatingCharacteristic_RatingGroupAttribute]
GO
ALTER TABLE [dbo].[RatingGroupAttributes]  WITH CHECK ADD  CONSTRAINT [RatingCondition_RatingGroupAttribute] FOREIGN KEY([RatingConditionID])
REFERENCES [dbo].[RatingConditions] ([RatingConditionID])
GO
ALTER TABLE [dbo].[RatingGroupAttributes] CHECK CONSTRAINT [RatingCondition_RatingGroupAttribute]
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_RatingGroupPhaseStatus_RatingGroups] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus] CHECK CONSTRAINT [FK_RatingGroupPhaseStatus_RatingGroups]
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus]  WITH CHECK ADD  CONSTRAINT [RatingPhase_RatingGroupPhaseStatus] FOREIGN KEY([RatingPhaseID])
REFERENCES [dbo].[RatingPhases] ([RatingPhaseID])
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus] CHECK CONSTRAINT [RatingPhase_RatingGroupPhaseStatus]
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus]  WITH CHECK ADD  CONSTRAINT [RatingPhaseGroup_RatingGroupPhaseStatus] FOREIGN KEY([RatingPhaseGroupID])
REFERENCES [dbo].[RatingPhaseGroups] ([RatingPhaseGroupID])
GO
ALTER TABLE [dbo].[RatingGroupPhaseStatus] CHECK CONSTRAINT [RatingPhaseGroup_RatingGroupPhaseStatus]
GO
ALTER TABLE [dbo].[RatingGroupResolutions]  WITH CHECK ADD  CONSTRAINT [RatingGroup_RatingGroupResolution] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[RatingGroupResolutions] CHECK CONSTRAINT [RatingGroup_RatingGroupResolution]
GO
ALTER TABLE [dbo].[RatingGroupResolutions]  WITH CHECK ADD  CONSTRAINT [User_RatingGroupResolution] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingGroupResolutions] CHECK CONSTRAINT [User_RatingGroupResolution]
GO
ALTER TABLE [dbo].[RatingGroups]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_RatingGroup] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RatingGroups] CHECK CONSTRAINT [RatingGroupAttribute_RatingGroup]
GO
ALTER TABLE [dbo].[RatingGroups]  WITH CHECK ADD  CONSTRAINT [TblColumn_RatingGroup] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[RatingGroups] CHECK CONSTRAINT [TblColumn_RatingGroup]
GO
ALTER TABLE [dbo].[RatingGroups]  WITH CHECK ADD  CONSTRAINT [TblRow_RatingGroup] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[RatingGroups] CHECK CONSTRAINT [TblRow_RatingGroup]
GO
ALTER TABLE [dbo].[RatingGroupStatusRecords]  WITH CHECK ADD  CONSTRAINT [FK_RatingGroupStatusRecords_RatingGroups] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[RatingGroupStatusRecords] CHECK CONSTRAINT [FK_RatingGroupStatusRecords_RatingGroups]
GO
ALTER TABLE [dbo].[RatingPhaseGroups]  WITH CHECK ADD  CONSTRAINT [User_RatingPhaseGroup] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingPhaseGroups] CHECK CONSTRAINT [User_RatingPhaseGroup]
GO
ALTER TABLE [dbo].[RatingPhases]  WITH CHECK ADD  CONSTRAINT [RatingPhaseGroup_RatingPhase] FOREIGN KEY([RatingPhaseGroupID])
REFERENCES [dbo].[RatingPhaseGroups] ([RatingPhaseGroupID])
GO
ALTER TABLE [dbo].[RatingPhases] CHECK CONSTRAINT [RatingPhaseGroup_RatingPhase]
GO
ALTER TABLE [dbo].[RatingPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_RatingPhaseStatus_RatingGroupPhaseStatus] FOREIGN KEY([RatingGroupPhaseStatusID])
REFERENCES [dbo].[RatingGroupPhaseStatus] ([RatingGroupPhaseStatusID])
GO
ALTER TABLE [dbo].[RatingPhaseStatus] CHECK CONSTRAINT [FK_RatingPhaseStatus_RatingGroupPhaseStatus]
GO
ALTER TABLE [dbo].[RatingPhaseStatus]  WITH CHECK ADD  CONSTRAINT [FK_RatingPhaseStatus_Ratings] FOREIGN KEY([RatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[RatingPhaseStatus] CHECK CONSTRAINT [FK_RatingPhaseStatus_Ratings]
GO
ALTER TABLE [dbo].[RatingPlans]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_RatingPlan] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RatingPlans] CHECK CONSTRAINT [RatingGroupAttribute_RatingPlan]
GO
ALTER TABLE [dbo].[RatingPlans]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_RatingPlan1] FOREIGN KEY([OwnedRatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RatingPlans] CHECK CONSTRAINT [RatingGroupAttribute_RatingPlan1]
GO
ALTER TABLE [dbo].[RatingPlans]  WITH CHECK ADD  CONSTRAINT [User_RatingPlan] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RatingPlans] CHECK CONSTRAINT [User_RatingPlan]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [FK_Ratings_UserRatings] FOREIGN KEY([MostRecentUserRatingID])
REFERENCES [dbo].[UserRatings] ([UserRatingID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [FK_Ratings_UserRatings]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [RatingCharacteristic_Rating] FOREIGN KEY([RatingCharacteristicsID])
REFERENCES [dbo].[RatingCharacteristics] ([RatingCharacteristicsID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [RatingCharacteristic_Rating]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [RatingGroup_Rating] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [RatingGroup_Rating]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [RatingGroup_Rating1] FOREIGN KEY([OwnedRatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [RatingGroup_Rating1]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [RatingGroup_Rating2] FOREIGN KEY([TopmostRatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [RatingGroup_Rating2]
GO
ALTER TABLE [dbo].[Ratings]  WITH CHECK ADD  CONSTRAINT [User_Rating] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Ratings] CHECK CONSTRAINT [User_Rating]
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers]  WITH CHECK ADD  CONSTRAINT [FK_RewardPendingPointsTrackers_TblRows] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers] CHECK CONSTRAINT [FK_RewardPendingPointsTrackers_TblRows]
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers]  WITH CHECK ADD  CONSTRAINT [FK_RewardPendingPointsTrackers_TblRows1] FOREIGN KEY([RewardTblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[RewardPendingPointsTrackers] CHECK CONSTRAINT [FK_RewardPendingPointsTrackers_TblRows1]
GO
ALTER TABLE [dbo].[RewardRatingSettings]  WITH CHECK ADD  CONSTRAINT [PointsManager_RewardRatingSetting] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[RewardRatingSettings] CHECK CONSTRAINT [PointsManager_RewardRatingSetting]
GO
ALTER TABLE [dbo].[RewardRatingSettings]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_RewardRatingSetting] FOREIGN KEY([RatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[RewardRatingSettings] CHECK CONSTRAINT [RatingGroupAttribute_RewardRatingSetting]
GO
ALTER TABLE [dbo].[RewardRatingSettings]  WITH CHECK ADD  CONSTRAINT [UserAction_RewardRatingSetting] FOREIGN KEY([UserActionID])
REFERENCES [dbo].[UserActions] ([UserActionID])
GO
ALTER TABLE [dbo].[RewardRatingSettings] CHECK CONSTRAINT [UserAction_RewardRatingSetting]
GO
ALTER TABLE [dbo].[SearchWordChoices]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordChoices_ChoiceInGroups] FOREIGN KEY([ChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[SearchWordChoices] CHECK CONSTRAINT [FK_SearchWordChoices_ChoiceInGroups]
GO
ALTER TABLE [dbo].[SearchWordChoices]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordChoices_SearchWords] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordChoices] CHECK CONSTRAINT [FK_SearchWordChoices_SearchWords]
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordHierarchyItems_HierarchyItems] FOREIGN KEY([HierarchyItemID])
REFERENCES [dbo].[HierarchyItems] ([HierarchyItemID])
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems] CHECK CONSTRAINT [FK_SearchWordHierarchyItems_HierarchyItems]
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordHierarchyItems_SearchWords] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordHierarchyItems] CHECK CONSTRAINT [FK_SearchWordHierarchyItems_SearchWords]
GO
ALTER TABLE [dbo].[SearchWordTblRowNames]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordTblRowNames_SearchWords] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordTblRowNames] CHECK CONSTRAINT [FK_SearchWordTblRowNames_SearchWords]
GO
ALTER TABLE [dbo].[SearchWordTblRowNames]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordTblRowNames_TblRows] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[SearchWordTblRowNames] CHECK CONSTRAINT [FK_SearchWordTblRowNames_TblRows]
GO
ALTER TABLE [dbo].[SearchWordTextFields]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordTextFields_SearchWords] FOREIGN KEY([SearchWordID])
REFERENCES [dbo].[SearchWords] ([SearchWordID])
GO
ALTER TABLE [dbo].[SearchWordTextFields] CHECK CONSTRAINT [FK_SearchWordTextFields_SearchWords]
GO
ALTER TABLE [dbo].[SearchWordTextFields]  WITH CHECK ADD  CONSTRAINT [FK_SearchWordTextFields_TextFields] FOREIGN KEY([TextFieldID])
REFERENCES [dbo].[TextFields] ([TextFieldID])
GO
ALTER TABLE [dbo].[SearchWordTextFields] CHECK CONSTRAINT [FK_SearchWordTextFields_TextFields]
GO
ALTER TABLE [dbo].[SubsidyAdjustments]  WITH CHECK ADD  CONSTRAINT [RatingGroupPhaseStatus_SubsidyAdjustment] FOREIGN KEY([RatingGroupPhaseStatusID])
REFERENCES [dbo].[RatingGroupPhaseStatus] ([RatingGroupPhaseStatusID])
GO
ALTER TABLE [dbo].[SubsidyAdjustments] CHECK CONSTRAINT [RatingGroupPhaseStatus_SubsidyAdjustment]
GO
ALTER TABLE [dbo].[SubsidyDensityRangeGroups]  WITH CHECK ADD  CONSTRAINT [User_SubsidyDensityRangeGroup] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[SubsidyDensityRangeGroups] CHECK CONSTRAINT [User_SubsidyDensityRangeGroup]
GO
ALTER TABLE [dbo].[SubsidyDensityRanges]  WITH CHECK ADD  CONSTRAINT [SubsidyDensityRangeGroup_SubsidyDensityRange] FOREIGN KEY([SubsidyDensityRangeGroupID])
REFERENCES [dbo].[SubsidyDensityRangeGroups] ([SubsidyDensityRangeGroupID])
GO
ALTER TABLE [dbo].[SubsidyDensityRanges] CHECK CONSTRAINT [SubsidyDensityRangeGroup_SubsidyDensityRange]
GO
ALTER TABLE [dbo].[TblColumnFormatting]  WITH CHECK ADD  CONSTRAINT [FK_TblColumnFormatting_TblColumns] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[TblColumnFormatting] CHECK CONSTRAINT [FK_TblColumnFormatting_TblColumns]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [FK_TblColumns_TblColumns] FOREIGN KEY([TblColumnID])
REFERENCES [dbo].[TblColumns] ([TblColumnID])
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [FK_TblColumns_TblColumns]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [FK_TblColumns_TrustTrackerUnits] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [FK_TblColumns_TrustTrackerUnits]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [RatingGroupAttribute_TblColumn] FOREIGN KEY([DefaultRatingGroupAttributesID])
REFERENCES [dbo].[RatingGroupAttributes] ([RatingGroupAttributesID])
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [RatingGroupAttribute_TblColumn]
GO
ALTER TABLE [dbo].[TblColumns]  WITH CHECK ADD  CONSTRAINT [TblTab_TblColumn] FOREIGN KEY([TblTabID])
REFERENCES [dbo].[TblTabs] ([TblTabID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TblColumns] CHECK CONSTRAINT [TblTab_TblColumn]
GO
ALTER TABLE [dbo].[TblRows]  WITH CHECK ADD  CONSTRAINT [FK_TblRows_TblRowFieldDisplays] FOREIGN KEY([TblRowFieldDisplayID])
REFERENCES [dbo].[TblRowFieldDisplays] ([TblRowFieldDisplayID])
GO
ALTER TABLE [dbo].[TblRows] CHECK CONSTRAINT [FK_TblRows_TblRowFieldDisplays]
GO
ALTER TABLE [dbo].[TblRows]  WITH CHECK ADD  CONSTRAINT [Tbl_TblRow] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[TblRows] CHECK CONSTRAINT [Tbl_TblRow]
GO
ALTER TABLE [dbo].[TblRowStatusRecord]  WITH CHECK ADD  CONSTRAINT [FK_TblRowStatusRecord_TblRows] FOREIGN KEY([TblRowId])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[TblRowStatusRecord] CHECK CONSTRAINT [FK_TblRowStatusRecord_TblRows]
GO
ALTER TABLE [dbo].[Tbls]  WITH CHECK ADD  CONSTRAINT [PointsManager_Tbl] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[Tbls] CHECK CONSTRAINT [PointsManager_Tbl]
GO
ALTER TABLE [dbo].[Tbls]  WITH CHECK ADD  CONSTRAINT [TblDimension_Tbl] FOREIGN KEY([TblDimensionsID])
REFERENCES [dbo].[TblDimensions] ([TblDimensionsID])
GO
ALTER TABLE [dbo].[Tbls] CHECK CONSTRAINT [TblDimension_Tbl]
GO
ALTER TABLE [dbo].[Tbls]  WITH CHECK ADD  CONSTRAINT [User_Tbl] FOREIGN KEY([Creator])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Tbls] CHECK CONSTRAINT [User_Tbl]
GO
ALTER TABLE [dbo].[TblTabs]  WITH CHECK ADD  CONSTRAINT [Tbl_TblTab] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TblTabs] CHECK CONSTRAINT [Tbl_TblTab]
GO
ALTER TABLE [dbo].[TextFieldDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_TextFieldDefinitions_FieldDefinitions] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinitions] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[TextFieldDefinitions] CHECK CONSTRAINT [FK_TextFieldDefinitions_FieldDefinitions]
GO
ALTER TABLE [dbo].[TextFields]  WITH CHECK ADD  CONSTRAINT [Field_TextField] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[TextFields] CHECK CONSTRAINT [Field_TextField]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackerForChoiceInGroups_ChoiceInGroups] FOREIGN KEY([ChoiceInGroupID])
REFERENCES [dbo].[ChoiceInGroups] ([ChoiceInGroupID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] CHECK CONSTRAINT [FK_TrustTrackerForChoiceInGroups_ChoiceInGroups]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackerForChoiceInGroups_Tbls] FOREIGN KEY([TblID])
REFERENCES [dbo].[Tbls] ([TblID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] CHECK CONSTRAINT [FK_TrustTrackerForChoiceInGroups_Tbls]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackerForChoiceInGroups_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroups] CHECK CONSTRAINT [FK_TrustTrackerForChoiceInGroups_Users]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackerForChoiceInGroupsUserRatingLinks_TrustTrackerForChoiceInGroups] FOREIGN KEY([TrustTrackerForChoiceInGroupID])
REFERENCES [dbo].[TrustTrackerForChoiceInGroups] ([TrustTrackerForChoiceInGroupID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks] CHECK CONSTRAINT [FK_TrustTrackerForChoiceInGroupsUserRatingLinks_TrustTrackerForChoiceInGroups]
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackerForChoiceInGroupsUserRatingLinks_UserRatings] FOREIGN KEY([UserRatingID])
REFERENCES [dbo].[UserRatings] ([UserRatingID])
GO
ALTER TABLE [dbo].[TrustTrackerForChoiceInGroupsUserRatingLinks] CHECK CONSTRAINT [FK_TrustTrackerForChoiceInGroupsUserRatingLinks_UserRatings]
GO
ALTER TABLE [dbo].[TrustTrackers]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackers_TrustTrackerUnits] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[TrustTrackers] CHECK CONSTRAINT [FK_TrustTrackers_TrustTrackerUnits]
GO
ALTER TABLE [dbo].[TrustTrackers]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackers_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[TrustTrackers] CHECK CONSTRAINT [FK_TrustTrackers_Users]
GO
ALTER TABLE [dbo].[TrustTrackerStats]  WITH CHECK ADD  CONSTRAINT [FK_TrustTrackerStats_TrustTrackers] FOREIGN KEY([TrustTrackerID])
REFERENCES [dbo].[TrustTrackers] ([TrustTrackerID])
GO
ALTER TABLE [dbo].[TrustTrackerStats] CHECK CONSTRAINT [FK_TrustTrackerStats_TrustTrackers]
GO
ALTER TABLE [dbo].[UniquenessLockReferences]  WITH CHECK ADD  CONSTRAINT [FK_UniquenessLockReferences_ToUniquenessLocks] FOREIGN KEY([UniquenessLockID])
REFERENCES [dbo].[UniquenessLocks] ([Id])
GO
ALTER TABLE [dbo].[UniquenessLockReferences] CHECK CONSTRAINT [FK_UniquenessLockReferences_ToUniquenessLocks]
GO
ALTER TABLE [dbo].[UserCheckIns]  WITH CHECK ADD  CONSTRAINT [FK_UserCheckIns_Users1] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserCheckIns] CHECK CONSTRAINT [FK_UserCheckIns_Users1]
GO
ALTER TABLE [dbo].[UserInfo]  WITH CHECK ADD  CONSTRAINT [User_UserInfo] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserInfo] CHECK CONSTRAINT [User_UserInfo]
GO
ALTER TABLE [dbo].[UserInteractions]  WITH CHECK ADD  CONSTRAINT [FK_UserInteractions_TrustTrackerUnits] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[UserInteractions] CHECK CONSTRAINT [FK_UserInteractions_TrustTrackerUnits]
GO
ALTER TABLE [dbo].[UserInteractions]  WITH CHECK ADD  CONSTRAINT [FK_UserInteractions_Users] FOREIGN KEY([OrigRatingUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserInteractions] CHECK CONSTRAINT [FK_UserInteractions_Users]
GO
ALTER TABLE [dbo].[UserInteractions]  WITH CHECK ADD  CONSTRAINT [FK_UserInteractions_Users1] FOREIGN KEY([LatestRatingUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserInteractions] CHECK CONSTRAINT [FK_UserInteractions_Users1]
GO
ALTER TABLE [dbo].[UserInteractionStats]  WITH CHECK ADD  CONSTRAINT [FK_UserInteractionStats_TrustTrackerStats] FOREIGN KEY([TrustTrackerStatID])
REFERENCES [dbo].[TrustTrackerStats] ([TrustTrackerStatID])
GO
ALTER TABLE [dbo].[UserInteractionStats] CHECK CONSTRAINT [FK_UserInteractionStats_TrustTrackerStats]
GO
ALTER TABLE [dbo].[UserInteractionStats]  WITH CHECK ADD  CONSTRAINT [FK_UserInteractionStats_UserInteractions] FOREIGN KEY([UserInteractionID])
REFERENCES [dbo].[UserInteractions] ([UserInteractionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserInteractionStats] CHECK CONSTRAINT [FK_UserInteractionStats_UserInteractions]
GO
ALTER TABLE [dbo].[UserRatingGroups]  WITH CHECK ADD  CONSTRAINT [FK_UserRatingGroups_RatingGroupPhaseStatus] FOREIGN KEY([RatingGroupPhaseStatusID])
REFERENCES [dbo].[RatingGroupPhaseStatus] ([RatingGroupPhaseStatusID])
GO
ALTER TABLE [dbo].[UserRatingGroups] CHECK CONSTRAINT [FK_UserRatingGroups_RatingGroupPhaseStatus]
GO
ALTER TABLE [dbo].[UserRatingGroups]  WITH CHECK ADD  CONSTRAINT [RatingGroup_UserRatingGroup] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[UserRatingGroups] CHECK CONSTRAINT [RatingGroup_UserRatingGroup]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_RatingPhaseStatus1] FOREIGN KEY([RatingPhaseStatusID])
REFERENCES [dbo].[RatingPhaseStatus] ([RatingPhaseStatusID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_RatingPhaseStatus1]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_Ratings] FOREIGN KEY([RatingID])
REFERENCES [dbo].[Ratings] ([RatingID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_Ratings]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_RewardPendingPointsTrackers] FOREIGN KEY([RewardPendingPointsTrackerID])
REFERENCES [dbo].[RewardPendingPointsTrackers] ([RewardPendingPointsTrackerID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_RewardPendingPointsTrackers]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_TrustTrackerUnits] FOREIGN KEY([TrustTrackerUnitID])
REFERENCES [dbo].[TrustTrackerUnits] ([TrustTrackerUnitID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_TrustTrackerUnits]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_UserRatingGroups] FOREIGN KEY([UserRatingGroupID])
REFERENCES [dbo].[UserRatingGroups] ([UserRatingGroupID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_UserRatingGroups]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_UserRatings] FOREIGN KEY([MostRecentUserRatingID])
REFERENCES [dbo].[UserRatings] ([UserRatingID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_UserRatings]
GO
ALTER TABLE [dbo].[UserRatings]  WITH CHECK ADD  CONSTRAINT [FK_UserRatings_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserRatings] CHECK CONSTRAINT [FK_UserRatings_Users]
GO
ALTER TABLE [dbo].[UserRatingsToAdd]  WITH CHECK ADD  CONSTRAINT [FK_UserRatingsToAdd_RatingGroups] FOREIGN KEY([TopRatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[UserRatingsToAdd] CHECK CONSTRAINT [FK_UserRatingsToAdd_RatingGroups]
GO
ALTER TABLE [dbo].[UserRatingsToAdd]  WITH CHECK ADD  CONSTRAINT [FK_UserRatingsToAdd_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserRatingsToAdd] CHECK CONSTRAINT [FK_UserRatingsToAdd_Users]
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [AdministrationRightsGroup_UsersAdministrationRightsGroup] FOREIGN KEY([AdministrationRightsGroupID])
REFERENCES [dbo].[AdministrationRightsGroups] ([AdministrationRightsGroupID])
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups] CHECK CONSTRAINT [AdministrationRightsGroup_UsersAdministrationRightsGroup]
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [PointsManager_UsersAdministrationRightsGroup] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups] CHECK CONSTRAINT [PointsManager_UsersAdministrationRightsGroup]
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups]  WITH CHECK ADD  CONSTRAINT [User_UsersAdministrationRightsGroup] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UsersAdministrationRightsGroups] CHECK CONSTRAINT [User_UsersAdministrationRightsGroup]
GO
ALTER TABLE [dbo].[UsersRights]  WITH CHECK ADD  CONSTRAINT [PointsManager_UsersRight] FOREIGN KEY([PointsManagerID])
REFERENCES [dbo].[PointsManagers] ([PointsManagerID])
GO
ALTER TABLE [dbo].[UsersRights] CHECK CONSTRAINT [PointsManager_UsersRight]
GO
ALTER TABLE [dbo].[UsersRights]  WITH CHECK ADD  CONSTRAINT [User_UsersRight] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UsersRights] CHECK CONSTRAINT [User_UsersRight]
GO
ALTER TABLE [dbo].[VolatilityTblRowTrackers]  WITH CHECK ADD  CONSTRAINT [FK_VolatilityTblRowTrackers_TblRows] FOREIGN KEY([TblRowID])
REFERENCES [dbo].[TblRows] ([TblRowID])
GO
ALTER TABLE [dbo].[VolatilityTblRowTrackers] CHECK CONSTRAINT [FK_VolatilityTblRowTrackers_TblRows]
GO
ALTER TABLE [dbo].[VolatilityTrackers]  WITH CHECK ADD  CONSTRAINT [FK_VolatilityTrackers_RatingGroups] FOREIGN KEY([RatingGroupID])
REFERENCES [dbo].[RatingGroups] ([RatingGroupID])
GO
ALTER TABLE [dbo].[VolatilityTrackers] CHECK CONSTRAINT [FK_VolatilityTrackers_RatingGroups]
GO
ALTER TABLE [dbo].[VolatilityTrackers]  WITH CHECK ADD  CONSTRAINT [FK_VolatilityTrackers_VolatilityTblRowTrackers] FOREIGN KEY([VolatilityTblRowTrackerID])
REFERENCES [dbo].[VolatilityTblRowTrackers] ([VolatilityTblRowTrackerID])
GO
ALTER TABLE [dbo].[VolatilityTrackers] CHECK CONSTRAINT [FK_VolatilityTrackers_VolatilityTblRowTrackers]
GO
USE [master]
GO
ALTER DATABASE [Norm0001] SET  READ_WRITE 
GO
