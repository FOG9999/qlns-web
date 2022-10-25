<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<%
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    Boolean CoTabIndex = (props["CoTabIndex"] == null) ? false : Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    String ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    String ParentID = ControlID + "_Search";
    String iID_MaChungTu = Request.QueryString["iID_MaChungTu"];

    String iLoai = Request.QueryString["iLoai"];    
    
%>

<div class="box_tong">
    <div id="nhapform">
        <div id="form2">
        
<form action="<%=Url.Action("SearchSubmit","CapPhat_ChungTuChiTiet",new {ParentID = ParentID, iID_MaChungTu = iID_MaChungTu})%>" method="post">

    <table class="mGrid1">
               
            </table>
             <iframe id="ifrChiTietChungTu" width="100%" height="530px" src="<%= Url.Action("ThuNopChiTiet_Frame", "ThuNop_ChungTuChiTiet", new {iID_MaChungTu=iID_MaChungTu,iLoai=iLoai})%>">
            </iframe>
</form>
   
        </div>
    </div>
</div>