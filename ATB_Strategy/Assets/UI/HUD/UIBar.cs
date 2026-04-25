using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [SerializeField] private RectTransform _bar;
    [SerializeField] private Text _text;

    public void SetValue(float value, float maxValue)
    {
        float percent = value / maxValue;

        _bar.localScale = new Vector3(percent, 1f, 1f);
        if (_text)
        {
            _text.text = value + " / " + maxValue;
        }
    }

    public void SetActiveText(bool active)
    {
        if (_text)
        {
            _text.gameObject.SetActive(active);
        }
    }
}
