#if UNITYUTILS_ADS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityUtils.Runtime {
    public class AdsManager : MonoBehaviour {

        public Vector2Int sceneLoadsBeforeAdMinMax = Vector2Int.one;
        protected int loadCount = 0;

        public float minSecondsBetweenInterstitials = 60;
        public float maxSecondsBetweenInterstitials = 120;
        protected float lastAd = 30;

        public AdsImplementation ads;
        public List<string> noInterstitialScenes = new();

        internal bool setup;

        [System.Flags, System.Serializable]
        public enum Ads {
            None = 0,
            Interstitial = 1,
            Rewarded = 2
        }

        public Ads adTypes = Ads.Interstitial;


        void Start() {
            Init();
        }

        virtual public void Init() {
            if (setup)
                return;

            //sceneLoadsBeforeAd = 0;
            loadCount = UnityEngine.Random.Range(sceneLoadsBeforeAdMinMax.x, sceneLoadsBeforeAdMinMax.y);
            lastAd = -30;

#if UNITY_EDITOR
            if (ads == null) Debug.LogError("Missing ads implementation");

            adTypes = Ads.None;
#endif
            ads.Init(adTypes);

            setup = true;
        }

        virtual public void OnLevelLoaded() {
            if (!setup || !adTypes.HasFlag(Ads.Interstitial))
                return;

            if (ShouldSceneLoadTriggerAd()) {
                loadCount--;
            } else {
                return;
            }
            
            if (ShowInterstitialAfterSceneLoad()) {
                if (IsInterstitialAdReady()) {
                    lastAd = Time.realtimeSinceStartup;
                    loadCount = UnityEngine.Random.Range(sceneLoadsBeforeAdMinMax.x, sceneLoadsBeforeAdMinMax.y);

                    ShowInterstitialAd();
                }
            }
        }

        virtual public bool ShouldSceneLoadTriggerAd() {
            return !noInterstitialScenes.Contains(SceneManager.GetActiveScene().name);
        }

        virtual public bool ShowInterstitialAfterSceneLoad() {
            return (loadCount <= 0 && Time.realtimeSinceStartup - lastAd >= minSecondsBetweenInterstitials) || Time.realtimeSinceStartup - lastAd >= maxSecondsBetweenInterstitials;
        }

        bool IsInterstitialAdReady() {
            if (!adTypes.HasFlag(Ads.Interstitial))
                return false;

            return ads.IsInterstitialAdReady();
        }

        void ShowInterstitialAd() {
            if (IsInterstitialAdReady())
                ads.ShowInterstitialAd();
        }

        public void IncreaseLoadCount(int c) {
            loadCount -= c;
        }

        public bool IsRewardedAdReady() {
            if (!adTypes.HasFlag(Ads.Rewarded))
                return false;

            return ads.IsRewardedAdReady();
        }

        public void OnRewardedAdFinished() {
            if (onRewardedAdFinished != null) {
                onRewardedAdFinished();
            }
        }

        public void ShowRewardedAd(Action callback) {
            onRewardedAdFinished = callback;

            if (IsRewardedAdReady())
                ads.ShowRewardedAd();
        }

        public Action onRewardedAdFinished;
    }
}

#endif