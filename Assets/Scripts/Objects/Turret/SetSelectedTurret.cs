using UnityEngine;
using TMPro;

public class SetSelectedTurret : MonoBehaviour
{
    public static SetSelectedTurret Instance;

    public TextMeshProUGUI TextElement;
    public GameObject[] TurretImage;
    public int TurretType;
    public Transform Canvas;
    public GameObject CurrentImage;
    public GameObject Turret;
    public GameObject Tile;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectTurret(GameObject NewTurret, GameObject NewTile)
    {
        if (!ComboManager.Instance.TurretCombosActive) return;

        Turret = NewTurret;
        Tile = NewTile;
        TurretType = NewTurret.name[6] - '1';
        if (CurrentImage != null) {Destroy(CurrentImage); }
        CurrentImage = Instantiate(TurretImage[TurretType],Canvas);
        ComboManager.Instance.ShowSpecificTurretComboStatuses(NewTurret);
    }
    public void DeselectTurret()
    {
        //if (Turret != null) ComboManager.Instance.ShowUserCombos(Turret);
        ComboManager.Instance.ShowUserCombos(Turret);

        Turret = null;
        Tile = null;
        if (CurrentImage != null) 
        { 
            Destroy(CurrentImage); 
        }
    }
}