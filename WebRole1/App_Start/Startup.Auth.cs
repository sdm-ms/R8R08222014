using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Facebook;
using System.Collections.Generic;
using Owin.Security.Providers.Yahoo;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Google;

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
                LoginPath = new PathString("/AccountLogin")
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //app.UseGoogleAuthentication(clientId: "1072934350544-dl5fr7svaedmn33tms38a89kppbvdse4.apps.googleusercontent.com", clientSecret: "didI13YDMmcMhsGmOJgIK9W_");

            app.UseMicrosoftAccountAuthentication(
                clientId: "0000000044126351",
                clientSecret: "jAZpCRiJKg5gQRxFxvYBtpy5rdopTlnW");

            //app.UseMicrosoftAccountAuthentication(
            //   clientId: "0000000048128D31",
            //   clientSecret: "vUmQPTbhTIctdtAhjWN-kDWodsNuJcNo");

  //          app.UseYahooAuthentication(
  //consumerKey: "dj0yJmk9N3Q1SkZVUjZJY29LJmQ9WVdrOVRXVldTR3BTTm5NbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD04Zg--",
  //consumerSecret: "f317e16a47112d7ac2831b0c43cac572e1e7916f");
            
 //           app.UseYahooAuthentication(
 //consumerKey: "dj0yJmk9cHd5M3pHeGJsWGJpJmQ9WVdrOVMyeE9VWEJrTjJFbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD1lNA--",
 //consumerSecret: "0613308963b76ffee39299c08894d9f5aff427b5");
            //var FbFriends = new FacebookAuthenticationOptions();

            //FbFriends.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(
               appId: "1526133034266985",
               appSecret: "1bbb93665abf17446745e9a1f1e5faa3");
            //app.UseFacebookAuthentication(
            //  appId: "271092676424493",
            //  appSecret: "6257d28e03a066d87fa527774d854b87");
            
           // app.UseGoogleAuthentication();

            var yahooAuthenticationOptions = new YahooAuthenticationOptions()
            {
                ConsumerKey = "dj0yJmk9TGVaNVhSTFdhU205JmQ9WVdrOVMyeE9VWEJrTjJFbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD00Mw--",
                ConsumerSecret = "1cdd4a7d9e35b684e12a4c87c45bbe83b1359133",
            };

            app.UseYahooAuthentication(yahooAuthenticationOptions);

           

            var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "773164954343-mn2gou556p3oj9m49nh9j6i2j9qqh3da.apps.googleusercontent.com",
                ClientSecret = "3m3S31nHlSJ-buxLG6b3KnkI",
            };
            app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);

            
        }
    }
}