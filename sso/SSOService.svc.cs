using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using SSOModels;
using DomainModel;
using DomainModel.Abstract;

namespace Developmentalmadness
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SSOService : ISSOService, ISSOPartnerService
    {
        #region ISSOService Members

        public IMembershipService MembershipService { get; set; }

        public SSOService()
        {
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            Connection.ConnectionString = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        }

        /// <summary>
        /// Returns the client's current identity token so it can
        /// assert its identity to a partner application
        /// </summary>
        /// <returns>encrypted identity token</returns>
        public SSOToken RequestToken()
        {
            // default response
            SSOToken token = new SSOToken
            {
                Token = string.Empty,
                Status = "DENIED",
            };

            // verify we've already authenticated the client
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                // get the current identity
                FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;

                // we'll send the client its own FormsAuthenticationTicket, but 
                // we'll encrypt it so only we can read it when the partner app
                // needs to validate who the client is through our service
                token.Token = FormsAuthentication.Encrypt(identity.Ticket);
                token.Status = "SUCCESS";
            }

            return token;
        }

        /// <summary>
        /// Log client out of SSO service
        /// </summary>
        /// <returns>true if successful</returns>
        public bool Logout()
        {
            HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddDays(-10000.0);
            HttpContext.Current.Response.Cookies.Add(cookie);
            return true;
        }

        /// <summary>
        /// Log client into SSO service
        /// </summary>
        /// <returns></returns>
        public SSOToken Login(string username, string password)
        {
            // default response
            SSOToken token = new SSOToken
            {
                Token = string.Empty,
                Status = "DENIED",
            };

            // authenticate user
            if (MembershipService.ValidateUser(username, password))
            {
                // mock data to simulate passing around additional data
                Guid temp = Guid.NewGuid();

                // manage cookie lifetime
                DateTime issueDate = DateTime.Now;
                DateTime expireDate = issueDate.AddMinutes(480);


                // create the ticket and protect it
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username, issueDate, expireDate, true, temp.ToString());
                string protectedTicket = FormsAuthentication.Encrypt(ticket);

                // save the protected ticket with a cookie
                HttpCookie authorizationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, protectedTicket);
                authorizationCookie.Expires = expireDate;

                // protect the cookie from session hijacking
                authorizationCookie.HttpOnly = true;

                // write the cookie to the response stream
                HttpContext.Current.Response.Cookies.Add(authorizationCookie);

                // update the response to indicate success
                token.Status = "SUCCESS";
                token.Token = protectedTicket;
            }

            return token;
        }

        /// <summary>
        /// Confirm the username is valid
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUserInfo(string username)
        {
            UserInfo vR = new UserInfo { Username = String.Empty, Fullname = String.Empty, Email = String.Empty, Password = String.Empty };

            if (String.IsNullOrEmpty(username)) username = "";

            SqlCommand cmd = new SqlCommand("SELECT * FROM aspnet_Users WHERE UserName=@UserName");
            cmd.Parameters.AddWithValue("@UserName", username);
            DataTable dt = Connection.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                cmd = new SqlCommand("SELECT * FROM aspnet_Membership WHERE UserId=@UserId");
                cmd.Parameters.AddWithValue("@UserId", dt.Rows[0]["UserId"]);
                DataTable dt1 = Connection.GetDataTable(cmd);
                if (dt1.Rows.Count > 0)
                {
                    vR.Username = Convert.ToString(dt.Rows[0]["LoweredUserName"]);
                    vR.Fullname = Convert.ToString(dt1.Rows[0]["FullName"]);
                    vR.Email = Convert.ToString(dt1.Rows[0]["Email"]);
                }
                dt1.Dispose();
            }
            dt.Dispose();
            return vR;
        }

        /// <summary>
        /// Confirms the client is signed into SSO service
        /// </summary>
        /// <param name="token">the client's asserted identity token</param>
        /// <returns>user's identity information</returns>
        public SSOUser ValidateToken(string token)
        {
            try
            {
                // if we can decrypt the ticket then it wasn't tampered with
                // and token is valid
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(token);

                return new SSOUser
                {
                    Username = ticket.Name,
                    SessionToken = new Guid(ticket.UserData)
                };
            }
            catch
            {
                // validation failed
                return new SSOUser
                {
                    Username = string.Empty,
                    SessionToken = Guid.Empty
                };
            }
        }

        public UserInfo ResetPassword(String username)
        {
            UserInfo vR = new UserInfo { Username = String.Empty, Fullname = String.Empty, Email = String.Empty, Password = String.Empty };

            if (String.IsNullOrEmpty(username)) username = "";

            SqlCommand cmd = new SqlCommand("SELECT * FROM aspnet_Users WHERE UserName=@UserName");
            cmd.Parameters.AddWithValue("@UserName", username);
            DataTable dt = Connection.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                cmd = new SqlCommand("SELECT * FROM aspnet_Membership WHERE UserId=@UserId");
                cmd.Parameters.AddWithValue("@UserId", dt.Rows[0]["UserId"]);
                DataTable dt1 = Connection.GetDataTable(cmd);
                if (dt1.Rows.Count > 0)
                {
                    vR.Username = Convert.ToString(dt.Rows[0]["LoweredUserName"]);
                    vR.Fullname = Convert.ToString(dt1.Rows[0]["FullName"]);
                    vR.Email = Convert.ToString(dt1.Rows[0]["Email"]);

                    Bang bang = new Bang("QT_QuenMatKhau");
                    bang.CmdParams.Parameters.AddWithValue("@sMaNguoiDung", username);
                    bang.Save();

                    //Gửi mail
                    String Email = vR.Email;
                    String MailFrom = "test.vicem@gmail.com";
                    String PassMailFrom = "abc.123456";

                    String sURL = String.Format("http://noibo.vicem.vn/Account/ConfirmResetPassword?TaiKhoan={0}&id={1}", username, bang.GiaTriKhoa);
                    String MailServer = "smtp.gmail.com";
                    String CC = null;
                    String BCC = null;
                    String Subject = "Xác nhận thiết lập lại mật khẩu " + "(" + DateTime.Now.ToString("HH:mm dd/MM/yyyy") + ")";
                    String Body = "Ban quản trị Vicem thông báo: Bạn hãy kích vào "
                                 + "<b>"
                                 + String.Format("<a href=\"{0}\">Xác nhận</a>", sURL)
                                 + "</b> để thiết lập lại mật khẩu cho tài khoản "
                                 + "<span style=\"background: yellow;\"><b>"
                                 + username
                                 + "</b></font>";
                    String[] Attach = new String[0];
                    HamRieng.SendMail(MailFrom, PassMailFrom, MailServer, Email, CC, BCC, Subject, Body, Attach);
                }
                dt1.Dispose();
            }
            dt.Dispose();
            return vR;
        }

        public UserInfo ConfirmResetPassword(String TaiKhoan, String id)
        {
            UserInfo vR = new UserInfo { Username = String.Empty, Fullname = String.Empty, Email = String.Empty, Password = String.Empty };

            SqlCommand cmd;
            cmd = new SqlCommand("SELECT Count(*) FROM QT_QuenMatKhau WHERE sMaNguoiDung=@sMaNguoiDung AND iID_MaQuenMatKhau=@iID_MaQuenMatKhau");
            cmd.Parameters.AddWithValue("@sMaNguoiDung", TaiKhoan);
            cmd.Parameters.AddWithValue("@iID_MaQuenMatKhau", id);
            Boolean XacNhan = (Convert.ToInt16(Connection.GetValue(cmd, 0)) > 0);
            cmd.Dispose();
            if (XacNhan)
            {
                MembershipUser user = Membership.GetUser(TaiKhoan);
                if (user.IsLockedOut)
                {
                    user.UnlockUser();
                }
                if (user.IsLockedOut == false)
                {
                    String Email = user.Email;
                    String MailFrom = "test.vicem@gmail.com";
                    String PassMailFrom = "abc.123456";

                    String NewPassword = user.ResetPassword();
                    String MailServer = "smtp.gmail.com";
                    String CC = null;
                    String BCC = null;
                    String Subject = NgonNgu.LayXau("Thiết lập lại mật khẩu ") + "(" + DateTime.Now.ToString("HH:mm dd/MM/yyyy") + ")";
                    String Body = NgonNgu.LayXau("Ban quản trị Vicem gửi đến bạn mật khẩu mới: ") + "<span style=\"background: yellow;\"><b>" + NewPassword + "</b></font>";
                    String[] Attach = new String[0];
                    HamRieng.SendMail(MailFrom, PassMailFrom, MailServer, Email, CC, BCC, Subject, Body, Attach);

                    vR.Username = TaiKhoan;
                    vR.Email = user.Email;
                    vR.Password = NewPassword;
                }
            }
            return vR;
        }

        public SSORegister Register(String username, String password, String email, String fullname)
        {
            if (String.IsNullOrEmpty(username) == false && String.IsNullOrEmpty(password) == false)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(username, password, email, fullname);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return new SSORegister { Error = "", Status = "SUCCESS" };
                }
            }
            return new SSORegister { Error = "Đăng ký không thành công.", Status = "UNSUCCESS" };
        }

        public SSORegister ChangePassword(String oldpassword, String newpassword)
        {
            if (String.IsNullOrEmpty(oldpassword) == false && String.IsNullOrEmpty(newpassword) == false && HttpContext.Current.Request.IsAuthenticated)
            {
                // get the current identity
                FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;

                String username = identity.Name;
                if (MembershipService.ChangePassword(username, oldpassword, newpassword))
                {
                    return new SSORegister { Error = "", Status = "SUCCESS" };
                }
            }
            return new SSORegister { Error = "Đổi mật khẩu không thành công.", Status = "UNSUCCESS" };
        }


        //Sau khi hệ thống đi vào sử dụng phải hủy chức năng này
        public SSORegister ChangeUserInfo(String username, String email, String fullname)
        {
            if (String.IsNullOrEmpty(username)) username = "";

            SqlCommand cmd = new SqlCommand("SELECT * FROM aspnet_Users WHERE UserName=@UserName");
            cmd.Parameters.AddWithValue("@UserName", username);
            DataTable dt = Connection.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                cmd = new SqlCommand("SELECT * FROM aspnet_Membership WHERE UserId=@UserId");
                cmd.Parameters.AddWithValue("@UserId", dt.Rows[0]["UserId"]);
                DataTable dt1 = Connection.GetDataTable(cmd);
                if (dt1.Rows.Count > 0)
                {

                    if (String.IsNullOrEmpty(email)) email = Convert.ToString(dt.Rows[0]["Email"]);
                    if (String.IsNullOrEmpty(fullname)) email = Convert.ToString(dt.Rows[0]["FullName"]);

                    cmd = new SqlCommand("UPDATE aspnet_Membership SET Email=@Email, LoweredEmail=@LoweredEmail, FullName=@FullName WHERE UserId=@UserId");
                    cmd.Parameters.AddWithValue("@UserId", dt.Rows[0]["UserId"]);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@LoweredEmail", email.ToLower());
                    cmd.Parameters.AddWithValue("@FullName", fullname);
                    Connection.UpdateDatabase(cmd);
                }
                dt1.Dispose();
            }
            dt.Dispose();
            return new SSORegister { Error = "Sửa thông tin thành công.", Status = "SUCCESS" };
        }
        #endregion
    }
}
