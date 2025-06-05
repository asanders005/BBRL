using UnityEngine;

public class MapSpaceSpriteDistributor : MonoBehaviour
{
    [SerializeField] private MapSpaceImagesData mapSpaceImagesData;

    public Sprite GetSprite(EncounterType type)
    {
        switch (type)
        {
            case EncounterType.Combat:
                return mapSpaceImagesData.combatSpace;
            case EncounterType.Rest:
                break;
            case EncounterType.Treasure:
                break;
            case EncounterType.Event:
                return mapSpaceImagesData.EventSpace;
            case EncounterType.Shop:
                break;
            case EncounterType.Boss:
                break;
            default:
                break;
        }

        Debug.LogError($"Encounter type {type} does not have a sprite assigned.");
        return null;
    }
}
