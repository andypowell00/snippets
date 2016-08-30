using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Configuration;
using System.Net;


namespace HttpCalls {


  public class Posts {

  public static bool postJson(string json)
        {
   
           
            string url = "http://www.randomurl.com/servicecall.json";
            Uri thirdpartyURI = new Uri(url, UriKind.Absolute);
            byte[] postData = Encoding.UTF8.GetBytes(json);
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Credentials = new NetworkCredential("randomuser", "randompass");  
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    
                    byte[] response =  wc.UploadData(url, postData);
                    //Console.WriteLine(Encoding.UTF8.GetString(response));
                    string jsonResponse = Encoding.UTF8.GetString(response);
                    dynamic jsonObj = JsonConvert.DeserializeObject(jsonResponse);

                    //check the response obejct's dynamic fields if needed here to ensure success
                    
                    return true;
                }
            }
            catch(Exception we)
            {
                
                //log.Error(we.Message);
                return false;
            }
         

        }//end postJson()





     }

  }