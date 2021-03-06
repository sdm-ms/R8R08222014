USE [master]
GO
/****** Object:  Database [Nrml0003]    Script Date: 7/10/2014 2:45:46 PM ******/
CREATE DATABASE [Nrml0003]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Nrml0003', FILENAME = N'C:\SQLData\Nrml0003.mdf' , SIZE = 113792KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Nrml0003_log', FILENAME = N'C:\SQLData\Nrml0003_log.ldf' , SIZE = 353216KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Nrml0003] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Nrml0003].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
USE [Nrml0003]
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

USE [master]
GO
ALTER DATABASE [Nrml0003] SET  READ_WRITE 
GO
