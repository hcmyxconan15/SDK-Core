using System;
using System.Collections.Generic;
using ConceptSystem.SeviceLocator.Scripts;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;
using Wolffun.Log;

namespace Watermelon
{
    public class AnalyticFirebase : MonoBehaviour
    {
        protected List<WaitEvent> _listWaitEvents = new List<WaitEvent>();
        protected Dictionary<string, object> objEvent = new Dictionary<string, object>();
        protected bool _isInitSuccess = false;

        protected virtual void Awake()
        {
            ServiceLocator.Global.Register(this);
            Firebase.RegisterOnDoneInitFirebaseCallback(dependencyStatus =>
            {
                if (dependencyStatus == DependencyStatus.Available)
                    Init();
            });
        }

        public void Init()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Set the recommended Crashlytics uncaught exception behavior.
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
#endif

            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
            _isInitSuccess = true;

            if (_listWaitEvents.Count > 0)
            {
                int count = _listWaitEvents.Count;
                for (int i = 0; i < count; i++)
                {
                    WaitEvent waitEvent = _listWaitEvents[i];
                    try
                    {
                        LogEventFirebase(waitEvent.eventKey, waitEvent.value);
                    }
                    catch (Exception ex)
                    {
                        LogDebugAnalytic(LogDebugType.EXCEPTION, waitEvent.eventKey, "AnalyticManager", "LogAnalytic",
                            ex.StackTrace.ToString());
                    }
                }
            }

            _listWaitEvents.Clear();
        }

        protected void LogEventFirebase(string eventName, Dictionary<string, object> values)
        {
            if (!_isInitSuccess)
            {
                var waitEvent = new WaitEvent
                {
                    eventKey = eventName,
                    value = values
                };
                if (!_listWaitEvents.Contains(waitEvent))
                {
                    _listWaitEvents.Add(waitEvent);
                }

                return;
            }

            try
            {
                CommonLog.Log($"LogEvent: {eventName}\n{Newtonsoft.Json.JsonConvert.SerializeObject(values)}");

                var listParam = new Parameter[values.Count];
                var index = 0;
                foreach (KeyValuePair<string, object> value in values)
                {
                    // if (value.Value == null)
                    //     continue;

                    if (value.Value == null)
                    {
                        listParam[index] = new Parameter(value.Key, "null");
                    }
                    else
                    {
                        var parameter = value.Value is double valueAsDouble
                            ? new Parameter(value.Key, valueAsDouble)
                            : new Parameter(value.Key, value.Value.ToString());
                        listParam[index] = parameter;
                    }

                    index++;
                }

#if UNITY_EDITOR
                CommonLog.Log($"LogEvent: {eventName}\n{Newtonsoft.Json.JsonConvert.SerializeObject(values)}");
                return;
#endif


#if (UNITY_ANDROID || UNITY_IOS)
                FirebaseAnalytics.LogEvent(eventName, listParam);
#endif
            }
            catch (Exception ex)
            {
                Debug.Log("Log event fail" + eventName + " | " + ex.Message);
            }
        }

        public void LogDebugAnalytic(LogDebugType logDebugType, string function, string className, string feature,
            string log)
        {
#if (UNITY_ANDROID || UNITY_IOS)
            objEvent.Clear();

            objEvent.Add("type", (int)logDebugType);
            objEvent.Add("function", function);
            objEvent.Add("className", className);
            objEvent.Add("feature", feature);
            if (log.Length >= 100)
                log = log.Substring(0, 100);
            objEvent.Add("log", log);
            objEvent.Add("TimeCur", Time.time);

            try
            {
                LogEventFirebase("LogDebug", objEvent);
            }
            catch (Exception ex)
            {
                CommonLog.LogError("Log debug exception - " + ex.StackTrace.ToString());
            }

#endif
        }

        #region USER PROPERTIES

