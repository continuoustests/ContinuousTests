using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
namespace AutoTest.Client.HTTP
{

    public class Analytics
    {
        // Tracker version.
        private const string Version = "4.4sa";

        private const string CookieName = "__utmmobile";

        // The path the cookie will be available to, edit this to use a different
        // cookie path.
        private const string CookiePath = "/";

        // Two years in seconds.
        private readonly TimeSpan CookieUserPersistence = TimeSpan.FromSeconds(63072000);

        // 1x1 transparent GIF
        private readonly byte[] GifData = {
      0x47, 0x49, 0x46, 0x38, 0x39, 0x61,
      0x01, 0x00, 0x01, 0x00, 0x80, 0xff,
      0x00, 0xff, 0xff, 0xff, 0x00, 0x00,
      0x00, 0x2c, 0x00, 0x00, 0x00, 0x00,
      0x01, 0x00, 0x01, 0x00, 0x00, 0x02,
      0x02, 0x44, 0x01, 0x00, 0x3b
  };

        private static readonly Regex IpAddressMatcher =
            new Regex(@"^([^.]+\.[^.]+\.[^.]+\.).*");

        // A string is empty in our terms, if it is null, empty or a dash.
        private static bool IsEmpty(string input)
        {
            return input == null || "-" == input || "" == input;
        }


        // Get a random number string.
        private static String GetRandomNumber()
        {
            Random RandomClass = new Random();
            return RandomClass.Next(0x7fffffff).ToString();
        }

        // Make a tracking request to Google Analytics from this server.
        // Copies the headers from the original request to the new one.
        // If request containg utmdebug parameter, exceptions encountered
        // communicating with Google Analytics are thown.
        private static void SendRequestToGoogleAnalytics(string utmUrl)
        {
            try
            {
                WebRequest connection = WebRequest.Create(utmUrl);

                ((HttpWebRequest)connection).UserAgent = "";
                connection.Headers.Add("Accept-Language",
                    "EN-US");

                using (WebResponse resp = connection.GetResponse())
                {
                    // Ignore response
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error contacting Google Analytics", ex);

            }
        }

        // Track a page view, updates all the cookies and campaign tracker,
        // makes a server side request to Google Analytics and writes the transparent
        // gif byte data to the response.
        private static void TrackPageView(string path)
        {
            TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime());
            string timeStamp = timeSpan.TotalSeconds.ToString();
            string domainName = "continuoustests.com";
            if (IsEmpty(domainName))
            {
                domainName = "";
            }

            var documentReferer = "-";

            string documentPath = path;
            var userAgent = "";

            // Try and get visitor cookie from the request.
            string utmGifLocation = "http://www.google-analytics.com/__utm.gif";

            // Construct the gif hit url.
            string utmUrl = utmGifLocation + "?" +
                "utmwv=" + Version +
                "&utmn=" + GetRandomNumber() +
                "&utmhn=" + "continuoustests.com" +
                "&utmr=" + "moose" +
                "&utmp=" + path.Replace("/", "%2F") +
                "&utmac=" + "MO-29683017-1" +
                "&utmcc=__utma%3D999.999.999.999.999.1%3B" +
                "&utmvid=" + (visitor - DateTime.Today.GetHashCode());

            SendRequestToGoogleAnalytics(utmUrl);
        }

        static int visitor = Guid.NewGuid().GetHashCode();

        private const string GaAccount = "MO-29683017-1";
        private const string GaPixel = "/ga.aspx";
        private static string GoogleAnalyticsGetImageUrl(string _url)
        {
            System.Text.StringBuilder url = new System.Text.StringBuilder();
            url.Append(GaPixel + "?");
            url.Append("utmac=").Append(GaAccount);
            Random RandomClass = new Random();
            url.Append("&utmn=").Append(RandomClass.Next(0x7fffffff));
            url.Append("&utmr=").Append("moose");
            url.Append("&utmp=").Append(_url.Replace("/", "%2F"));
            url.Append("&guid=ON"); 
            return url.ToString().Replace("&", "&amp;");
        }

        private static Func<bool> CanSendEvents = () => true;
        public static void CanSendEventsWhen(Func<bool> query)
        {
            CanSendEvents = query;
        }

        private static Version MMVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static void SendEvent(string name)
        {
            if (!CanSendEvents())
                return;

            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    var versionString =
                        string.Format("{0}.{1}.{2}",
                                      MMVersion.Major, MMVersion.Minor, MMVersion.Build);
                    TrackPageView("/event/" + name + "/"+ versionString);
                }
                catch (Exception ex) { Logging.Logger.Write(ex); }
            });
        }
    }
}
