DECLARE @loaitiennhap int		set @loaitiennhap = 1 /* 1 - Chuy?n ??i t? USD -> VND, 2: Chuy?n ??i t? VND -> USD*/
DECLARE @sotiennhap float		set @sotiennhap = 1000
DECLARE @matygia uniqueidentifier		set @matygia = '00000000-0000-0000-0000-000000000000'
--#DECLARE#--

/* L?y danh s?ch m?c l?c ng?n s?ch theo tham s? truy?n v?o*/

declare @matientegoc nvarchar(20);
declare @tygia float;
declare @tienchuyendoi float;

set @matientegoc = (select sMaTienTeGoc from  NH_DM_TiGia as tg where ID = @matygia);

/*N?u lo?i ti?n nh?p l? USD th? chuy?n sang VND v? ng??c l?i*/

	if @loaitiennhap = 1
			begin
				if UPPER(@matientegoc) = 'USD'
					begin
						set @tygia = (select fTiGia from NH_DM_TiGia as tg
											left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
											where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='VND');
						if @tygia IS NOT NULL
							begin
								set @tienchuyendoi = @sotiennhap * @tygia;
							end
					end
				else 
					begin
						if UPPER(@matientegoc) = 'VND'
							begin
								set @tygia = (select fTiGia from NH_DM_TiGia as tg
												left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
												where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='USD');
								if @tygia IS NOT NULL
									begin
										set @tienchuyendoi = @sotiennhap / @tygia;
									end
							end
						else
							begin
								--Chuy?n ??i t? USD -> t? gi? kh?c
								set @tygia = (select fTiGia from NH_DM_TiGia as tg
												left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
												where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='USD')
								if @tygia IS NOT NULL
									begin
										set @tienchuyendoi = @sotiennhap / @tygia;
									end

								--Chuy?n ??i t? t? gi? kh?c sang VND
								set @tygia = (select fTiGia from NH_DM_TiGia as tg
												left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
												where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='VND')
								if @tygia IS NOT NULL
									begin
										set @tienchuyendoi =  @tienchuyendoi * @tygia;
									end
							end
					end;
			end;

	else
		  
		  begin
			if UPPER(@matientegoc) = 'USD'
				begin
					set @tygia = (select fTiGia from NH_DM_TiGia as tg
										left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
										where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='VND');
					if @tygia IS NOT NULL
						begin
							set @tienchuyendoi = @sotiennhap / @tygia;
						end
				end
			else 
				begin
					if UPPER(@matientegoc) = 'VND'
						begin
							set @tygia = (select fTiGia from NH_DM_TiGia as tg
											left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
											where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='USD')
							if @tygia IS NOT NULL
								begin
									set @tienchuyendoi =  @sotiennhap * @tygia;
								end
						end
						
					else
						begin
							--Chuy?n ??i t? VND -> t? gi? kh?c
							set @tygia = (select fTiGia from NH_DM_TiGia as tg
											left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
											where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='VND')
							if @tygia IS NOT NULL
								begin
									set @tienchuyendoi = @sotiennhap / @tygia;
								end

							--Chuy?n ??i t? t? gi? kh?c sang USD
							set @tygia = (select fTiGia from NH_DM_TiGia as tg
											left join NH_DM_TiGia_ChiTiet as ct on tg.ID = ct.iID_TiGiaID
											where tg.ID = @matygia and UPPER(sMaTienTeQuyDoi) ='USD')
							if @tygia IS NOT NULL
								begin
									set @tienchuyendoi =  @tienchuyendoi * @tygia;
								end
						end
				end;
		end;

select @tienchuyendoi;