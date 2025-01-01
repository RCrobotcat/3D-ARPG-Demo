using UnityEngine;
using QFramework;

public interface Istorage : IUtility
{
    void SaveCharacterNums(string key, float value);
    float LoadCharacterNums(string key);
}

public class storage : Istorage
{
    public void SaveCharacterNums(string key, float value)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.SetFloat(key, value);
    }

    public float LoadCharacterNums(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }
}
