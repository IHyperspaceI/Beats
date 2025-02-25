using Unity.Mathematics;
using UnityEngine;

public class CubesWithMin : AbstractVisualizer
{
    public float yScale;
    public float xScale;
    public float radius;
    public float innerRadius;
    GameObject[] cubes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        cubes = new GameObject[(int) (audioSource.samples.Length * range)];

        for (int i = 0; i < cubes.Length; i++) {
            GameObject instanceSampleCube = (GameObject) Instantiate(prefab);
            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "Sample-" + i;
            this.transform.eulerAngles = new Vector3(0, 360f / (float) (audioSource.samples.Length * range) * i, 0);
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
                if (audioSource.samples[i] * boost >= threshold + audioSource.averageAmp) {
                    cubes[i].transform.localScale = new Vector3
                    (
                        0.5f * xScale * (math.PI / audioSource.samples.Length),
                        yScale,
                        radius * audioSource.samples[i] * boost + 0.1f
                    );
                } else {
                    cubes[i].transform.localScale = new Vector3
                    (
                        0.5f * xScale * (math.PI / audioSource.samples.Length * range),
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
