using System;
using UnityEngine;

public class HealthPointGUI : MonoBehaviour
{
    [SerializeField] private Vector3 _size = new Vector3(1f,0.5f, 1f);
    [SerializeField] private float _armorSizeMod = 1.25f;
    [SerializeField] private Color _healthFullColor = Color.forestGreen;
    [SerializeField] private Color _armorFullColor = Color.gray3;
    [SerializeField] private Color _healthColor = Color.darkOliveGreen;
    [SerializeField] private Color _armorColor = Color.gray7;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetState(bool isArmor, bool isFull)
    {
        if (isArmor)
        {
            transform.localScale = _size * _armorSizeMod;
            if (isFull)
            {
                _spriteRenderer.color = _armorFullColor;
            }
            else
            {
                _spriteRenderer.color = _armorColor;
            }
        }
        else
        {
            transform.localScale = _size;
            if (isFull)
            {
                _spriteRenderer.color = _healthFullColor;
            }
            else
            {
                _spriteRenderer.color = _healthColor;
            }
        }
    }
}
