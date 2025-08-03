using System;
using UnityEngine;

namespace Watermelon
{
    public abstract class LoadingProgressUIBase : MonoBehaviour
    {
        protected const float COMPLETE_PROGRESS = 0.9999f;

        [Header("Progress Animation"), BoxGroup("Progress Animation"), Space(12)] 
        [SerializeField] protected float _maxSpeedIncreasing = 2.0f;
        
        [Header("UI View"), BoxGroup("UI View"), Space(12)]
        [SerializeField] Camera _loadingCamera;
        
        [BoxGroup("UI View")]
        [SerializeField] private Canvas _canvas;

        protected float _animProgress = 0;
        protected float _currentProgress = 0f;

        protected IStringLoading _defaultLoadingStr;

        protected void Awake()
        {
            Application.targetFrameRate = 60;
            _canvas.worldCamera = Camera.main;
            SetupDefaultLoadingStr();
        }

        protected abstract void SetupDefaultLoadingStr();

        public virtual void ResetProgress(bool isShowProgress)
        {
            _animProgress = 0.01f;
            _currentProgress = 0.01f;
        }
        
        public void UpdateCurrentProgress(float amount)
        {
            _currentProgress = amount;
        }
        
        public bool IsAnimProgressDone()
        {
            return _animProgress >= COMPLETE_PROGRESS;
        }

        public void ForceDone()
        {
            _currentProgress = 1;
        }
        
        private void Update()
        {
            if (!(_animProgress < _currentProgress)) return;
            
            var deltaProgress = Mathf.Min(_maxSpeedIncreasing * Time.deltaTime, _currentProgress - _animProgress);

            if (!(deltaProgress > 0f)) return;
            
            _animProgress += deltaProgress;

            SetProgress(_animProgress);
        }
        
        protected abstract void SetProgress(float value);
        
        public abstract void SetTitle(IStringLoading title);
    }
}