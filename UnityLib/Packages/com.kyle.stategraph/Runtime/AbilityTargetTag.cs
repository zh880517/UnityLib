using UnityEngine;

public class AbilityTargetTag : ScriptableObject
{
    [SerializeField]
    private string[] tags;
    public string[] Tags
    {
        get
        {
            if (tags == null || tags.Length == 0)
            {
                tags = new string[32];
                for (int i = 0; i < 31; ++i)
                {
                    tags[i] = $"Tag {i + 1}";
                }
                tags[31] = "None Flag";
            }
            return tags;
        }
    }
}
