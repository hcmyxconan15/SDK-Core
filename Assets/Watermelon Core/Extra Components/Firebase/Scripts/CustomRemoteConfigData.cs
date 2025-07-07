using System.Collections.Generic;
using Firebase.RemoteConfig;
using UnityEngine;

namespace Watermelon
{
    public abstract class CustomRemoteConfigData : ScriptableObject
    {
        public abstract Dictionary<string, object> GetDefaultRemoteConfig();
        
        public abstract void ApplyRemoteConfig(FirebaseRemoteConfig firebaseRemoteConfig);
    }
}