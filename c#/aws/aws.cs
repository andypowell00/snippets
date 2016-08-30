using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.IO;
using log4net;
using Newtonsoft.Json;
using System.Net;

namespace AWS
{
    public static class Bucket
    {
        #region variables 
        private static string accessKey = System.Configuration.ConfigurationSettings.AppSettings.Get("accessKey");
        private static string secretKey = System.Configuration.ConfigurationSettings.AppSettings.Get("secretAccessKey");
        private static string bucketName = System.Configuration.ConfigurationSettings.AppSettings.Get("bucketName");
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public static void uploadFile(string[] paths, string id)
        {

            AmazonS3Client client = (AmazonS3Client)Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, RegionEndpoint.USWest2);

            TransferUtility fileUploadUtil = new TransferUtility(client);
            try
            {
                foreach (string path in paths)
                {
                    
                    fileUploadUtil.Upload(path, bucketName);
                    
                    //upload successful
                }
                callRadioAPI(id);
                
            }
            catch (AmazonS3Exception e)
            {

                log.Error(e.Message);
            }
        }//end uploadFile()

        private static void listObjects()
        {
            AmazonS3Client client = (AmazonS3Client)Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, RegionEndpoint.USWest2);


            ListObjectsRequest lor = new ListObjectsRequest();
            lor.BucketName = bucketName;
            ListObjectsResponse resp = client.ListObjects(lor);
            foreach (var o in resp.S3Objects)
            {
                //loop over the contents of the bucket
                
               
            }


        }//end listObjects()

    }
}