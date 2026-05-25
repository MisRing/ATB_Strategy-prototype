using UnityEngine;

public class HealthPointGUI : MonoBehaviour
{
    [SerializeField] private Vector3 _size = new Vector3(1f,0.5f, 1f);
    [SerializeField] private float _armorSizeMod = 1.25f;
    [SerializeField] private Color _healthFullColor = Color.forestGreen;
    [SerializeField] private Color _healthFullEnemyColor = Color.red;
    [SerializeField] private Color _armorFullColor = Color.gray3;
    [SerializeField] private Color _healthColor = Color.darkOliveGreen;
    [SerializeField] private Color _armorColor = Color.gray7;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetState(bool isArmor, bool isFull, bool isEnemy)
    {
        if (isArmor)
        {
            transform.localScale = _size * _armorSizeMod;
            _spriteRenderer.color = isFull ? _armorFullColor : _armorColor;
        }
        else
        {
            transform.localScale = _size;
            _spriteRenderer.color = isFull ? (isEnemy ? _healthFullEnemyColor : _healthFullColor) : _healthColor;
        }
    }
}
