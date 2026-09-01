using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType { Straight, Wave }

    [Header("Bullet Attributes")]
    public GameObject bullet; //which bullet PREFAB are we using
    public float bulletLife; //seconds until the bullet dies
    public float speed; //how fast the bullet goes

    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType; //which bullet are we using from the list of bullets
    [SerializeField] private float firingRate; //how often is a burst fired
    [SerializeField] int burstAmount; //how many are fired in a row
    [SerializeField] float burstRate; //how long before each shot in the burst
    [SerializeField] int spreadAmount; //how many bullets is shot per shot
    [SerializeField] float spreadAngle; //0 is default and shoots straight forwards, anything above is for multiple shots at once and spreads out

    private GameObject spawnedBullet;
    private float firerateTimer;
    private float burstrateTimer;
    private float degreeBetween;
    public bool shouldSpawn;
    // Update is called once per frame
    void Update()
    {
        if (shouldSpawn == true)
        {
            //Shoots if the timer is exceeded
            firerateTimer += Time.deltaTime;
            if (firerateTimer >= firingRate)
            {
                StartCoroutine(Fire());
                firerateTimer = 0;
            }
            //
        }
    }

    private IEnumerator Fire()
    {
        //this part checks if the angle bullets are gonna overlap because of the angles, and reduces the angle if thats the case
        if (spreadAngle > (360 - 360 / spreadAmount))
        {
            spreadAngle = 360 - (360 / spreadAmount);
        }
        degreeBetween = spreadAngle / (spreadAmount - 1);
        //

        //This part does some straight bullshit
        for (int i = 0; i < burstAmount; i++)
        {
            for (int j = 0; j < spreadAmount; j++)
            {
                spawnedBullet = Instantiate(bullet, transform.position, this.transform.rotation);
                if (spreadAmount > 1)
                {
                    if (spawnerType == SpawnerType.Wave)
                    { spawnedBullet.transform.Rotate(Vector3.forward, degreeBetween * j); }

                    else
                    { spawnedBullet.transform.Rotate(Vector3.forward, (degreeBetween * j) - spreadAngle / 2 + 1); }
                }
                if (spawnerType == SpawnerType.Wave)
                { spawnedBullet.transform.Rotate(Vector3.forward, -90f); }

                spawnedBullet.GetComponent<Bullet>().speed = speed;
                spawnedBullet.GetComponent<Bullet>().lifetime = bulletLife;
            }
            yield return new WaitForSeconds(burstRate);
        }
        //
    }
}
