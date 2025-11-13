using UnityEngine;
using Fusion;

public class NetworkRunnerHandler : MonoBehaviour
{
    [HideInInspector] public NetworkRunner Runner;
    [HideInInspector] public NetworkSceneManagerDefault SceneManager;

    private void Awake()
    {
        Runner = GetComponent<NetworkRunner>();
        SceneManager = GetComponent<NetworkSceneManagerDefault>();

        if (Runner == null)
            Runner = gameObject.AddComponent<NetworkRunner>();

        if (SceneManager == null)
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        DontDestroyOnLoad(gameObject);
    }
}
