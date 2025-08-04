using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Watermelon
{
    public class LoadingProgressUI : LoadingProgressUIBase
    {
        protected const string LOADING_PROGRESS_TEXT = "{0}%";
        
        protected string VERSION_FORMAT = "version {0}";
        
        [Header("Loading progress"), BoxGroup("Loading progress")]
        [SerializeField] protected TextMeshProUGUI _txtProgress;
        
        [Space(10), BoxGroup("Loading progress")]
        [SerializeField] protected Slider _imgProgress;
        
        [BoxGroup("Loading progress")]
        [SerializeField] protected Image _imgSlider;
        
        [BoxGroup("Loading progress")]
        [SerializeField] protected TextMeshProUGUI _txtLoadingTitleWithProgress;

        [Space(10), BoxGroup("Loading progress")]
        [SerializeField] protected TextMeshProUGUI _txtLoadingTitleWithoutProgress;
        
        protected TextMeshProUGUI _txtCurUsing;
        
        public override void ResetProgress(bool isShowProgress)
        {
            base.ResetProgress(isShowProgress);

            SetProgress(_currentProgress);

            if (isShowProgress)
            {
                _txtLoadingTitleWithProgress?.gameObject.SetActive(true);
                _txtProgress?.gameObject.SetActive(true);
                _imgProgress?.gameObject.SetActive(true);

                _txtLoadingTitleWithoutProgress?.gameObject.SetActive(false);

                _txtCurUsing = _txtLoadingTitleWithProgress;  
            }
            else
            {
                _txtLoadingTitleWithProgress?.gameObject.SetActive(false);
                _txtProgress?.gameObject.SetActive(false);
                _imgProgress?.gameObject.SetActive(false);

                _txtLoadingTitleWithoutProgress?.gameObject.SetActive(true);

                _txtCurUsing = _txtLoadingTitleWithoutProgress;
            }

            SetTitle(_defaultLoadingStr);
        }

        public override async void SetTitle(IStringLoading title)
        {
            if (_txtCurUsing && title != null)
            {
                _txtCurUsing.text = await title.GetStringAsync();
            }
        }
        
        protected override void SetupDefaultLoadingStr()
        {
            _defaultLoadingStr = new InCodeStringLoading("Loading...");
        }

        protected override void SetProgress(float value)
        {
            if (_imgProgress)
            {
                _imgProgress.value = value;
            }
            
            if (_imgSlider)
            {
                _imgSlider.fillAmount = value;
            }

            if (_txtProgress)
            {
                _txtProgress.text = string.Format(LOADING_PROGRESS_TEXT, (value * 100));
            }
        }
    }
}