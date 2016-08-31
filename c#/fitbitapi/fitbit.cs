using Fitbit.Api;
using Fitbit.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using fb_mvc.Models;
using System.Web.Mvc;
using log4net;
using System.Resources;
using System.Globalization;
using fb_mvc.Classes;


namespace fb_mvc.Classes
{
    public static class FitbitMethods
    {   //this is using the fitbit wrapper found here:  https://github.com/aarondcoleman/Fitbit.NET
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static DateTime dt = new DateTime(2015, 03, 24);
        private static FitBitContextDataContext context = new FitBitContextDataContext();
        private static ResourceManager rm = Resources.Resources.ResourceManager;
        public static FitbitClient GetFitbitClient()
        {

            FitbitUser curr_usr = new FitbitUser();
            curr_usr = (FitbitUser)HttpContext.Current.Session["curr_user"];
            FitbitClient client = new FitbitClient(ConfigurationManager.AppSettings["FitbitConsumerKey"],
                ConfigurationManager.AppSettings["FitbitConsumerSecret"],
                curr_usr.auth_token,
                curr_usr.auth_secret);



            return client;
        }

        public static FitbitClient getOtherFitbitClient(string token, string secret)
        {
            FitbitClient client;
            try
            {
                client = new FitbitClient(ConfigurationManager.AppSettings["FitbitConsumerKey"],
                    ConfigurationManager.AppSettings["FitbitConsumerSecret"],
                   token,
                   secret);

                return client;
            }
            catch (Exception e)
            {
                log.Error(e.Message + "getotherfitbitclient");
                client = null;
                return client;
            }

        }//end 
        public static string getUserScoreFromFitbit(FitbitClient client, TimeSeriesResourceType datatype, DateTime start, DateTime end)
        {
            decimal total = 0;
            try
            {
                TimeSeriesDataList seriesOfUserData = client.GetTimeSeries(datatype, end, start);

                foreach (var dat in seriesOfUserData.DataList)
                {
                    total += decimal.Parse(dat.Value, CultureInfo.InvariantCulture);
                    total = Math.Round(total, 2);
                }
                return total.ToString();
            }
            catch (Exception e)
            {
                log.Error(e.Message + "getuserscorefromfit");
                return "-1";
                //log
            }


        }
        public static string userProfileKilometers()
        {
            FitbitClient client = GetFitbitClient();

            if (client != null)
            {
                decimal totalKilos = 0;
                try
                {
                    TimeSeriesDataList kiloSeries = client.GetTimeSeries(TimeSeriesResourceType.Distance, DateTime.UtcNow, dt);
                    foreach (var day in kiloSeries.DataList)
                    {
                        totalKilos += decimal.Parse(day.Value, CultureInfo.InvariantCulture);
                    }
                    totalKilos = Math.Round(totalKilos, 2);
                    return totalKilos.ToString();
                }
                catch (Exception e) { log.Error(e.Message + "kilometers"); return "-1"; }
            }
            else
            {

                return "-1";
            }

        }

        public static string averageSleep()
        {

            int x = 0; //represents # of days that aren't 0
            decimal totalSleepminutes = 0;

            FitbitClient client = GetFitbitClient();
            if (client != null)
            {
                try
                {
                    TimeSeriesDataList sleepyTimes = client.GetTimeSeries(TimeSeriesResourceType.MinutesAsleep, DateTime.UtcNow, dt);
                    foreach (var val in sleepyTimes.DataList)
                    {
                        if (Int32.Parse(val.Value) > 0)
                        {
                            totalSleepminutes += decimal.Parse(val.Value);
                            x++;
                        }


                    }//end foreach
                    if (totalSleepminutes > 0)
                    {
                        decimal avgSleepHolder = totalSleepminutes / x;
                        avgSleepHolder = Math.Round(avgSleepHolder, 2);
                        return avgSleepHolder.ToString();
                    }
                    else
                    {

                        return "0";
                    }
                }
                catch (Exception e) { log.Error(e.Message + "avgsleep"); return "-1"; }
            }
            else
            {

                return "-1";
            }



        }//end averageSleep()
        public static string totalSteps()
        {
            FitbitClient client = GetFitbitClient();
            if (client != null)
            {
                decimal totalSteps = 0;
                try
                {
                    TimeSeriesDataList stepsSeries = client.GetTimeSeries(TimeSeriesResourceType.Steps, DateTime.UtcNow, dt);

                    foreach (var day in stepsSeries.DataList)
                    {
                        totalSteps += decimal.Parse(day.Value);
                    }

                    return totalSteps.ToString();
                }
                catch (Exception e) { log.Error(e.Message + "totalsteps"); return "-1"; }
            }
            else
            {

                return "-1";
            }


        }//end totalSteps()
        public static string totalActiveMins()
        {
            FitbitClient client = GetFitbitClient();
            if (client != null)
            {
                decimal totalActivemins = 0;
                try
                {
                    TimeSeriesDataList activeMinSeries = client.GetTimeSeries(TimeSeriesResourceType.MinutesVeryActive, DateTime.UtcNow, dt);

                    foreach (var day in activeMinSeries.DataList)
                    {
                        totalActivemins += decimal.Parse(day.Value);
                    }
                    return totalActivemins.ToString();
                }
                catch (Exception e) { log.Error(e.Message + "totalactivemins"); return "-1"; }
            }
            else
            {

                return "-1";
            }


        }//end totalSActivemins()
        public static string totalCalsBurned()
        {
            FitbitClient client = GetFitbitClient();
            if (client != null)
            {
                decimal totalCalsBurned = 0;
                try
                {
                    TimeSeriesDataList activeCalsBurned = client.GetTimeSeries(TimeSeriesResourceType.CaloriesOut, DateTime.UtcNow, dt);

                    foreach (var day in activeCalsBurned.DataList)
                    {
                        totalCalsBurned += decimal.Parse(day.Value);
                    }
                    return totalCalsBurned.ToString();
                }
                catch (Exception e) { log.Error(e.Message + "totalcalsburned"); return "-1"; }
            }
            else
            {

                return "-1";
            }

        }//end totalCalsBurned()
        public static UserProfile grabUserProfile()
        {
            UserProfile up;
            FitbitClient client = GetFitbitClient();
            if (client != null)
            {
                try
                {

                    up = client.GetUserProfile();

                    return up;
                }
                catch (Exception e)
                {
                    log.Error(e.Message + " grabuserprof");
                    up = null;
                    return up;
                }
            }
            else
            {
                up = null;
                return up;
            }

        }//end grabUserProfile()
    }
}