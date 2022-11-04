<%@ Page Title="" Language="C#"  Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="VIETTEL.Models" %>

<!DOCTYPE html>
<script src="/Scripts/js/jquery-3.1.0.min.js" />
    <script src="/Scripts/js/jquery-ui.min.js" />
    <script src="/Scripts/js/jquery.unobtrusive-ajax.min.js"/>
    <script src="/Scripts/js/bootstrap.js"/>
    <script src="/Scripts/js/jquery.validate.min.js"/>
    <script src="/Scripts/js/jquery.validate.unobtrusive.min.js"/>
    <script src="/content/plugins/sweetalert2/sweetalert2.min.js"/>
    <script src="/Scripts/js/js_ui.js"/>
    <script src="/Scripts/js/js_app.js"/>
    <script src="/Scripts/js/js_layout.js"/>
    <script src="/Scripts/js/js_TableHelper.js"/>
    <script src="/Scripts/plugins/datapicker/bootstrap-datepicker.js"/>
    <script src="/Scripts/plugins/datapicker/bootstrap-datepicker.vi.js"/>
    <script src="/Scripts/js/select2.min.js"/>
    <script src="/Scripts/js/js_editor.js"/>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#dvText').show();
            $('body').append('<div id="fade"></div>'); //Add the fade layer to bottom of the body tag.
            $('#fade').css({ 'filter': 'alpha(opacity=40)' }).fadeIn(); //Fade in the fade layer 
            $(window).resize(function () {
                $('.popup_block').css({
                    position: 'absolute',
                    left: ($(window).width() - $('.popup_block').outerWidth()) / 2,
                    top: ($(window).height() - $('.popup_block').outerHeight()) / 2
                });
            });
            // To initially run the function:
            $(window).resize();
            //Fade in Background
        });
    </script>
    <center>
        <div id="dvText" class="popup_block">
            <img src="/Content/ajax-loader.gif" alt="" /><br />
	        <p>Hệ thống đang thực hiện đăng nhập...</p>
        </div>
    </center>

    <%
        String ReturnURL = Convert.ToString(Request.QueryString["ReturnURL"]);
        String strURL = "";
        if (ReturnURL != "" && ReturnURL != null)
        {
           strURL = ReturnURL.Replace("%26","&");
        }
    %>

	<script type="text/javascript">
        $(function () {
            $.get('<%=HamRiengModels.SSODomain%>/user/RequestToken?callback=?', {},
                function (ssodata) {
                    // get url to logon page in case this operation fails
                    var logonPage = '<%=Url.Action("LogOn", "Account", new { ReturnUrl = strURL}) %>';
				    if (ssodata.Status == 'SUCCESS') {
				        // get target url for successful authentication
				        var redirect = '<%=strURL%>';
				        if (redirect == '')
				            redirect = '<%=Url.Action("Index", "Home") %>';

				        // verify the token is genuine
				        $.post('<%=Url.Action("Authenticate", "Account", new { ReturnUrl = strURL}) %>',
                            { token: ssodata.Token, createPersistentCookie: false },
                            function (data) {
                                // redirect user based on result
                                if (data.result == 'SUCCESS')
                                    document.location = redirect;
                                else
                                    document.location = logonPage;
                                // just regular json here
                            }, 'json');
                    } else {
                        // user needs to logon to SSO service
                        document.location = logonPage;
                    }
                    // tell jQuery to use JSONP 
                }, 'jsonp');
        });

    </script>

