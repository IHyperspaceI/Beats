using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UI : MonoBehaviour
{
    public GetMacAudio audioSource;
    public Emitters emitters;
    public Lines lines;
    public CubesWithMin cubes;
    public CubesWithMin lowerCubes;

    public void OnDropdownValueChanged(int value) {
        if (value == 0) {
            lines.enabled = false;
            emitters.enabled = false;
            lowerCubes.enabled = false;

            audioSource.enabled = false;
            audioSource.samples = new float[512];
            audioSource.enabled = true;

            cubes.enabled = true;

            foreach (Transform child in lines.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in emitters.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in lowerCubes.transform) {
                if (child != null) Destroy(child.gameObject);
            }
        } else if (value == 1) {
            lines.enabled = false;
            cubes.enabled = false;
            lowerCubes.enabled = false;

            audioSource.enabled = false;
            audioSource.samples = new float[64];
            audioSource.enabled = true;

            emitters.enabled = true;

            foreach (Transform child in lines.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in cubes.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in lowerCubes.transform) {
                if (child != null) Destroy(child.gameObject);
            }
        } else if (value == 2) {
            emitters.enabled = false;
            cubes.enabled = false;
            lowerCubes.enabled = false;

            audioSource.enabled = false;
            audioSource.samples = new float[64];
            audioSource.enabled = true;

            lines.enabled = true;

            foreach (Transform child in cubes.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in emitters.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in lowerCubes.transform) {
                if (child != null) Destroy(child.gameObject);
            }
        } else if (value == 3) {
            lines.enabled = false;
            emitters.enabled = false;
            cubes.enabled = false;

            audioSource.enabled = false;
            audioSource.samples = new float[512];
            audioSource.enabled = true;

            lowerCubes.enabled = true;

            foreach (Transform child in cubes.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in emitters.transform) {
                if (child != null) Destroy(child.gameObject);
            }

            foreach (Transform child in lines.transform) {
                if (child != null) Destroy(child.gameObject);
            }
        }
    }
}
