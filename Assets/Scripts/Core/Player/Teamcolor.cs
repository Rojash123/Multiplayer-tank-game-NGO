using UnityEngine;

[CreateAssetMenu(fileName ="NewTeamColor",menuName = "NewTeamColor/TeamColor")]
public class Teamcolor : ScriptableObject
{
    [SerializeField] Color[] teamcolors;

    public Color GetTeamColor(int index)
    {
        if (index < 0 || index >= teamcolors.Length)
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        else
        {
            return teamcolors[index];
        }
    }

}

