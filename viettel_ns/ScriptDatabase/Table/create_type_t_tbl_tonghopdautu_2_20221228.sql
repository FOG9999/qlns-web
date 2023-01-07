
CREATE TYPE [dbo].[t_tbl_tonghopdautu_2] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[iID_ChungTu] [uniqueidentifier] NOT NULL,
	[iID_DuAnID] [uniqueidentifier] NULL,
	[sMaNguon] [nvarchar](100) NULL,
	[sMaNguonCha] [nvarchar](100) NULL,
	[sMaDich] [nvarchar](100) NULL,
	[fGiaTri] [float] NULL,
	[ILoaiUng] [int] NULL,
	[iStatus] [int] NULL,
	[bIsLog] [bit] NULL,
	[iId_MaNguonCha] [uniqueidentifier] NULL,
	[iThuHoiTUCheDo] [int] NULL,
	[IIDMucID] [uniqueidentifier] NULL,
	[IIDTieuMucID] [uniqueidentifier] NULL,
	[IIDTietMucID] [uniqueidentifier] NULL,
	[IIDNganhID] [uniqueidentifier] NULL,
	IIDLoaiCongTrinh [uniqueidentifier]
)
GO


