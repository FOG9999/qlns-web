<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="VIETTEL.Models.TimKiemChiTieu" %>
<%@ Import Namespace="System.ComponentModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%  
        String LoadLai = Convert.ToString(ViewData["LoadLai"]);
        if (LoadLai == "1")
        {
    %>
    <script type="text/javascript">
        $(document).ready(function () {
            parent.reloadPage();

        });
    </script>
    <%
        }
        else
        {        
        String MaND = User.Identity.Name;
        String IPSua = Request.UserHostAddress;

        String DSTruong = "dDotNgay,sQD," + ChiTieuModels.strDSTruong + ",iID_MaPhongBan,sID_MaNguoiDungTao";
        String strDSTruongDoRong = ChiTieuModels.strDSTruongDoRong;
        
        String[] arrDSTruong = DSTruong.Split(',');
        String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
        Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
        
        for (int i = 0; i < arrDSTruong.Length; i++)
        {
            arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
        }

        TimKiemChiTieu_BangDuLieu bang = new TimKiemChiTieu_BangDuLieu(arrGiaTriTimKiem, MaND, IPSua);
        
        String BangID = "BangDuLieu";
        int Bang_Height = 470;
        int Bang_FixedRow_Height = 50;                
                
        String BackURL = Url.Action("Index", "Home");
    
    %>
       
        <%Html.RenderPartial("~/Views/Shared/BangDuLieu/BangDuLieu.ascx", new { BangID = BangID, bang = bang, Bang_Height = Bang_Height, Bang_FixedRow_Height = Bang_FixedRow_Height }); %>
        <div style="display: none;">
            <input type="hidden" id="idXauDoRongCot" value="<%=HttpUtility.HtmlEncode(bang.strDSDoRongCot)%>" />
            <input type="hidden" id="idXauKieuDuLieu" value="<%=HttpUtility.HtmlEncode(bang.strType)%>" />
            <input type="hidden" id="idXauChiSoCha" value="<%=HttpUtility.HtmlEncode(bang.strCSCha)%>" />
            <input type="hidden" id="idBangChiDoc" value="<%=HttpUtility.HtmlEncode(bang.strChiDoc)%>" />
            <input type="hidden" id="idXauEdit" value="<%=HttpUtility.HtmlEncode(bang.strEdit)%>" />
            <input type="hidden" id="idViewport_N" value="<%=HttpUtility.HtmlEncode(bang.Viewport_N)%>" />
            <input type="hidden" id="idNC_Fixed" value="<%=HttpUtility.HtmlEncode(bang.nC_Fixed)%>" />
            <input type="hidden" id="idNC_Slide" value="<%=HttpUtility.HtmlEncode(bang.nC_Slide)%>" />            
            <input type="hidden" id="idAction" name="idAction" value="0" />
            <input type="hidden" id="idXauDuLieuThayDoi" name="idXauDuLieuThayDoi" value="<%=HttpUtility.HtmlEncode(bang.strThayDoi)%>" />
            <input type="hidden" id="idXauLaHangCha" name="idXauLaHangCha" value="<%=HttpUtility.HtmlEncode(bang.strLaHangCha)%>" />
            <input type="hidden" id="idXauMaCacHang" name="idXauMaCacHang" value="<%=HttpUtility.HtmlEncode(bang.strDSMaHang)%>" />
            <input type="hidden" id="idXauMaCacCot" name="idXauMaCacCot" value="<%=HttpUtility.HtmlEncode(bang.strDSMaCot)%>" />
            <input type="hidden" id="idXauGiaTriChiTiet" name="idXauGiaTriChiTiet" value="<%=HttpUtility.HtmlEncode(bang.strDuLieu)%>" />
            <input type="submit" id="btnXacNhanGhi" value="XN" />
            <input type="hidden" id="idXauCacHangDaXoa" name="idXauCacHangDaXoa" value="" />            
        </div>                   
    <script src="<%= Url.Content("~/Scripts/jsBang_Editable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>         
    <script src="<%= Url.Content("~/Scripts/jsBang_KeyTable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_Data.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>    
    <script type="text/javascript">
        function Huy() {
            window.parent.location.href = '<%=BackURL %>';
        }     
        $(document).ready(function () {
            Bang_keys.fnSetFocus(0, 0);
        }); 
    </script>                         
    <%} %>      
</asp:Content>