        public async UniTaskVoid SetUserProperties()
        {
            await UniTask.WaitUntil(() => _isInitSuccess);

            FirebaseAnalytics.SetUserProperty("level", PlayerPrefs.GetInt("CurrentLevel", 0).ToString());
            FirebaseAnalytics.SetUserProperty("match_number", PlayerPrefs.GetInt("MatchNumber", 0).ToString());
            FirebaseAnalytics.SetUserProperty("number_inter", string.Empty);
            FirebaseAnalytics.SetUserProperty("number_rewarded", "0");
            FirebaseAnalytics.SetUserProperty("number_banner", string.Empty);
        }

        #endregion

        #region LOGIN SUCCESS

        public void LogLoginSuccess()
        {
            string eventName = "login_success";
            try
            {
                objEvent.Clear();

                objEvent.Add("match_number", PlayerPrefs.GetInt("MatchNumber", 0));
                objEvent.Add("level", PlayerPrefs.GetInt("CurrentLevel", 0) + 1);
                objEvent.Add("device_ram", SystemInfo.systemMemorySize);
                objEvent.Add("network_status", Application.internetReachability);

                LogEventFirebase(eventName, objEvent);
            }
            catch (Exception e)
            {
                CommonLog.LogError("LogLoginSuccess error: " + e.Message);
                LogDebugAnalytic(LogDebugType.EXCEPTION, eventName, "AnalyticManager", "LogAnalytic",
                    e.StackTrace);
            }
        }

        #endregion

        #region TRACKING ADS

        public void LogAdTrigger(AdTriggerParam adTriggerParam)
        {
            string eventName = "ad_trigger";
            try
            {
                // objEvent.Clear();
                // objEvent.Add(FirebaseAnalytics.ParameterAdPlatform, adTriggerParam.AdInfo.adNetwork.Equals("Google AdMob") ? "Google AdMob" : "AppLovin");
                // objEvent.Add(FirebaseAnalytics.ParameterAdSource, adTriggerParam.AdInfo.adNetwork);
                // objEvent.Add("ad_unit_id", adTriggerParam.AdInfo.adUnitId);
                // objEvent.Add(FirebaseAnalytics.ParameterAdUnitName, adTriggerParam.AdInfo.adType);
                // objEvent.Add("match_number", PlayerPrefs.GetInt("MatchNumber", 0));
                // objEvent.Add("level", PlayerPrefs.GetInt("CurrentLevel", 0) + 1);
                // objEvent.Add("screen", adTriggerParam.Screen);

                LogEventFirebase(eventName, objEvent);
            }
            catch (Exception e)
            {
                CommonLog.LogError("LogAdTrigger error: " + e.StackTrace);
                LogDebugAnalytic(LogDebugType.EXCEPTION, eventName, "AnalyticManager", "LogAnalytic",
                    e.StackTrace.ToString());
            }
        }

        public void LogAdImpression(AdImpressionParam adImpressionParam)
        {
            string eventName = "ad_impression";
            try
            {
                // objEvent.Clear();
                // objEvent.Add(FirebaseAnalytics.ParameterAdPlatform, "Google AdMob");
                // objEvent.Add(FirebaseAnalytics.ParameterAdSource, "Google AdMob");
                // objEvent.Add("ad_unit_id", adImpressionParam.AdInfo.adUnitId);
                // objEvent.Add(FirebaseAnalytics.ParameterAdUnitName, adImpressionParam.AdInfo.adType);
                // objEvent.Add(FirebaseAnalytics.ParameterAdFormat, adImpressionParam.AdInfo.adType);
                // objEvent.Add("match_number", PlayerPrefs.GetInt("MatchNumber", 0));
                // objEvent.Add("level", PlayerPrefs.GetInt("CurrentLevel", 0) + 1);
                // objEvent.Add("screen", adImpressionParam.Screen);
                // objEvent.Add("reward_type", adImpressionParam.Booster);
                // objEvent.Add("reward_amount", 1);
                // objEvent.Add(FirebaseAnalytics.ParameterValue, adImpressionParam.AdInfo.revenue);
                // objEvent.Add(FirebaseAnalytics.ParameterCurrency, "USD");

                LogEventFirebase(eventName, objEvent);
            }
            catch (System.Exception ex)
            {
                CommonLog.LogError($"LogEvent error {eventName}: {ex.GetBaseException()}\n{ex.StackTrace}");
                LogDebugAnalytic(LogDebugType.EXCEPTION, eventName, "AnalyticManager", "LogAnalytic",
                    ex.StackTrace.ToString());
            }
        }

