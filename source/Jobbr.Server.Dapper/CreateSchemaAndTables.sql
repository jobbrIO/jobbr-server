/* EXECUTE this manually in your FIS DB */

/****** Object:  Schema [Jobbr]    Script Date: 11.03.2015 14:17:37 ******/
CREATE SCHEMA [Jobbr]
GO
/****** Object:  Table [Jobbr].[JobRuns]    Script Date: 11.03.2015 14:17:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Jobbr].[JobRuns](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[TriggerId] [bigint] NOT NULL,
	[UniqueId] [char](36) NOT NULL,
	[JobParameters] [varchar](max) NULL,
	[InstanceParameters] [varchar](max) NULL,
	[Name] [varchar](50) NULL,
	[PlannedStartDateTimeUtc] [datetime2](7) NOT NULL,
	[ActualStartDateTimeUtc] [datetime2](7) NULL,
	[EstimatedEndDateTimeUtc] [datetime2](7) NULL,
	[ActualEndDateTimeUtc] [datetime2](7) NULL,
	[Progress] [float] NULL,
	[State] [varchar](15) NOT NULL,
	[Pid] [int] NULL,
	[WorkingDir] [varchar](300) NULL,
	[TempDir] [varchar](300) NULL,
 CONSTRAINT [PK_JobRuns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Jobbr].[Jobs]    Script Date: 11.03.2015 14:17:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Jobbr].[Jobs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Parameters] [varchar](max) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
	[UpdatedDateTimeUtc] [datetime2](7) NULL,
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Jobbr].[Triggers]    Script Date: 11.03.2015 14:17:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Jobbr].[Triggers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[TriggerType] [varchar](10) NOT NULL,
	[Definition] [varchar](20) NULL,
	[StartDateTimeUtc] [datetime2](7) NULL,
	[EndDateTimeUtc] [datetime2](7) NULL,
	[DelayedInMinutes] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[UserId] [bigint] NULL,
	[UserName] [nvarchar](50) NULL,
	[UserDisplayName] [varchar](100) NULL,
	[Parameters] [varchar](max) NULL,
	[Comment] [varchar](max) NULL,
	[CreatedDateTimeUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Triggers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
USE [master]
GO
ALTER DATABASE [JobbrDemo] SET  READ_WRITE 
GO
