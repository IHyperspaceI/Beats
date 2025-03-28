/*using UnityEngine;

public class GetMacAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public float[] samples = new float[512];
    public float boost;
    public float averageAmp;

    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

        // Start recording from the default input device (virtual cable)
        audioSource.clip = Microphone.Start(null, true, 1, 44100);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { } // Wait for recording to start
        audioSource.Play();
    }

    void Update()
    {
        GetSpectrumAudioSource();
    }

    void GetSpectrumAudioSource() {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

	float sum = 0;

        for (int i = 0; i < samples.Length; i++)
        {
            float frequency = i * (44100 / 2f) / samples.Length; // Get frequency of this index
            float scalingFactor = Mathf.Pow(i + 1, boost); // Exponential boost

            samples[i] *= scalingFactor; // Apply scaling to balance high frequencies
	    
	        sum += samples[i];
        }

	    averageAmp = sum / (float) samples.Length;
    }
}
*/
using UnityEngine;

public class GetMacAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public float[] samples = new float[512];
    public float boost = 1.0f;
    public float averageAmp;

    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        
        audioSource.clip = Microphone.Start(null, true, 1, 44100);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { } // Wait for the microphone to start
        audioSource.Play();
    }

    void Update()
    {
        // Get the spectrum data from the microphone input
        audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] *= Mathf.Pow(i + 1, boost); // Apply boost
            sum += samples[i];
        }

        averageAmp = sum / samples.Length; // Calculate the average amplitude
    }
}
