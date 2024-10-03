using UnityEngine;
using TMPro;

public class VolumeText : MonoBehaviour
{
    private TextMeshProUGUI volumeText;
    [SerializeField] private string introText;
    [SerializeField] private string volumeName;

    private void Awake()
    {
        volumeText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdateVolume();
    }


    private void UpdateVolume()
    {
        float volume = PlayerPrefs.GetFloat(volumeName, 1) * 100;
        volumeText.text = introText + volume.ToString();
    }
}
