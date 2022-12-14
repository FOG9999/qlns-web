<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ConfigurationManager.AppSettings["TitleView"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string ParentID = "DTBS_ChungTu";
        string MaND = User.Identity.Name;
        //string ChiNganSach = Request.QueryString["ChiNganSach"];
        string iID_MaChungTu_TLTH = Request.QueryString["iID_MaChungTu"];
        //string bTLTH = Request.QueryString["bTLTH"];
        string sLNS = Request.QueryString["sLNS"];
        string iSoChungTu = Request.QueryString["SoChungTu"];
        string sTuNgay = Request.QueryString["TuNgay"];
        string sDenNgay = Request.QueryString["DenNgay"];
        string sLNS_TK = Request.QueryString["sLNS_TK"];
        string iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
        string page = Request.QueryString["page"];
        string sNguon = Convert.ToString(ViewData["sNguon"]);
        string dNgayChungTu;
        if (ViewData["dNgayChungTu"] != null)
        {
            dNgayChungTu = ViewData["dNgayChungTu"].ToString();
            //dNgayChungTu = Convert.ToString(DateTime.Parse(ViewData["dNgayChungTu"]));
        }
        else
        {
            dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        }
        if (String.IsNullOrEmpty(sLNS))
        {
            sLNS = "-1";
        }
        if (HamChung.isDate(sTuNgay) == false) sTuNgay = "";
        if (HamChung.isDate(sDenNgay) == false) sDenNgay = "";
        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1") iID_MaTrangThaiDuyet = "";
        string iThemMoi = "";
        if (ViewData["bThemMoi"] != null)
        {
            if (Convert.ToBoolean(ViewData["bThemMoi"]))
            {
                iThemMoi = "on";
            }
        }

        //Tất cả trạng thái
        DataTable dtTrangThai_All;
        dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet_All();

        //Trạng thái được xem
        DataTable dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(DuToanModels.iID_MaPhanHe, MaND);
        dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
        dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = -1;
        dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
        dtTrangThai.Dispose();

        //Danh sách loại ngân sách
        DataTable dtLoaiNganSach = NganSach_HamChungModels.DSLNS_LocCuaPhongBan(MaND, sLNS);
        dtLoaiNganSach.Rows.InsertAt(dtLoaiNganSach.NewRow(), 0);
        dtLoaiNganSach.Rows[0]["sLNS"] = "";
        dtLoaiNganSach.Rows[0]["sTen"] = "-- Chọn loại ngân sách --";
        SelectOptionList slLoaiNganSach = new SelectOptionList(dtLoaiNganSach, "sLNS", "sTen");
        dtLoaiNganSach.Dispose();

        //Danh sách nguồn
        //DataTable dtNguon = DuToanBSChungTuModels.getNguon();
        //SelectOptionList slNguon = new SelectOptionList(dtNguon, "iID_MaNguon", "TenHT");
        //dtNguon.Dispose();

        int CurrentPage = 1;
        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }

        //Kiểm tra role trợ lý phòng ban
        Boolean check = LuongCongViecModel.KiemTra_TroLyPhongBan(MaND);

        //Kiểm tra role trợ lý tổng hợp
        Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
        Boolean checkTroLy11_02 = LuongCongViecModel.KiemTra_TroLyPB11_02(MaND);
        Boolean CheckNDtao = false;
        if (check) CheckNDtao = true;
        if (checkTroLyTongHop) check = true;
        if (checkTroLy11_02) check = true;

        //Lấy danh sách chứng từ.
        DataTable dt = DuToanBSChungTuModels.LayDanhSachChungTu(iID_MaChungTu_TLTH, sLNS, "", MaND, iSoChungTu, sTuNgay, sDenNgay, sLNS_TK, iID_MaTrangThaiDuyet, CheckNDtao, "0", CurrentPage, Globals.PageSize);

        //Lấy tổng số lượng chứng từ
        double nums = DuToanBSChungTuModels.LayDanhSachChungTu(iID_MaChungTu_TLTH, sLNS, "", MaND, iSoChungTu, sTuNgay, sDenNgay, sLNS_TK, iID_MaTrangThaiDuyet, CheckNDtao, "0").Rows.Count;

        //Phân trang
        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        String strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { sLNS = sLNS, SoChungTu = iSoChungTu, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x }));
        if (checkTroLy11_02 && !string.IsNullOrEmpty(iID_MaChungTu_TLTH))
            strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { sLNS = sLNS, iID_MaChungTu=iID_MaChungTu_TLTH,SoChungTu = iSoChungTu, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x }));
        int SoCot = 1;
        String[] arrLNS = sLNS.Split(',');
    %>
    <%--Lien Ket Nhanh--%>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 10%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform: uppercase; color: #ec3237;">
                    <b>
                        <%= NgonNgu.LayXau("Liên kết nhanh: ") %>
                    </b>
                </div>
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color: #ec3237;">
                    <%= MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ") %>
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold; padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>

    <%--Box Tim Kiem--%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Tìm kiếm đợt dự toán</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <%
                    using (Html.BeginForm("TimKiemChungTu", "DuToanBSChungTu", new { parentID = ParentID, sLNS = sLNS, maChungTu = iID_MaChungTu_TLTH }))
                    {
                %>
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="td_form2_td1" style="width: 15%">
                            <div>
                                <b>Loại ngân sách</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%= MyHtmlHelper.DropDownList(ParentID, slLoaiNganSach, sLNS_TK, "ddlLNStk", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Đợt dự toán từ ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%= MyHtmlHelper.DatePicker(ParentID, sTuNgay, "dTuNgay", "", "class=\"form-control\"") %>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Đến ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%= MyHtmlHelper.DatePicker(ParentID, sDenNgay, "dDenNgay", "", "class=\"form-control\"") %>
                            </div>
                        </td>
                     </tr>
                    <tr>
                        <td class="td_form2_td1" style="width: 15%">
                            <div>
                                <b>Trạng thái</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%= MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "ddlIDMaTrangThai", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%"></td>
                        <td class="td_form2_td5" style="width: 15%"></td>
                        <td class="td_form2_td1" style="width: 10%"></td>
                        <td class="td_form2_td5" style="width: 15%">
                            <%--<input type="submit" class="button" id="btSearch" value="Tìm kiếm" />--%>
                            <button class="btn btn-info" type="submit" id="btSearch" style="float: right"><i class="fa fa-search"></i>Tìm kiếm</button>
                        </td>
                    </tr>
                </table>
                <% } %>
            </div>
        </div>
    </div>
    <br />

    <%--Box Them Moi--%>
    <%
        if (String.IsNullOrEmpty(iID_MaChungTu_TLTH) && (check || checkTroLyTongHop))
        {
    %>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thêm Một Đợt Cấp Mới </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <%
                    using (Html.BeginForm("ThemSuaChungTu", "DuToanBSChungTu", new { parentID = ParentID, sLNS = sLNS }))
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
                                                <%= NgonNgu.LayXau("Bổ sung đợt mới") %>
                                            </b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= MyHtmlHelper.CheckBox(ParentID, iThemMoi, "iThemMoi", "", "onclick=\"CheckThemMoi()\"") %>
                                            <span style="color: brown;">(Trường hợp bổ sung đợt mới, đánh dấu chọn "Bổ sung đợt mới". Nếu không chọn đợt bổ sung dưới lưới) 
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" border="0" width="50%" class="table_form2"
                                id="tb_DotNganSach">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Loại ngân sách</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="overflow: scroll; width: 50%; height: 300px">
                                            <table class="mGrid" style="width: 100%">
                                                <tr>
                                                    <th align="center" style="width: 40px;">
                                                        <input type="checkbox" id="ddlLNS" onclick="CheckAll(this.checked)" />
                                                    </th>
                                                    <%
                                                        for (int c = 0; c < SoCot * 2 - 1; c++)
                                                        {
                                                    %>
                                                    <th></th>
                                                    <%
                                                        } %>
                                                </tr>
                                                <%
                                                    string strTen = "";
                                                    string strMa = "";
                                                    string strChecked = "";
                                                    for (int i = 1; i < dtLoaiNganSach.Rows.Count; i = i + SoCot)
                                                    {
                                                %>
                                                <tr>
                                                    <% for (int c = 0; c < SoCot; c++)
                                                        {
                                                            if (i + c < dtLoaiNganSach.Rows.Count)
                                                            {
                                                                strChecked = "";
                                                                strTen = Convert.ToString(dtLoaiNganSach.Rows[i + c]["sTen"]);
                                                                strMa = Convert.ToString(dtLoaiNganSach.Rows[i + c]["sLNS"]);
                                                                if (arrLNS.Contains(strMa))
                                                                {
                                                                    strChecked = "checked=\"checked\"";
                                                                }
                                                    %>
                                                    <td align="center" style="width: 40px;">
                                                        <input type="checkbox" value="<%= strMa %>" <%= strChecked %> check-group="LNS" id="sLNS" name="sLNS" />
                                                    </td>
                                                    <td align="left">
                                                        <%= strTen %>
                                                    </td>
                                                    <% } %>
                                                    <% } %>
                                                </tr>
                                                <% } %>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">&nbsp;
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_sLNS") %>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <%--<td class="td_form2_td1">
                                        <div>
                                            <b>Nguồn ngân sách</b>
                                        </div>
                                    </td>--%>
                                    <%--TuNB Hardcode iID_MaNguon--%>
                                    <%= Html.Hidden(ParentID + "_iID_MaNguon", 1) %>
                                    <%--<td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slNguon, sNguon, "iID_MaNguon1", "", "class=\"input1_2\"")%>
                                        </div>
                                    </td>--%>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Ngày tháng</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 200px; float: left;">
                                            <%= MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"form-control\"  style=\"width: 200px; resize: none;\" ")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">&nbsp;
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu") %>
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
                                        <div style="width: 550px; float: left;">
                                            <%= MyHtmlHelper.TextArea(ParentID, "", "sNoiDung", "", "class=\"input1_2\" style=\"height: 100px; resize: none\"")%>
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
                                            <%--<input type="submit" class="button" id="btTaoMoi" value="<%= NgonNgu.LayXau("Thêm mới") %>" />--%>
                                             <button class="btn btn-primary" type="submit" id="btTaoMoi"><i class="fa fa-plus"></i><%= NgonNgu.LayXau("Thêm mới") %></button>
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
    </div>
    <% } %>
    <br/>
    <%--Box Danh Sach--%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách đợt dự toán bổ sung ngân sách</span>
                    </td>
                    <td align="right" style="padding-right: 10px;"></td>
                </tr>
            </table>
        </div>
        <table class="mGrid" id="<%= ParentID %>_thList">
            <tr>
                <th style="width: 40px;" align="center">STT
                </th>
                <th style="width: 120px;" align="center">LNS
                </th>
                <th style="width: 40px;" align="center">Nguồn
                </th>
                <th style="width: 100px;" align="center">Đợt dự toán
                </th>
                <th align="center">Nội dung đợt dự toán
                </th>
                <th style="width: 40px;" align="center">Chi tiết
                </th>
                <th style="width: 120px;" align="center">Số tiền
                </th>
                <th style="width: 40px;" align="center">Sửa
                </th>
                <th style="width: 40px;" align="center">Xóa
                </th>
                <th style="width: 80px;" align="center">Người tạo
                </th>
                <th style="width: 150px;" align="center">Trạng thái
                </th>
                <th style="width: 190px;" align="center">Lý do
                </th>
                <th style="width: 40px;" align="center">Duyệt
                </th>
                <th style="width: 50px;" align="center">Từ chối
                </th>
            </tr>
            <%
                for (int i = 0; i < dt.Rows.Count; i++)
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

                    String strTongTien = "";
                    strTongTien = CommonFunction.DinhDangSo(DuToanBSChungTuModels.ThongKeTongSoTien_ChungTu(R));

                    String strEdit = "";
                    String strDelete = "";
                    if (checkTroLyTongHop && (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) ||
                                              (LuongCongViecModel.KiemTra_TrangThaiTuChoi(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))))
                    {
                        strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                        strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                    }
                    else
                    {
                        if ((LuongCongViecModel.NguoiDung_DuocThemChungTu(DuToanModels.iID_MaPhanHe, MaND) &&
                             (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"]))))) && check)
                        {
                            strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                            strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                        }
                    }

                    String LNS = R["sDSLNS"].ToString();
                    if (LNS.Length >= 15)
                        LNS = LNS.Substring(0, 15);

            %>
            <tr <%= strColor %>>
                <td align="center">
                    <%= R["rownum"] %>
                </td>
                <td align="center">
                    <b><%= MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new {iID_MaChungTu = R["iID_MaChungTu"]}).ToString(), LNS , "Detail", null, "") %></b>
                </td>
                <td align="center">
                    <%=R["iID_MaNguon"]%>
                </td>
                <td align="center">
                    <b><%= MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new {iID_MaChungTu = R["iID_MaChungTu"]}).ToString(), NgayChungTu,  "Detail", null, "") %></b>
                </td>
                <td align="left">
                    <%= HttpUtility.HtmlEncode(dt.Rows[i]["sNoiDung"]) %>
                </td>
                <td align="center">
                    <b><%= MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"] }).ToString(), "<img src='../Content/Themes/images/btnSetting.png' alt='' />", "Detail", null, "title=\"Xem chi tiết chứng từ\"")%></b>
                </td>
                <td align="right" style="color: black;">
                    <b>
                        <%=strTongTien%></b>
                </td>
                <td align="center">
                    <%= strEdit %>
                </td>
                <td align="center">
                    <%= strDelete %>
                </td>
                <td align="center">
                    <%= R["sID_MaNguoiDungTao"] %>
                </td>
                <td align="center">
                    <%= sTrangThai %>
                </td>
                <td align="left">
                    <%= R["sLyDo"] %>
                </td>
                <td align="center">
                    <div onclick="OnInit_CT_NEW(500, 'Duyệt chứng từ');">
                        <%= Ajax.ActionLink("Trình Duyệt", "Index", "NhapNhanh", new { id = "DUTOANBS_TRINHDUYETCHUNGTU", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iID_MaChungTu_TLTH = iID_MaChungTu_TLTH }, new AjaxOptions { }, new { @class = "buttonDuyet" })%>
                    </div>
                </td>
                <td align="center">
                    <div onclick="OnInit_CT_NEW(500, 'Từ chối chứng từ');">
                        <%= Ajax.ActionLink("Từ Chối", "Index", "NhapNhanh", new { id = "DUTOANBS_TUCHOICHUNGTU", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu"], iID_MaChungTu_TLTH = iID_MaChungTu_TLTH, sLNS = sLNS }, new AjaxOptions { }, new { @class = "button123" })%>
                    </div>
                </td>
            </tr>
            <% } %>
            <tr class="pgr">
                <td colspan="14" align="right">
                    <%= strPhanTrang %>
                </td>
            </tr>
        </table>
    </div>
    <%
        dt.Dispose();
        dtTrangThai.Dispose();
    %>
    <script type="text/javascript">

        function CheckAll(value) {
            $("input:checkbox[check-group='LNS']").each(function (i) {
                this.checked = value;
            });
        }

        $(function () {
            $('.button123').text('');
        });
        $(function () {
            $('.buttonDuyet').text('');
        });

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
                    $(event.target).parent().css('top', '10px');
                }
            });
        }

        function OnLoad_CT(v) {
            document.getElementById("idDialog").innerHTML = v;
        }

        CheckThemMoi();

        function CheckThemMoi() {
            var isChecked = document.getElementById("<%= ParentID %>_iThemMoi").checked;
            if (isChecked == true) {
                document.getElementById('tb_DotNganSach').style.display = '';
            } else {
                document.getElementById('tb_DotNganSach').style.display = 'none';
            }
        }
    </script>
    <div id="idDialog" style="display: none;">
    </div>

</asp:Content>
