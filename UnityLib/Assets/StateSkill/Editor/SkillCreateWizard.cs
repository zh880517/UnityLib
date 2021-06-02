using System.Linq;

public class SkillCreateWizard : StateCreatWizard<StateSkillGraph>
{
    protected override string GetSaveDirectory()
    {
        return StateSkillGraph.SavePath;
    }

    protected override string NameCheck()
    {
        if (SkillGraphCollector.Instance.Graphs.Any(it=>it.name == Name))
        {
            return string.Format("文件名已存在 => {0}", Name);
        }
        return null;
    }
}
