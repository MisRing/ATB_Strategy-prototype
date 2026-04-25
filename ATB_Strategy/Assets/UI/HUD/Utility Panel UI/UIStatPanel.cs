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

        _nameText.text = "Name: " + unit.UnitStats.Name;

        _statText.text = "";

        _statText.text += "Health: " + unit.UnitStats.Health.Value + " / " + unit.UnitStats.MaxHealth.ToString();
        _statText.text += "\n";

        _statText.text += "Armor: " + unit.UnitStats.Armor.Value + " / " + unit.UnitStats.MaxArmor.ToString();
        _statText.text += "\n";

        _statText.text += "Dodge: " + unit.UnitStats.Dodge.ToString();
        _statText.text += "\n";

        _statText.text += "Accuracy: " + unit.UnitStats.Accuracy.ToString();
        _statText.text += "\n";

        _statText.text += "Speed: " + unit.UnitStats.Speed.ToString();
        _statText.text += "\n";

        _statText.text += "Vision Range: " + unit.UnitStats.VisionRange.ToString();
        _statText.text += "\n";

        _statText.text += "Selfcontrol: " + unit.UnitStats.SelfControl.ToString();
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
