using Unity.Mathematics;
using UnityEngine;

public class CubesWithMin : MonoBehaviour
{
    public GameObject sampleCubePrefab;
    public GetMacAudio peer;
    public float yScale;
    public float xScale;
    public float radius;
    public float innerRadius;
    public float threshold;
    public float boost = 100;
    public float range;
    GameObject[] cubes;
    public int framerate = 60;
    public int avgFrameRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        cubes = new GameObject[(int) (peer.samples.Length * range)];

        for (int i = 0; i < cubes.Length; i++) {
            GameObject instanceSampleCube = (GameObject) Instantiate(sampleCubePrefab);
            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "Sample-" + i;
            this.transform.eulerAngles = new Vector3(0, 360f / (float) (peer.samples.Length * range) * i, 0);
            instanceSampleCube.transform.position = Vector3.forward * innerRadius;

            float hue = (float)i / cubes.Length; // Normalize between 0 and 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            instanceSampleCube.GetComponent<Renderer>().material.color = color;

            cubes[i] = instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < cubes.Length; i++) {
            if (cubes != null) {
                if (peer.samples[i] * boost >= threshold) {
                    cubes[i].transform.localScale = new Vector3
                    (
                        0.5f * xScale * (math.PI / peer.samples.Length),
                        yScale,
                        radius * peer.samples[i] * boost + 0.1f
                    );
                } else {
                    cubes[i].transform.localScale = new Vector3
                    (
                        0.5f * xScale * (math.PI / peer.samples.Length * range),
                        0.1f,
                        0.1f
                    );
                }
            }
        }

        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = framerate;

        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
    }
}
