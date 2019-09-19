USE [master]
GO
/****** Object:  Database [Sepes]    Script Date: 19.09.2019 12:34:36 ******/
CREATE DATABASE [Sepes]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Sepes', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\Sepes.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Sepes_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\Sepes_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Sepes] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Sepes].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Sepes] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Sepes] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Sepes] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Sepes] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Sepes] SET ARITHABORT OFF 
GO
ALTER DATABASE [Sepes] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Sepes] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Sepes] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Sepes] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Sepes] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Sepes] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Sepes] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Sepes] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Sepes] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Sepes] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Sepes] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Sepes] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Sepes] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Sepes] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Sepes] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Sepes] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Sepes] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Sepes] SET RECOVERY FULL 
GO
ALTER DATABASE [Sepes] SET  MULTI_USER 
GO
ALTER DATABASE [Sepes] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Sepes] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Sepes] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Sepes] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Sepes] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Sepes', N'ON'
GO
ALTER DATABASE [Sepes] SET QUERY_STORE = OFF
GO
USE [Sepes]
GO
/****** Object:  Table [dbo].[lnkStudy2Dataset]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lnkStudy2Dataset](
	[StudyID] [int] NOT NULL,
	[DatasetID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[lnkUser2Study]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lnkUser2Study](
	[UserID] [int] NOT NULL,
	[StudyID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDataset]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDataset](
	[DatasetID] [int] IDENTITY(1,1) NOT NULL,
	[DatasetName] [nvarchar](25) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_tblDataset] PRIMARY KEY CLUSTERED 
(
	[DatasetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPod]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPod](
	[PodID] [int] IDENTITY(1,1) NOT NULL,
	[PodName] [nvarchar](25) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_tblPod] PRIMARY KEY CLUSTERED 
(
	[PodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblStudy]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblStudy](
	[StudyID] [int] IDENTITY(1,1) NOT NULL,
	[StudyName] [nvarchar](25) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
	[PodID] [int] NULL,
 CONSTRAINT [PK_tblStudy] PRIMARY KEY CLUSTERED 
(
	[StudyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUser]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUser](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](25) NOT NULL,
	[UserEmail] [nvarchar](30) NOT NULL,
	[UserGroup] [nvarchar](25) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblUser_Email]    Script Date: 19.09.2019 12:34:36 ******/
CREATE NONCLUSTERED INDEX [IX_tblUser_Email] ON [dbo].[tblUser]
(
	[UserEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblDataset] ADD  CONSTRAINT [DF_tblDataset_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[tblDataset] ADD  CONSTRAINT [DF_tblDataset_EditDate]  DEFAULT (getdate()) FOR [EditDate]
GO
ALTER TABLE [dbo].[tblPod] ADD  CONSTRAINT [DF_tblPod_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[tblPod] ADD  CONSTRAINT [DF_tblPod_EditDate]  DEFAULT (getdate()) FOR [EditDate]
GO
ALTER TABLE [dbo].[tblStudy] ADD  CONSTRAINT [DF_tblStudy_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[tblStudy] ADD  CONSTRAINT [DF_tblStudy_EditDate]  DEFAULT (getdate()) FOR [EditDate]
GO
ALTER TABLE [dbo].[tblUser] ADD  CONSTRAINT [DF_tblUser_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[tblUser] ADD  CONSTRAINT [DF_tblUser_EditDate]  DEFAULT (getdate()) FOR [EditDate]
GO
ALTER TABLE [dbo].[lnkStudy2Dataset]  WITH CHECK ADD  CONSTRAINT [FK_lnkStudy2Dataset_tblDataset] FOREIGN KEY([DatasetID])
REFERENCES [dbo].[tblDataset] ([DatasetID])
GO
ALTER TABLE [dbo].[lnkStudy2Dataset] CHECK CONSTRAINT [FK_lnkStudy2Dataset_tblDataset]
GO
ALTER TABLE [dbo].[lnkStudy2Dataset]  WITH CHECK ADD  CONSTRAINT [FK_lnkStudy2Dataset_tblStudy] FOREIGN KEY([StudyID])
REFERENCES [dbo].[tblStudy] ([StudyID])
GO
ALTER TABLE [dbo].[lnkStudy2Dataset] CHECK CONSTRAINT [FK_lnkStudy2Dataset_tblStudy]
GO
ALTER TABLE [dbo].[lnkUser2Study]  WITH CHECK ADD  CONSTRAINT [FK_lnkUser2Study_tblStudy] FOREIGN KEY([StudyID])
REFERENCES [dbo].[tblStudy] ([StudyID])
GO
ALTER TABLE [dbo].[lnkUser2Study] CHECK CONSTRAINT [FK_lnkUser2Study_tblStudy]
GO
ALTER TABLE [dbo].[lnkUser2Study]  WITH CHECK ADD  CONSTRAINT [FK_lnkUser2Study_tblUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUser] ([UserID])
GO
ALTER TABLE [dbo].[lnkUser2Study] CHECK CONSTRAINT [FK_lnkUser2Study_tblUser]
GO
ALTER TABLE [dbo].[tblStudy]  WITH CHECK ADD  CONSTRAINT [FK_tblStudy_tblPod] FOREIGN KEY([PodID])
REFERENCES [dbo].[tblPod] ([PodID])
GO
ALTER TABLE [dbo].[tblStudy] CHECK CONSTRAINT [FK_tblStudy_tblPod]
GO
/****** Object:  Trigger [dbo].[tgr_edittime]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tgr_edittime]
ON [dbo].[tblDataset]
AFTER UPDATE AS
  UPDATE dbo.tblDataset
  SET EditDate = GETDATE()
  WHERE DatasetID IN (SELECT DISTINCT DatasetID FROM Inserted)
GO
ALTER TABLE [dbo].[tblDataset] ENABLE TRIGGER [tgr_edittime]
GO
/****** Object:  Trigger [dbo].[tgr_editPod]    Script Date: 19.09.2019 12:34:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[tgr_editPod]
ON [dbo].[tblPod]
AFTER UPDATE AS
  UPDATE dbo.tblPod
  SET EditDate = GETDATE()
  WHERE PodID IN (SELECT DISTINCT PodID FROM Inserted)
GO
ALTER TABLE [dbo].[tblPod] ENABLE TRIGGER [tgr_editPod]
GO
/****** Object:  Trigger [dbo].[tgr_editStudy]    Script Date: 19.09.2019 12:34:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[tgr_editStudy]
ON [dbo].[tblStudy]
AFTER UPDATE AS
  UPDATE dbo.tblStudy
  SET EditDate = GETDATE()
  WHERE StudyID IN (SELECT DISTINCT StudyID FROM Inserted)
GO
ALTER TABLE [dbo].[tblStudy] ENABLE TRIGGER [tgr_editStudy]
GO
/****** Object:  Trigger [dbo].[tgr_useredit]    Script Date: 19.09.2019 12:34:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[tgr_useredit]
ON [dbo].[tblUser]
AFTER UPDATE AS
  UPDATE dbo.tblUser
  SET EditDate = GETDATE()
  WHERE UserID IN (SELECT DISTINCT UserID FROM Inserted)
GO
ALTER TABLE [dbo].[tblUser] ENABLE TRIGGER [tgr_useredit]
GO
USE [master]
GO
ALTER DATABASE [Sepes] SET  READ_WRITE 
GO
