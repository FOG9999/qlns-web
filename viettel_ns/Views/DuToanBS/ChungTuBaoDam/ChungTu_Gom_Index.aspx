<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int i;
        string MaND = User.Identity.Name;
        string ParentID = "DuToan_ChungTu";
        string ChiNganSach = Request.QueryString["ChiNganSach"];
        string MaDotNganSach = Convert.ToString(ViewData["MaDotNganSach"]);
        string iID_MaChungTu_TLTHCuc = Request.QueryString["iID_MaChungTu"];
        string sLNS = Request.QueryString["sLNS"];
        
        string iSoChungTu = Request.QueryString["SoChungTu"];
        string sTuNgay = Request.QueryString["TuNgay"];
        string sDenNgay = Request.QueryString["DenNgay"];
        string sLNS_TK = Request.QueryString["sLNS_TK"];
        string iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
        string page = Request.QueryString["page"];
        string iLoai = Request.QueryString["iLoai"];
        string iKyThuat = Request.QueryString["iKyThuat"]; ;
        
        sLNS = "1040100";
        int CurrentPage = 1;
        if (HamChung.isDate(sTuNgay) == false) sTuNgay = "";
        if (HamChung.isDate(sDenNgay) == false) sDenNgay = "";

        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1") iID_MaTrangThaiDuyet = "";
        Boolean bThemMoi = false;
        String iThemMoi = "";
        if (ViewData["bThemMoi"] != null)
        {
            bThemMoi = Convert.ToBoolean(ViewData["bThemMoi"]);
            if (bThemMoi)
                iThemMoi = "on";
        }
        string dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        DataTable dtTrangThai_All;
        DataTable dtTrangThai;
        if (iKyThuat == "1")
        {
             dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeChiTieu);
             dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(PhanHeModels.iID_MaPhanHeChiTieu, MaND);
        }
        else
        {
             dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeDuToan);
             dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(PhanHeModels.iID_MaPhanHeDuToan, MaND);
        }
        dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
        dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = -1;
        dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
        dtTrangThai.Dispose();
        string[] arrChungTu = new String[2];
        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }
        //kiem tra nguoi dung co phan tro ly tong hop
        Boolean check = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
        Boolean CheckNDtao=false;
        if (check) CheckNDtao = true;
        
        //Lấy danh sách chứng từ để gom
        DataTable dtChungTuDuyet = DuToanBS_ChungTuModels.LayDanhSachChungDeGomTLTH(MaND, sLNS);
        
        //Lấy danh sách chứng từ TLTH của page
        DataTable dt = DuToanBS_ChungTuModels.LayDanhSachChungTuTLTH(iID_MaChungTu_TLTHCuc, sLNS, MaND, CheckNDtao, sTuNgay, sDenNgay, iID_MaTrangThaiDuyet, CurrentPage, Globals.PageSize);

        //Lấy tổng số chứng từ TLTH
        double nums = DuToanBS_ChungTuModels.LayDanhSachChungTuTLTH(iID_MaChungTu_TLTHCuc, sLNS, MaND, CheckNDtao, sTuNgay, sDenNgay, iID_MaTrangThaiDuyet, CurrentPage, Globals.PageSize).Rows.Count;
        
        //Phân trang
        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        string strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { SoChungTu = iSoChungTu, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x }));
        string strThemMoi = Url.Action("Edit", "DuToanBS_ChungTu", new { MaDotNganSach = MaDotNganSach, sLNS = sLNS, ChiNganSach = ChiNganSach });
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
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold;
                padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>

    <%--Tìm Kiếm--%>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Tìm kiếm đợt dự toán ngân sách bảo đảm</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <%
                    using (Html.BeginForm("TimKiemChungTu", "DuToanBS_ChungTu", new { ParentID = ParentID, sLNS = sLNS, iLoai = 1 }))
                    {       
                %>
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="td_form2_td1" style="width: 10%">
                            <div><b>Đợt dự toán từ ngày</b></div>
                        </td>
                        <td class="td_form2_td5" style="width: 10%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, sTuNgay, "dTuNgay", "", "class=\"input1_2\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div><b>Đến ngày</b></div>
                        </td>
                        <td class="td_form2_td5" style="width: 10%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, sDenNgay, "dDenNgay", "", "class=\"input1_2\"")%>
                            </div>
                        </td>
                          <td class="td_form2_td1" style="width: 10%">
                            <div><b>Trạng thái</b></div>
                        </td>
                        <td class="td_form2_td5" style="width: 10%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "iID_MaTrangThaiDuyet", "", "class=\"input1_2\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 20%">
                            <input type="submit" class="button" value="Tìm kiếm" />
                        </td>
                    </tr>
                </table>
                <%} %>
            </div>
        </div>
    </div>
    <br />
    <%--Thêm mới--%>
    <% if (check)
       { %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thêm một đợt gom mới ngân sách bảo đảm</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <%
                    using (Html.BeginForm("ThemSuaChungTu", "DuToanBS_ChungTu_BaoDam", new { ParentID = ParentID, sLNS1 = sLNS }))
                    {
                %>
                <%= Html.Hidden(ParentID + "_DuLieuMoi", 1) %>
                <table cellpadding="0" cellspacing="0" width="100%" class="table_form2">
                    <tr>
                        <td style="width: 80%">
                            <table cellpadding="0" cellspacing="0" width="50%" class="table_form2">
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                            <b>
                                                <%= NgonNgu.LayXau("Bổ sung đợt mới") %></b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= MyHtmlHelper.CheckBox(ParentID, iThemMoi, "iThemMoi", "", "onclick=\"CheckThemMoi(this.checked)\"") %>
                                            <span style="color: brown;">(Trường hợp bổ sung đợt mới, đánh dấu chọn "Bổ sung đợt
                                                mới". Nếu không chọn đợt bổ sung dưới lưới) </span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" border="0" width="50%" class="table_form2" id="tb_DotNganSach">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Chọn đợt</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                       <table  class="mGrid" style="width: 100%">
                                            <tr>
                                                <th align="center" style="width: 40px;"> <input type="checkbox"  id="abc" onclick="CheckAll(this.checked)" /></th>
                                            <% for (int c = 0; c < 1*2 - 1; c++)
                                            {%>
                                                <th></th>
                                            <% } %>
                                            </tr>
                                            <%
                                                String strsTen = "", MaDonVi = "", strChecked = "";
                                                for (int z = 0; z < dtChungTuDuyet.Rows.Count; z = z + 1)
                                                {
                                                    for (int c = 0; c < 1; c++)
                                                    {
                                                        if (z + c < dtChungTuDuyet.Rows.Count)
                                                        {
                                                            strChecked = "";
                                                            strsTen = Convert.ToString(dtChungTuDuyet.Rows[z + c]["sDSLNS"]) + '-' +
                                                                         CommonFunction.LayXauNgay(
                                                                             Convert.ToDateTime(dtChungTuDuyet.Rows[z + c]["dNgayChungTu"])) + '-' +
                                                                         Convert.ToString(dtChungTuDuyet.Rows[z + c]["sID_MaNguoiDungtao"])+ '-' +
                                                                          Convert.ToString(dtChungTuDuyet.Rows[z + c]["sNoiDung"]);
                                                            MaDonVi = Convert.ToString(dtChungTuDuyet.Rows[z + c]["iID_MaChungTu"]);
                                                            if (arrChungTu.Contains(MaDonVi))
                                                            {
                                                                strChecked = "checked=\"checked\"";
                                                                break;
                                                            }%>
                                            <tr>
                                                <td align="center" style="width: 40px;">
                                                    <input type="checkbox" value="<%= MaDonVi %>" <%= strChecked %> check-group="DonVi" id="iID_MaChungTu" name="iID_MaChungTu" />
                                                </td>
                                                <td align="left">
                                                    <%= strsTen %>
                                                </td>
                                            <% } %>
                                            <% } %>
                                            </tr>
                                            <% }%>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Ngày tháng</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 200px; float: left;">
                                            <%= MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"input1_2\"  style=\"width: 200px;\"") %>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu") %>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Nội dung đợt</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= MyHtmlHelper.TextArea(ParentID, "", "sNoiDung", "", "class=\"input1_2\" style=\"height: 100px;\"") %>
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
                                            <input type="submit" class="button" id="Submit1" value="<%= NgonNgu.LayXau("Thêm mới") %>" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <% } %>
            </div>
        </div>
    </div> <% } %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách đợt dự toán tổng hợp phần ngân sách bảo đảm</span>
                    </td>
                    <td align="right" style="padding-right: 10px;">
                        <%--<input id="Button1" type="submit" class="button_title" value="Trình duyệt" onclick="javascript:location.href=''" />--%>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid" id="<%= ParentID %>_thList">
            <tr>
                <th style="width: 2%;" align="center">
                    STT
                </th>
                <th style="width: 15%;" align="center">
                    Đợt dự toán
                </th>
                <th align="center">
                    Nội dung đợt dự toán
                </th>
                <th style="width: 5%;" align="center">
                    Chi tiết
                </th>
                <th style="width: 5%;" align="center">
                    Sửa
                </th>
                <th style="width: 5%;" align="center">
                    Xóa
                </th>
                <th style="width: 5%;" align="center">
                    Người tạo
                </th>
                <th style="width: 15%;" align="center">
                    Trạng thái
                </th>
                <th style="width: 5%;" align="center">
                    Duyệt
                </th>
                 <th style="width: 5%;" align="center">
                    Từ chối
                </th>
            </tr>
            <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    String NgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayChungTu"]));
                    String sTrangThai = "";
                    String strColor = "";
                    for (int j = 0; j < dtTrangThai_All.Rows.Count; j++)
                    {
                        if (Convert.ToString(R["iID_MaTrangThaiDuyet"]) == Convert.ToString(dtTrangThai_All.Rows[j]["iID_MaTrangThaiDuyet"]))
                        {
                            sTrangThai = Convert.ToString(dtTrangThai_All.Rows[j]["sTen"]);
                            strColor = String.Format("style='background-color: {0}; background-repeat: repeat;'", dtTrangThai_All.Rows[j]["sMauSac"]);
                            break;
                        }
                    }
                    String strEdit = "";
                    String strDelete = "";
                    if (check) 
                    {
                        strEdit = MyHtmlHelper.ActionLink(Url.Action("ThemSuaChungTu", "DuToanBS_ChungTu_BaoDam", new { iID_MaChungTu = R["iID_MaChungTu_TLTH"], sLNS = sLNS }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                        strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBS_ChungTu_BaoDam", new { iID_MaChungTu = R["iID_MaChungTu_TLTH"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                    }
            %>
            <tr <%=strColor %>>
                <td align="center">
                    <b><%=R["rownum"]%></b>
                </td>
                <td align="center">
                    <b><%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBS_ChungTu_BaoDam", new { ChiNganSach = ChiNganSach, MaDotNganSach = MaDotNganSach, sLNS = sLNS, bTLTH = 1, iID_MaChungTu = R["iID_MaChungTu_TLTH"] }).ToString(), "Đợt ngày: " + NgayChungTu, "Detail", "")%></b>
                </td>
                <td align="left">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["sNoiDung"])%>
                </td>
                <td align="center">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBS_ChungTu_BaoDam", new { iID_MaChungTu = R["iID_MaChungTu_TLTH"], sLNS = sLNS, bTLTH = 1 }).ToString(), "<img src='../Content/Themes/images/btnSetting.png' alt='' />", "Detail", null, "title=\"Xem chi tiết chứng từ\"")%>
                </td>
                <td align="center">
                    <%=strEdit%>
                </td>
                <td align="center">
                    <%=strDelete%>
                </td>
                <td align="center">
                    <%=R["sID_MaNguoiDungTao"]%>
                </td>
                <td align="center">
                    <%=sTrangThai%>
                </td>
               <td align="center">
                    <div onclick="OnInit_CT_NEW(500, 'Duyệt chứng từ');">
                        <%= Ajax.ActionLink("Trình Duyệt", "Index", "NhapNhanh", new { id = "DUTOANBS_TRINHDUYETCHUNGTU_GOM", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu_TLTH"], sLNS = sLNS}, new AjaxOptions { }, new { @class = "buttonDuyet" })%>
                    </div>
                </td>
                <td align="center">
                    <div onclick="OnInit_CT_NEW(500, 'Từ chối chứng từ');">
                        <%= Ajax.ActionLink("Từ Chối", "Index", "NhapNhanh", new { id = "DUTOANBS_TUCHOICHUNGTU_GOM", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu_TLTH"]  , sLNS = sLNS }, new AjaxOptions { }, new { @class = "button123" })%>
                    </div>
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
    <div id="idDialog" style="display: none;"></div>
    <%  
        dt.Dispose();
        dtTrangThai.Dispose();
    %>
    <script type="text/javascript">
        $(function () {
            $('.button123').text('');
        });
        $(function () {
            $('.buttonDuyet').text('');
        });
        CheckThemMoi(false);
        function CheckThemMoi(value) {
            if (value == true) {
                document.getElementById('tb_DotNganSach').style.display = ''
            } else {
                document.getElementById('tb_DotNganSach').style.display = 'none'
            }
        }
        function OnInit_CT_NEW(value, title) {
            $("#idDialog").dialog("destroy");
            document.getElementById("idDialog").title = title;
            document.getElementById("idDialog").innerHTML = "";
            $("#idDialog").dialog({
                resizeable: false,
                draggable: true,
                width: value,
                modal: true,
                open: function (event, ui) {
                    $(event.target).parent().css('position', 'fixed');
                    $(event.target).parent().css('top', '100px');
                }
            });
        }
        function OnLoad_CT(v) {
            document.getElementById("idDialog").innerHTML = v;
        }
        function funcTest(value) {
            $("#dialogTest").dialog({
                resizeable: false,
                draggable: true,
                width: value,
                modal: true,
                open: function (event, ui) {
                    $("#dialogTest div.label").html(value);
                    $("#dialogTest #txtCode").val(value);
                }
            });
            return false;
        }
        function saveCode() {
            alert($("#dialogTest #txtCode").val());
        }
    </script>
     <script type="text/javascript">
         function CheckAll(value) {
             $("input:checkbox[check-group='DonVi']").each(function (i) {
                 this.checked = value;
             });
         }                                            
 </script>
<%-- <div id="dialogTest">
     <div class="label"></div>
     <input id="txtCode"/>
     <button id="btnSaveCode" onclick="return saveCode();">ok</button>
 </div>--%>
</asp:Content>
