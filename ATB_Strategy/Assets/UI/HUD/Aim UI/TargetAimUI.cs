using UnityEngine;
using UnityEngine.UI;

public class TargetAimUI : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Text _hitChanceText;
    [SerializeField] private Text _critChanceText;
    [SerializeField] private Vector3 _offset;

    private Transform _target;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetTarget(Transform target, int hitChance, int critChance)
    {
        _target = target;
        _hitChanceText.text = hitChance.ToString() + "%";
        _critChanceText.text = critChance.ToString() + "%";
    }

    private void Update()
    {
        if (_target == null)
            return;
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        
        Vector2 halfScreenSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 screenPosition = new Vector3(screenPos.x, screenPos.y, 0);
        screenPosition -= halfScreenSize;

        _rectTransform.localPosition = screenPosition;
    }
}
