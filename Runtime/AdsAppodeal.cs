#if UNITYUTILS_ADS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if APPODEAL

using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;

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
                adTypes |= AppodealShowStyle.Interstitial;
            }

            if (ads.HasFlag(AdsManager.Ads.Rewarded)) {
                adTypes |= AppodealShowStyle.RewardedVideo;
            }


            Appodeal.SetLogLevel(AppodealLogLevel.None);
            Appodeal.SetTesting(false);

            Appodeal.SetLocationTracking(false);
            Appodeal.SetChildDirectedTreatment(false);
            Appodeal.MuteVideosIfCallsMuted(true);

            Appodeal.SetInterstitialCallbacks(this);
            Appodeal.SetRewardedVideoCallbacks(this);

            Appodeal.Cache(AppodealShowStyle.Interstitial);
            Appodeal.Cache(AppodealShowStyle.RewardedVideo);

            Appodeal.Initialize(appodealId, adTypes, (IAppodealInitializationListener)this);
        }

        public override bool IsInterstitialAdReady() {
            return Appodeal.IsLoaded(AppodealShowStyle.Interstitial);
        }

        public override bool IsRewardedAdReady() {
            return Appodeal.IsLoaded(AppodealShowStyle.RewardedVideo);
        }

        public void OnInitializationFinished(List<string> errors) {
            //Debug.Log("onInitializationFinished " + (errors != null ? errors.Count + "" : "none"));
        }
        public void OnInterstitialClicked() {

        }

        public void OnInterstitialClosed() {
#if UNITY_IOS
                //Debug.Log("closed interstitial");
                Time.timeScale = 1; //TODO test if necessary
#endif
        }
        public void OnInterstitialExpired() {
            //Debug.Log("onInterstitialExpired");
        }

        public void OnInterstitialFailedToLoad() {
            //Debug.Log("onInterstitialFailedToLoad");
        }

        public void OnInterstitialLoaded(bool isPrecache) {
            //Debug.Log("onInterstitialLoaded");
        }

        public void OnInterstitialShowFailed() {
            //Debug.Log("onInterstitialShowFailed");
        }

        public void OnInterstitialShown() {
            //Debug.Log("onInterstitialShown");
        }

        public void OnRewardedVideoClicked() {

        }

        public void OnRewardedVideoClosed(bool finished) {

        }

        public void OnRewardedVideoExpired() {

        }

        public void OnRewardedVideoFailedToLoad() {

        }

        public void OnRewardedVideoFinished(double amount, string name) {
            RewardedVideoAdRewardedEvent();
        }

        public void OnRewardedVideoLoaded(bool precache) {

        }

        public void OnRewardedVideoShowFailed() {

        }

        public void OnRewardedVideoShown() {

        }

        public override void ShowInterstitialAd() {
            if (IsInterstitialAdReady()) {
#if UNITY_IOS
            //Debug.Log("showing interstitial");
            Time.timeScale = 0; //TODO test if necessary
#endif
                Appodeal.Show(AppodealShowStyle.Interstitial);
            }
        }

        public override void ShowRewardedAd() {
            if (IsRewardedAdReady())
                Appodeal.Show(AppodealShowStyle.RewardedVideo);
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