using UnityEngine;

public class DebugKiller : MonoBehaviour
{
    [SerializeField] private bool _debugButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void KillSomething()
    {
        HealthSCript[] objs = FindObjectsByType<HealthSCript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var item in objs)
        {
            if (item.CompareTag("Player"))
            {
                continue;
            }
            item.Damage(100000000);
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            KillSomething();
        }
    }
}
