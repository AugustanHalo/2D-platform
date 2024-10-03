using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private TMP_Text[] textComponents;

    [Header("Pause Game")]
    [SerializeField] private GameObject pauseMenu;


    private float animationDuration = 1f;

    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);
    }
    #region GameOver
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        StartCoroutine(TextAppear());
        //AudioManager.instance.PlayAudioClip(gameOverSound);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false; // only works in the editor
#endif
    }

    private IEnumerator TextAppear()
    {
        if (textComponents == null)
        {
            Debug.LogError("Game Over Text is not assigned!");
            yield break;
        }
        List<Material> newMaterials = new List<Material>();

        // Create new materials for each text component
        foreach (TMP_Text text in textComponents)
        {
            if (text == null)
            {
                Debug.LogWarning("A text component in the array is null. Skipping.");
                continue;
            }
            if(text.IsActive() == false)
            {
                continue;
            }

            Material currentMaterial = text.fontSharedMaterial;
            if (currentMaterial == null)
            {
                Debug.LogWarning($"Cannot find font material for {text.name}. Skipping.");
                continue;
            }

            Material newMaterial = new Material(currentMaterial);
            text.fontMaterial = newMaterial;
            newMaterials.Add(newMaterial);

            // Set initial values
            newMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1f);
            newMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 1f);
        }

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;

            // Update all materials
            foreach (Material material in newMaterials)
            {
                // Lerp the face dilate from -1 to 0
                float faceDilate = Mathf.Lerp(-1f, 0f, t);
                material.SetFloat(ShaderUtilities.ID_FaceDilate, faceDilate);
                float outlineSoftness = Mathf.Lerp(1f, 0f, t);
                material.SetFloat(ShaderUtilities.ID_OutlineSoftness, outlineSoftness);
            }

            // Update all text components
            foreach (TMP_Text text in textComponents)
            {
                if (text != null)
                {
                    text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Ensure final state for all materials
        foreach (Material material in newMaterials)
        {
            material.SetFloat(ShaderUtilities.ID_FaceDilate, 0f);
            material.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0f);
        }

        // Final update for all text components
        foreach (TMP_Text text in textComponents)
        {
            if (text != null)
            {
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }
        Time.timeScale = 0;
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseMenu.activeInHierarchy)
            {
                PauseGame(false);
            }
            else
            {
                PauseGame(true);
            }
        }
    }
    #region PauseMenu
    public void PauseGame(bool pause)
    {
        pauseMenu.SetActive(pause);

        //if pause the game time stop (time scale = 0)
        //if unpause the game time continue normally (time scale = 1)
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void SoundVolume()
    {
        AudioManager.instance.SoundVolumeChange(0.2f);
    }
    public void MusicVolume()
    {
        AudioManager.instance.MusicVolumeChange(0.2f);
    }


    #endregion
}

