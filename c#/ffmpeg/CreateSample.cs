using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Net;
using log4net;


namespace SaveTrack
{
    class CreateTrack
    { //this class was used to combine mp3 and ogg samples into one track using ffmpeg wrapper 
        #region variables
        
        //grab values from config file
        private string ffmpegPath = System.Configuration.ConfigurationSettings.AppSettings.Get("ffmpegPath");
        private string sampleRootPath = System.Configuration.ConfigurationSettings.AppSettings.Get("sampleRootPath");
        private string jsonPath = System.Configuration.ConfigurationSettings.AppSettings.Get("jsonPath");
        private string outputPath = System.Configuration.ConfigurationSettings.AppSettings.Get("outputPath");
        private string wsROOT = System.Configuration.ConfigurationSettings.AppSettings.Get("webserviceROOTurl");
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string trackName;
        private string fileID;
        private string oggPath;
        private string mp3Path;
        #endregion

        public CreateTrack(string fileID){
            if (stringChecker(fileID))
            {
                this.fileID = fileID;
                //oggPath = outputPath + fileID + ".ogg";
                //mp3Path = outputPath + fileID + ".mp3";

            }
  
        }//end constr

        public void init() {
            //loading json and starting track creation process.........
            DataContext db = new DataContext();
            
            if (stringChecker(fileID))
            {
                try
                {
                   //this method grabs information on certain tracks stored in our db
                   //string json = File.ReadAllText(jsonPath + fileID + ".json");
                      long idHolder2 = long.Parse(fileID);
                      var dbCall = db.Tracks.Where(x => x.ID == idHolder2).FirstOrDefault();
                      string json = dbCall.trackInfo;
                      //log.Debug(fileID);
                    if (stringChecker(json))
                    {
                        dynamic track = JsonConvert.DeserializeObject(json);
                        trackName = dbCall.trackname;
                        oggPath = outputPath + trackName + ".ogg";
                        mp3Path = outputPath + trackName + ".mp3";
                        if (!filecheck(mp3Path,oggPath))
                        {
                            createFFMPEGCommand(track);
                            
                        }
                    }
                    else
                    {

                        //no db entry found.......
                    }

                }
                catch (Exception e)
                {
                    
                    log.Error(e.ToString() + " init");
                }
            }
            else
            {
                //id is null or empty

            }
         
        }//end loadJSON()

        private void createFFMPEGCommand(dynamic track)
        {
            // create command line text for ffmpeg based on json data
            var samples = track.samples;
            StringBuilder inputs = new StringBuilder();
            StringBuilder filterComplex = new StringBuilder();
            string fiveInput = "", fiveFiltComplex = "";
            
            int x = 0;

            foreach (var sample in samples)
            {
                if (stringChecker((string)sample.name))
                {
                    inputs.Append(@"-i """ + sampleRootPath + sample.name + @""" ");
                    //assuming volume won't be null...
                    filterComplex.Append("[" + x + ":a]volume=" + sample.volume + ":precision=fixed[a" + x + "];");
                    if (x == 0)
                    {
                        fiveInput = @"-i """ + sampleRootPath + sample.name + @""" ";
                        fiveFiltComplex = "[" + x + ":a]volume=" + sample.volume + ":precision=fixed[a" + "5" + "];";

                    }
                    x++;
                }
            }
            //log.Debug("x== " + x);
            if (x==5)
            {

                inputs.Append(fiveInput);
                filterComplex.Append(fiveFiltComplex);
               

            }
            for (int j = 0;j<=x-1;j++)
            {
                filterComplex.Append("[a" + j.ToString() +"]");
            }
            if (x==5)
            {

                filterComplex.Append("[a5]");
                x++;
            }
        


            string customCommand = String.Format(@"{0} -filter_complex ""{1}amerge=inputs={2}[a]"" -map ""[a]"" -c:a libmp3lame -b:a 320k ""{3}""", inputs.ToString(), filterComplex.ToString(), x.ToString(), mp3Path);
            //log.Debug(customCommand);
            mergeSamples(customCommand);

            #region example command string
            /* example command
             
        -i "\mp3 clips\1.mp3" -i "\mp3 clips\2.mp3" -i "\mp3 clips\4.mp3" -i "\Desktop\mp3 clips\pb.mp3" -filter_complex "[0:a]volume=0.3:precision=fixed[a0];[1:a]volume=0.5:precision=fixed[a1];
       [2:a]volume=0.7:precision=fixed[a2];[3:a]volume=0.7:precision=fixed[a3];[a0][a1][a2][a3]amerge=inputs=4[a]" -map "[a]" -c:a libmp3lame -q:a 5 "C:\Users\AP\Desktop\mp3 clips\output2.mp3"
               
        */
            #endregion

        }//end createFFMPEGCommand()

        private void mergeSamples(string command)
        {                    
            try
            {
                MediaHandler _mhandler = new MediaHandler();  // wrapper for ffmpeg
                _mhandler.FFMPEGPath = ffmpegPath;
                _mhandler.CustomCommand = command;
                _mhandler.Execute_FFMPEG();
                //file check
                if (File.Exists(mp3Path))
                {
                    createOgg();
                }
                else
                {

                    //do something ffmpeg f'ed up......

                }
            }
            catch(Exception e)
            {
                log.Error(e.Message + "mergsamples");

            }


        }//end mergeSamples()
        private void createOgg()
        {
            string command = @"-i """ + mp3Path + @""" -c:a libvorbis """ + oggPath + @"""";
            MediaHandler _mhandler = new MediaHandler(); 
            _mhandler.FFMPEGPath = ffmpegPath;
            _mhandler.CustomCommand = command;
            try
            {
                _mhandler.Execute_FFMPEG();
                
                if (filecheck(mp3Path, oggPath))
                {
                    callUpdateTrack();
                    uploadTrack(mp3Path, oggPath);
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message + "createogg");
            }
        }//end createOgg



        private void callUpdateTrack()
        {
            try
            {   //this was used to update that track in the local server's file system before uploading, using a WS 
                string url = wsROOT + "updateTrack.aspx?id=" + fileID + "&loc=" + trackName + ".mp3"; 
                Uri updateTrackURI = new Uri(url, UriKind.Absolute);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateTrackURI);
                
                request.ContentLength = 0;
                request.Method = "POST";

                var respStuff  = request.GetResponse();
               
                
            }
            catch(WebException e)
            {
                log.Error(e.Message + "callupdatetrack");
            }

        }//end callUpdateTrack()

        private void uploadTrack(string mp3Path, string oggPath)
        {
            //upload to aws bucket
            string[] paths = new string[] { mp3Path, oggPath };
            Bucket.uploadFile(paths,fileID);
            
        }
        private Boolean stringChecker(string str)
        {
            if(!string.IsNullOrEmpty(str))
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        private Boolean filecheck(string pathmp3,string pathogg)
        {
            try
            {
                FileInfo fimp3 = new FileInfo(pathmp3);
                FileInfo fiogg = new FileInfo(pathogg);
                if (fimp3.Exists && fiogg.Exists)
                {  //throw in file check real quick, making sure files are > 0
                    if (fimp3.Length > 0 && fiogg.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        fimp3.Delete();
                        fiogg.Delete();
                        log.Error("files size is 0 and were deleted");  //if files are still present, and try to recreate the files
                        return false;
                    }
                }
                else
                {
                    //log.Error("file does not exist");
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Error("checking for file:" + e.Message);
                return false;
            }

        }

    }
}
