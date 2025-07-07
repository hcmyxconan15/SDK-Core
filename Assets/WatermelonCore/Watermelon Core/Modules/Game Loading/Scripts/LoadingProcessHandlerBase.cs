using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
#else
#endif

namespace Watermelon
{
    public abstract class LoadingProcessHandlerBase : ScriptableObject
    {
#if UNITASK_ENABLED
        protected delegate UniTask<bool> LoadingProcess(float inTotalPercent, CancellationToken inCancellationToken);
#else
        protected delegate Task<bool> LoadingProcess(float inTotalPercent, CancellationToken inCancellationToken);
#endif
        protected struct LoadingStep
        {
            public LoadingStep(LoadingProcess inActionToDo, float inTotalPercentForThisStep, IStringLoading inStepTitle)
            {
                actionToDo = inActionToDo;
                totalPercentForThisStep = inTotalPercentForThisStep;
                stepTitle = inStepTitle;
            }
            public LoadingProcess actionToDo;
            public float totalPercentForThisStep;
            public IStringLoading stepTitle;
        }

        [SerializeField] protected bool _isShowProgress = true;

        [SerializeField] protected string _loadingName;

        protected List<LoadingStep> _listSteps;
        
        protected LoadingProgressUIBase _loadingProgressUIBase;
        
        protected event Action _onDoneLoadProcessCallBack;
        
        protected float _curPercentageProcess = 0f;
        
        protected CancellationToken _currentCancellationToken;

        protected const int TIME_OUT_CONNECT_TO_SERVER = 15000;
        
        protected abstract void InitStep();
        
        public virtual void ResetData(LoadingProgressUIBase inLoadingProgressUI, Action inCallbackOnDoneLoadProcess)
        {
            _loadingProgressUIBase = inLoadingProgressUI;
            _onDoneLoadProcessCallBack = inCallbackOnDoneLoadProcess;
            _curPercentageProcess = 0;
            InitStep();

            _loadingProgressUIBase.ResetProgress(_isShowProgress);
            _loadingProgressUIBase.gameObject.SetActive(false);

            OnBeforeStartProcess();
        }
        
        protected virtual void OnBeforeStartProcess()
        {

        }
        
        protected virtual async UniTask OnDoneProcess()
        {
            Debug.Log("OnDoneProcess");
            _loadingProgressUIBase.ForceDone();
            
#if UNITASK_ENABLED
            await UniTask.WaitUntil(_loadingProgressUIBase.IsAnimProgressDone);
#else
            while (!_loadingProgressUIBase.IsAnimProgressDone)
            {
                await Task.Delay(16); // ~60fps polling
            }
#endif
            _loadingProgressUIBase.gameObject.SetActive(false);
            _onDoneLoadProcessCallBack?.Invoke();
        }
        
                
        protected virtual void OnBeforeProcessEachStep(int stepIndex){}

        protected virtual void OnAfterProcessEachStep(int stepIndex, bool isDone){}

        protected virtual void OnOperationCanceledExceptionCatch(OperationCanceledException ocex, int stepIndex){}

        protected virtual void OnNormalExceptionCatch(Exception ex, int stepIndex){}
        
        public async void StartProcess(CancellationToken cancellationToken)
        {
            _currentCancellationToken = cancellationToken;
            int index = 0;
            try
            {
                _loadingProgressUIBase.gameObject.SetActive(true);

                for (; index < _listSteps.Count; index++)
                {
                    OnBeforeProcessEachStep(index);

                    _loadingProgressUIBase.SetTitle(_listSteps[index].stepTitle);
                    float totalStepProgress = _listSteps[index].totalPercentForThisStep;

                    bool isDoneStep = false;

                    isDoneStep = await _listSteps[index].actionToDo.Invoke(totalStepProgress, _currentCancellationToken);

                    OnAfterProcessEachStep(index, isDoneStep);

                    if (!isDoneStep)
                    {
                        Debug.Log("StartProcess Step = " + index + " DONE = " + isDoneStep);

                        return;
                    }

                }
                OnDoneProcess().Forget();
            }
            catch (OperationCanceledException ocex)
            {
                Debug.LogError(this.GetType() + " is handle cancel token at: " + index);
                Debug.LogError("StartProcess error trace:\n" + ocex.StackTrace);

                OnOperationCanceledExceptionCatch(ocex, index);
            }
            catch (Exception ex)
            {
                Debug.LogError("StartProcess error trace: + " + ex.GetBaseException() + "\n" + ex.StackTrace);

                OnNormalExceptionCatch(ex, index);
            }
        }
        
        protected void UpdateProgress(float totalPercentage, int totalMinorStep)
        {
            Debug.Log("UpdateProgress: " + _curPercentageProcess + "__" + (totalPercentage / totalMinorStep));
            _curPercentageProcess += totalPercentage / totalMinorStep;
            _loadingProgressUIBase.UpdateCurrentProgress(_curPercentageProcess);
        }
    }
}