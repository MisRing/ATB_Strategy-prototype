using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorCostDisplay : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private Text _text;
    [SerializeField] private Vector2 _offset = new Vector2(20, 20);

    private RectTransform _rectTransform;
    private PlayerInputController _inputController;

    public void Init(/*PlayerInputController inputController*/)
    {
        _rectTransform = GetComponent<RectTransform>();
        //_inputController = inputController;
        _text.enabled = false;
    }

    public void SetCost(int cost)
    {
        _text.enabled = true;
        _text.text = cost.ToString();
    }

    private void Update()
    {
        if (!_text.enabled) return;
        //_rectTransform.position = _inputController.MouseScreenPosition + _offset;
    }

    public void UnsetCost()
    {
        _text.enabled = false;
    }
}
