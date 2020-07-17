using System.Collections.Generic;

public class TConfigMapTable<T> where T : ConfigDataBase
{
    public Dictionary<int, T> Units = new Dictionary<int, T>();
}
