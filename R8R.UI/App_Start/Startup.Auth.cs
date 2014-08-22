using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Facebook;
using System.Collections.Generic;
using Owin.Security.Providers.Yahoo;
using System;

namespace R8R.UI
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
           // List<string> Social = new List<string>() { "email", "friends_about_me", "friends_photos" };

            //var FbFriends = new FacebookAuthenticationOptions();

            //FbFriends.Scope.Add("email");
            //FbFriends.Scope.Add("friends_about_me");
            //FbFriends.Scope.Add("friends_photos");
            //FbFriends.AppId = "1526133034266985";
            //FbFriends.AppSecret = "1bbb93665abf17446745e9a1f1e5faa3";

            //FbFriends.Provider = new FacebookAuthenticationProvider()
            //{
            //    OnAuthenticated = async FbContext =>
            //    {

            //        FbContext.Identity.AddClaim(
            //        new System.Security.Claims.Claim("FacebookAccessToken", FbContext.AccessToken));
            //    }
            //};

            //FbFriends.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            //app.UseFacebookAuthentication(FbFriends);

            //// Uncomment the following lines to enable logging in with third party login providers
            
            app.UseMicrosoftAccountAuthentication(
                clientId: "0000000044126351",
                clientSecret: "jAZpCRiJKg5gQRxFxvYBtpy5rdopTlnW");
            
          //  app.UseYahooAuthentication("dj0yJmk9ZHIxSXk1Zm1vVmFyJmQ9WVdrOVRXeEdNekZ6TjJFbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD05Zg", "28209691b1a787c3ab4f5569aa7aed584cd01015");
            app.UseYahooAuthentication(
  consumerKey: "dj0yJmk9ZkdoUTlYWnZGZnE2JmQ9WVdrOWFFMUJVVFJ1TlRBbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD05Nw--",
  consumerSecret: "ec02e56ede5fe2a6e853cce7df12bd8ce508fd86");  

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");
            var FbFriends = new FacebookAuthenticationOptions();
            FbFriends.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(
               appId: "1526133034266985",
               appSecret: "1bbb93665abf17446745e9a1f1e5faa3");

            app.UseGoogleAuthentication();

            //var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
            //{
            //    ClientId = "MYCLIENTID",
            //    ClientSecret = "MYSECRET",
            //};
            //app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);
           
        }
    }
}