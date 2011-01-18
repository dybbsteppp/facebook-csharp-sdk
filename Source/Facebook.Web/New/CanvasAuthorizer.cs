namespace Facebook.Web.New
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Web;

    /// <summary>
    /// Facebook canvas authorizer.
    /// </summary>
    public class CanvasAuthorizer : Authorizer
    {
        /// <summary>
        /// Facebook canvas settings
        /// </summary>
        private readonly ICanvasSettings canvasSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasAuthorizer"/> class.
        /// </summary>
        /// <param name="facebookSettings">
        /// The facebook settings.
        /// </param>
        /// <param name="canvasSettings">
        /// The canvas settings.
        /// </param>
        /// <param name="httpContext">
        /// The http context.
        /// </param>
        public CanvasAuthorizer(IFacebookSettings facebookSettings, ICanvasSettings canvasSettings, HttpContextBase httpContext)
            : base(facebookSettings, httpContext)
        {
            Contract.Requires(facebookSettings != null);
            Contract.Requires(!string.IsNullOrEmpty(facebookSettings.AppId));
            Contract.Requires(!string.IsNullOrEmpty(facebookSettings.AppSecret));
            Contract.Requires(httpContext != null);
            Contract.Requires(httpContext.Request != null);
            Contract.Requires(httpContext.Request.Params != null);
            Contract.Requires(httpContext.Response != null);
            Contract.Requires(canvasSettings != null);

            this.canvasSettings = canvasSettings;
        }

        /// <summary>
        /// Gets the Facebook canvas settings.
        /// </summary>
        public ICanvasSettings CanvasSettings
        {
            get
            {
                Contract.Ensures(Contract.Result<ICanvasSettings>() != null);
                return this.canvasSettings;
            }
        }

        /// <summary>
        /// Handle unauthorized requests.
        /// </summary>
        public override void HandleUnauthorizedRequest()
        {
            this.HttpResponse.ContentType = "text/html";
            this.HttpResponse.Write(CanvasUrlBuilder.GetCanvasRedirectHtml(this.GetLoginUrl(null)));
        }

        public Uri GetLoginUrl(IDictionary<string, object> parameters)
        {
            var defaultParameters = new Dictionary<string, object>();
            defaultParameters["display"] = this.LoginDisplayMode;

            if (!string.IsNullOrEmpty(this.Perms))
            {
                defaultParameters["scope"] = this.Perms;
            }

            var urlBuilder = new CanvasUrlBuilder(this.canvasSettings, this.HttpRequest);
            return urlBuilder.GetLoginUrl(this.FacebookSettings, this.ReturnUrlPath, this.State, defaultParameters);
        }

        [ContractInvariantMethod]
        private void InvarientObject()
        {
            Contract.Invariant(this.canvasSettings != null);
        }
    }
}