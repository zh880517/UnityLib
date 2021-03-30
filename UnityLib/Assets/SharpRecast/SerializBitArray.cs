[System.Serializable]
public class SerializBitArray
{
    [UnityEngine.SerializeField]
    private int[] iArray;
    [UnityEngine.SerializeField]
    private int length;

    public int Count { get { return length; } }

    public SerializBitArray(int length) : this(length, false) { }

    public SerializBitArray(System.Collections.BitArray bitArray)
    {
        length = bitArray.Length;
        iArray = new int[GetArrayLength(length, 32)];
        bitArray.CopyTo(iArray, 0);
    }

    public SerializBitArray(int length, bool defaultValue)
    {
        iArray = new int[GetArrayLength(length, 32)];
        this.length = length;
        int val = defaultValue ? -1 : 0;
        for (int i = 0; i < iArray.Length; i++)
        {
            iArray[i] = val;
        }
    }

    public bool this[int index]
    {
        get
        {
            return Get(index);
        }
        set
        {
            Set(index, value);
        }
    }

    public bool Get(int index)
    {
        if (index < 0 || index >= length)
        {
            throw new System.ArgumentOutOfRangeException("index", "ArgumentOutOfRangeIndex");
        }
        return (iArray[index / 32] & 1 << index % 32) != 0;
    }

    public void Set(int index, bool value)
    {
        if (index < 0 || index >= length)
        {
            throw new System.ArgumentOutOfRangeException("index", "ArgumentOutOfRangeIndex");
        }
        if (value)
        {
            iArray[index / 32] |= 1 << index % 32;
        }
        else
        {
            iArray[index / 32] &= ~(1 << index % 32);
        }
    }

    public void SetAll(bool value)
    {
        int num = value ? -1 : 0;
        for (int i = 0; i < iArray.Length; i++)
        {
            iArray[i] = num;
        }
    }

    private static int GetArrayLength(int n, int div)
    {
        if (n <= 0)
        {
            return 0;
        }
        return (n - 1) / div + 1;
    }
}
