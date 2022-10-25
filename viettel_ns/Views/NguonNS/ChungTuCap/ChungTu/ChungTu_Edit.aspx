<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="Viettel.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string ParentID = "NguonNS_ChungTuChi_Edit";
        string UserID = User.Identity.Name;
        string Id_CTCap = Convert.ToString(ViewData["Id_CTCap"]);
        string MaPhongBanNguoiDung = NganSach_HamChungModels.MaPhongBanCuaMaND(UserID);

        INguonNSService _nguonNSService = NguonNSService.Default;

        DataRow R = _nguonNSService.GetChungTu("Nguon_CTChi", Id_CTCap);

        DataTable dtLoaiChungTu = _nguonNSService.GetLoaiChungTu("cap");
        SelectOptionList slLoaiChungTu = new SelectOptionList(dtLoaiChungTu, "loaiChungTu", "TenChungTu");
        dtLoaiChungTu.Dispose();

        var SoChungTu = "PCBQP - " + R["SoChungTu"].ToString();
        var LoaiCT = R["LoaiCT"].ToString();
        var SoQD = R["SoQD"].ToString();
        var NgayQD = R["NgayQD"].ToString();
        var NoiDung = R["NoiDung"].ToString();

        using (Html.BeginForm("EditSubmit", "ChungTuCap_ChungTu", new { ParentID = ParentID, Id_CTCap = Id_CTCap}))
        {
    %>    
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td><span>                        
                        <%=NgonNgu.LayXau("Sửa thông tin chứng từ")%>                       
                    </span></td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <div style="width: 80%; float: left;">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td class="td_form2_td1" style="width: 15%;">
                                <div><b>Số chứng từ</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <b><%=SoChungTu%></b>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Loại chứng từ</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <b><%=LoaiCT%></b>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Quyết định số</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.TextArea(ParentID, SoQD, "SoQD", "", "class=\"input1_2\" style=\"width: 50%;height: 22px;\"")%></div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Ngày quyết định</b></div>
                            </td>
                            <td class="td_form2_td5"> 
                                <div style="width:50%">
                                    <%=MyHtmlHelper.DatePicker(ParentID, NgayQD, "NgayQD", "", "class=\"input1_2\" onblur=isDate(this);")%><br />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Nội dung</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.TextArea(ParentID, NoiDung, "NoiDung", "", "class=\"input1_2\" style=\"width: 50%;height: 100px;\"")%></div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1"></td>
                            <td class="td_form2_td5">
                                <div>
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td align="right" class="td_form2_td1">
                                                <input type="submit" class="button" id="Submit1" value="Lưu" />
                                            </td>
                                            <td>&nbsp;</td>
                                            <td class="td_form2_td1">
                                                <input class="button" type="button" value="Hủy" onclick="history.go(-1)" />
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
        }
    %>
</asp:Content>