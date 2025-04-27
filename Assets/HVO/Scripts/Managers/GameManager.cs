using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    public void Test()
    {
        Debug.Log("Test Game Manager");
    }
}
