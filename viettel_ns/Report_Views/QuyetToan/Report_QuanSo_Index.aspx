<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
    String sLoaiBaoCao = Request.QueryString["sLoai"];    
%>
<div class="custom_css box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	<tr>
            	<td>
                	<span>Danh sách báo cáo</span>
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
                    <td align="center" style="padding: 3px 2px;">FB</td>
                    <td align="center" style="padding: 3px 2px;">FB02/B02</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_THQS_TungDonVi_1"), "Báo cáo quân số quyết toán(tháng/quý)")%></td>                        
                    <td></td>
                </tr> 
                          
                  
                    <tr>
                    <td align="center" style="padding: 3px 2px;">1</td>
                    <td align="center" style="padding: 3px 2px;">QS01</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_TongHop"), "Tổng hợp quân số quyết toán")%></td>                        
                    <td></td>
                </tr>  
                <tr>
                    <td align="center" style="padding: 3px 2px;">2</td>
                    <td align="center" style="padding: 3px 2px;">QS02</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_ThuongXuyen"), "Bảng thông kê quân số quyết toán thường xuyên")%></td>                        
                    <td></td>
                </tr> 
                  <tr>
                    <td align="center" style="padding: 3px 2px;">3</td>
                    <td align="center" style="padding: 3px 2px;">QS03</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_Thang"), "Tổng hợp quân số quyết toán bình quân")%></td>                        
                    <td></td>
                </tr>  
                  <tr>
                    <td align="center" style="padding: 3px 2px;">4</td>
                    <td align="center" style="padding: 3px 2px;">QS04</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_THQS_TungDonVi_Nam"), "Báo cáo tình hình thực hiện quân số")%></td>                        
                    <td></td>
                </tr>
                  <tr>
                    <td align="center" style="padding: 3px 2px;">5</td>
                    <td align="center" style="padding: 3px 2px;">QS05</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_LienTham"), "Tổng hợp liên thẩm quân số quyết toán")%></td>                        
                    <td></td>
                </tr>
                 <tr>
                    <td align="center" style="padding: 3px 2px;">6</td>
                    <td align="center" style="padding: 3px 2px;">QS06</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_TangGiam"), "Tổng hợp quân số tăng giảm")%></td>                        
                    <td></td>
                </tr>
                
                 <tr>
                    <td align="center" style="padding: 3px 2px;">7</td>
                    <td align="center" style="padding: 3px 2px;">QS07</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_TangGiam_TongHop"), "Tổng hợp quân số tăng giảm theo đơn vị")%></td>                        
                    <td></td>
                </tr>   
                      <tr>
                    <td align="center" style="padding: 3px 2px;">8</td>
                    <td align="center" style="padding: 3px 2px;">QS08</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_RaQuan"), "Tổng hợp quân số ra quân")%></td>                        
                    <td></td>
                </tr> 
                 <tr>
                    <td align="center" style="padding: 3px 2px;">9</td>
                    <td align="center" style="padding: 3px 2px;">QS09</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_THQS_TungDonVi"), "Tổng hợp tình hình quân số")%></td>                        
                    <td></td>
                </tr>     
                     <tr>
                    <td align="center" style="padding: 3px 2px;">10</td>
                    <td align="center" style="padding: 3px 2px;">QS10</td>
                    <td style="padding: 3px 2px;"><%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptQTQS_THQS_TongHop"), "Tổng hợp tình hình quân số theo đơn vị")%></td>                        
                    <td></td>
                </tr>      
            </table>
        </div>
    </div>
</div>
</asp:Content>