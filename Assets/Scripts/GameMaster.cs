using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameMaster", order = 1)]
public class GameMaster : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public Vector3 latestPosInWorldMap;
}