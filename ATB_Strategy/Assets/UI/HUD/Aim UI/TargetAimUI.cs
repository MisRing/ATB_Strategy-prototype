using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetAimUI : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TextMeshProUGUI _hitChanceText;
    [SerializeField] private TextMeshProUGUI _critChanceText;
    [SerializeField] private Vector3 _offset;

    [Header("Animation settings")]
    [SerializeField] private float _aimFollowSpeed = 5f;
    [SerializeField] private Image _hitSlider;
    [SerializeField] private Image _critSlider;
    [SerializeField] private float _chanceFollowSpeed = 10f;
    [SerializeField] private RectTransform _side0;
    [SerializeField] private RectTransform _side1;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private float _speedMultiplayer = 1.5f;

    private Transform _target;
    private RectTransform _rectTransform;

    private int _hitchance;
    private int _critchance;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _hitSlider.fillAmount = 0f;
        _critSlider.fillAmount = 0f;
    }

    public void SetTarget(Transform target, int hitChance, int critChance)
    {
        _target = target;

        _hitchance = hitChance;
        _critchance = critChance;

        _hitChanceText.text = hitChance.ToString() + "%";
        _critChanceText.text = critChance.ToString() + "%";
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        
        Vector2 halfScreenSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 screenPosition = new Vector3(screenPos.x, screenPos.y, 0);
        screenPosition -= halfScreenSize;

        _rectTransform.localPosition = Vector2.Lerp(_rectTransform.localPosition, screenPosition, Time.deltaTime * _aimFollowSpeed);

        _side0.eulerAngles = new Vector3(0f, 0f, Mathf.Repeat(_side0.eulerAngles.z - _rotationSpeed * Time.deltaTime, 360f));
        _side1.eulerAngles = new Vector3(0f, 0f, Mathf.Repeat(_side1.eulerAngles.z + _rotationSpeed * Time.deltaTime * _speedMultiplayer, 360f));

        _hitSlider.fillAmount = Mathf.Lerp(_hitSlider.fillAmount, _hitchance / 100f, Time.deltaTime * _chanceFollowSpeed);
        _critSlider.fillAmount = Mathf.Lerp(_critSlider.fillAmount, _critchance / 100f, Time.deltaTime * _chanceFollowSpeed);
    }
}
