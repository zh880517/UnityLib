using UnityEngine;

[CreateAssetMenu(menuName = "按键配置", fileName = "NewGamePadInputConfig")]
public class GamePadInputConfig : ScriptableObject
{
    public string Commit;
    public GamePadInputKey[] InputKeys;
}
