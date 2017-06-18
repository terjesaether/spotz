using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Json;
using System.Net.Http;
using Android.Graphics;

namespace SpotzDroid
{
    class SpotzService
    {

        private string _messageTitle;
        private string _messageBody;

        public async Task<List<SpotzViewModel>> GetSpotzesFromJson(string latitude, string longitude, string apiUrl, Context context)
        {
            if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
            {

                latitude = LoadLocationInSharedPreferences()[0];
                longitude = LoadLocationInSharedPreferences()[1];

                _messageTitle = Application.Context.GetString(Resource.String.MessageNoLocaton);

                Toast.MakeText(context, _messageTitle, ToastLength.Long).Show();

            }

            var url = apiUrl + "getspotzesfromdistance?latitude=" + latitude + "&longitude=" + longitude;

            using (var client = new HttpClient())
            {
                var result = new List<SpotzViewModel>();

                try
                {

                    Toast.MakeText(context, url, ToastLength.Long).Show();

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<SpotzViewModel>>(json);
                }
                catch (Exception)
                {
                    _messageTitle = Application.Context.GetString(Resource.String.MessageServerErrorTitle);
                    _messageBody = Application.Context.GetString(Resource.String.MessageServerErrorBody);

                    ShowAlertMessage(_messageTitle, _messageBody, context);

                    //var alert = new AlertDialog.Builder(context);
                    //alert.SetTitle(messageTitle);
                    //alert.SetMessage(messageBody);
                    //alert.SetPositiveButton("Ok", (senderAlert, args) =>
                    //{

                    //});
                    //alert.Create();
                    //alert.Show();
                }

                if (result.Count == 0) // No spotz nearby
                {
                    _messageTitle = Application.Context.GetString(Resource.String.MessageNoSpotzTitle);
                    _messageBody = Application.Context.GetString(Resource.String.MessageNoSpotzBody);

                    ShowAlertMessage(_messageTitle, _messageBody, context);

                    //var alert = new AlertDialog.Builder(context);
                    //alert.SetTitle(messageTitle);
                    //alert.SetMessage(messageBody);
                    //alert.SetPositiveButton("Ok", (senderAlert, args) =>
                    //{

                    //});
                    //alert.Create();
                    //alert.Show();
                }

                return result;
            }
        }

        public async Task<SpotzViewModel> GetOneSpotzFromJson(string id, string baseUrl, Context context)
        {


            var url = baseUrl + "api/getonespotz/" + id;

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SpotzViewModel>(json);
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;

                }


            }
        }



        public Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        // Lagrer locasjon for bruk når app starter på nytt eller problemer med å finne lokasjon
        public void SaveLocationInSharedPreferences(string lo, string la)
        {
            var savedLocation = Application.Context.GetSharedPreferences("MyLocation", FileCreationMode.Private);
            var lastLatitude = savedLocation.Edit();
            var lastLongitue = savedLocation.Edit();
            lastLatitude.PutString("lastLatitude", la);
            lastLongitue.PutString("lastLongitue", lo);
            lastLatitude.Commit();
            lastLongitue.Commit();
        }

        // Henter locasjon for bruk når app starter på nytt eller problemer med å finne lokasjon
        public string[] LoadLocationInSharedPreferences()
        {
            var coordinatesArray = new string[2];
            try
            {
                var savedLocation = Application.Context.GetSharedPreferences("MyLocation", FileCreationMode.Private);
                if (savedLocation != null)
                {
                    coordinatesArray[0] = savedLocation.GetString("lastLatitude", null);
                    coordinatesArray[1] = savedLocation.GetString("lastLongitue", null);
                }
                else
                {
                    coordinatesArray[0] = "59.9273494";
                    coordinatesArray[1] = "10.776439";
                }
            }
            catch (Exception)
            {
                coordinatesArray[0] = "59.9273494";
                coordinatesArray[1] = "10.776439";
                return coordinatesArray;
            }

            return coordinatesArray;
        }

        private static void ShowAlertMessage(string title, string body, Context context)
        {
            var alert = new AlertDialog.Builder(context);
            alert.SetTitle(title);
            alert.SetMessage(body);
            alert.SetPositiveButton("Ok", (senderAlert, args) =>
            {

            });
            alert.Create();
            alert.Show();
        }
    }
}