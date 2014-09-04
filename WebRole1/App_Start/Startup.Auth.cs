using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Facebook;
using System.Collections.Generic;
using Owin.Security.Providers.Yahoo;
using System;

namespace WebRole1
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //ExpireTimeSpan = TimeSpan.FromSeconds(5),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseMicrosoftAccountAuthentication(
                clientId: "0000000044126351",
                clientSecret: "jAZpCRiJKg5gQRxFxvYBtpy5rdopTlnW");

            app.UseYahooAuthentication(
  consumerKey: "dj0yJmk9T2NDVnZ0cU0yUnVSJmQ9WVdrOWN6aFNUV3BXTTJNbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD01Mw--",
  consumerSecret: "197e780574ff379dfb3d39de15b8c63b3372609e");

            //var FbFriends = new FacebookAuthenticationOptions();

            //FbFriends.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(
               appId: "1526133034266985",
               appSecret: "1bbb93665abf17446745e9a1f1e5faa3");
            app.UseGoogleAuthentication();

        }
    }
}