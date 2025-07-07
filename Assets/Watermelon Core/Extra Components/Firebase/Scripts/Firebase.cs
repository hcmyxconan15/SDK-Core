using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Wolffun.Log;

namespace Watermelon
{
    public static class Firebase
    {
        private static bool _isInitialized;
        private static DependencyStatus _dependencyStatus;
        private static Action<DependencyStatus> _onDoneInitFirebaseCallback;

        public static async UniTask InitSDK()
        {
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
                CommonLog.LogError($"Failed to initialize Firebase SDK with exception: {e}");
            }
        }

        public static void RegisterOnDoneInitFirebaseCallback(Action<DependencyStatus> callback)
        {
            if (_isInitialized)
            {
                callback?.Invoke(_dependencyStatus);
                return;
            }

            _onDoneInitFirebaseCallback += callback;
        }
    }
}