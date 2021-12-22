using UnityEngine;

[System.Serializable]
public class GamePadInputKey
{
    public int Key;
    public string BtnName;
    public string XAxis;
    public string YAxis;
    public string Commit;

    public int UID { get; set; }
    public bool TryGet(out Vector2 val)
    {
        val = Vector2.zero;
        bool isPress = false;
        if (!string.IsNullOrWhiteSpace(BtnName))
        {
            isPress = Input.GetAxisRaw(BtnName) != 0;
            if (!isPress)
                return false;
        }
        if (!isPress)
        {
            float rawX = 0, rawY = 0;
            if (!string.IsNullOrWhiteSpace(XAxis))
            {
                rawX = Input.GetAxisRaw(XAxis);
            }
            if (!string.IsNullOrWhiteSpace(YAxis))
            {
                rawY = Input.GetAxisRaw(YAxis);
            }
            isPress = rawX != 0 || rawY != 0;
        }

        if (isPress)
        {
            if (!string.IsNullOrWhiteSpace(XAxis))
            {
                val.x = Input.GetAxis(XAxis);
            }
            if (!string.IsNullOrWhiteSpace(YAxis))
            {
                val.y = Input.GetAxis(YAxis);
            }
        }
        return isPress || val.x != 0 || val.y !=0 ;

    }
}
