using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Watermelon
{
    public static class GameLoadingController
    {
        private static List<LoadingProcessHandlerBase> _loadingProcessHandlers;

        private static LoadingProgressUIBase _loadingProgressUI;

        private static LoadingProcessHandlerBase _curLoadingHandler;
        
        private static CancellationTokenSource _stopLoadingToken;

        public static void Initialise(List<LoadingProcessHandlerBase> loadingProcessHandlers, LoadingProgressUIBase loadingProgressUI)
        {
            _curLoadingHandler = null;
            _loadingProcessHandlers = loadingProcessHandlers;
            _loadingProgressUI = loadingProgressUI;
        }

        #region Loading Steps

        public static void LoadingOpenGame()
        {
            if (_curLoadingHandler != null)
            {
                Debug.LogError("Can't load LoadingStartToHome. Sth else is loading: " + _curLoadingHandler.GetType());
                return;
            }

            ShowScreen(true);

            CheckAndCreateToken();

            _curLoadingHandler = _loadingProcessHandlers[0];
            _curLoadingHandler.ResetData(_loadingProgressUI,
                () =>
                {
                    OnDoneLoadingProcess(true);
                });

            _curLoadingHandler.StartProcess(_stopLoadingToken.Token);
        }

        #endregion
        
        private static void OnDoneLoadingProcess(bool isBGFadeOut)
        {
            _curLoadingHandler = null;
            if (isBGFadeOut)
            {
                ShowScreen(false);
            }
        }
        
        private static void CheckAndCreateToken()
        {
            if (_stopLoadingToken != null)
            {
                _stopLoadingToken.Dispose();
            }

            _stopLoadingToken = new CancellationTokenSource();
        }
        
        public static void ShowScreen(bool isActive)
        {
            _loadingProgressUI.gameObject.SetActive(isActive);
        }
    }
}