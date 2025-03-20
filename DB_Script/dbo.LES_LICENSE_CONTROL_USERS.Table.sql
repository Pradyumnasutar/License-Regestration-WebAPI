USE [LES_LICENSE_REGISTRY]
GO
/****** Object:  Table [dbo].[LES_LICENSE_CONTROL_USERS]    Script Date: 25-02-2025 12:55:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LES_LICENSE_CONTROL_USERS](
	[USERID] [int] IDENTITY(1,1) NOT NULL,
	[USERNAME] [nvarchar](50) NOT NULL,
	[EMAIL] [nvarchar](100) NOT NULL,
	[HASHED_PASSWORD] [nvarchar](255) NOT NULL,
	[HASH_SALT] [nvarchar](50) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
