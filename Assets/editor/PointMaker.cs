using UnityEditor;
using UnityEngine;

public class SpritePixelSpawnerWindow : EditorWindow
{
    //Yes this script is Ai genreated
    private GameObject prefabToSpawn;
    private bool toolEnabled;

    [MenuItem("Tools/Sprite Pixel Spawner")]
    public static void ShowWindow()
    {
        GetWindow<SpritePixelSpawnerWindow>("Pixel Spawner");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Sprite Pixel Spawner",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField(
            "Prefab",
            prefabToSpawn,
            typeof(GameObject),
            false
        );

        EditorGUILayout.Space();

        bool newEnabled = EditorGUILayout.Toggle(
            "Enable Tool",
            toolEnabled
        );

        if (newEnabled != toolEnabled)
        {
            toolEnabled = newEnabled;

            if (toolEnabled)
                SceneView.duringSceneGui += OnSceneGUI;
            else
                SceneView.duringSceneGui -= OnSceneGUI;

            SceneView.RepaintAll();
        }

        if (toolEnabled)
        {
            EditorGUILayout.HelpBox(
                "Click a SpriteRenderer in the Scene view to spawn the selected prefab on the center of the pixel.",
                MessageType.Info
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Tool disabled.",
                MessageType.None
            );
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type != EventType.MouseDown || e.button != 0)
            return;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Sprite Pixel Spawner: No prefab selected.");
            return;
        }

        GameObject selected = Selection.activeGameObject;

        if (selected == null)
            return;

        SpriteRenderer spriteRenderer =
            selected.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        Sprite sprite = spriteRenderer.sprite;

        // Mouse position -> world ray
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // Sprite plane
        Plane plane = new Plane(
            spriteRenderer.transform.forward,
            spriteRenderer.transform.position
        );

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 worldPosition = ray.GetPoint(distance);

        // World -> sprite local position
        Vector3 localPosition =
            spriteRenderer.transform.InverseTransformPoint(worldPosition);

        // Make sure we're inside the sprite bounds
        if (!sprite.bounds.Contains(localPosition))
            return;

        float pixelsPerUnit = sprite.pixelsPerUnit;

        // Local position -> pixel position
        float pixelX =
            (localPosition.x - sprite.bounds.min.x) * pixelsPerUnit;

        float pixelY =
            (localPosition.y - sprite.bounds.min.y) * pixelsPerUnit;

        int x = Mathf.FloorToInt(pixelX);
        int y = Mathf.FloorToInt(pixelY);

        // Center of the pixel
        Vector3 pixelCenter = new Vector3(
            sprite.bounds.min.x + (x + 0.5f) / pixelsPerUnit,
            sprite.bounds.min.y + (y + 0.5f) / pixelsPerUnit,
            0f
        );

        // Local -> world
        Vector3 spawnPosition =
            spriteRenderer.transform.TransformPoint(pixelCenter);

        SpawnPrefab(spawnPosition);

        e.Use();
    }

    private void SpawnPrefab(Vector3 position)
    {
        GameObject spawnedObject =
            (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn, Selection.activeGameObject.transform);

        Undo.RegisterCreatedObjectUndo(
            spawnedObject,
            "Spawn Prefab On Pixel"
        );

        spawnedObject.transform.position = position;

        Selection.activeGameObject = spawnedObject;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}