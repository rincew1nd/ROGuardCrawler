using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ROGuardCrawler.Utils
{
    public static class WebHelper
    {
        public static string LoadPage(string pageUrl)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(pageUrl);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Response status code is {response.StatusCode}!");
                    }

                    var receiveStream = response.GetResponseStream();
                    if (receiveStream == null)
                    {
                        throw new Exception($"Can not load page {pageUrl}. No response stream found!");
                    }

                    using (var readStream = response.CharacterSet == null ?
                        new StreamReader(receiveStream) :
                        new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)))
                    {
                        return readStream.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured during web page loading.\nMessage: {e.Message}\nStackTrace: {e.StackTrace}");
                return null;
            }
        }
    }
}
