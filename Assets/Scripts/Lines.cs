using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lines : MonoBehaviour
{
    public GameObject sampleCubePrefab;
    public GetMacAudio peer;
    public float xScale;
    GameObject[] cubes;
    public float threshold;
    public float boost = 100;
    public float rate;
    public float lifetime;
    public float speed;

    public int numLines = 4;

    public int framerate = 60;
    public int avgFrameRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        cubes = new GameObject[(int) (peer.samples.Length / 1.5f)];

        for (int i = 0; i < numLines; i++) {
            GameObject instanceSampleCube = (GameObject) Instantiate(sampleCubePrefab);
            instanceSampleCube.transform.position = transform.position + Vector3.forward * xScale * i;
            instanceSampleCube.transform.rotation = transform.rotation;
            instanceSampleCube.transform.parent = transform;
            instanceSampleCube.name = "Sample-" + i;

            cubes[i] = instanceSampleCube;
        }
    }


    int[] FindLoudestFrequencies(float[] spectrum, int count)
    {
        // Create an array of indices (0 to spectrum.Length)
        int[] indices = Enumerable.Range(0, spectrum.Length).ToArray();

        // Sort indices by amplitude (highest first)
        var sortedIndices = indices
            .OrderByDescending(i => spectrum[i]) // Sort by amplitude
            .ToList(); // Convert to list for removal operations

        List<int> selectedIndices = new List<int>();

        foreach (int index in sortedIndices)
        {
            // Ensure at least 10 indexes away from already selected frequencies
            if (selectedIndices.All(selected => Mathf.Abs(selected - index) >= 2))
            {
                selectedIndices.Add(index);
            }

            // Stop once we have enough frequencies
            if (selectedIndices.Count >= count)
                break;
    }

    return selectedIndices.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numLines; i++) {
            cubes[i].transform.position = transform.position + Vector3.forward * xScale * FindLoudestFrequencies(peer.samples, numLines)[i];

            var emission = cubes[i].GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = rate;
            
            float hue = (float)FindLoudestFrequencies(peer.samples, numLines)[i] / peer.samples.Length; // Normalize between 0 and 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness

            cubes[i].GetComponent<Renderer>().material.color = color;
            var main = cubes[i].GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
            main.startSpeed = speed;
            main.startLifetime = lifetime;
        }

        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = framerate;

        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
    }
}
