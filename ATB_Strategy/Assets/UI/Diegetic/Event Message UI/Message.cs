using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float _maxY = 1.5f;
    [SerializeField] private float _lifeTime = 2f;
    [SerializeField] private Text _text;

    public void SetMessage(string message, Transform target)
    {
        _text.text = message;
        transform.position = target.position;

        StartCoroutine(Move(target));
    }

    private IEnumerator Move(Transform target)
    {
        float time = 0;
        
        Vector3 endPosition = new Vector3(0, _maxY, 0);

        float yPos = 0;

        while (time < _lifeTime)
        {
            transform.rotation = Camera.main.transform.rotation;
            yPos = Mathf.Lerp(yPos, _maxY, (time / _lifeTime));
            transform.position = target.position + new Vector3(0, yPos, 0);
            
            time += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
