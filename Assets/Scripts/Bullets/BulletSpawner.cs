using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType { Straight, Wave }

    [Header("Bullet Attributes")]
    public GameObject bullet;
    public float bulletLife;
    public float speed;

    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float firingRate;

    private GameObject spawnedBullet;
    private float timer;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (spawnerType == SpawnerType.Straight) transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f);
        if(timer >= firingRate)
        {
            Fire();
            timer = 0;
        }
    }

    private void Fire()
    {
        if(bullet)
        {
            spawnedBullet = Instantiate(bullet,transform.position,bullet.transform.rotation);
            /*spawnedBullet.GetComponent<Bullet>().speed = speed;
            spawnedBullet.GetComponent<Bullet>().lifetime = bulletLife;
            if(spawnerType == SpawnerType.Wave)
            {
                spawnedBullet.GetComponent<SinusMove>().target.position = new Vector2(GameObject.FindWithTag("Player").transform.position.x, GameObject.FindWithTag("Player").transform.position.y); ;
            }
            spawnedBullet.transform.rotation = transform.rotation;
            */
        }
    }
}
