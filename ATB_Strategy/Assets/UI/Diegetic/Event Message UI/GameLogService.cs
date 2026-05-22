using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLogService : MonoBehaviour
{
    private static GameLogService _instance;
    [SerializeField] private GameObject _messagePrefab;
    private const float MassageQueueDelay = 0.2f;
    private static float _massageQueueTime = 0f;
    private static Queue<MessageData> _queue;

    public static event Action<Transform, int, float> OnMessageFocus;

    public struct MessageData
    {
        public string message;
        public Color color;
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

        _massageQueueTime = MassageQueueDelay;
        MessageData data = _queue.Dequeue();
        
        StartCoroutine(ShowMassageCoroutine(data));
    }

    public static void ShowMessage(string message, Transform target, float delay, int priority, float focusTime, bool focusCamera = false)
    {
        MessageData data = new MessageData()
        {
            message = message,
            color = Color.aliceBlue,
            target = target,
            delay = delay,
            priority = priority,
            focusTime = focusTime,
            focusCamera = focusCamera
        };
        
        _queue.Enqueue(data);
    }
    
    public static void ShowMessageDeath(Transform target)
    {
        MessageData data = new MessageData()
        {
            message = "DEAD",
            color = Color.red,
            target = target,
            delay = 0,
            priority = 5,
            focusTime = 2f,
            focusCamera = true
        };
        
        _queue.Enqueue(data);
    }

    private IEnumerator ShowMassageCoroutine(MessageData data)
    {
        if (data.focusCamera)
        {
            OnMessageFocus?.Invoke(data.target, data.priority, data.focusTime);
        }
        if (data.delay > 0)
        {
            GridTile tile = GridParameters.LevelGrid.GetTileByWorldPos(data.target.position);
            if (tile != null)
            {
                FogOfWarUtility.ForceVisibility(tile, data.focusTime);
            }
            
            TurnManager.SetEventSlowdown(data.focusTime, 0.25f);
        }

        while (data.delay > 0)
        {
            data.delay -= TimeService.TimeSpeedDelta;

            yield return null;
        }
        
        GameObject messageObj = Instantiate(_messagePrefab);
        messageObj.GetComponent<Message>().SetMessage(data.message, data.color, data.target.position);
    }
}
