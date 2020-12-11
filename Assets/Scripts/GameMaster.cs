using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameMaster", order = 1)]
public class GameMaster : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate = 3;
    public Vector3 latestPosInWorldMap;
}