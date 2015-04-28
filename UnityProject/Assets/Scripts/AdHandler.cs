using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class AdHandler : MonoBehaviour {

#if UNITY_IOS
	private string PAUSE_BANNER_AD_ID = "ca-app-pub-4745551853234420/8203231191";
	private string INTERSTITIAL_AD_ID = "ca-app-pub-4745551853234420/6726497995";
#else
    private string PAUSE_BANNER_AD_ID = "ca-app-pub-4745551853234420/8131009194";
    private string INTERSTITIAL_AD_ID = "ca-app-pub-4745551853234420/5956271992";
#endif

    private BannerView bannerAd;
    private InterstitialAd interstitialAd;

    public static int INTERSTITIAL_FREQUENCY = 10;      // every 10 deaths we throw up an interstitial

    private int interstitialCountDown;

    public int InterstitialCountDown
    {
        get { return interstitialCountDown; }
        set { interstitialCountDown = value; }
    }

    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        interstitialCountDown = INTERSTITIAL_FREQUENCY;
    }

    public void RequestBanner(AdPosition position)
	{
		interstitialCountDown = 0;
		Debug.Log("banner ad request");
        // Make sure the bannerAd isn't already active
        if (bannerAd != null)
			return;
		Debug.Log("banner ad pass");

        bannerAd = new BannerView(PAUSE_BANNER_AD_ID, AdSize.Banner, position);

		AdRequest request = new AdRequest.Builder()
#if UNITY_IOS
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("616c27ce10cf4aa8bd807de44d18e496") // Calvin's IPhone
#endif
			.Build();

        bannerAd.LoadAd(request);
    }

    public void DestroyBanner()
    {
        if (bannerAd != null)
        {
            bannerAd.Destroy();
            bannerAd = null;
        }
    }

    public void RequestInterstitual()
	{
		Debug.Log("Interstitial ad request");
        if (interstitialAd != null)
			return;
		Debug.Log("Interstitial ad pass");

        interstitialAd = new InterstitialAd(INTERSTITIAL_AD_ID);
        AdRequest request = new AdRequest.Builder()
#if UNITY_IOS
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("616c27ce10cf4aa8bd807de44d18e496") // Calvin's IPhone
#endif
			.Build();
        interstitialAd.LoadAd(request);

        interstitialAd.AdClosed += interstitialAd_AdClosed;
    }

    void interstitialAd_AdClosed(object sender, System.EventArgs e)
    {
        Debug.Log("Interstitial ad closed");
        interstitialAd.Destroy();
        interstitialAd = null;
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
    }

    public void DestroyInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }
}
