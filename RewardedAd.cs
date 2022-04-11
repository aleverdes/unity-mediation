using System;
using System.Collections;
using Unity.Services.Mediation;
using UnityEngine;
namespace AffenCode
{
    public abstract class RewardedAd : MonoBehaviour
    {
        [SerializeField] private string _androidAdUnitId;
        [SerializeField] private string _iOSAdUnitId;

        public string AdUnitId => Application.platform == RuntimePlatform.Android ? _androidAdUnitId : _iOSAdUnitId;

        public AdState AdState => _ad?.AdState ?? AdState.Unloaded;
        
        private IRewardedAd _ad;

        public event Action Loaded;
        public event Action FailedToLoad;
        public event Action Shown;
        public event Action FailedToShow;
        public event Action UserRewarded;
        public event Action Closed;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => AdsManager.Initialized);
            
            _ad = MediationService.Instance.CreateRewardedAd(AdUnitId);
            
            _ad.OnLoaded += OnLoaded;
            _ad.OnFailedLoad += OnFailedToLoad;
            
            _ad.OnShowed += OnShown;
            _ad.OnFailedShow += OnFailedToShow;
            _ad.OnUserRewarded += OnUserRewarded;
            _ad.OnClosed += OnClosed;

            StartCoroutine(StartLoading());
        }

        private void OnDestroy()
        {
            _ad.OnLoaded -= OnLoaded;
            _ad.OnFailedLoad -= OnFailedToLoad;
            
            _ad.OnShowed -= OnShown;
            _ad.OnFailedShow -= OnFailedToShow;
            _ad.OnUserRewarded -= OnUserRewarded;
            _ad.OnClosed -= OnClosed;

            _ad = null;
        }

        public IEnumerator StartLoading()
        {
            yield return new WaitUntil(() => AdsManager.Initialized);
            Load();
        }

        public IEnumerator StartLoadingAndShow()
        {
            yield return new WaitUntil(() => AdsManager.Initialized);
            Load();
            yield return new WaitUntil(() => _ad.AdState == AdState.Loaded);
            Show();
        }

        public void Load()
        {
            if (_ad.AdState != AdState.Loaded)
            {
                _ad.Load();
            }
        }

        public void Show()
        {
            if (_ad.AdState == AdState.Loaded)
            {
                _ad.Show();
            }
            else
            {
                StartCoroutine(StartLoadingAndShow());
            }
        }
        
        protected virtual void OnLoaded(object sender, EventArgs eventArgs)
        {
            Debug.Log($"Ad {AdUnitId} loaded.");
            Loaded?.Invoke();
        }

        protected virtual void OnFailedToLoad(object sender, LoadErrorEventArgs eventArgs)
        {
            Debug.LogError($"Ad {AdUnitId} failed to load.");
            FailedToLoad?.Invoke();
        }

        protected virtual void OnShown(object sender, EventArgs args)
        {
            Debug.Log($"Ad {AdUnitId} shown successfully.");
            Shown?.Invoke();
            StartCoroutine(StartLoading());
        }

        protected virtual void OnUserRewarded(object sender, RewardEventArgs args)
        {
            Debug.Log($"Ad {AdUnitId} has rewarded user.");
            UserRewarded?.Invoke();
        }

        protected virtual void OnFailedToShow(object sender, ShowErrorEventArgs args)
        {
            Debug.LogError($"Ad {AdUnitId} failed to show.");
            FailedToShow?.Invoke();
        }

        protected virtual void OnClosed(object sender, EventArgs e)
        {
            Debug.Log($"Ad {AdUnitId} is closed.");
            Closed?.Invoke();
        }
    }
}
