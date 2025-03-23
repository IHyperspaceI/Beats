using System.Collections;
using UnityEngine;

public class Particles : AbstractVisualizer
{
    public float yScale, xScale, zScale, minSize, speed, size, lifetime;
    private Color[] colors;
    private GameObject[] particlePool;
    private Renderer[] renderers;
    private Rigidbody[] rigidbodies;
    private MaterialPropertyBlock[] materialBlocks;
    private bool[] isActive;
    public int poolSize = 100;
    private int poolIndex = 0;

    void OnEnable()
    {
        colors = new Color[(int)(audioSource.samples.Length * range)];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.HSVToRGB((float)i / colors.Length, 1f, 1f);
        }

        particlePool = new GameObject[poolSize];
        renderers = new Renderer[poolSize];
        rigidbodies = new Rigidbody[poolSize];
        isActive = new bool[poolSize];
        materialBlocks = new MaterialPropertyBlock[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(prefab);
            particle.transform.parent = this.transform;
            particle.SetActive(false);
            renderers[i] = particle.GetComponent<Renderer>();
            rigidbodies[i] = particle.GetComponent<Rigidbody>();
            SetMaterialTransparent(renderers[i].material);
            particlePool[i] = particle;

            materialBlocks[i] = new MaterialPropertyBlock();
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = framerate;
    }

    void Update()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (audioSource.samples[i] * boost >= threshold + audioSource.averageAmp)
            {
                SpawnParticle(i);
            }
        }
    }

    private void SpawnParticle(int index)
    {
        float sampleValue = audioSource.samples[index] * boost * size;
        if (sampleValue < minSize) return;

        GameObject particle = particlePool[poolIndex];

        isActive[poolIndex] = true;
        poolIndex = (poolIndex + 1) % poolSize;

        particle.SetActive(true);

        Vector3 position = new Vector3(
            Random.Range(-xScale, xScale) * index,
            Random.Range(-yScale, yScale) + transform.position.y,
            Random.Range(-zScale, zScale) * index
        );

        Vector3 scale = new Vector3(sampleValue, 0.01f, sampleValue);
        particle.transform.position = position;
        particle.transform.localScale = scale;

        rigidbodies[poolIndex].linearVelocity = position * speed;

        // Apply color using MaterialPropertyBlock
        MaterialPropertyBlock block = materialBlocks[poolIndex];
        Color startColor = colors[index];
        startColor.a = 1f; // Ensure alpha starts fully visible
        block.SetColor("_Color", startColor);
        block.SetColor("_BaseColor", startColor); // Fix for URP/HDRP
        renderers[poolIndex].SetPropertyBlock(block);

        StartCoroutine(FadeAndDeactivate(poolIndex, lifetime));
    }

    private IEnumerator FadeAndDeactivate(int index, float duration)
    {
        MaterialPropertyBlock block = materialBlocks[index];
        float elapsedTime = 0;

        Color startColor;
        renderers[index].GetPropertyBlock(block);
        startColor = block.GetColor("_Color");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float fadeAmount = 1 - (elapsedTime / duration);
            Color fadedColor = new Color(startColor.r, startColor.g, startColor.b, fadeAmount);

            block.SetColor("_Color", fadedColor);
            block.SetColor("_BaseColor", fadedColor); // Ensure fading works across pipelines
            renderers[index].SetPropertyBlock(block);

            yield return null;
        }

        particlePool[index].SetActive(false);
        isActive[index] = false;
    }

    private void SetMaterialTransparent(Material mat)
    {
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetFloat("_Mode", 2); // Fade mode for Standard Shader
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