        #endregion

        #region RATING

        public void LogRating(RatingParam ratingParam)
        {
            string eventName = "rating";
            try
            {
                objEvent.Clear();
                objEvent.Add("star", ratingParam.StarNum);
                objEvent.Add("button_click", ratingParam.ButtonClick);

                LogEventFirebase(eventName, objEvent);
            }
            catch (Exception e)
            {
                CommonLog.LogError("LogRating error: " + e.StackTrace);
                LogDebugAnalytic(LogDebugType.EXCEPTION, eventName, "AnalyticManager", "LogAnalytic",
                    e.StackTrace.ToString());
            }
        }

        #endregion

        #region ACTIVITY

        public void LogActivityExecute(ActivityExecuteParam activityExecute)
        {
            string eventName = "activity_execute";
            try
            {
                objEvent.Clear();
                objEvent.Add("activity_name", activityExecute.ActivityName);
                objEvent.Add("currency_type", activityExecute.CurrencyType);
                objEvent.Add("currency_amount", activityExecute.CurrencyAmount);
                objEvent.Add("group_name", activityExecute.GroupName);
                objEvent.Add("group_order", activityExecute.GroupOrder);
                objEvent.Add("sub_group_name", activityExecute.SubGroupName);
                objEvent.Add("sub_group_order", activityExecute.SubGroupOrder);
                LogEventFirebase(eventName, objEvent);
            }
            catch (Exception e)
            {
                CommonLog.LogError("LogActivity error: " + e.StackTrace);
                LogDebugAnalytic(LogDebugType.EXCEPTION, eventName, "AnalyticManager", "LogAnalytic", e.StackTrace);
            }
        }

        public void LogActivityComplete(ActivityCompleteParam activityComplete)
        {
            string eventName = "activity_complete";
            try
            {
                objEvent.Clear();
                objEvent.Add("activity_name", activityComplete.ActivityName);
                objEvent.Add("group_name", activityComplete.GroupName);
                objEvent.Add("group_order", activityComplete.GroupOrder);


                LogEventFirebase(eventName, objEvent);
            }
            catch (Exception e)
            {
                CommonLog.LogError("LogActivityComplete error: " + e.StackTrace);
                LogDebugAnalytic(LogDebugType.EXCEPTION, eventName, "AnalyticManager", "LogAnalytic", e.StackTrace);
            }
        }

        #endregion


        public struct AdTriggerParam
        {
            // public AdInfo AdInfo;
            public string Screen;
        }

        public struct AdImpressionParam
        {
            // public AdInfo AdInfo;
            public string Screen;
            public string Booster;
        }

        public struct ReviveParam
        {
            public string ReviveAt;
            public ReviveType ReviveType;
            public int CoinRevive;
        }

        public enum ReviveType
        {
            WATCH_ADS = 1,
            COIN = 2
        }

        public struct ShopOpenParam
        {
            public string Entry;
        }

        public struct RatingParam
        {
            public string ButtonClick;
            public int StarNum;
        }

        public struct ActivityExecuteParam
        {
            public string ActivityName;
            public string CurrencyType;
            public int CurrencyAmount;
            public string GroupName;
            public int GroupOrder;
            public string SubGroupName;
            public int SubGroupOrder;
        }

        public struct ActivityCompleteParam
        {
            public string ActivityName;
            public string GroupName;
            public int GroupOrder;
            public int Coin;
            public int Life;
        }

        public enum LogDebugType
        {
            NONE = 0,
            LOG = 1,
            ERROR = 2,
            EXCEPTION = 4
        }

        public struct WaitEvent
        {
            public string eventKey;
            public Dictionary<string, object> value;
        }
    }
}