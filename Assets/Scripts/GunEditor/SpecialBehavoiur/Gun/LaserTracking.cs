using UnityEngine;

public class LaserTracking : SpecialGunBeheavoiur
{
    LineRenderer lineRenderer;
    TopDownGunData topDownGunData;
    LayerMask mask;

    public static LaserTracking Instance;

    const float MAX_DISTANCE = 30;
    public override void OnStart()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
        topDownGunData = GetComponent<TopDownGunData>();
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.startColor = new(1, 0, 0, 0.4f);
        lineRenderer.endColor = new(1, 0, 0, 0.4f);
        lineRenderer.startWidth = 0.04f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.material = Canvas.GetDefaultCanvasMaterial();
        mask = new();
        mask = LayerMask.GetMask("Walls");
    }

    public override void OnUpdate()
    {
        if (Instance == null)
        {
            Init();
        }
        Vector2 startPos = topDownGunData.EndPoints[0].position;

        RaycastHit2D hit = Physics2D.Raycast(startPos, transform.up, MAX_DISTANCE, mask);

        float distance;

        if (hit.collider == null)
        {
            distance = MAX_DISTANCE;
        }
        else
        {
            distance = (hit.point - startPos).magnitude;
        }


        lineRenderer.SetPositions(new Vector3[] { startPos, (Vector3)startPos + transform.up * distance });
    }

    public override void OnDestory()
    {
        Instance = null;
    }
}
