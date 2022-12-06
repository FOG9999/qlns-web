using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Ajax.Samples;

namespace Developmentalmadness
{
	// NOTE: If you change the interface name "ISSOService" here, you must also update the reference to "ISSOService" in Web.config.
	[ServiceContract]
	public interface ISSOService
	{
		[OperationContract]
		[WebGet(UriTemplate="/RequestToken",
			BodyStyle=WebMessageBodyStyle.WrappedRequest,
			ResponseFormat=WebMessageFormat.Json)]
		[JSONPBehavior(callback = "callback")]
		SSOToken RequestToken();

		[OperationContract]
		// javascript can't post w/ jsonp - has to be WebGet
		[WebGet(UriTemplate="/Login?username={username}&password={password}",
			BodyStyle = WebMessageBodyStyle.WrappedResponse,
			ResponseFormat = WebMessageFormat.Json)]
		[JSONPBehavior(callback = "callback")]
		SSOToken Login(string username, string password);

        [OperationContract]
        // javascript can't post w/ jsonp - has to be WebGet
        [WebGet(UriTemplate = "/GetUserInfo?username={username}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "callback")]
        UserInfo GetUserInfo(string username);


        [OperationContract]
        // javascript can't post w/ jsonp - has to be WebGet
        [WebGet(UriTemplate = "/ResetPassword?username={username}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "callback")]
        UserInfo ResetPassword(string username);

        [OperationContract]
        // javascript can't post w/ jsonp - has to be WebGet
        [WebGet(UriTemplate = "/ConfirmResetPassword?TaiKhoan={TaiKhoan}&id={id}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "callback")]
        UserInfo ConfirmResetPassword(string TaiKhoan, string id);

        [OperationContract]
        // javascript can't post w/ jsonp - has to be WebGet
        [WebGet(UriTemplate = "/Register?username={username}&password={password}&email={email}&fullname={fullname}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "callback")]
        SSORegister Register(string username, string password, string email, string fullname);

        [OperationContract]
        // javascript can't post w/ jsonp - has to be WebGet
        [WebGet(UriTemplate = "/ChangeUserInfo?username={username}&email={email}&fullname={fullname}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "callback")]
        SSORegister ChangeUserInfo(string username, string email, string fullname);

        [OperationContract]
        // javascript can't post w/ jsonp - has to be WebGet
        [WebGet(UriTemplate = "/ChangePassword?oldpassword={oldpassword}&newpassword={newpassword}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            ResponseFormat = WebMessageFormat.Json)]
        [JSONPBehavior(callback = "callback")]
        SSORegister ChangePassword(String oldpassword, String newpassword);

		[OperationContract]
		[WebGet(UriTemplate = "/Logout",
			BodyStyle = WebMessageBodyStyle.WrappedResponse,
			ResponseFormat = WebMessageFormat.Json)]
		[JSONPBehavior(callback = "callback")]
		bool Logout();
	}

	[ServiceContract]
	public interface ISSOPartnerService
	{
		[OperationContract]
		SSOUser ValidateToken(string token);

	}

	[DataContract]
	public class SSOToken
	{
		[DataMember]
		public string Token { get; set; }

		[DataMember]
		public string Status { get; set; }
	}

	[DataContract]
	public class SSOUser
	{
		[DataMember]
		public string Username { get; set; }

		[DataMember]
		public Guid SessionToken { get; set; }
	}

    [DataContract]
    public class SSORegister
    {
        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public string Status { get; set; }
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Fullname { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
