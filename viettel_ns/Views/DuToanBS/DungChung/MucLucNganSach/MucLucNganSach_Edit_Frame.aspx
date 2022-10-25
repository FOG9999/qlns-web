<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
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
        String iID_MaMucLucNganSach = Request.QueryString["iID_MaMucLucNganSach"];        
        //Cập nhập các thông tin tìm kiếm        
        String DSTruong = MucLucNganSachModels.strDSTruong;
        String strDSTruongDoRong = MucLucNganSachModels.strDSTruongDoRong;
        //MucLucNganSachModels.CapNhapLai();
        String[] arrDSTruong = DSTruong.Split(',');
        String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
        Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
        String sLNS = Request.QueryString["sLNS"];
        if (String.IsNullOrEmpty(sLNS)) sLNS = "0";
        for (int i = 0; i < arrDSTruong.Length; i++)
        {
            arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
        }        
        MucLucNganSach_BangDuLieu bang = new MucLucNganSach_BangDuLieu(arrGiaTriTimKiem, MaND, IPSua);
        
        String BangID = "BangDuLieu";
        int Bang_Height = 470;
        int Bang_FixedRow_Height = 50;
        String LuuThanhCong = Convert.ToString(Request.QueryString["LuuThanhCong"]);        
        
        String DetailSubmit = Url.Action("DetailSubmit", "MucLucNganSach", new { sLNS = sLNS });
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
            <%  
                if (bang.ChiDoc == false)
                {
            %>
            <form action="<%=DetailSubmit%>" method="post">
            <%
                } %>
            <input type="hidden" id="idAction" name="idAction" value="0" />
            <input type="hidden" id="idXauDuLieuThayDoi" name="idXauDuLieuThayDoi" value="<%=HttpUtility.HtmlEncode(bang.strThayDoi)%>" />
            <input type="hidden" id="idXauLaHangCha" name="idXauLaHangCha" value="<%=HttpUtility.HtmlEncode(bang.strLaHangCha)%>" />
            <input type="hidden" id="idXauMaCacHang" name="idXauMaCacHang" value="<%=HttpUtility.HtmlEncode(bang.strDSMaHang)%>" />
            <input type="hidden" id="idXauMaCacCot" name="idXauMaCacCot" value="<%=HttpUtility.HtmlEncode(bang.strDSMaCot)%>" />
            <input type="hidden" id="idXauGiaTriChiTiet" name="idXauGiaTriChiTiet" value="<%=HttpUtility.HtmlEncode(bang.strDuLieu)%>" />
            <input type="submit" id="btnXacNhanGhi" value="XN" />
            <input type="hidden" id="idXauCacHangDaXoa" name="idXauCacHangDaXoa" value="" />
            <%
                if (bang.ChiDoc == false)
                {
            %>
            </form>
            <%
                }
            %>
        </div> 
        <%
            if (bang.ChiDoc == false)
            {
        %>
            <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td align="right" style="padding-right:10px;width:6%;">
                        <input type="button" id="btnLuu" class="button" onclick="javascript:return Bang_HamTruocKhiKetThuc();"
                            value="<%=NgonNgu.LayXau("Thực hiện")%>" />
                    </td>
                    <td style="width:5%">
                        &nbsp;
                    </td>
                    <td align="right" style="padding-right:10px;width:6%"> 
                        <input class="button" type="button" value="<%=NgonNgu.LayXau("Quay lại")%>" onclick="Huy()" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>             
            </table>
        <%
            }
        %>           
    <script src="<%= Url.Content("~/Scripts/jsBang_MucLucNganSach.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>  
    <script src="<%= Url.Content("~/Scripts/jsBang_Editable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>         
    <script src="<%= Url.Content("~/Scripts/jsBang_KeyTable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_Data.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script type="text/javascript">            
        luuthanhcong();
        function luuthanhcong(){
            var luuThanhCong=<%=LuuThanhCong%>
            if(luuThanhCong==1)
            alert("Lưu thành công");
        }            
    </script> 
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
