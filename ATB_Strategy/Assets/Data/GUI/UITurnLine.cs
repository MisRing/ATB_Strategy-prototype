using UnityEngine;
using UnityEngine.UI;

public class UITurnLine : MonoBehaviour
{
    [SerializeField] private Text _currentTurnText;

    private void Update()
    {
        _currentTurnText.text = TurnManager.CurrentTurn.ToString();
    }
}
