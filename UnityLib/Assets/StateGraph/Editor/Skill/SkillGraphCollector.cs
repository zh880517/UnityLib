public class SkillGraphCollector : StateGraphCollector<StateSkillGraph>
{
    private static SkillGraphCollector _instance;
    public static SkillGraphCollector Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillGraphCollector();
            }
            return _instance;
        }
    }

    public override string GetSavePath()
    {
        return StateSkillGraph.SavePath;
    }
}
