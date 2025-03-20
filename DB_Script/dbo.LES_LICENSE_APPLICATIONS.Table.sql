USE [LES_LICENSE_REGISTRY]
GO
/****** Object:  Table [dbo].[LES_LICENSE_APPLICATIONS]    Script Date: 25-02-2025 12:55:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LES_LICENSE_APPLICATIONS](
	[LICENSEAPPLICATIONID] [int] IDENTITY(1,1) NOT NULL,
	[LICENSEID] [int] NOT NULL,
	[APPLICATION_NAME] [nvarchar](50) NULL,
	[APPLICATION_VERSION] [nvarchar](50) NULL,
	[LAST_ACCESSED_DATE] [datetime] NULL,
	[LAST_ACCESSED_IP] [nvarchar](50) NULL,
 CONSTRAINT [PK_LES_LICENSE_APPLICATIONS] PRIMARY KEY CLUSTERED 
(
	[LICENSEAPPLICATIONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LES_LICENSE_APPLICATIONS]  WITH CHECK ADD  CONSTRAINT [FK_LES_LICENSE_APPLICATIONS_LES_LICENSE_REGISTRY] FOREIGN KEY([LICENSEID])
REFERENCES [dbo].[LES_LICENSE_REGISTRY] ([LICENSEID])
GO
ALTER TABLE [dbo].[LES_LICENSE_APPLICATIONS] CHECK CONSTRAINT [FK_LES_LICENSE_APPLICATIONS_LES_LICENSE_REGISTRY]
GO
