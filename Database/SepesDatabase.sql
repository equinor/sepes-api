USE [master]
GO
/****** Object:  Database [Sepes]    Script Date: 19.09.2019 15:42:15 ******/
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
/****** Object:  Table [dbo].[tblStudy]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblStudy](
	[StudyID] [int] IDENTITY(1,1) NOT NULL,
	[StudyName] [nvarchar](25) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_tblStudy] PRIMARY KEY CLUSTERED 
(
	[StudyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[lnkUser2Study]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lnkUser2Study](
	[UserID] [int] NOT NULL,
	[StudyID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUser]    Script Date: 19.09.2019 15:42:15 ******/
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
/****** Object:  View [dbo].[UserStudies]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UserStudies]
AS
SELECT dbo.tblUser.UserName AS Name, dbo.tblUser.UserEmail AS Email, dbo.tblStudy.StudyName AS [Study Name], dbo.tblStudy.CreationDate AS [Study Creation Date]
FROM   dbo.lnkUser2Study INNER JOIN
             dbo.tblStudy ON dbo.lnkUser2Study.StudyID = dbo.tblStudy.StudyID INNER JOIN
             dbo.tblUser ON dbo.lnkUser2Study.UserID = dbo.tblUser.UserID
GO
/****** Object:  Table [dbo].[tblPod]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPod](
	[PodID] [int] IDENTITY(1,1) NOT NULL,
	[StudyID] [int] NOT NULL,
	[PodName] [nvarchar](25) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_tblPod] PRIMARY KEY CLUSTERED 
(
	[PodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UserStudyPod]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UserStudyPod]
AS
SELECT dbo.tblUser.UserName AS [User], dbo.tblUser.UserEmail AS Email, dbo.tblStudy.StudyName AS Study, dbo.tblPod.PodName AS Pod
FROM   dbo.lnkUser2Study INNER JOIN
             dbo.tblStudy ON dbo.lnkUser2Study.StudyID = dbo.tblStudy.StudyID INNER JOIN
             dbo.tblPod ON dbo.tblStudy.PodID = dbo.tblPod.PodID INNER JOIN
             dbo.tblUser ON dbo.lnkUser2Study.UserID = dbo.tblUser.UserID
GO
/****** Object:  Table [dbo].[lnkStudy2Dataset]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lnkStudy2Dataset](
	[StudyID] [int] NOT NULL,
	[DatasetID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDataset]    Script Date: 19.09.2019 15:42:15 ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblUser_Email]    Script Date: 19.09.2019 15:42:15 ******/
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
ALTER TABLE [dbo].[tblPod]  WITH CHECK ADD  CONSTRAINT [FK_tblPod_tblStudy] FOREIGN KEY([StudyID])
REFERENCES [dbo].[tblStudy] ([StudyID])
GO
ALTER TABLE [dbo].[tblPod] CHECK CONSTRAINT [FK_tblPod_tblStudy]
GO
/****** Object:  Trigger [dbo].[tgr_editDataset]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tgr_editDataset]
ON [dbo].[tblDataset]
AFTER UPDATE AS
  UPDATE dbo.tblDataset
  SET EditDate = GETDATE()
  WHERE DatasetID IN (SELECT DISTINCT DatasetID FROM Inserted)
GO
ALTER TABLE [dbo].[tblDataset] ENABLE TRIGGER [tgr_editDataset]
GO
/****** Object:  Trigger [dbo].[tgr_editPod]    Script Date: 19.09.2019 15:42:15 ******/
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
/****** Object:  Trigger [dbo].[tgr_editStudy]    Script Date: 19.09.2019 15:42:15 ******/
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
/****** Object:  Trigger [dbo].[tgr_editUser]    Script Date: 19.09.2019 15:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[tgr_editUser]
ON [dbo].[tblUser]
AFTER UPDATE AS
  UPDATE dbo.tblUser
  SET EditDate = GETDATE()
  WHERE UserID IN (SELECT DISTINCT UserID FROM Inserted)
GO
ALTER TABLE [dbo].[tblUser] ENABLE TRIGGER [tgr_editUser]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "lnkUser2Study"
            Begin Extent = 
               Top = 11
               Left = 363
               Bottom = 154
               Right = 585
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tblStudy"
            Begin Extent = 
               Top = 40
               Left = 656
               Bottom = 237
               Right = 878
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "tblUser"
            Begin Extent = 
               Top = 17
               Left = 46
               Bottom = 214
               Right = 268
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserStudies'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserStudies'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[41] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "lnkUser2Study"
            Begin Extent = 
               Top = 104
               Left = 1227
               Bottom = 247
               Right = 1449
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tblPod"
            Begin Extent = 
               Top = 40
               Left = 145
               Bottom = 237
               Right = 367
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tblStudy"
            Begin Extent = 
               Top = 13
               Left = 521
               Bottom = 210
               Right = 743
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tblUser"
            Begin Extent = 
               Top = 9
               Left = 894
               Bottom = 206
               Right = 1116
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserStudyPod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'UserStudyPod'
GO
USE [master]
GO
ALTER DATABASE [Sepes] SET  READ_WRITE 
GO
