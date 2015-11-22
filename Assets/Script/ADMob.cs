using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;  // 반드시 추가
using System;

public class ADMob : MonoBehaviour
{
	InterstitialAd interstitial;
	BannerView bannerView;

	#if UNITY_EDITOR
	string adUnitId = "unused";
	string adBannerId = "unused";
	#elif UNITY_ANDROID
	string adUnitId = "ca-app-pub-8449608955539666/5398962439";
	string adBannerId = "ca-app-pub-8449608955539666/3922229230";
	#elif UNITY_IPHONE
	string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
	string adBannerId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
	#else
	string adUnitId = "unexpected_platform";
	string adBannerId = "unexpected_platform";
	#endif
	// Use this for initialization
	public System.Action m_interstitialOpenFunctor;

	void Awake()
	{
		interstitial = new InterstitialAd(adUnitId);
		interstitial.AdLoaded += HandleInterstitialLoaded;
		interstitial.AdFailedToLoad += HandleInterstitialFailedToLoad;
		interstitial.AdOpened += HandleInterstitialOpened;
		interstitial.AdClosing += HandleInterstitialClosing;
		interstitial.AdClosed += HandleInterstitialClosed;
		interstitial.AdLeftApplication += HandleInterstitialLeftApplication;

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adBannerId, AdSize.Banner, AdPosition.Top);
		bannerView.AdLoaded += HandleBannerLoaded;

		DateTime dateTime = DateTime.Now.AddYears(-12);
		AdRequest request = new AdRequest.Builder()
			.AddKeyword("Game")
				.SetBirthday(dateTime)
				.Build();
		// Load the Ads with the request.
		
		interstitial.LoadAd(request);
		bannerView.LoadAd(request);
	}

	public void ShowInterstitial()
	{		
		interstitial.Show();

		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			HandleInterstitialOpened(null, null);
		}
	}

	public void ShowBanner(bool show)
	{		
		if (show == true)
			bannerView.Show();
		else
			bannerView.Hide();
	}

	public void HandleBannerLoaded(object sender, System.EventArgs args)
	{
		ShowBanner(false);
		Debug.Log("HandleBannerLoaded event received.");
	}

	public void HandleInterstitialLoaded(object sender, System.EventArgs args)
	{
		Debug.Log("HandleInterstitialLoaded event received.");
	}
	
	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}
	
	public void HandleInterstitialOpened(object sender, EventArgs args)
	{
		Debug.Log("HandleInterstitialOpened event received");
		if (m_interstitialOpenFunctor != null)
			m_interstitialOpenFunctor();
	}
	
	void HandleInterstitialClosing(object sender, EventArgs args)
	{
		Debug.Log("HandleInterstitialClosing event received");
	}
	
	public void HandleInterstitialClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleInterstitialClosed event received");


	}
	
	public void HandleInterstitialLeftApplication(object sender, EventArgs args)
	{
		Debug.Log("HandleInterstitialLeftApplication event received");
	}

}

