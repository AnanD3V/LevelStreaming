using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "Scene/Scene Config")]
public class SceneConfig : ScriptableObject
{
    public List<SceneData> scenes; 
}