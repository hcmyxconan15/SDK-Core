using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if FIREBASE_ENABLED
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
#endif
using UnityEngine;

namespace Watermelon
{
    public class RemoteConfigFirebase : MonoBehaviour
    {
        [SerializeField] private int fetchIntervalSecond = 300;
        [SerializeField] private CustomRemoteConfigData[] listCustomRemoteConfigData;
        private bool _isDoneFetch = false;
        public bool IsDoneFetch => _isDoneFetch;

        Dictionary<string, object> _defaultConfigs = new Dictionary<string, object>();

        public List<string> versions = new();

        protected void Awake()
        {
#if FIREBASE_ENABLED
            ServiceLocator.Global.Register(this);

            Firebase.RegisterOnDoneInitFirebaseCallback(dependencyStatus => { InitRemoteConfigs(); });
#endif
        }

        public void InitRemoteConfigs()
        {
            //
#if (UNITY_ANDROID || UNITY_IOS)
            _isDoneFetch = false;
            InitDefaultConfigs();
            FetchingConfigFromRemoteConfig();
#else
        _isDoneFetch = true;
        InitDefaultConfigs();
#endif
        }

        public void InitDefaultConfigs()
        {
            _defaultConfigs.Clear();
            foreach (var customRemoteConfigData in listCustomRemoteConfigData)
            {
                try
                {
                    var customDefaultConfigData = customRemoteConfigData.GetDefaultRemoteConfig();

                    if (customDefaultConfigData != null)
                    {
                        foreach (var kp in customDefaultConfigData)
                            _defaultConfigs.TryAdd(kp.Key, kp.Value);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        $"Exception when get DefaultConfig of {customRemoteConfigData.GetType()}: {e.Message}");
                }
            }


            versions = new() { Application.version };
            _defaultConfigs["Version"] = versions;
#if FIREBASE_ENABLED
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(_defaultConfigs);
#endif
        }

        private void FetchingConfigFromRemoteConfig()
        {
#if FIREBASE_ENABLED
            try
            {
                var firebaseRemote = FirebaseRemoteConfig.DefaultInstance;
                TimeSpan timespan = new TimeSpan(0, 0, fetchIntervalSecond);
                Task fetchTask = firebaseRemote.FetchAsync(timespan);
                fetchTask.ContinueWithOnMainThread(async task =>
                    {
                        try
                        {
                            await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                            var info = firebaseRemote.Info;
                            switch (info.LastFetchStatus)
                            {
                                case LastFetchStatus.Success:
                                    //FirebaseRemoteConfig.ActivateFetched();
                                    Debug.LogError(
                                        $"Remote data loaded and ready (last fetch time {info.FetchTime}).");
                                    break;
                                case LastFetchStatus.Failure:
                                    Debug.LogError($"Failure {info.LastFetchFailureReason}");

                                    switch (info.LastFetchFailureReason)
                                    {
                                        case FetchFailureReason.Error:
                                            Debug.LogError("Fetch failed for unknown reason");
                                            break;
                                        case FetchFailureReason.Throttled:
                                            Debug.LogError($"Fetch throttled until {info.ThrottledEndTime}");
                                            break;
                                        case FetchFailureReason.Invalid:
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }

                                    _isDoneFetch = true;
                                    return;
                                case LastFetchStatus.Pending:
                                    Debug.LogError("Remote Config pending....");
                                    _isDoneFetch = true;
                                    break;
                            }

                            versions = JsonConvert.DeserializeObject<List<string>>(firebaseRemote.GetValue("Version")
                                .StringValue);

                            if (listCustomRemoteConfigData != null)
                            {
                                foreach (var customRemoteConfigData in listCustomRemoteConfigData)
                                {
                                    try
                                    {
                                        customRemoteConfigData.ApplyRemoteConfig(firebaseRemote);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.LogError(
                                            $"Exception when apply RemoteConfig into {customRemoteConfigData.GetType()}: {e.Message}");
                                    }
                                }
                            }

                            _isDoneFetch = true;
                            Debug.Log("Fetch Remote Data success");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("RemoteConfigManager get data is error: " + ex.GetBaseException() +
                                               "\n" +
                                               ex.StackTrace);
                            _isDoneFetch = true;
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                _isDoneFetch = true;
            }
#endif
        }
    }

    [Serializable]
    public class IntervalShowInterstitialAds
    {
        public int LowerMatchFrequencyThreshold;
        public int UpperMatchFrequencyThreshold;
        public int IntervalShowAdsInGame;
        public int IntervalShowAdsInMenu;
    }
}