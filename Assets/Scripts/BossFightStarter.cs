using UnityEngine;

public class BossFightStarter : MonoBehaviour
{
    private Transform _teleportPos;
    private Animator _teleportAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _teleportPos = Generation.Instance.TeleportObj;
        _teleportAnim = Generation.Instance.TpAnim;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Teleport()
    {
        GameObject player = FindAnyObjectByType<PlayerMovementScript>(FindObjectsInactive.Include).gameObject;
        StartBossFight(player);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _teleportAnim.SetTrigger("Start");
            //StartBossFight(collision.gameObject);
        }
    }

    void StartBossFight(GameObject player)
    {
        player.transform.position = _teleportPos.position;
    }
}
