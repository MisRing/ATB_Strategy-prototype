using System.Collections;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float _maxY = 1.5f;
    [SerializeField] private float _lifeTime = 2f;
    [SerializeField] private TextMeshPro _text;

    public void SetMessage(string message, Transform target)
    {
        _text.text = message;
        transform.position = target.position;

        StartCoroutine(Move(target));
    }

    private IEnumerator Move(Transform target)
    {
        float time = 0;
        float yPos = 0;

        while (time < _lifeTime)
        {
            transform.rotation = Camera.main.transform.rotation;
            float t = Mathf.SmoothStep(0f, 1f, time / _lifeTime);
            yPos = Mathf.Lerp(yPos, _maxY, t);
            transform.position = target.position + new Vector3(0, yPos, 0);
            
            time += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
