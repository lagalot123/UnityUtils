#if UNITYUTILS_ADS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils.Runtime {
    public abstract class AdsImplementation : MonoBehaviour {
        abstract public void Init(AdsManager.Ads ads);
        abstract public bool IsInterstitialAdReady();
        abstract public void ShowInterstitialAd();
        abstract public bool IsRewardedAdReady();
        abstract public void ShowRewardedAd();
    }
}

#endif