using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace HelperUtil
{
    public static class helpers
    {
  
    private string GetRandomString() {
      var random = new Random();
      return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[random.Next(s.Length)]).ToArray());
    }//end GetRandomString()
   


    }
}