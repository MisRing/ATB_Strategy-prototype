using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameLogService : MonoBehaviour
{
    private static GameLogService _instance;
    [SerializeField] private GameObject _messagePrefab;
    private static float _massageQueueDelay = 0.1f;
    private static float _massageQueueTime = 0f;
    private static Queue<MessageData> _queue;

    public static event Action<Transform, int, float> OnMessageFocus;
    
    private struct MessageData
    {
        public string message;
        public Transform target;
        public float delay;
        public int priority;
        public float focusTime;
        public bool focusCamera;
    }

    private void Awake()
    {
        _instance = this;
        _queue = new Queue<MessageData>();
    }

    private void Update()
    {
        _massageQueueTime -= Time.deltaTime;

        if (_massageQueueTime > 0f) return;

        if (_queue.Count <= 0) return;

        _massageQueueTime = _massageQueueDelay;
        MessageData data = _queue.Dequeue();
        
        StartCoroutine(ShowMassageCoroutine(data.message, data.target, data.delay, data.priority, data.focusTime, data.focusCamera));
    }

    public static void ShowMessage(string message, Transform target, float delay, int priority, float focusTime, bool focusCamera = false)
    {
        MessageData data = new MessageData()
        {
            message = message,
            target = target,
            delay = delay,
            priority = priority,
            focusTime = focusTime,
            focusCamera = focusCamera
        };
        
        _queue.Enqueue(data);
    }

    public IEnumerator ShowMassageCoroutine(string message, Transform target, float delay, int priority, float focusTime, bool focusCamera)
    {
        if (focusCamera)
        {
            OnMessageFocus?.Invoke(target, priority, focusTime);
        }

        while (delay > 0)
        {
            delay -= TimeService.TimeSpeedDelta;

            yield return null;
        }
        
        GameObject messageObj = Instantiate(_messagePrefab);
        messageObj.GetComponent<Message>().SetMessage(message, target.position);
    }
}
