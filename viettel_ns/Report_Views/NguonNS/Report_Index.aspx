<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	<tr>
            	<td>
                	<span>Danh sách báo cáo nguồn ngân sách</span>
                </td>
            </tr>
        </table>
    </div>
    <div id="nhapform">
        <div id="form2">
            <table class="mGrid">
                <tr>
                    <th style="width: 3%;">STT</th>
                    <th style="width: 15%;">Mã báo cáo - phụ lục</th>                        
                    <th style="width: 60%;">Tên báo cáo</th>
                    <th style="width: 22%;">Ghi chú</th>
                </tr>                
                <tr>
                    <td align="center" style="padding: 3px 2px;" rowspan="2"><b>1</b></td>
                    <td align="center" style="padding: 3px 2px;" rowspan="2"><b>THNGUONBTC</b></td>
                    <td style="padding: 3px 2px;" rowspan="2"><b>Tổng hợp nguồn ngân sách được Bộ Tài chính cấp</b></td>                        
                    <td> <%=MyHtmlHelper.ActionLink(Url.Action("ViewPDF", "rptNguonNS_THNguonBTC"), "Xem báo cáo")%></td>
                </tr>
                <tr>                    
                    <td> <%=MyHtmlHelper.ActionLink(Url.Action("DownloadExcel", "rptNguonNS_THNguonBTC"), "Xuất file excel")%></td>
                </tr>
                <tr>
                    <td align="center" style="padding: 3px 2px;" rowspan="2"><b>2</b></td>
                    <td align="center" style="padding: 3px 2px;" rowspan="2"><b>THCHIBQP</b></td>
                    <td style="padding: 3px 2px;" rowspan="2"><b>Chi tiết các khoản chi Bộ Quốc phòng</b></td>                        
                    <td> <%=MyHtmlHelper.ActionLink(Url.Action("ViewPDF", "rptNguonNS_THChiBQP"), "Xem báo cáo")%></td>
                </tr>
                <tr>                    
                    <td> <%=MyHtmlHelper.ActionLink(Url.Action("DownloadExcel", "rptNguonNS_THChiBQP"), "Xuất file excel")%></td>
                </tr>                
            </table>
        </div>
    </div>
</div>
</asp:Content>

