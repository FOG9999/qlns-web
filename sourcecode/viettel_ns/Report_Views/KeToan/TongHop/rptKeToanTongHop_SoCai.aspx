﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.KeToan" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        div.login1
        {
            text-align: center;
            background: transparent url(/Content/Report_Image/login.gif) no-repeat top center;
        }
        div.login1 a
        {
            color: white;
            text-decoration: none;
            font: bold 16px "Museo 700";
            display: block;
            width: 50px;
            height: 20px;
            line-height: 20px;
            margin: 0px auto;
            background: transparent url(/Content/Report_Image/arrow.png) no-repeat 20px -29px;
            -webkit-border-radius: 2px;
            border-radius: 2px;
        }
        div.login1 a.active
        {
            background-position: 20px 1px;
        }
        div.login1 a:active, a:focus
        {
            outline: none;
        }
        ul.inlineBlock{
	        list-style: none inside;			
        }
        ul.inlineBlock li{			
	        /*-webkit-box-shadow: 2px 2px 0 #cecece;
	        box-shadow: 2px 2px 0 #cecece;	*/	
	        -webkit-box-shadow: rgba(200, 200, 200, 0.7) 0 4px 10px -1px;
            box-shadow: rgba(200, 200, 200, 0.7) 0 4px 10px -1px;
	        padding: 2px 5px;
	        display: inline-block;
	        vertical-align: middle; /*Mở comment để xem thuộc tính vertical-align*/
	        margin-right: 3px;
	        margin-left: 0px;
	        font-size: 13px;			
	        border-radius: 3px;
	        position: relative;
	    /*fix for IE 7*/
	        zoom:1;
	        *display: inline;		        
        }
        ul.inlineBlock li span
        {
            padding:2px 1px;   
        }
        ul.inlineBlock li p{
            padding:1px;    
        }
        ul.inlineBlock li fieldset{border:1px solid #cecece;border-radius:3px;-moz-border-radius:3px;-webkit-border-radius:3px;}
        ul.inlineBlock li fieldset span{
            padding:2px 3px 2px 1px;  
            margin-left:2px;  
        }
        select{padding:2px; border:1px solid #dedede;}
        p.liFirst{text-align:right;}
        span.pFirst{float:left;}
        ul.inlineBlock li fieldset legend{text-align:left; padding:3px 5px;}
    </style>
    <script type="text/javascript" language="javascript" src="../../../Scripts/jquery-latest.js"></script>
</head>
<body>
    <% 
        String ParentID = "KeToan";
        String PageLoad = "";
        DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(User.Identity.Name);
        PageLoad = Convert.ToString(ViewData["PageLoad"]);
        String NamLamViec = Convert.ToString(ViewData["NamLamViec"]);
        if (String.IsNullOrEmpty(NamLamViec))
        {
            NamLamViec = dtCauHinh.Rows[0]["iNamLamViec"].ToString();
        }
        DateTime dNgayHienTai = DateTime.Now;
        String NamHienTai = Convert.ToString(dNgayHienTai.Year);
        DataTable dtNam = DanhMucModels.DT_Nam();
        SelectOptionList slNam = new SelectOptionList(dtNam, "MaNam", "TenNam");
        dtNam.Dispose();

        String ThangLamViec = Convert.ToString(ViewData["ThangLamViec"]);
        if (String.IsNullOrEmpty(ThangLamViec))
        {
            ThangLamViec = dtCauHinh.Rows[0]["iThangLamViec"].ToString();
        }
        dtCauHinh.Dispose();
        DataTable dtThang = DanhMucModels.DT_Thang_CoThangKhong();
        SelectOptionList slThang = new SelectOptionList(dtThang, "MaThang", "TenThang");
        dtThang.Dispose();
        String iID_MaPhuongAn = Convert.ToString(ViewData["iID_MaPhuongAn"]);
        DataTable dtPhuongAn = KeToan_DanhMucThamSoModels.Get_dtDanhSachThamSoCuaBaoCao("rptKeToanTongHop_SoCaiController", NamLamViec);
        SelectOptionList slPhuongAn = new SelectOptionList(dtPhuongAn, "sThamSo", "sThamSo");
        if (String.IsNullOrEmpty(iID_MaPhuongAn))
        {
            if (dtPhuongAn.Rows.Count > 0)
            {
                iID_MaPhuongAn = dtPhuongAn.Rows[0]["sThamSo"].ToString();
            }
            else
            {
                iID_MaPhuongAn = Guid.Empty.ToString();
            }

        }
        dtPhuongAn.Dispose();
        String KhoGiay = Convert.ToString(ViewData["KhoGiay"]);
        DataTable dtKhoGiay = ReportModels.LoaiKhoGiay();
        SelectOptionList slKhoGiay = new SelectOptionList(dtKhoGiay, "MaKhoGiay", "TenKhoGiay");
        if (String.IsNullOrEmpty(KhoGiay))
        {
            KhoGiay = "1";
        }
        dtKhoGiay.Dispose();
        String iID_MaTaiKhoan = Convert.ToString(ViewData["iID_MaTaiKhoan"]);
        DataTable dtTaiKhoan = rptKeToanTongHop_SoCaiController.KeToan_ToIn(iID_MaPhuongAn, KhoGiay);
        SelectOptionList slTaiKhoan = new SelectOptionList(dtTaiKhoan, "MaTo", "TenTo");
        if (String.IsNullOrEmpty(iID_MaTaiKhoan))
        {
            if (dtTaiKhoan.Rows.Count > 0)
            {
                iID_MaTaiKhoan = dtTaiKhoan.Rows[0]["MaTo"].ToString();

            }
            else
            {
                iID_MaTaiKhoan = Guid.Empty.ToString();
            }

        }
        dtTaiKhoan.Dispose();
        DataTable dtTrangThai = HamChung.GetTrangThai(PhanHeModels.iID_MaPhanHeKeToanTongHop, LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeKeToanTongHop), true, "--Tất cả--");
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
        String iTrangThai = Convert.ToString(Request.QueryString["iID_MaTrangThaiDuyet"]);
        if (String.IsNullOrEmpty(iTrangThai))
            iTrangThai = dtTrangThai.Rows.Count > 0 ? Convert.ToString(dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"]) : Guid.Empty.ToString(); 
        String urlReport = "";
        if (PageLoad == "1")
            urlReport = Url.Action("ViewPDF", "rptKeToanTongHop_SoCai", new { NamLamViec = NamLamViec, ThangLamViec = ThangLamViec, iID_MaPhuongAn = iID_MaPhuongAn, iID_MaTaiKhoan = iID_MaTaiKhoan, KhoGiay = KhoGiay });
        String URL = Url.Action("Index", "KeToan_Report");
        using (Html.BeginForm("EditSubmit", "rptKeToanTongHop_SoCai", new { ParentID = ParentID }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td width="47.9%">
                       <span>Báo cáo sổ cái năm <%=NamLamViec %></span>
                    </td>
                    <td width="52%" style="text-align: left;">
                        <div class="login1" style="width: 50px; height: 20px; text-align: left;">
                            <a style="cursor: pointer;"></a>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="table_form2" class="table_form2">
            <div id="rptMain" style="width: 100%; margin: 0px auto; padding: 0px 0px; overflow: visible; text-align:center;">                
                <%=MyHtmlHelper.Hidden(ParentID, NamLamViec, "NamLamViec", "")%>
                <ul class="inlineBlock">
                    <li>
                        <p class="liFirst"><span class="pFirst"><%=NgonNgu.LayXau("Tháng") %></span><%=MyHtmlHelper.DropDownList(ParentID, slThang, ThangLamViec, "ThangLamViec", "", "class=\"input1_2\" style=\"width: 70px;\" onchange=\"CheckKhoaSo(this.value)\"")%></p>
                        <p class="liFirst"><span class="pFirst"><%=NgonNgu.LayXau("Khổ giấy:")%></span><%=MyHtmlHelper.DropDownList(ParentID, slKhoGiay, KhoGiay, "KhoGiay", "", "class=\"input1_2\" style=\"width: 120px;\" onChange=ChonKieuIn()")%></p>
                        <p class="liFirst"><span class="pFirst"></span></p>
                    </li>
                    <li>
                        <fieldset>
                            <legend><%=NgonNgu.LayXau("Phương án") %></legend>
                            <p>
                                <%=MyHtmlHelper.DropDownList(ParentID, slPhuongAn, iID_MaPhuongAn, "iID_MaPhuongAn", "", "class=\"input1_2\" style=\"width: 610px;\" size='3' tab-index='-1' onChange=ChonKieuIn()")%>
                            </p>
                        </fieldset>
                    </li>
                    <li>
                        <fieldset>
                            <legend><%=NgonNgu.LayXau("Chọn tờ")%></legend>
                            <p id="<%=ParentID%>_tdNoiDung">
                                <% rptKeToanTongHop_SoCaiController rpt1 = new rptKeToanTongHop_SoCaiController();%>
                                <%=rpt1.obj_data(ParentID,iID_MaPhuongAn,iID_MaTaiKhoan,KhoGiay) %>
                            </p>
                        </fieldset>    
                    </li>                   
                </ul><!--End .inlineBlock-->
                <p style="text-align:center; padding:4px;"><input type="submit" class="button" id="Submit2" value="<%=NgonNgu.LayXau("Thực hiện")%>" style="display:inline-block; margin-right:5px;" /><input class="button" type="button" value="<%=NgonNgu.LayXau("Hủy")%>"  onclick="Huy()" style="display:inline-block; margin-left:5px;" /></p>                    
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {
            $('div.login1 a').click(function () {
                $('div#rptMain').slideToggle('normal');
                $(this).toggleClass('active');
                return false;
            });
        });       
    </script>
    <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptKeToanTongHop_SoCai", new { NamLamViec = NamLamViec, ThangLamViec = ThangLamViec, iID_MaPhuongAn = iID_MaPhuongAn, iID_MaTaiKhoan = iID_MaTaiKhoan, KhoGiay = KhoGiay,ToSo="1" }), "Xuất ra Excel")%>
    <script type="text/javascript">
        function ChonKieuIn() {
            var iID_MaPhuongAn = document.getElementById("<%=ParentID %>_iID_MaPhuongAn").value;
            var KhoGiay = document.getElementById("<%=ParentID %>_KhoGiay").value;
            jQuery.ajaxSetup({ cache: false });
            var url = unescape('<%= Url.Action("ds_NhomDonVi?ParentID=#0&iID_MaPhuongAn=#1&iID_MaTaiKhoan=#2&KhoGiay=#3", "rptKeToanTongHop_SoCai") %>');
            url = unescape(url.replace("#0", "<%= ParentID %>"));
            url = unescape(url.replace("#1", iID_MaPhuongAn));
            url = unescape(url.replace("#2", "<%= iID_MaTaiKhoan %>"));
            url = unescape(url.replace("#3", KhoGiay));
            $.getJSON(url, function (data) {
                document.getElementById("<%= ParentID %>_tdNoiDung").innerHTML = data;
            });
        }
        CheckKhoaSo('<%=ThangLamViec %>');
        function CheckKhoaSo(value) {
            jQuery.ajaxSetup({ cache: false });
            var url = unescape('<%= Url.Action("CheckKhoaSo?iThang=#0&iNam=#1", "rptKeToanTongHop_SoCai") %>');
            url = unescape(url.replace("#0", value));
            url = unescape(url.replace("#1", '<%=NamLamViec %>'));
            $.getJSON(url, function (sThangChuaKhoaSo) {
                if (sThangChuaKhoaSo != "") {
                    alert('Tháng ' + sThangChuaKhoaSo + ' chưa được khóa sổ');
                }
            });
        }                                           
    </script>
    <script type="text/javascript">
        function Huy() {
            window.location.href = '<%=URL %>';
        }
    </script>
    <%} %>
    <iframe src="<%=urlReport%>" height="600px" width="100%"></iframe>
</body>
</html>
