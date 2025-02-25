using Unity.Mathematics;
using UnityEngine;

public class InstantiateCubes : AbstractVisualizer
{
    public float yScale;
    public float xScale;
    public float radius;
    public float innerRadius;
    GameObject[] cubes;
    private float[] normalizedSamples;
    public float frequencyAdjustment;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cubes = new GameObject[audioSource.samples.Length];

        for (int i = 0; i < cubes.Length; i++) {
            GameObject instanceSampleCube = (GameObject) Instantiate(prefab);
            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "Sample-" + i;
            this.transform.eulerAngles = new Vector3(0, 360f / (float) audioSource.samples.Length * i, 0);
            instanceSampleCube.transform.position = Vector3.forward * innerRadius;

            float hue = (float)i / cubes.Length; // Normalize between 0 and 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            instanceSampleCube.GetComponent<Renderer>().material.color = color;

            cubes[i] = instanceSampleCube;
        }

        normalizedSamples = Normalize(audioSource.samples);
    }
    
    public static float[] Normalize(float[] values)
    {
        if (values.Length == 0) return values;

        // Find min and max manually
        float min = values[0];
        float max = values[0];

        foreach (float v in values)
        {
            if (v < min) min = v;
            if (v > max) max = v;
        }

        if (min == max) return new float[values.Length]; // Avoid division by zero

        // Normalize values
        float[] normalized = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            normalized[i] = (values[i] - min) / (max - min);
        }

        return normalized;
    }

    // Update is called once per frame
    void Update()
    {
        normalizedSamples = Normalize(audioSource.samples);

        for (int i = 0; i < cubes.Length; i++) {
            if (cubes != null) {
                // cubes[i].transform.localScale = new Vector3(0.5f * xScale * (math.PI / peer.samples.Length) + 0.1f, peer.samples[i] * yScale + 0.1f, zScale * (float) normalizedSamples[i] + 0.1f);
                /*cubes[i].transform.localScale = new Vector3
                (
                    0.5f * xScale * (math.PI / peer.samples.Length),
                    (peer.samples[i] * yScale) / (i * frequencyAdjustment) + 0.1f,
                    (radius * peer.samples[i]) / (i * frequencyAdjustment) + 0.01f
                );*/

                cubes[i].transform.localScale = new Vector3
                (
                    0.5f * xScale * (math.PI / audioSource.samples.Length),
                    (audioSource.samples[i] * yScale) * (frequencyAdjustment * i) + 0.1f,
                    (radius * audioSource.samples[i]) * (frequencyAdjustment * i) + 0.01f
                );
            }
        }

        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = framerate;

        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
    }
}
