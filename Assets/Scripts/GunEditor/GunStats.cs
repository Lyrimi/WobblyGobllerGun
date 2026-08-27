using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GunStats : MonoBehaviour
{
    enum Stats
    {
        FireRate,
        Damage,
        Spreed,
        Speed,
        LifeTime,
        Projectiles,
    }

    [SerializeField]
    string Prefix;
    [SerializeField]
    Stats stat;

    [SerializeField]
    TMP_Text text;

    string value = "NaN";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateText()
    {
        switch (stat)
        {
            case Stats.Damage:
                value = GunData.bulletData.Damage.ToString();
                break;
            case Stats.FireRate:
                value = GunData.FireRate.ToString();
                break;
            case Stats.Spreed:
                value = GunData.bulletData.Spreed.ToString();
                break;
            case Stats.Speed:
                value = GunData.bulletData.Speed.ToString();
                break;
            case Stats.LifeTime:
                value = GunData.bulletData.LifeTime.ToString();
                break;
            case Stats.Projectiles:
                value = GunData.bulletData.Projectiles.ToString();
                break;
            default:
                break;
        }
        text.text = $"{Prefix} | {value}";

    }
}
