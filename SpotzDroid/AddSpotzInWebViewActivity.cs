using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace SpotzDroid
{
    [Activity(Label = "Add Spotz!")]
    public class AddSpotzInWebViewActivity : Activity
    {
        private string _url;
        private WebView _webViewAddSpotz;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSpotzInWebViewLayout);

            _webViewAddSpotz = FindViewById<WebView>(Resource.Id.webViewAddSpotz);

            _url = Intent.GetStringExtra("web_url");

            _webViewAddSpotz.LoadUrl(_url);
        }


    }
}