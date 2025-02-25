using UnityEngine;

public abstract class AbstractVisualizer : MonoBehaviour
{
    public GameObject prefab;
    public GetMacAudio audioSource;
    public int numSamples;
    public float threshold;
    public float boost = 100;
    public float range;
    public int framerate = 120;
    public int avgFrameRate;
}
