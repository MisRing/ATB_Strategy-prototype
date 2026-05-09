using UnityEngine;
using System.Collections.Generic;

public class TileCursor : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private GameObject _spriteObject;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.1f, 0);

    [Header("Cover settings")]
    [SerializeField] private List<SpriteRenderer> _covers;
    [SerializeField] private Sprite _fullCover;
    [SerializeField] private Sprite _lowCover;

    public void Init()
    {
        _spriteObject.SetActive(false);
        foreach (SpriteRenderer sprite in _covers)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    public void SetTileCursor(GridTileDemo tile)
    {
        if(tile == null)
        {
            _spriteObject.SetActive(false);
            return;
        }

        _spriteObject.SetActive(true);
        transform.position = tile.WorldPosition + _offset;

        for (int i = 0; i < 4; i++)
        {
            switch (tile.Covers[i])
            {
                case(TileCover.Low) :
                    _covers[i].gameObject.SetActive(true);
                    _covers[i].sprite = _lowCover;
                    break;
                case(TileCover.Full) :
                    _covers[i].gameObject.SetActive(true);
                    _covers[i].sprite = _fullCover;
                    break;
                
                default:
                    _covers[i].gameObject.SetActive(false);
                    break;
            }
        }
    }

    public void UnsetTileCursor()
    {
        _spriteObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            _covers[i].gameObject.SetActive(false);
        }
    }
}
