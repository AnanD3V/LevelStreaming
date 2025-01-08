using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneStreamer : MonoBehaviour
{
    public SceneConfig sceneConfig;
    public float loadDistance = 100f;
    public float unloadDistance = 120f;
    public float checkInterval = 0.5f; // Time in seconds between checks

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        // Ensure all scenes are unloaded at the start
        foreach (var scene in sceneConfig.scenes)
        {
            if (SceneManager.GetSceneByName(scene.sceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene.sceneName);
            }
        }

        // Start the coroutine for periodic checks
        StartCoroutine(CheckScenes());
    }

    IEnumerator CheckScenes()
    {
        while (true)
        {
            foreach (var scene in sceneConfig.scenes)
            {
                // Check if the player is inside the bounds
                if (scene.sceneBounds.Contains(player.position))
                {
                    // If inside, ensure the scene is loaded
                    if (!SceneManager.GetSceneByName(scene.sceneName).isLoaded)
                    {
                        Debug.Log($"Player inside bounds. Loading scene: {scene.sceneName}");
                        SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
                    }
                }
                else
                {
                    // If outside, calculate distance to bounds
                    Vector3 closestPoint = scene.sceneBounds.ClosestPoint(player.position);
                    float distanceToBounds = Vector3.Distance(closestPoint, player.position);

                    if (distanceToBounds <= loadDistance &&
                        !SceneManager.GetSceneByName(scene.sceneName).isLoaded)
                    {
                        Debug.Log($"Loading scene: {scene.sceneName}");
                        SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
                    }

                    if (distanceToBounds > unloadDistance &&
                        SceneManager.GetSceneByName(scene.sceneName).isLoaded)
                    {
                        Debug.Log($"Unloading scene: {scene.sceneName}");
                        SceneManager.UnloadSceneAsync(scene.sceneName);
                    }
                }
            }

            // Wait for the next check interval
            yield return new WaitForSeconds(checkInterval);
        }
    }

}
