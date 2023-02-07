using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Apeek.Common.Extensions;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
namespace Apeek.Web.Framework.Auth.AuthClients
{
    public class FacebookClient : OAuth2Client, IExternalAuthClient
    {
        #region Constants and Fields
        /// <summary>
        /// The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://www.facebook.com/v2.0/dialog/oauth";
        /// <summary>
        /// The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://graph.facebook.com/v2.0/oauth/access_token";
        /// <summary>
        /// The _app id.
        /// </summary>
        private readonly string appId;
        /// <summary>
        /// The _app secret.
        /// </summary>
        private readonly string appSecret;
        private readonly string[] fields = {"email","location","first_name","last_name"};
        #endregion
        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookClient"/> class.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public FacebookClient(string appId, string appSecret)
            : base(AuthClient.Facebook)
        {
            this.appId = appId;
            this.appSecret = appSecret;
        }
        #endregion
        #region Methods
        /// <summary>
        /// The get service login url.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>An absolute URI.</returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var builder = new UriBuilder(AuthorizationEndpoint);
            builder.AppendQueryArgument("client_id",this.appId);
            builder.AppendQueryArgument("redirect_uri",returnUrl.AbsoluteUri);
            builder.AppendQueryArgument("scope", "email,user_website,user_location");
            //builder.AppendQueryArgument("scope", "email,user_likes,friends_likes,user_birthday,publish_checkins,publish_stream");
            return builder.Uri;
        }
        /// <summary>
        /// The get user data.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <returns>A dictionary of profile data.</returns>
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            FacebookData graphData;
            var request = CreateRequest(accessToken);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    graphData = JsonHelper.Deserialize<FacebookData>(responseStream);
                }
            }
            // this dictionary must contains 
            var userData = new Dictionary<string, string>();
            userData.AddItemIfNotEmpty("id", graphData.Id);
            userData.AddItemIfNotEmpty("username", graphData.Email);
            userData.AddItemIfNotEmpty("name", graphData.Name);
            userData.AddItemIfNotEmpty("link", graphData.Link == null ? null : graphData.Link.AbsoluteUri);
            userData.AddItemIfNotEmpty("gender", graphData.Gender);
            userData.AddItemIfNotEmpty("birthday", graphData.Birthday);
            userData.AddItemIfNotEmpty("location", graphData.Locatoin == null ? null : graphData.Locatoin.OriginalName);
            userData.AddItemIfNotEmpty("website", graphData.Website);
            return userData;
        }
        private WebRequest CreateRequest(string accessToken)
        {
            return WebRequest.Create(String.Format("https://graph.facebook.com/me?fields={0}&locale=ru_RU&access_token={1}", String.Join(",", fields), EscapeUriDataStringRfc3986(accessToken)));
        }
        public bool UserExists(string userId, string accessToken)
        {
            FacebookData graphData;
            var request = CreateRequest(accessToken);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    graphData = JsonHelper.Deserialize<FacebookData>(responseStream);
                }
            }
            return string.Compare(graphData.Id, userId) == 0;
        }
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };
        internal static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));
            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }
            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }
        /// <summary>
        /// Obtains an access token given an authorization code and callback URL.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="authorizationCode">
        /// The authorization code.
        /// </param>
        /// <returns>
        /// The access token.
        /// </returns>
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            WebClient client = new WebClient();
            var builder = new StringBuilder(TokenEndpoint);
            builder.Append("?client_id=");
            builder.Append(appId);
            builder.Append("&client_secret=");
            builder.Append(appSecret);
            builder.Append("&redirect_uri=");
            builder.Append(HttpUtility.UrlEncode(returnUrl.ToString()));
            builder.Append("&code=");
            builder.Append(authorizationCode);
            string content = client.DownloadString(builder.ToString());
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(content);
            {
                string result = nameValueCollection["access_token"];
                return result;
            }
        }
        /// <summary>
        /// Converts any % encoded values in the URL to uppercase.
        /// </summary>
        /// <param name="url">The URL string to normalize</param>
        /// <returns>The normalized url</returns>
        /// <example>NormalizeHexEncoding("Login.aspx?ReturnUrl=%2fAccount%2fManage.aspx") returns "Login.aspx?ReturnUrl=%2FAccount%2FManage.aspx"</example>
        /// <remarks>
        /// There is an issue in Facebook whereby it will rejects the redirect_uri value if
        /// the url contains lowercase % encoded values.
        /// </remarks>
        private static string NormalizeHexEncoding(string url)
        {
            var chars = url.ToCharArray();
            for (int i = 0; i < chars.Length - 2; i++)
            {
                if (chars[i] == '%')
                {
                    chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
                    chars[i + 2] = char.ToUpperInvariant(chars[i + 2]);
                    i += 2;
                }
            }
            return new string(chars);
        }
        #endregion
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataContract]
    public class FacebookData
    {
        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }
        [DataMember(Name = "email")]
        public string Email { get; set; }
        [DataMember(Name = "gender")]
        public string Gender { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "link")]
        public Uri Link { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "website")]
        public string Website { get; set; }
        [DataMember(Name = "location")]
        public FacebookDataLocation Locatoin { get; set; }
    }
    [DataContract]
    public class FacebookDataLocation
    {
        [DataMember(Name = "name")]
        public string OriginalName { get; set; }
    }
}
