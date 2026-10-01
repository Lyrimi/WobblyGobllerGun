using UnityEngine;

public class BossFightStarter : MonoBehaviour
{
    private Transform _teleportPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _teleportPos = Generation.Instance.TeleportObj;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartBossFight(collision.gameObject);
        }
    }

    void StartBossFight(GameObject player)
    {
        player.transform.position = _teleportPos.position;
    }
}
