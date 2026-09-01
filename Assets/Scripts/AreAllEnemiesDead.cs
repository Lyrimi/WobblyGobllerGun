using TMPro;
using UnityEngine;

public class AreAllEnemiesDead : MonoBehaviour
{

    private int enemyAmount;
    public int enemyKills;

    [SerializeField] TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyAmount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    // Update is called once per frame
    void Update()
    {

        text.text = enemyKills + "/" + enemyAmount + " Enemies killed"; 
        if (enemyKills == enemyAmount)
        {
            Debug.Log("you win");
        }
    }
}
