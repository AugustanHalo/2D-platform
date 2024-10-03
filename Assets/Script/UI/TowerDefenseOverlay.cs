using TMPro;
using UnityEngine;

public class TowerDefenseOverlay : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI moneyAmmount;
    [SerializeField] public TextMeshProUGUI healthAmmount;

    private void Awake()
    {
        //moneyAmmount = transform.Find("MoneyAmount").GetComponent<TextMeshProUGUI>();
        //healthAmmount = transform.Find("HealthAmount").GetComponent<TextMeshProUGUI>();
    }
    
    public void SetMoneyAmmount(int ammount)
    {
        moneyAmmount.text = ammount.ToString();
    }

    public void SetHealthAmmount(int ammount)
    {
        healthAmmount.text = ammount.ToString();
    }
}
