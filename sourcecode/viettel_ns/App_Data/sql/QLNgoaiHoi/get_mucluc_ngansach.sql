DECLARE @namthuchien int 		set @namthuchien = 2022
DECLARE @sotrang int		set @sotrang = 1
DECLARE @sobanghi int		set @sobanghi = 20
--#DECLARE#--

/* L?y danh sách m?c l?c ngân sách theo tham s? truy?n vào*/

DECLARE @Start int, @End int
SET @Start = (((@sotrang - 1) * @sobanghi) + 1)
SET @End = (@Start + @sobanghi - 1)

CREATE TABLE #TemporaryTable (
  Row int IDENTITY(1,1) PRIMARY KEY,
  iID_MaMucLucNganSach uniqueidentifier,
  iID_MaMucLucNganSach_Cha uniqueidentifier,
  sXauNoiMa		nvarchar(100),
  sLNS nvarchar(50),
  sL nvarchar(50),
  sK nvarchar(50), 
  sM nvarchar(50),
  sTM nvarchar(50),
  sTTM nvarchar(50),
  sNG nvarchar(50),
  sTNG nvarchar(50),
  sMoTa nvarchar(MAX),
  sChuong nvarchar(50),
  iNamLamViec int
 )

 Insert into #TemporaryTable
 select iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha,sXauNoiMa,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,sChuong,iNamLamViec
 from NS_MucLucNganSach where iNamLamViec= @namthuchien

 select iID_MaMucLucNganSach, iID_MaMucLucNganSach_Cha,sXauNoiMa,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,sChuong,iNamLamViec
 from #TemporaryTable where (Row >= @Start) AND (Row <= @End);