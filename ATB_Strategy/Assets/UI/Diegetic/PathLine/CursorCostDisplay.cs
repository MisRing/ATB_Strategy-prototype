using UnityEngine;
using UnityEngine.UI;

public class CursorCostDisplay : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private Text _text;
    [SerializeField] private Vector2 _offset = new Vector2(20, 20);

    private RectTransform _rectTransform;

    public void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _text.gameObject.SetActive(false);
    }

    public void SetCost(int cost)
    {
        _text.gameObject.SetActive(true);
        _text.text = cost.ToString();
    }

    private void Update()
    {
        if (!_text.gameObject.activeSelf) return;
        Vector2 halfScreenSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 screenPosition = (PlayerInputController.MouseScreenPosition + _offset) - halfScreenSize;
        _rectTransform.localPosition = screenPosition;
    }

    public void UnsetCost()
    {
        _text.gameObject.SetActive(false);
    }
}
