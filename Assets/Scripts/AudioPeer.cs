using UnityEngine;

public class AudioPeer : MonoBehaviour
{
    AudioSource source;
    public float[] samples = new float[64];
    public float boost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
    }

    void GetSpectrumAudioSource() {
        source.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < samples.Length; i++)
        {
            float frequency = i * (44100 / 2f) / samples.Length; // Get frequency of this index
            float scalingFactor = Mathf.Pow(i + 1, boost); // Exponential boost

            samples[i] *= scalingFactor; // Apply scaling to balance high frequencies
        }
    }
}
