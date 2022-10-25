<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.ThuNop" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        String ParentID = "ThuNop_ChungTu";
        String iLoai = Request.QueryString["iLoai"];
        String MaND = User.Identity.Name;
        String MaDonVi = Request.QueryString["MaDonVi"];
        String iSoChungTu = Request.QueryString["SoChungTu"];
        String sThang = Request.QueryString["Thang"];
        String dTuNgay = Request.QueryString["TuNgay"];
        String dDenNgay = Request.QueryString["DenNgay"];
        String iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
        String page = Request.QueryString["page"];

        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1") iID_MaTrangThaiDuyet = "";

        DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
        SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "TenHT");
        DataRow R1 = dtDonVi.NewRow();
        R1["iID_MaDonVi"] = "";
        R1["TenHT"] = "--- Chọn đơn vị ---";
        dtDonVi.Rows.InsertAt(R1, 0);
        dtDonVi.Dispose();

        DataTable dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(SharedModels.iID_MaPhanHe, MaND);
        dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
        dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = -1;
        dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");

        int CurrentPage = 1;
        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }

        DataTable dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(SharedModels.iID_MaPhanHe);
        DataTable dt = ThuNop_ChungTuModels.Get_DanhSachChungTu(iLoai, MaND, iSoChungTu, sThang, dTuNgay, dDenNgay, MaDonVi, iID_MaTrangThaiDuyet, CurrentPage, Globals.PageSize);
        DataTable dtCopy = dt.Copy();

        double nums = ThuNop_ChungTuModels.Get_DanhSachChungTu_Count(iLoai, MaND, iSoChungTu, dTuNgay, dDenNgay, MaDonVi, iID_MaTrangThaiDuyet);
        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        String strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { iLoai = iLoai, MaND = MaND, SoChungTu = iSoChungTu, TuNgay = dTuNgay, DenNgay = dDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x }));

        String strThemMoi = Url.Action("Edit", "ThuNop_ChungTu", new { iLoai = iLoai });
        Boolean bThemMoi = false;
        String iThemMoi = "";
        if (ViewData["bThemMoi"] != null)
        {
            bThemMoi = Convert.ToBoolean(ViewData["bThemMoi"]);
            if (bThemMoi)
                iThemMoi = "on";
        }

        DataTable dtThang = DateTimeExtension.getDSThang();
        SelectOptionList slThang = new SelectOptionList(dtThang, "MaTG", "TenTG");
        dtThang.Dispose();

        String dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        String sTienToChungTu = PhanHeModels.LayTienToChungTu(SharedModels.iID_MaPhanHe);
        int iSoChungTuMax = ThuNop_ChungTuModels.GetMaxChungTu() + 1;

    %>
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
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Tìm kiếm thông tin đợt thu nộp</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <%  using (Html.BeginForm("SearchSubmit", "ThuNop_ChungTu", new { ParentID = ParentID, iLoai = iLoai }))
                    { %>
                <table cellpadding="5" cellspacing="5" width="100%" border="0">
                    <tr>
                        <td class="td_form2_td1" style="width: 10%">
                            <div style="text-align: center">
                                <b>Đơn vị</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slDonVi, MaDonVi, "iID_MaDonVi", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Ngày nhập từ ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, dTuNgay, "dTuNgay", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Số liệu thu nộp của tháng</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slThang, sThang, "iThang_Quy", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="width: 10%">
                            <div style="text-align: center">
                                <b>Số chứng từ</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 10%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, iSoChungTu, "iSoChungTu", "", "class=\"form-control\"")%>
                            </div>
                        </td>

                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Đến ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 10%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, dDenNgay, "dDenNgay", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td></td>
                        <td class="td_form2_td1" style="width: 10%; text-align: right">
                            <%--<input type="submit" class="button" value="Tìm kiếm" />--%>
                            <button class="btn btn-info" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                        </td>
                    </tr>
                </table>
                <%  } %>
            </div>
        </div>
    </div>
    <br />
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thêm một đợt mới </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <%
                    using (Html.BeginForm("EditSubmit", "ThuNop_ChungTu", new { ParentID = ParentID }))
                    {
                %>
                <%= Html.Hidden(ParentID + "_DuLieuMoi", 1)%>
                <%= Html.Hidden(ParentID + "_iLoai", iLoai)%>
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
                                            <b>Số chứng từ: </b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 200px; float: left;">
                                            <b><%=sTienToChungTu%><%=iSoChungTuMax%></b>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Đơn vị: </b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="float: left;">
                                            <%=MyHtmlHelper.DropDownList(ParentID, slDonVi, MaDonVi, "iID_MaDonVi", "", "class=\"form-control\"")%>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_iID_MaDonVi")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Số liệu thu nộp của tháng: </b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="float: left;">
                                            <%=MyHtmlHelper.DropDownList(ParentID, slThang, sThang, "iThang_Quy", "", "class=\"form-control\"")%><br />
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_iThang_Quy")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Ngày nhập: </b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="float: left;">
                                            <%=MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"form-control\"")%>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Nội dung đợt: </b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
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
                                            <button class="btn btn-primary" type="submit" id="Submit1"><i class="fa fa-plus"></i><%=NgonNgu.LayXau("Thêm mới")%></button>
                                            <%--<input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thêm mới")%>" />--%>
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
    <br />    
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <%
                            switch (iLoai)
                            {
                                case "1":
                        %>
                        <span>Danh sách chứng từ thông tri thu</span>
                        <%
                                break;
                            case "2":
                        %>
                        <span>Danh sách chứng từ tổng hợp thu năm</span>
                        <%
                                break;
                            case "3":
                        %>
                        <span>Danh sách chứng từ quyết toán các khoản chi từ nguồn thu để lại</span>
                        <%
                                break;
                            }
                        %>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid">
            <tr>
                <th style="width: 5%;" align="center">STT
                </th>
                <th style="width: 10%;" align="center">Số liệu thu nộp của tháng
                </th>
                <th style="width: 15%;" align="center">Ngày nhập chứng từ
                </th>
                <th style="width: 10%;" align="center">Số chứng từ
                </th>
                <th style="width: 15%;" align="center">Đơn vị
                </th>
                <th style="width: 20%;" align="center">Nội dung đợt thu nộp
                </th>
                <th style="width: 10%;" align="center">Trạng thái
                </th>
                <th></th>
            </tr>
            <%
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    int STT = i + 1;
                    String NgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayChungTu"]));
                    String sTrangThai = "", strColor = "";
                    for (int j = 0; j < dtTrangThai_All.Rows.Count; j++)
                    {
                        if (Convert.ToString(R["iID_MaTrangThaiDuyet"]) == Convert.ToString(dtTrangThai_All.Rows[j]["iID_MaTrangThaiDuyet"]))
                        {
                            sTrangThai = Convert.ToString(dtTrangThai_All.Rows[j]["sTen"]);
                            strColor = String.Format("style='background-color: {0}; background-repeat: repeat;'", dtTrangThai_All.Rows[j]["sMauSac"]);
                            break;
                        }
                    }
                    String strThang = "";
                    for (int j = 0; j < dtThang.Rows.Count; j++)
                    {
                        String sThang_Quy = Convert.ToString(dtThang.Rows[j]["MaTG"]);
                        if (Convert.ToString(R["iThang_Quy"]).Equals(sThang_Quy))
                        {
                            strThang = Convert.ToString(dtThang.Rows[j]["TenTG"]);
                        }
                    }

                    String strMaDV = Convert.ToString(R["iID_MaDonVi"]);
                    String strTenDonvi = strMaDV + " - " + Convert.ToString(DonViModels.Get_TenDonVi(strMaDV, MaND));

                    String strEdit = "";
                    String strDelete = "";
                    if (LuongCongViecModel.NguoiDung_DuocThemChungTu(SharedModels.iID_MaPhanHe, MaND) &&
                                        LuongCongViecModel.KiemTra_TrangThaiKhoiTao(SharedModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))
                    {
                        strEdit = MyHtmlHelper.ActionLink(Url.Action("Edit", "ThuNop_ChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], iLoai = iLoai }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "");
                        //bool checkXoaThuNop = SharedModels.checkXoaThuNop(Convert.ToString(R["iID_MaChungTu"]));
                        //if(checkXoaThuNop==true)
                        strDelete = MyHtmlHelper.ActionLink(Url.Action("Delete", "ThuNop_ChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], iLoai = iLoai }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "");
                    }



            %>
            <tr <%=strColor %>>
                <td align="center">
                    <%=R["rownum"]%>
                </td>
                <td align="center">
                    <b><%=strThang%></b>
                </td>
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ThuNop_ChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], iLoai = iLoai }).ToString(), "Ngày nhập: " + NgayChungTu, "Detail", "")%></b>
                </td>
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ThuNop_ChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], iLoai = iLoai }).ToString(), Convert.ToString(R["sTienToChungTu"]) + Convert.ToString(R["iSoChungTu"]), "Detail", "")%></b>
                </td>
                <td align="left">
                    <%=strTenDonvi %>
                </td>
                <td align="left">
                    <%=dt.Rows[i]["sNoiDung"]%>
                </td>
                <td align="center">
                    <%=sTrangThai %>
                </td>

                <td align="center">
                    <%=strEdit%>
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
        dtTrangThai_All.Dispose();
        dtTrangThai.Dispose();
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
