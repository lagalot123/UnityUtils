#if UNITYUTILS_ADS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if APPODEAL
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif


namespace UnityUtils.Runtime {

#if APPODEAL

    public class AdsAppodeal : AdsImplementation, IAppodealInitializationListener, IRewardedVideoAdListener, IInterstitialAdListener {

#if UNITY_IOS
        public string appodealId = "";
#else
        public string appodealId = "";
#endif

        private AdsManager.Ads adTypes;
        public override void Init(AdsManager.Ads ads) {
            if (ads == AdsManager.Ads.None)
                return;

            int adTypes = 0;

            if (ads.HasFlag(AdsManager.Ads.Interstitial)) {
                adTypes |= Appodeal.INTERSTITIAL;
            }

            if (ads.HasFlag(AdsManager.Ads.Rewarded)) {
                adTypes |= Appodeal.REWARDED_VIDEO;
            }


            Appodeal.setLogLevel(Appodeal.LogLevel.None);
            Appodeal.setTesting(false);

            Appodeal.disableLocationPermissionCheck();
            Appodeal.setChildDirectedTreatment(false);
            Appodeal.muteVideosIfCallsMuted(true);

            Appodeal.setInterstitialCallbacks(this);
            Appodeal.setRewardedVideoCallbacks(this);

            Appodeal.cache(Appodeal.INTERSTITIAL);
            Appodeal.cache(Appodeal.REWARDED_VIDEO);

            Appodeal.initialize(appodealId, adTypes, (IAppodealInitializationListener)this);
        }

        public override bool IsInterstitialAdReady() {
            return Appodeal.isLoaded(Appodeal.INTERSTITIAL);
        }

        public override bool IsRewardedAdReady() {
            return Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
        }

        public void onInitializationFinished(List<string> errors) {
            //Debug.Log("onInitializationFinished " + (errors != null ? errors.Count + "" : "none"));
        }
        public void onInterstitialClicked() {

        }

        public void onInterstitialClosed() {
#if UNITY_IOS
                //Debug.Log("closed interstitial");
                Time.timeScale = 1; //TODO test if necessary
#endif
        }
        public void onInterstitialExpired() {
            //Debug.Log("onInterstitialExpired");
        }

        public void onInterstitialFailedToLoad() {
            //Debug.Log("onInterstitialFailedToLoad");
        }

        public void onInterstitialLoaded(bool isPrecache) {
            //Debug.Log("onInterstitialLoaded");
        }

        public void onInterstitialShowFailed() {
            //Debug.Log("onInterstitialShowFailed");
        }

        public void onInterstitialShown() {
            //Debug.Log("onInterstitialShown");
        }

        public void onRewardedVideoClicked() {

        }

        public void onRewardedVideoClosed(bool finished) {

        }

        public void onRewardedVideoExpired() {

        }

        public void onRewardedVideoFailedToLoad() {

        }

        public void onRewardedVideoFinished(double amount, string name) {
            RewardedVideoAdRewardedEvent();
        }

        public void onRewardedVideoLoaded(bool precache) {

        }

        public void onRewardedVideoShowFailed() {

        }

        public void onRewardedVideoShown() {

        }

        public override void ShowInterstitialAd() {
            if (IsInterstitialAdReady()) {
#if UNITY_IOS
            //Debug.Log("showing interstitial");
            Time.timeScale = 0; //TODO test if necessary
#endif
                Appodeal.show(Appodeal.INTERSTITIAL);
            }
        }

        public override void ShowRewardedAd() {
            if (IsRewardedAdReady())
                Appodeal.show(Appodeal.REWARDED_VIDEO);
        }

        void RewardedVideoAdRewardedEvent() {

        }
    }
#else
    public class AdsAppodeal : AdsImplementation {
        public override void Init(AdsManager.Ads ads) {
            throw new System.NotImplementedException();
        }

        public override bool IsInterstitialAdReady() {
            throw new System.NotImplementedException();
        }

        public override bool IsRewardedAdReady() {
            throw new System.NotImplementedException();
        }

        public override void ShowInterstitialAd() {
            throw new System.NotImplementedException();
        }

        public override void ShowRewardedAd() {
            throw new System.NotImplementedException();
        }
    }
#endif

}

#endif