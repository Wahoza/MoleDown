using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapHolder", menuName = "Scriptable Objects/MapHolder")]
public class MapHolder : ScriptableObject
{
    [field : SerializeField] public List<Texture2D> maps { get; private set; }    
}
