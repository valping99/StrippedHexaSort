using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    [SerializeField] private RaiseStateEventSO _raiseStateEvent;
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _sliderValueText;

    [SerializeField] private float _timeDuration = .5f;

    public void SetupProgressBar(float maxValue)
    {
        _slider.minValue = 0;
        _slider.maxValue = maxValue;
        _slider.value = 0;

        UpdateProgress(_slider.value);
    }

    public void UpdateProgress(float value)
    {
        _slider.DOValue(_slider.value + value, _timeDuration).SetEase(Ease.Linear);

        DOTween.To(() => float.Parse(_sliderValueText.text.Split('/')[0].Trim()),
            x => _sliderValueText.text = $"{Mathf.RoundToInt(x)} / {_slider.maxValue}",
            _slider.value + value, _timeDuration).SetEase(Ease.Linear).OnComplete(() => CheckProgress());
        _slider.value += value;
    }

    private void CheckProgress()
    {
        if (_slider.value >= _slider.maxValue)
        {
            _raiseStateEvent?.RaiseEvent(EGameState.NextLevel);
        }
    }
}