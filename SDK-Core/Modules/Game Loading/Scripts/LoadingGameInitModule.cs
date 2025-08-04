using UnityEngine;
using UnityEngine.Serialization;

namespace Watermelon
{
    [RegisterModule("Game Loading")]
    public class LoadingGameInitModule : InitModule
    {
        public override string ModuleName => "Game Loading Settings";

        [Tooltip("If manual mode is enabled, the loading screen will be active until GameLoading.MarkAsReadyToHide method has been called.")]
        [SerializeField] bool manualControlMode;
        [SerializeField] LoadingProgressUIBase _loadingUI;

        [SerializeField] LoadingProcessHandlerBase _loadingGameHandler;

        public override void CreateComponent(GameObject holderObject)
        {
            if(_loadingUI != null)
            {
                LoadingProgressUIBase tempObject = Instantiate(_loadingUI);
                // tempObject.transform.ResetGlobal();
                GameLoadingController.Initialise(new ()
                {
                    _loadingGameHandler
                }, tempObject);
                
                GameLoadingController.LoadingOpenGame();
            }
            
        }
    }
}