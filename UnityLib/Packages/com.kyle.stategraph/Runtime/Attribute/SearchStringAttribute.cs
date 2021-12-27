using System;

public class SearchStringAttribute : PropertyCustomDrawerAttribute
{
    public override bool TypeCheck(Type type)
    {
        return type == typeof(string);
    }

}

public class HealFormulaSelectAttribute : SearchStringAttribute
{
}

public class DamageFormulaSelectAttribute : SearchStringAttribute
{
}

public class BuffSelectAttribute : SearchStringAttribute 
{
}

public class SkillGraphSelectAttribute : SearchStringAttribute
{
}
//属性选择
public class PropertySelectAttribute : SearchStringAttribute
{
}

public class AblityAttributeSelectAttribute : SearchStringAttribute
{
}

public class EffctGroupSelectAttribute : SearchStringAttribute
{
}

public class SoundEventSelectAttribute : SearchStringAttribute
{
}
