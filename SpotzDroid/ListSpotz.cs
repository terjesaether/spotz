using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Json;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Android.Locations;
using Android.Util;

namespace SpotzDroid
{
    [Activity(Label = "Find closest Spotz!",
        MainLauncher = true,
        Icon = "@drawable/icon")]
    public class ListSpotz : Activity, ILocationListener
    {

        // Henter baseUrl fra strings-values
        private readonly string _baseUrl = Application.Context.GetString(Resource.String.BaseUrl);

        private RelativeLayout _loadingPanel;
        private readonly SpotzService _spotzService = new SpotzService();
        private ListSpotzAdapter _adapter;
        private ListView _listSpotz;
        private bool _isSpotzListed = false;
        private string _message;
        private string _messageTitle;
        private string _messageBody;


        // LocationVariables:
        private Location _currentLocation;
        private LocationManager _locationManager;
        private string _locationProvider;

        private string _latitude;
        private string _longitude;

        private Button _btnAddSpotz;
        private Button _btnRefresh;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ListSpotzLayout);
            _loadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            _listSpotz = FindViewById<ListView>(Resource.Id.list);
            _btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);
            _btnAddSpotz = FindViewById<Button>(Resource.Id.btnAddSpotz);

            InitializeLocationManager();

            _listSpotz.ItemClick += ListSpotz_ItemClick;
            _btnRefresh.Click += BtnRefresh_Click;
            _btnAddSpotz.Click += BtnAddSpotz_Click;

        }

        // Begynner å listen etter lokasjon når app kommer i forgrunn
        protected override void OnResume()
        {
            base.OnResume();

            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }

        // Pauser listener i bakgrunnen:
        protected override void OnPause()
        {
            base.OnPause();
            _spotzService.SaveLocationInSharedPreferences(_latitude, _longitude);
            _locationManager.RemoveUpdates(this);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {

            _spotzService.SaveLocationInSharedPreferences(_latitude, _longitude);

            // always call the base implementation!
            base.OnSaveInstanceState(outState);

        }


        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            _loadingPanel.Visibility = ViewStates.Visible;
            GetListOfSpotzes();
        }

        private void BtnAddSpotz_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AddSpotzInWebViewActivity));
            intent.PutExtra("web_url", _baseUrl + "Home/NewzSpotz");

            StartActivity(intent);
        }

        private void ListSpotz_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(SpotzDetailActivity));

            // Selve iden til Spotz:
            intent.PutExtra("spotz.spotzId", _adapter[(int)e.Id].SpotzId);
            intent.PutExtra("spotz.latitude", _adapter[(int)e.Id].Latitude);
            intent.PutExtra("spotz.longitude", _adapter[(int)e.Id].Longitude);


            StartActivityForResult(intent, 0);
        }


        public void GetListOfSpotzes()
        {
            RunOnUiThread(async () =>
            {

                // await gjør om Task<List> til vanlig List

                var spotzes = await _spotzService.GetSpotzesFromJson(_latitude, _longitude, _baseUrl + "api/", this);

                _adapter = new ListSpotzAdapter(this, spotzes, _baseUrl);
                _listSpotz.Adapter = _adapter;
                _loadingPanel.Visibility = ViewStates.Gone;
            });
        }




        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }

        }

        // Henter ny location
        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                //_locationText.Text = "Unable to determine your location. Try again in a short while.";
                _message = Application.Context.GetString(Resource.String.MessageCantFindLocation);
                Toast.MakeText(this, _message, ToastLength.Long).Show();
            }
            else
            {
                _latitude = _currentLocation.Latitude.ToString(CultureInfo.InvariantCulture);
                _longitude = _currentLocation.Longitude.ToString(CultureInfo.InvariantCulture);

                _spotzService.SaveLocationInSharedPreferences(_latitude, _longitude);

                //string phoneLocation = $"{_currentLocation.Latitude:f6},{_currentLocation.Longitude:f6}";
                //_locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                //Toast.MakeText(this, phoneLocation, ToastLength.Long).Show();

                // Gjør at man laster når pos er funnet.
                if (_isSpotzListed) return;
                GetListOfSpotzes();
                _isSpotzListed = true;


                //var spotzes = await spotzService.GetSpotzFromJson(latitude, longitude, baseUrl + "api/");
                //adapter = new ListSpotzAdapter(this, spotzes, baseUrl);
                //listSpotz.Adapter = adapter;
                //loadingPanel.Visibility = ViewStates.Gone;

                //Address address = await ReverseGeocodeCurrentLocation();
                //DisplayAddress(address);
            }
        }



        public void OnProviderDisabled(string provider)
        {
            _messageTitle = Application.Context.GetString(Resource.String.MessageTurnOnGpsTitle);
            _messageBody = Application.Context.GetString(Resource.String.MessageTurnOnGpsBody);

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle(_messageTitle);
            builder.SetMessage(_messageBody);
            builder.Show();
        }

        public void OnProviderEnabled(string provider)
        {
            _message = Application.Context.GetString(Resource.String.MessageTurnOnGpsTitle);
            Toast.MakeText(this, _message, ToastLength.Long).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            _message = Application.Context.GetString(Resource.String.MessageStatusChanged);
            Toast.MakeText(this, _message + status.ToString(), ToastLength.Long).Show();
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        //void DisplayAddress(Address address)
        //{
        //    if (address != null)
        //    {
        //        StringBuilder deviceAddress = new StringBuilder();
        //        for (int i = 0; i < address.MaxAddressLineIndex; i++)
        //        {
        //            deviceAddress.AppendLine(address.GetAddressLine(i));
        //        }
        //        // Remove the last comma from the end of the address.
        //        //_addressText.Text = deviceAddress.ToString();
        //        Toast.MakeText(this, deviceAddress.ToString(), ToastLength.Long).Show();
        //    }
        //    else
        //    {
        //        //_addressText.Text = "Unable to determine the address. Try again in a few minutes.";
        //        //Toast.MakeText(this, "Unable to determine the address. Try again in a few minutes.", ToastLength.Long).Show();

        //    }
        //}
    }
}