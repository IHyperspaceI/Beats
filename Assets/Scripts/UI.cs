using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UI : MonoBehaviour
{
    public GetMacAudio audioSource;
    public MonoBehaviour[] scripts;

    public void OnDropdownValueChanged(int value) {
        foreach (MonoBehaviour script in scripts) {
            script.enabled = false;

            foreach (Transform child in script.gameObject.transform) {
                if (child != null) Destroy(child.gameObject);
            }
        }

        AbstractVisualizer visualizerScript = (AbstractVisualizer) scripts[value];

        if ((AbstractVisualizer) scripts[value]) {
            audioSource.enabled = false;
            audioSource.samples = new float[visualizerScript.numSamples];
            audioSource.enabled = true;
        }

        visualizerScript.enabled = true;
    }
}
