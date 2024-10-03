using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeOverlay : MonoBehaviour
{
    private static UpgradeOverlay Instance { get; set; }
    [SerializeField] private Button_Sprite upgradeTowerBtn;
    [SerializeField] private Button_Sprite sellTowerBtn;
    [SerializeField] private Button_Sprite closeBtn;
    [SerializeField] private Transform rangeTransform;
    private Tower tower;

    private void Awake()
    {
        Instance = this;

        if (upgradeTowerBtn == null) upgradeTowerBtn = transform.Find("UpgradeTowerBtn").GetComponent<Button_Sprite>();
        if (sellTowerBtn == null) sellTowerBtn = transform.Find("SellTowerBtn").GetComponent<Button_Sprite>();
        if (closeBtn == null) closeBtn = transform.Find("CloseBtn").GetComponent<Button_Sprite>();
        if (rangeTransform == null) rangeTransform = transform.Find("Range");

        upgradeTowerBtn.ClickFunc = UpgradeTower;
        sellTowerBtn.ClickFunc = SellTower;
        closeBtn.ClickFunc = Hide;

        Hide();
    }

    private void Update()
    {
        
    }

    private void SellTower()
    {
        Debug.Log("Sell Tower");
    }

    private void UpgradeTower()
    {
        tower.UpgradeTower();
        ResetTowerVisualRange();
    }

    private void ResetTowerVisualRange()
    {
        transform.Find("Range").localScale = new Vector3(tower.GetRange() * 2, tower.GetRange() * 2);
    }

    public static void Show_Static(Tower tower)
    {
        Instance.Show(tower);
    }

    private void Show(Tower tower)
    {
        this.tower = tower;
        ResetTowerVisualRange();
        gameObject.SetActive(true);
        transform.position = tower.transform.position;
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
