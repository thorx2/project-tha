using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Configuration", menuName = "ProjectTha/Gameplay/Game Configuration")]
public class GameCoreParameters : ScriptableObject
{
    public int RoundDurationInMins;
    public int XpPerPickup;
    public SerializableDictionary<int, int> XpLevelMapData;
}