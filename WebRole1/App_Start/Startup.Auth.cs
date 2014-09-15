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

            bool _useCloudAppClientIDs = true;

            if (_useCloudAppClientIDs)
            {
                app.UseMicrosoftAccountAuthentication(
                clientId: "0000000044126351",
                clientSecret: "jAZpCRiJKg5gQRxFxvYBtpy5rdopTlnW");

                app.UseFacebookAuthentication(
               appId: "1526133034266985",
               appSecret: "1bbb93665abf17446745e9a1f1e5faa3");

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
            else
            {
                //R8R.wiki IDs

                app.UseMicrosoftAccountAuthentication(
               clientId: "0000000048128D31",
               clientSecret: "vUmQPTbhTIctdtAhjWN-kDWodsNuJcNo");

                app.UseFacebookAuthentication(
               appId: "271092676424493",
               appSecret: "6257d28e03a066d87fa527774d854b87");

                var yahooAuthenticationOptions = new YahooAuthenticationOptions()
                {
                    ConsumerKey = "dj0yJmk9OEp2RlhPSkVMOE80JmQ9WVdrOVR6WjNZVTFJTXpBbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD04NA—",
                    ConsumerSecret = "34c4dde389ff43a49e4e91e205e744e5d874a85a",
                };
                app.UseYahooAuthentication(yahooAuthenticationOptions);

                var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = "1072934350544-dl5fr7svaedmn33tms38a89kppbvdse4.apps.googleusercontent.com",
                    ClientSecret = "didI13YDMmcMhsGmOJgIK9W_",
                };
                app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);

            }

        }
    }
}