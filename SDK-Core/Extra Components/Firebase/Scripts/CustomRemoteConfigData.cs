using System.Collections.Generic;
#if FIREBASE_ENABLED
using Firebase.RemoteConfig;
#endif
using UnityEngine;

namespace Watermelon
{
    public abstract class CustomRemoteConfigData : ScriptableObject
    {
        public abstract Dictionary<string, object> GetDefaultRemoteConfig();
        
#if FIREBASE_ENABLED
        public abstract void ApplyRemoteConfig(FirebaseRemoteConfig firebaseRemoteConfig);
#endif
    }
}