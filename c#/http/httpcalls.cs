using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Configuration;
using System.Net;


namespace HttpCalls {

 //this class is mainly for reference , functions used in past projects
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

     

     //this method takes in an object containing fields specified by the third party and an optional uid param
     public static string callWS(CUSTOMOBJECT obj, string uid = "") {
      string url = ConfigurationManager.AppSettings["WSUrl"].ToString();
     
      Uri mssURI = new Uri(url, UriKind.Absolute);
      byte[] postData = Encoding.UTF8.GetBytes("realartfeed= " + JsonConvert.SerializeObject(obj));

      try {
        using (var wc = new WebClient()) {

          wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
         
          byte[] response = wc.UploadData(url, postData);

          string jsonResponse = Encoding.UTF8.GetString(response);
          dynamic jsonObj = JsonConvert.DeserializeObject(jsonResponse);

          if (jsonObj.error == false) {
            CreateLead(obj, "success", uid);  //method for an old project, remove/replace 
            return "true";

          } else {
            CreateLead(obj, jsonObj.errormessage, uid);
            return jsonObj.errormessage + " " + JsonConvert.SerializeObject(obj);
          }

        }
      } 
      catch (Exception we) {
        return we.Message;
      }
    }
    }

  }