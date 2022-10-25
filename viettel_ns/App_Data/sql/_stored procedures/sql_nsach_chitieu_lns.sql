/*
	
	author:	hieppm
	date:	21/07/2018
	decs:	Lấy chỉ tiêu ngân sách theo lns
	params:	lns	 

*/

ALTER PROCEDURE [dbo].[sql_nsach_chitieu_lns]

	(
		@iNamLamViec int,
		@iID_MaPhongBan nvarchar(2),		
		@sLNS nvarchar(7)
	)

AS

	DECLARE @iID_MaDonVi nvarchar(MAX)
	SET @iID_MaDonVi = [dbo].f_ns_getdsdvi(@iNamLamViec,@iID_MaPhongBan)

	CREATE TABLE #TempChiTieu
	(
		maDonVi char(2), lns char(7), l char(3), k char(3), 
		m char(4), tm char(4), ttm char(2), ng char(2), 
		CT_DauNam [Decimal] (18,0), CT_BoSung [Decimal] (18,0), QT_rTuChi [Decimal] (18,0)
	)

	INSERT INTO #TempChiTieu(maDonVi,lns,l,k,m,tm,ttm,ng,CT_DauNam,CT_BoSung,QT_rTuChi)

	SELECT		iID_MaDonVi
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, SUM(CT_DauNam)
				, SUM(CT_BoSung)
				, SUM(QT_rTuChi)
	FROM		[dbo].f_ns_chitieu_lns(@iNamLamViec,@iID_MaPhongBan,@iID_MaDonVi)
	WHERE		sLNS = @sLNS
	GROUP BY	sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, iID_MaDonVi
	HAVING		SUM(CT_DauNam) <> 0
				or SUM(CT_BoSung) <> 0
				or SUM(QT_rTuChi) <> 0
					
	--SELECT		m
	--			, tm
	--			, ttm
	--			, ng
	--			, SUM(CT_DauNam)
	--			, SUM(CT_BoSung)
	--			, SUM(QT_rTuChi) 
	--FROM		#TempChiTieu
	--GROUP BY	m
	--			, tm
	--			, ttm
	--			, ng
	--ORDER BY	m
	--			, tm
	--			, ttm
	--			, ng

	--SELECT		iID_MaDonVi
	--			, SUM(CT_DauNam)
	--			, SUM(CT_BoSung)
	--			, SUM(QT_rTuChi) 
	--FROM		#TempChiTieu
	--GROUP BY	iID_MaDonVi
	--ORDER BY	iID_MaDonVi

	CREATE TABLE #TempDonVi
	(
		maDonVi char(2), tenDonVi nvarchar(MAX)
	)

	INSERT INTO #TempDonVi(maDonVi,tenDonVi)

	SELECT DISTINCT maDonVi
					, [dbo].F_GetTenDonVi(@iNamLamViec,maDonVi)
	FROM			#TempChiTieu	

	CREATE TABLE #TempChiTiet
	(
		lns char(7), l char(3), k char(3),
		m char(4), tm char(4), ttm char(2), ng char(2), ten nvarchar(MAX)
	)

	INSERT INTO		#TempChiTiet(ng,lns,l,k,m,tm,ttm)
	SELECT DISTINCT ng,lns,l,k,m,tm,ttm
	FROM			#TempChiTieu

	UPDATE	#TempChiTiet 
	SET		ten = B.sMoTa 
	FROM	#TempChiTiet AS A 
			INNER JOIN 
			NS_MucLucNganSach AS B ON
				A.lns = B.sLNS AND A.l=B.sL AND A.k=B.sK AND A.m=B.sM  AND A.tm =B.sTM AND A.ttm=B.sTTM And A.ng = B.sNG 
				AND B.sTNG = '' AND B.iNamLamViec = @iNamLamViec
	
	Create Table #TempTM
	(		
		lns char(7), l char(3), k char(3),
		m Char(4), tm Char(4), ten nvarchar(MAX)
	)

	Insert Into #TempTM(tm,lns,l,k,m)
	Select Distinct	tm,lns,l,k,m  
	From			#TempChiTiet 
		
	Update	#TempTM 
	Set		ten = B.sMoTa 
	From	#TempTM As A 
			Inner Join 
			NS_MucLucNganSach AS B ON
				A.lns = B.sLNS And A.l = B.sL And A.k = B.sK And A.m = B.sM And A.tm = B.sTM And B.sTTM = ''

	Create Table #TempM
	( 	
		lns char(7), l char(3), k char(3),	
		m Char(4), ten nvarchar(MAX)
	)

	Insert Into #TempM(m,lns,l,k)
	Select Distinct m,lns,l,k
	From			#TempChiTiet 
	
	Update	#TempM 
	Set		ten = B.sMoTa 
	From	#TempM As A 
			Inner Join 
			NS_MucLucNganSach AS B ON
				A.lns = B.sLNS And A.l = B.sL And A.k = B.sK And A.m = B.sM And B.sTM = ''

	Select		m
				, ten 
	From		#TempM 
	Order by	m

	Select		m
				, tm
				, ten 
	From		#TempTM 
	Order by	lns,l,k,m,tm
	
	SELECT		m
				, tm
				, ttm
				, ng
				, ten
	FROM		#TempChiTiet 
	Order by	m,tm,ttm,ng	

	SELECT		maDonVi
				, tenDonVi
	FROM		#TempDonVi 
	Order by	maDonVi

	SELECT		maDonVi
				, [dbo].F_GetTenDonVi(@iNamLamViec,maDonVi) as tenDonVi
				, m
				, tm
				, ttm
				, ng
				, CT_DauNam
				, CT_BoSung
				, QT_rTuChi
	FROM		#TempChiTieu 
	ORDER BY	maDonVi
				, m
				, tm
				, ttm
				, ng

RETURN