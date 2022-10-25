<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        String ParentID = "Edit";
        String UserID = User.Identity.Name;
        String ChiNganSach = Convert.ToString(ViewData["ChiNganSach"]);
        String MaChungTu = Convert.ToString(ViewData["MaChungTu"]);
        String MaDotNganSach = Convert.ToString(ViewData["MaDotNganSach"]);
        String sLNS = Convert.ToString(ViewData["sLNS"]);
        String MaPhongBanNguoiDung = NganSach_HamChungModels.MaPhongBanCuaMaND(UserID);

        DataTable dtChungTu = DuToan_ChungTuModels.GetChungTu(MaChungTu);
        DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(UserID);
        SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "TenHT");
        DataRow R1 = dtDonVi.NewRow();
        R1["iID_MaDonVi"] = "-1";
        R1["TenHT"] = "--- Chọn đơn vị (Không chọn sẽ cấp cho tất cả đơn vị quản lý) ---";
        dtDonVi.Rows.InsertAt(R1, 0);
        dtDonVi.Dispose();
        DataRow R;
        String iSoChungTu = "", dNgayChungTu = "", sNoiDung = "", iID_MaDonVi = "";
        if (dtChungTu.Rows.Count > 0)
        {
            R = dtChungTu.Rows[0];
            dNgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayChungTu"]));
            if (String.IsNullOrEmpty(Convert.ToString(R["iID_MaDonVi"])))
            {
                iID_MaDonVi = "Chứng từ nhiều đơn vị";
            }
            else
            {
                iID_MaDonVi = Convert.ToString(R["iID_MaDonVi"]);
            }
            sNoiDung = Convert.ToString(R["sNoiDung"]);
            iSoChungTu = Convert.ToString(R["iSoChungTu"]);
        }
        else
        {
            dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        }

        using (Html.BeginForm("EditSubmit", "DuToan_ChungTu", new { ParentID = ParentID, MaChungTu = MaChungTu, sLNS1 = sLNS }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", ViewData["DuLieuMoi"])%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td><span>
                        <%
                            if (ViewData["DuLieuMoi"] == "1")
                            {
                        %>
                        <%=NgonNgu.LayXau("Thêm mới chứng từ")%>
                        <%
                            }
                            else
                            {
                        %>
                        <%=NgonNgu.LayXau("Sửa thông tin chứng từ")%>
                        <%
                            }
                        %>&nbsp; &nbsp;
                    </span></td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <div style="width: 50%; float: left;">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td class="td_form2_td1" style="width: 15%;">
                                <div>Số chứng từ</div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=iSoChungTu%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div>Đơn vị</div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=iID_MaDonVi%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div>Ngày chứng từ</div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"form-control\"")%>
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu")%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div>Nội dung</div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.TextArea(ParentID, sNoiDung, "sNoiDung", "", "class=\"form-control\" style=\"height: 100px;\"")%></div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1"></td>
                            <td class="td_form2_td5">
                                <div>
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td width="65%" class="td_form2_td5">&nbsp;</td>
                                            <td width="30%" align="right" class="td_form2_td5">
                                                <%--<input type="submit" class="button" id="Submit1" value="Lưu" />--%>
                                                <button class="btn btn-primary" id="Submit1" type="submit"><i class="fa fa-download"></i> Lưu</button>
                                            </td>
                                            <td width="5px">&nbsp;</td>
                                            <td class="td_form2_td5">
                                                <%--<input class="button" type="button" value="Hủy" onclick="history.go(-1)" />--%>
                                                 <button class="btn btn-default" onclick="history.go(-1)" type="button"><i class="fa fa-ban"></i>Hủy</button>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%
            dtChungTu.Dispose();
        }
    %>
</asp:Content>

