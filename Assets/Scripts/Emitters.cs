using Unity.IntegerTime;
using UnityEngine;

public class Emitters : AbstractVisualizer
{
    public float xScale;
    GameObject[] cubes;
    public float rate;
    public float lifetime;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        cubes = new GameObject[(int) (audioSource.samples.Length / 1.5f)];

        for (int i = 0; i < cubes.Length; i++) {
            GameObject instanceSampleCube = (GameObject) Instantiate(prefab);
            instanceSampleCube.transform.position = transform.position + Vector3.forward * xScale * i;
            instanceSampleCube.transform.rotation = transform.rotation;
            instanceSampleCube.transform.parent = transform;
            instanceSampleCube.name = "Sample-" + i;

            float hue = (float)i / cubes.Length; // Normalize between 0 and 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness

            instanceSampleCube.GetComponent<Renderer>().material.color = color;
            var main = instanceSampleCube.GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);

            cubes[i] = instanceSampleCube;
        }
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
        for (int i = 0; i < cubes.Length; i++) {
            if (cubes != null) {
                var emission = cubes[i].GetComponent<ParticleSystem>().emission;
                emission.rateOverTime = rate;

                if (audioSource.samples[i] * i * boost >= threshold) {
                    var main = cubes[i].GetComponent<ParticleSystem>().main;
                    main.startLifetime = lifetime;
                    emission.enabled = true;
                    main.startSpeed = speed;
                } else {
                    var main = cubes[i].GetComponent<ParticleSystem>().main;
                    //main.startLifetime = 0.001f;
                    emission.enabled = false;
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
