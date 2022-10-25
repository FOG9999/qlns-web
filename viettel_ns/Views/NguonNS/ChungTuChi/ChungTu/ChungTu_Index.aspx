<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="Viettel.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int i;
        string MaND = User.Identity.Name;
        string ParentID = "NguonNS_ChungTuChi";
        string page = Request.QueryString["page"];
        string Nam = SharedService.Default.LayNamLamViec(User.Identity.Name);
        
        int CurrentPage = 1;

        Boolean bThemMoi = false;
        string iThemMoi = "";
        if (ViewData["bThemMoi"] != null)
        {
            bThemMoi = Convert.ToBoolean(ViewData["bThemMoi"]);
            if (bThemMoi)
                iThemMoi = "on";
        }           

        INguonNSService _nguonNSService = NguonNSService.Default;

        DataTable dtLoaiChungTu = _nguonNSService.GetLoaiChungTu("chi");       
        SelectOptionList slLoaiChungTu = new SelectOptionList(dtLoaiChungTu, "loaiChungTu", "TenChungTu");
        dtLoaiChungTu.Dispose();

        if (string.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }
        DataTable dtALL = _nguonNSService.GetDSChungTu("Nguon_CTChi", "chi", Nam);     

        double nums = dtALL.Rows.Count;
        var dt = dtALL.Copy();
        if (nums > 0)
            dt = dtALL.AsEnumerable().Skip((CurrentPage - 1) * Globals.PageSize).Take(Globals.PageSize).CopyToDataTable();

        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        string strPhanTrang = MyHtmlHelper.PageLinks(string.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { page = x}));
        string strThemMoi = Url.Action("Edit", "ChungTuChi_ChungTu");
    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 9%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform: uppercase; color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold; padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>    
    <br />    
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thêm đợt mới nhận nguồn Bộ Tài chính cấp </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <%
                using (Html.BeginForm("EditSubmit", "ChungTuChi_ChungTu", new { ParentID = ParentID}))
                {
                %>
                <%= Html.Hidden(ParentID + "_DuLieuMoi", 1)%>
                <table cellpadding="0" cellspacing="0" width="100%" class="table_form2">
                    <tr>
                        <td style="width: 80%">
                            <table cellpadding="0" cellspacing="0" width="50%" class="table_form2">
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                            <b>
                                                <%=NgonNgu.LayXau("Bổ sung đợt mới")%></b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">                                       
                                        <div>
                                            <%=MyHtmlHelper.CheckBox(ParentID, iThemMoi, "iThemMoi", "", "onclick=\"CheckThemMoi(this.checked)\"")%>
                                            <span style="color: brown;">(Đánh dấu chọn "Bổ sung đợt
                                                mới" để bổ sung đợt mới, ) </span>
                                        </div>                                       
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" border="0" width="50%" class="table_form2"
                                id="tb_DotNganSach">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Loại chứng từ</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slLoaiChungTu, "1", "Loai", "", "class=\"input1_2\" style=\"width: 301px\"")%><br />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Quyết định số</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextArea(ParentID, "", "SoQD", "", "class=\"input1_2\" style=\"height: 18px; width: 300px\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Quyết định ngày</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 250px; float: left;">
                                            <%=MyHtmlHelper.DatePicker(ParentID, CommonFunction.LayXauNgay(DateTime.Now), "NgayQD", "", "class=\"input1_2\"  style=\"width: 299px;\"")%>
                                        </div>
                                    </td>
                                </tr>                                                              
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Nội dung đợt</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 250px; float: left;">
                                            <%=MyHtmlHelper.TextArea(ParentID, "", "NoiDung", "", "class=\"input1_2\" style=\"height: 100px; width: 300px\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thêm mới")%>" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%  } %>
            </div>
        </div>
    </div>   
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách đợt nhận nguồn Bộ Tài chính</span>
                    </td>
                    <td align="right" style="padding-right: 10px;">
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid" id="<%= ParentID %>_thList">
            <tr>
                <th style="width: 30px;" align="center">STT
                </th>
                <th style="width: 200px;" align="center">Loại chứng từ
                </th>
                <th style="width: 100px;" align="center">Số chứng từ
                </th>
                <th style="width: 100px;" align="center">Số quyết định
                </th>
                <th style="width: 100px;" align="center">Ngày quyết định
                </th>
                <th align="center">Nội dung đợt
                </th>
                <th style="width: 60px;" align="center">Chi tiết
                </th>
                <th style="width: 140px;" align="center">Số tiền
                </th>
                <th style="width: 100px;" align="center">Người lập
                </th>
                <th style="width: 60px;" align="center">Sửa
                </th>
                <th style="width: 60px;" align="center">Xóa
                </th>                
            </tr>
            <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    int STT = i + 1;
                    string strEdit = "";
                    string strDelete = "";

                    strEdit = MyHtmlHelper.ActionLink(Url.Action("Edit", "ChungTuChi_ChungTu", new { Id = R["Id"] }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                    strDelete = MyHtmlHelper.ActionLink(Url.Action("Delete", "ChungTuChi_ChungTu", new { Id = R["Id"] }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");

                    string strTongTien = CommonFunction.DinhDangSo(Convert.ToString(R["TongTien"]));
                    string strURL = MyHtmlHelper.ActionLink(Url.Action("Index", "ChungTuChi_ChungTuChiTiet", new { Id_CTChi = R["Id"] }).ToString(), "<img src='../Content/Themes/images/btnSetting.png' alt='' />", "Detail", null, "title=\"Xem chi tiết chứng từ\"");                   
                   
            %>
            <tr>
                <td align="center">
                    <b>
                        <%=STT%></b>
                </td>
                <td align="center">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["LoaiCT"])%>
                </td>                
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ChungTuChi_ChungTuChiTiet", new { Id_CTChi = R["Id"] }).ToString(),dt.Rows[i]["ChungTu"].ToString(), "Detail", "")%></b>
                </td>
                <td align="center">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["SoQD"])%>
                </td>
                <td align="center">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["NgayQD"])%>
                </td>
                <td align="left">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["NoiDung"])%>
                </td>
                <td align="center">
                    <%=strURL %>
                </td>
                <td align="right" style="color: black;">
                    <b>
                        <%=strTongTien%></b>
                </td>
                <td align="center">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["NguoiTao"])%>
                </td>
                <td align="center">
                    <%=strEdit%>
                </td>
                <td align="center">
                    <%=strDelete%>
                </td>                
            </tr>
            <%} %>
            <tr class="pgr">
                <td colspan="10" align="right">
                    <%=strPhanTrang%>
                </td>
            </tr>
        </table>
    </div>
    <%  
        dt.Dispose();
    %>
    <script type="text/javascript">
        CheckThemMoi(false);
        function CheckThemMoi(value) {
            if (value == true) {
                document.getElementById('tb_DotNganSach').style.display = '';
            } else {
                document.getElementById('tb_DotNganSach').style.display = 'none';
            }
        }        
    </script>
</asp:Content>
