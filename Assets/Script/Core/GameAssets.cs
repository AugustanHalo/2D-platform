using UnityEngine;

public class GameAssets : MonoBehaviour
{
    
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    public static Transform GetGetTowerPrefab(string towerType)
    {
        switch (towerType)
        {
            case "Fire":
                return i.pfFireTower;
            case "Ice":
                return i.pfIceTower;
            case "Wizard":
                return i.pfWizardTower;
            case "Barrack":
                return i.pgBarrackTower;
            default:
                return i.pfTower;
        }
    }


    [SerializeField]
    public Transform pfProjectileArrow;
    public Transform pfEnemy;
    public Transform pfTower;
    public GameObject pfHealthBar;
    public Transform pfFastEnemy;
    public Transform pfTankEnemy;
    public Transform pfExplodingEnemy;
    public Transform pfHealingEnemy;
    public Transform pfWizardTower;
    public Transform pfFireTower;
    public Transform pfIceTower;
    public Transform pfFireArrow;
    public Transform pfIceArrow;
    public Transform pfKitchen;
    public Transform pfProjectileMagic;
    public Transform pfSoldier;
    public Transform pgBarrackTower;
}
