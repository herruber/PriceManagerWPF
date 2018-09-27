using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PriceManagerWPF
{
    public static class Controller
    {
        public static string Url = "http://localhost:63918/"; //Home/GetPriceList

        public static string GenerateRequest(string controller, string action, string method, object data)
        {
            // Create a request for the URL.
            string json = JsonConvert.SerializeObject(data);

            string url = Url + "/" + controller + "/" + action;

            if (method.ToLower().Equals("post"))
            {
                //url = string.Format(Url + "/?data={0}", json);
            }

            WebRequest request = WebRequest.Create(url);
            request.Method = method;
            Stream dataStream;

            if (data != null)
            {
                request.ContentType = "application/json";
                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                request.ContentLength = byteArray.Length;

                using (var streamwriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamwriter.Write(json);
                    streamwriter.Flush();
                    streamwriter.Close();
                }

            }

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response;

            try
            {
                // Get the response.
                response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                reader.Close();
                response.Close();
                return responseFromServer;

            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }
    }
}
