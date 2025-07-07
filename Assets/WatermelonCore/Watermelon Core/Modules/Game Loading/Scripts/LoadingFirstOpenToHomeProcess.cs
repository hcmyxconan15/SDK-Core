using System.Collections.Generic;
using System.Threading;
using ConceptSystem.SeviceLocator.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "LoadingFirstOpenToHomeProcess", menuName = "Core/Game Loading/Loading First Open To Home Process")]
    public class LoadingFirstOpenToHomeProcess : LoadingProcessHandlerBase
    {
        [SerializeField] private SceneObject _sceneObject;
        protected override void InitStep()
        {
            _listSteps = new List<LoadingStep>()
            {
                new LoadingStep(WaitForInit, 0.15f, null),
                new LoadingStep(LoggingInCheck, 0.4f, null),
                new LoadingStep(LoadingScene,0.45f, null)
            };
        }
        
        private async UniTask<bool> LoggingInCheck(float percentageThisStep, CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(2);
            UpdateProgress(percentageThisStep, 1);
            return true;
        }
        
        private async UniTask<bool> WaitForInit(float percentageThisStep, CancellationToken cancellationToken)
        {
            // Init SDK, ...
            ServiceLocator.Global.TryGet(out RemoteConfigFirebase remoteConfigFirebase);
            if (remoteConfigFirebase)
                await UniTask.WaitUntil(() => remoteConfigFirebase.IsDoneFetch);
            
            UpdateProgress(percentageThisStep, 1);
            return true;
        }

        private async UniTask<bool> LoadingScene(float percentageThisStep, CancellationToken cancellationToken)
        {
#if ADDRESSABLE_ENABLED
            await Addressables.LoadSceneAsync(_sceneObject.Name, LoadSceneMode.Single);
#else
            SceneManager.LoadScene(_sceneObject.Name, LoadSceneMode.Single);
#endif
            ServiceLocator.Global.TryGet(out AnalyticFirebase analyticManager);
            // if (analyticManager)
            // {
                analyticManager.SetUserProperties().Forget();
                analyticManager.LogLoginSuccess();
            // }
            UpdateProgress(percentageThisStep, 1);
            return true;
        }
    }
}