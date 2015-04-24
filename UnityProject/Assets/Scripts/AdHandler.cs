﻿using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class AdHandler : MonoBehaviour {

    private string ANDROID_PAUSE_BANNER_AD_ID = "ca-app-pub-4745551853234420/8131009194";
    private string ANDROID_INTERSTITIAL_AD_ID = "ca-app-pub-4745551853234420/5956271992";

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
        bannerAd = new BannerView(ANDROID_PAUSE_BANNER_AD_ID, AdSize.Banner, position);

        AdRequest request = new AdRequest.Builder().Build();
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
        interstitialAd = new InterstitialAd(ANDROID_INTERSTITIAL_AD_ID);
        AdRequest request = new AdRequest.Builder().Build();
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
