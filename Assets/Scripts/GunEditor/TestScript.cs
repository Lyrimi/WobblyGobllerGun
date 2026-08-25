using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Attachment attachment1;
    public Attachment attachment2;

    public bool DebugButton;
    public bool DebugButton2;
    public bool DebugButtonPop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (DebugButton)
        {
            DebugButton = !DebugButton;

            attachment1.TryAddChild(attachment2, 0);
        }

        if (DebugButton2)
        {
            DebugButton2 = !DebugButton2;

            attachment1.Redraw();
        }
        if (DebugButtonPop)
        {
            DebugButtonPop = !DebugButtonPop;

            attachment1.RemoveChild(0);
        }
    }
}
