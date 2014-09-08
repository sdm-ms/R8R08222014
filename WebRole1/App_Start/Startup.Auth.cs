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

            //app.UseMicrosoftAccountAuthentication(
            //   clientId: "0000000048128D31",
            //   clientSecret: "vUmQPTbhTIctdtAhjWN-kDWodsNuJcNo");

            app.UseYahooAuthentication(
  consumerKey: "dj0yJmk9N3Q1SkZVUjZJY29LJmQ9WVdrOVRXVldTR3BTTm5NbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD04Zg--",
  consumerSecret: "ff86ea149040af9b0c1052b8e9d3d0ceabd0faf7");

 //           app.UseYahooAuthentication(
 //consumerKey: "dj0yJmk9OEp2RlhPSkVMOE80JmQ9WVdrOVR6WjNZVTFJTXpBbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD04NA--",
 //consumerSecret: "f317e16a47112d7ac2831b0c43cac572e1e7916f");
            //var FbFriends = new FacebookAuthenticationOptions();

            //FbFriends.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(
               appId: "1526133034266985",
               appSecret: "1bbb93665abf17446745e9a1f1e5faa3");
            //app.UseFacebookAuthentication(
            //  appId: "271092676424493",
            //  appSecret: "6257d28e03a066d87fa527774d854b87");
            app.UseGoogleAuthentication();

        }
    }
}