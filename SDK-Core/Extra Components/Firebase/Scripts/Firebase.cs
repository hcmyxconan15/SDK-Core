using System;
using UnityEngine;
#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
#if FIREBASE_ENABLED
using Firebase;
using Firebase.Extensions;
#endif

namespace Watermelon
{
    public static class Firebase
    {
        private static bool _isInitialized;
#if FIREBASE_ENABLED
        private static DependencyStatus _dependencyStatus;
        private static Action<DependencyStatus> _onDoneInitFirebaseCallback;
#endif
#if UNITASK_ENABLED
        public static async UniTask InitSDK()
#else
        public static async Task InitSDK()
#endif
        {
#if FIREBASE_ENABLED
            try
            {
                await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    _dependencyStatus = task.Result;
                    if (_dependencyStatus != DependencyStatus.Available)
                    {
                        CommonLog.LogError($"Could not resolve all Firebase dependencies: {_dependencyStatus}");
                    }

                    _onDoneInitFirebaseCallback?.Invoke(_dependencyStatus);
                    _isInitialized = true;

                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Firebase SDK with exception: {e}");
            }
#endif
        }
        
#if FIREBASE_ENABLED
        public static void RegisterOnDoneInitFirebaseCallback(Action<DependencyStatus> callback)
        {
            if (_isInitialized)
            {
                callback?.Invoke(_dependencyStatus);
                return;
            }

            _onDoneInitFirebaseCallback += callback;
        }
#endif
    }
}