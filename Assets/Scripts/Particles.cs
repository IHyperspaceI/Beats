using System.Collections;
using UnityEngine;

public class Particles : AbstractVisualizer
{
    public float yScale;
    public float xScale;
    public float zScale;
    public float minSize;
    public float speed;
    public float size;
    public float lifetime;
    private Color[] colors;

    private GameObject[] particlePool;
    public int poolSize = 100; 
    private int poolIndex = 0; 

    void OnEnable()
    {
        colors = new Color[(int)(audioSource.samples.Length * range)];

        for (int i = 0; i < colors.Length; i++)
        {
            float hue = (float)i / colors.Length;
            colors[i] = Color.HSVToRGB(hue, 1f, 1f);
        }

        particlePool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(prefab);
            particle.SetActive(false);
            SetMaterialTransparent(particle.GetComponent<Renderer>().material); // Set material transparency mode
            particlePool[i] = particle;
        }
    }

    void Update()
    {
        int activeParticles = 0;

        for (int i = 0; i < colors.Length; i++)
        {
            if (audioSource.samples[i] * boost >= threshold + audioSource.averageAmp)
            {
                if (activeParticles < poolSize)
                {
                    SpawnParticle(i);
                    activeParticles++;
                }
            }
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = framerate;

        avgFrameRate = (int)(Time.frameCount / Time.time);
    }

    private void SpawnParticle(int index)
    {
        if (audioSource.samples[index] * boost * size < minSize) return;
        
        GameObject particle = particlePool[poolIndex];

        poolIndex = (poolIndex + 1) % poolSize;

        particle.SetActive(true);
        Vector3 position = new Vector3(
            UnityEngine.Random.Range(-xScale, xScale) * index, 
            UnityEngine.Random.Range(-zScale, zScale) * index + this.transform.position.y, 
            UnityEngine.Random.Range(-yScale, yScale) * index
        );
        Vector3 direction = new Vector3(
            UnityEngine.Random.Range(-1.0f, 1.0f), 
            UnityEngine.Random.Range(-1, 1), 
            UnityEngine.Random.Range(-1, 1)
        );
        Vector3 scale = new Vector3(
            audioSource.samples[index] * boost * size, 
            0.01f, 
            audioSource.samples[index] * boost * size
        );

        particle.transform.position = position;
        particle.transform.localScale = scale;
        Material mat = particle.GetComponent<Renderer>().material;
        mat.color = colors[index];
        particle.GetComponent<Rigidbody>().linearVelocity = direction * speed;

        StartCoroutine(FadeAndDeactivate(particle, lifetime, mat));
    }

    private IEnumerator FadeAndDeactivate(GameObject particle, float duration, Material mat)
    {
        float elapsedTime = 0;
        Color startColor = mat.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float fadeAmount = 1 - (elapsedTime / duration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, fadeAmount);
            yield return null;
        }

        particle.SetActive(false);
    }

    private void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 2);  // Fade mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;  // Transparent queue
    }
}
