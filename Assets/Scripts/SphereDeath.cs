using System.Collections;
using UnityEngine;

public class SphereDeath : MonoBehaviour
{
    [SerializeField] GameObject[] Explosions;
    BossRoom bossRoom;
    [SerializeField] Transform mainCamTrans;

    IEnumerator StartDeath()
    {
        Time.timeScale = 0;

        yield return bossRoom.LerpCam(mainCamTrans.position, transform.position, 2f);
        foreach (var item in Explosions)
        {
            item.SetActive(true);
            yield return new WaitForSecondsRealtime(0.15f);
        }
        bossRoom.GameEnd();
        Destroy(gameObject);



    }


    public void Death()
    {
        StartCoroutine(StartDeath());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossRoom = transform.parent.GetComponentInParent<BossRoom>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
