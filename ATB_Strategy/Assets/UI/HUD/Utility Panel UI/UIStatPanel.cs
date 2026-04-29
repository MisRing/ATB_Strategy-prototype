using UnityEngine;
using UnityEngine.UI;

public class UIStatPanel : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _statText;


    public void SetStatPanel(UnitController unit)
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(Close);

        _nameText.text = "Name: " + unit.Stats.Name;

        _statText.text = "";

        _statText.text += "Health: " + unit.Stats.Health.Value + " / " + unit.Stats.MaxHealth.ToString();
        _statText.text += "\n";

        _statText.text += "Armor: " + unit.Stats.Armor.Value + " / " + unit.Stats.MaxArmor.ToString();
        _statText.text += "\n";

        _statText.text += "Dodge: " + unit.Stats.Dodge.ToString();
        _statText.text += "\n";

        _statText.text += "Accuracy: " + unit.Stats.Accuracy.ToString();
        _statText.text += "\n";

        _statText.text += "Speed: " + unit.Stats.Speed.ToString();
        _statText.text += "\n";

        _statText.text += "Vision Range: " + unit.Stats.VisionRange.ToString();
        _statText.text += "\n";

        _statText.text += "Selfcontrol: " + unit.Stats.SelfControl.ToString();
        _statText.text += "\n";
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerInputController.Cancel += Close;
    }

    private void OnDisable()
    {
        PlayerInputController.Cancel -= Close;
    }
}
