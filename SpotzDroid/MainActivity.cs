using Android.App;
using Android.Widget;
using Android.OS;

namespace SpotzDroid
{
    [Activity(Label = "Spotz!"
        //, 
        //MainLauncher = true, 
        //Icon = "@drawable/icon"
        )]
    public class MainActivity : Activity
    {

        //private Button btnLogin;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //StartActivity(typeof(ListSpotz));

            FindViewById<Button>(Resource.Id.btnLogin).Click += MainActivity_Click;
        }

        private void MainActivity_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(ListSpotz));
        }
    }
}

