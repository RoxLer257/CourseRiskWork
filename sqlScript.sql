USE [RiskBD]
GO
/****** Object:  Table [dbo].[MitigationStrategies]    Script Date: 31.01.2025 20:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MitigationStrategies](
	[StrategyID] [int] IDENTITY(1,1) NOT NULL,
	[RiskID] [int] NOT NULL,
	[StrategyName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[ImplementationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[StrategyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RiskAssessments]    Script Date: 31.01.2025 20:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RiskAssessments](
	[AssessmentID] [int] IDENTITY(1,1) NOT NULL,
	[RiskID] [int] NOT NULL,
	[AssessmentDate] [date] NOT NULL,
	[LevelID] [int] NOT NULL,
 CONSTRAINT [PK__RiskAsse__3D2BF83E3183B00C] PRIMARY KEY CLUSTERED 
(
	[AssessmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RiskCategories]    Script Date: 31.01.2025 20:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RiskCategories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RiskLevels]    Script Date: 31.01.2025 20:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RiskLevels](
	[LevelID] [int] IDENTITY(1,1) NOT NULL,
	[LevelName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Risks]    Script Date: 31.01.2025 20:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Risks](
	[RiskID] [int] IDENTITY(1,1) NOT NULL,
	[RiskName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Probability] [float] NULL,
	[Impact] [float] NULL,
 CONSTRAINT [PK__Risks__435363D6B77BA995] PRIMARY KEY CLUSTERED 
(
	[RiskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 31.01.2025 20:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[MitigationStrategies] ON 

INSERT [dbo].[MitigationStrategies] ([StrategyID], [RiskID], [StrategyName], [Description], [ImplementationDate]) VALUES (5, 1, N'Диверсификация доходов', N'Расширение источников доходов', CAST(N'2025-02-01T00:00:00.000' AS DateTime))
INSERT [dbo].[MitigationStrategies] ([StrategyID], [RiskID], [StrategyName], [Description], [ImplementationDate]) VALUES (6, 2, N'Резервное оборудование', N'Установка резервного оборудования для минимизации простоев', CAST(N'2025-03-15T00:00:00.000' AS DateTime))
INSERT [dbo].[MitigationStrategies] ([StrategyID], [RiskID], [StrategyName], [Description], [ImplementationDate]) VALUES (7, 3, N'Юридическая проверка', N'Регулярная проверка договоров и документов', CAST(N'2025-04-10T00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[MitigationStrategies] OFF
GO
SET IDENTITY_INSERT [dbo].[RiskAssessments] ON 

INSERT [dbo].[RiskAssessments] ([AssessmentID], [RiskID], [AssessmentDate], [LevelID]) VALUES (3, 1, CAST(N'2025-01-20' AS Date), 3)
INSERT [dbo].[RiskAssessments] ([AssessmentID], [RiskID], [AssessmentDate], [LevelID]) VALUES (4, 2, CAST(N'2025-01-21' AS Date), 2)
INSERT [dbo].[RiskAssessments] ([AssessmentID], [RiskID], [AssessmentDate], [LevelID]) VALUES (5, 3, CAST(N'2025-01-22' AS Date), 1)
SET IDENTITY_INSERT [dbo].[RiskAssessments] OFF
GO
SET IDENTITY_INSERT [dbo].[RiskCategories] ON 

INSERT [dbo].[RiskCategories] ([CategoryID], [CategoryName], [Description]) VALUES (1, N'Финансовые риски', N'Риски, связанные с финансовыми потерями')
INSERT [dbo].[RiskCategories] ([CategoryID], [CategoryName], [Description]) VALUES (2, N'Операционные риски', N'Риски, связанные с операционной деятельностью')
INSERT [dbo].[RiskCategories] ([CategoryID], [CategoryName], [Description]) VALUES (3, N'Юридические риски', N'Риски, связанные с законодательством и правовыми вопросами')
SET IDENTITY_INSERT [dbo].[RiskCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[RiskLevels] ON 

INSERT [dbo].[RiskLevels] ([LevelID], [LevelName]) VALUES (3, N'Высокий')
INSERT [dbo].[RiskLevels] ([LevelID], [LevelName]) VALUES (1, N'Низкий')
INSERT [dbo].[RiskLevels] ([LevelID], [LevelName]) VALUES (2, N'Средний')
SET IDENTITY_INSERT [dbo].[RiskLevels] OFF
GO
SET IDENTITY_INSERT [dbo].[Risks] ON 

INSERT [dbo].[Risks] ([RiskID], [RiskName], [Description], [CategoryID], [Probability], [Impact]) VALUES (1, N'Потеря доходов', N'Снижение доходов из-за экономического спада', 2, 0.7, 0.8)
INSERT [dbo].[Risks] ([RiskID], [RiskName], [Description], [CategoryID], [Probability], [Impact]) VALUES (2, N'Сбой в производстве', N'Остановка производства из-за технических неполадок', 3, 0.5, 0.9)
INSERT [dbo].[Risks] ([RiskID], [RiskName], [Description], [CategoryID], [Probability], [Impact]) VALUES (3, N'Судебные разбирательства', N'Риск судебных исков со стороны клиентов', 1, 0.3, 0.6)
INSERT [dbo].[Risks] ([RiskID], [RiskName], [Description], [CategoryID], [Probability], [Impact]) VALUES (28, N'test', N'tsets', 3, 0.7, 0.9)
INSERT [dbo].[Risks] ([RiskID], [RiskName], [Description], [CategoryID], [Probability], [Impact]) VALUES (29, N'erar', N'setsetset', 3, 1, 0.9)
SET IDENTITY_INSERT [dbo].[Risks] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Role]) VALUES (1, N'admin', N'admin1', N'Администратор')
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Role]) VALUES (2, N'manager', N'manager1', N'Менеджер')
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Role]) VALUES (3, N'analyst', N'analyst1', N'Аналитик')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__RiskLeve__9EF3BE7B89F430C6]    Script Date: 31.01.2025 20:45:12 ******/
ALTER TABLE [dbo].[RiskLevels] ADD UNIQUE NONCLUSTERED 
(
	[LevelName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MitigationStrategies]  WITH CHECK ADD  CONSTRAINT [FK__Mitigatio__RiskI__52593CB8] FOREIGN KEY([RiskID])
REFERENCES [dbo].[Risks] ([RiskID])
GO
ALTER TABLE [dbo].[MitigationStrategies] CHECK CONSTRAINT [FK__Mitigatio__RiskI__52593CB8]
GO
ALTER TABLE [dbo].[RiskAssessments]  WITH CHECK ADD  CONSTRAINT [FK__RiskAsses__Level__571DF1D5] FOREIGN KEY([LevelID])
REFERENCES [dbo].[RiskLevels] ([LevelID])
GO
ALTER TABLE [dbo].[RiskAssessments] CHECK CONSTRAINT [FK__RiskAsses__Level__571DF1D5]
GO
ALTER TABLE [dbo].[RiskAssessments]  WITH CHECK ADD  CONSTRAINT [FK__RiskAsses__RiskI__5535A963] FOREIGN KEY([RiskID])
REFERENCES [dbo].[Risks] ([RiskID])
GO
ALTER TABLE [dbo].[RiskAssessments] CHECK CONSTRAINT [FK__RiskAsses__RiskI__5535A963]
GO
ALTER TABLE [dbo].[Risks]  WITH CHECK ADD  CONSTRAINT [FK__Risks__CategoryI__4AB81AF0] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[RiskCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[Risks] CHECK CONSTRAINT [FK__Risks__CategoryI__4AB81AF0]
GO
