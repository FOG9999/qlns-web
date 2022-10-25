<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string MaND = User.Identity.Name;
        string ParentID = "DuToanBS_ChungTu_BD";
        string ChiNganSach = Request.QueryString["ChiNganSach"];
        string iID_MaChungTu_TLTH = Request.QueryString["iID_MaChungTu"];
        string MaDotNganSach = Convert.ToString(ViewData["MaDotNganSach"]);
        string iSoChungTu = Request.QueryString["SoChungTu"];
        string sTuNgay = Request.QueryString["TuNgay"];
        string sDenNgay = Request.QueryString["DenNgay"];
        string sLNS_TK = Request.QueryString["sLNS_TK"];
        string iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
        string page = Request.QueryString["page"];
        string iKyThuat = Request.QueryString["iKyThuat"];
        string sLNS = Request.QueryString["sLNS"];
        int CurrentPage = 1;
        if (!HamChung.isDate(sTuNgay)) sTuNgay = "";
        if (!HamChung.isDate(sDenNgay)) sDenNgay = "";
        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1") iID_MaTrangThaiDuyet = "";
        bool bThemMoi = false;
        string iThemMoi = "";
        if (ViewData["bThemMoi"] != null)
        {
            bThemMoi = Convert.ToBoolean(ViewData["bThemMoi"]);
            if (bThemMoi)
                iThemMoi = "on";
        }
        string dNgayChungTu;
        if (ViewData["dNgayChungTu"] != null)
        {
            dNgayChungTu = Convert.ToString(ViewData["dNgayChungTu"]);
        }
        else
        {
            dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        }

        //Trạng thái all
        DataTable dtTrangThai_All;
        if (iKyThuat == "1")
        {
            dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeChiTieu);
        }
        else
        {
            dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeDuToan);
        }
        DataTable dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(PhanHeModels.iID_MaPhanHeChiTieu, MaND);
        dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
        dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = -1;
        dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
        dtTrangThai.Dispose();

        //Loại ngân sách dropdown list
        sLNS = "104,109";
        if (iKyThuat == "1") sLNS = "1040100";
        DataTable dtLoaiNganSach = NganSach_HamChungModels.DSLNS_LocCuaPhongBan(MaND, sLNS);
        SelectOptionList slLoaiNganSach = new SelectOptionList(dtLoaiNganSach, "sLNS", "sTen");
        dtLoaiNganSach.Dispose();

        string iID_MaNguon = Request.QueryString["iID_MaNguon"];
        //DataTable dtNguon = DuToanBSChungTuModels.getNguon();
        //SelectOptionList slNguon = new SelectOptionList(dtNguon, "iID_MaNguon", "TenHT");
        //dtNguon.Dispose();

        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }
        //kiem tra nguoi dung co phan tro ly phong ban
        Boolean check = LuongCongViecModel.KiemTra_TroLyPhongBan(MaND);
        Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
        Boolean CheckNDtao = false;
        if (check) CheckNDtao = true;
        if (checkTroLyTongHop) check = true;

        //Lấy danh sách chứng từ page hiện tại
        DataTable dt = DuToanBSChungTuModels.LayDanhSachChungTu(iID_MaChungTu_TLTH, sLNS, MaDotNganSach, MaND, iSoChungTu, sTuNgay, sDenNgay, sLNS_TK, iID_MaTrangThaiDuyet, CheckNDtao, iKyThuat, CurrentPage, Globals.PageSize);

        //Lấy số lượng tổng cộng chứng từ
        double nums = DuToanBSChungTuModels.LayDanhSachChungTu(iID_MaChungTu_TLTH, sLNS, MaDotNganSach, MaND, iSoChungTu, sTuNgay, sDenNgay, sLNS_TK, iID_MaTrangThaiDuyet, CheckNDtao, iKyThuat).Rows.Count;

        //Phân trang
        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        string strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { SoChungTu = iSoChungTu, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x, sLNS = sLNS }));
        int SoCot = 1;
        String[] arrLNS = sLNS.Split(',');
        string sKyThuat = "";
        if (iKyThuat == "1")
            sKyThuat = "Ngành kỹ thuật";
    %>
    <%--Liên kết nhanh--%>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 10%;">
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

    <%--Tìm kiếm--%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Tìm kiếm đợt dự toán ngân sách bảo đảm
                            <%=sKyThuat %></span>
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
                                <%=MyHtmlHelper.DropDownList(ParentID, slLoaiNganSach,sLNS_TK,"sLNS_TK", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 15%">
                            <div>
                                <b>Đợt dự toán từ ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, sTuNgay, "dTuNgay", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Đến ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, sDenNgay, "dDenNgay", "", "class=\"form-control\"")%>
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
                                <%=MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "ddlIDMaTrangThai", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 15%"></td>
                        <td class="td_form2_td5" style="width: 15%"></td>
                        <td class="td_form2_td1" style="width: 10%"></td>
                        <td class="td_form2_td5" style="width: 15%">
                             <button class="btn btn-info" id="" type="submit" style="float: right"><i class="fa fa-search"></i>Tìm kiếm</button>
                            <%--<input type="submit" class="button" value="Tìm kiếm" />--%>
                        </td>
                    </tr>
                </table>
                <%} %>
            </div>
        </div>
    </div>
    <br />
    <%--Thêm Mới--%>
    <%if (check || checkTroLyTongHop)
        {%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thêm một đợt cấp mới ngân sách bảo đảm
                            <%=sKyThuat %>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <%
                    using (Html.BeginForm("ThemSuaChungTu", "DuToanBSChungTu", new { ParentID = ParentID, sLNS = sLNS, iKyThuat = iKyThuat }))
                    {
                %>
                <%= Html.Hidden(ParentID + "_DuLieuMoi", 1)%>
                <%= Html.Hidden(ParentID + "_iKyThuat", iKyThuat)%>
                <table cellpadding="0" cellspacing="0" width="100%" class="table_form2">
                    <tr>
                        <td style="width: 80%">
                            <table cellpadding="0" cellspacing="0" width="50%" class="table_form2">
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                            <b><%=NgonNgu.LayXau("Bổ sung đợt mới")%></b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.CheckBox(ParentID, iThemMoi, "iThemMoi", "", "onclick=\"CheckThemMoi()\"")%>
                                            <span style="color: brown;">(Trường hợp bổ sung đợt mới, đánh dấu chọn "Bổ sung đợt
                                                mới". Nếu không chọn đợt bổ sung dưới lưới) </span>
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
                                                    for (int i = 0; i < dtLoaiNganSach.Rows.Count; i = i + SoCot)
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
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_sLNS")%>
                                        </div>
                                    </td>
                                </tr>
                                <%--<tr>
                                    <td class="td_form2_td1">
                                        <div><b>Nguồn ngân sách</b></div>
                                        TuNB Hard Code MaNguon
                                        <%=Html.Hidden(ParentID + "_iID_MaNguon",1)%>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slNguon, iID_MaNguon, "iID_MaNguon1", "", "class=\"form-control\"")%></div>
                                        <%= Html.ValidationMessage(ParentID + "_" + "err_iID_MaNguon")%>
                                    </td>
                                </tr>--%>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Ngày tháng</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 200px; float: left;">
                                            <%=MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"form-control\"  style=\"width: 200px;\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">&nbsp;
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu")%>
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
                                            <%=MyHtmlHelper.TextArea(ParentID, "", "sNoiDung", "", "class=\"form-control\" style=\"height: 100px;\"")%>
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
                                            <button class="btn btn-primary" id="Submit1" type="submit"><i class="fa fa-plus"></i><%=NgonNgu.LayXau("Thêm mới")%></button>
                                            <%--<input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thêm mới")%>" />--%>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                </table>
                <%  } %>
            </div>
        </div>
    </div>
    <br />
    <%} %>
    <%--Danh sách--%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách đợt dự toán ngân sách bảo đảm <%=sKyThuat %></span>
                    </td>
                    <td align="right" style="padding-right: 10px;">
                        <%--<input id="Button1" type="submit" class="button_title" value="Trình duyệt" onclick="javascript:location.href=''" />--%>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid" id="<%= ParentID %>_thList">
            <tr>
                <th style="width: 2%;" align="center">STT
                </th>
                <th style="width: 5%;" align="center">Phòng ban
                </th>
                <th style="width: 5%;" align="center">LNS
                </th>
                <th style="width: 15%;" align="center">Đợt dự toán
                </th>
                <th align="center">Nội dung đợt dự toán
                </th>
                <th style="width: 5%;" align="center">Chi tiết
                </th>
                <th style="width: 5%;" align="center">Sửa
                </th>
                <th style="width: 5%;" align="center">Xóa
                </th>
                <th style="width: 5%;" align="center">Người tạo
                </th>
                <th style="width: 10%;" align="center">Trạng thái
                </th>
                <th style="width: 15%;" align="center">Lý do
                </th>
                <th style="width: 5%;" align="center">Duyệt
                </th>
                <th style="width: 5%;" align="center">Từ chối
                </th>
            </tr>
            <%
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    string subSLNS = R["sDSLNS"].ToString().Length > 15 ? (R["sDSLNS"].ToString().Substring(0, 15) + ",...") : R["sDSLNS"].ToString();
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
                    if (iKyThuat == "1")
                    {
                        if (checkTroLyTongHop && (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeChiTieu, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(PhanHeModels.iID_MaPhanHeChiTieu, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))))
                        {
                            strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                            strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                        }
                        else
                        {
                            if ((LuongCongViecModel.NguoiDung_DuocThemChungTu(PhanHeModels.iID_MaPhanHeChiTieu, MaND) &&
                                               (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeChiTieu, Convert.ToInt32(R["iID_MaTrangThaiDuyet"]))
                                                || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(PhanHeModels.iID_MaPhanHeChiTieu, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))))
                                                && check)
                            {
                                strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                                strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                            }
                        }
                    }
                    else
                    {
                        if (checkTroLyTongHop && (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeDuToan, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(PhanHeModels.iID_MaPhanHeDuToan, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))))
                        {
                            strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                            strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                        }
                        else
                        {
                            if (LuongCongViecModel.NguoiDung_DuocThemChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND) &&
                                               LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeDuToan, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(PhanHeModels.iID_MaPhanHeDuToan, Convert.ToInt32(R["iID_MaTrangThaiDuyet"]))) && check)
                            {
                                strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                                strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "DuToanBSChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach, iKyThuat = iKyThuat }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                            }
                        }
                    }
            %>
            <tr <%=strColor %>>
                <td align="center">
                    <b><%=R["rownum"]%></b>
                </td>
                <td align="center">
                    <b><%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = "104" }).ToString(), R["iID_MaPhongBanDich"], "Detail", "")%></b>
                </td>
                <td align="center">
                    <b><%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = "104" }).ToString(), subSLNS, "Detail", "")%></b>
                </td>
                <td align="center">
                    <b><%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new { ChiNganSach = ChiNganSach, MaDotNganSach = MaDotNganSach, iID_MaChungTu = R["iID_MaChungTu"], sLNS = "104" }).ToString(), "Đợt ngày: " + NgayChungTu, "Detail", "")%></b>
                </td>
                <td align="left">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["sNoiDung"])%>
                </td>
                <td align="center">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToanBSChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = "104" }).ToString(), "<img src='../Content/Themes/images/btnSetting.png' alt='' />", "Detail", null, "title=\"Xem chi tiết chứng từ\"")%>
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
                <td align="left">
                    <%=R["sLyDo"]%>
                </td>
                <td align="center">
                    <div onclick="OnInit_CT_NEW(500, 'Duyệt chứng từ');">
                        <%= Ajax.ActionLink("Trình Duyệt", "Index", "NhapNhanh", new { id = "DUTOANBS_TRINHDUYETCHUNGTU", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS,iKyThuat=iKyThuat, iID_MaChungTu_TLTH = iID_MaChungTu_TLTH }, new AjaxOptions { }, new { @class = "buttonDuyet" })%>
                    </div>
                </td>
                <td align="center">
                    <div onclick="OnInit_CT_NEW(500, 'Từ chối chứng từ');">
                        <%= Ajax.ActionLink("Từ Chối", "Index", "NhapNhanh", new { id = "DUTOANBS_TUCHOICHUNGTU", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu"], iID_MaChungTu_TLTH = iID_MaChungTu_TLTH, sLNS = sLNS, iKyThuat = iKyThuat }, new AjaxOptions { }, new { @class = "button123" })%>
                    </div>
                </td>
            </tr>
            <%} %>
            <tr class="pgr">
                <td colspan="13" align="right">
                    <%=strPhanTrang%>
                </td>
            </tr>
        </table>
    </div>
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
