using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private Manager() { }
    public static Manager Instance { get; private set; }

    public PoolManager Pool = new PoolManager();
    public ResourceManager Resource = new ResourceManager();
    public ObjectManager Object = new ObjectManager();

    private void Awake()                            //싱글톤이기 때문에 처리해주는 것들
    {
        if (Instance != null)
        {
            Destroy(gameObject);                    //같은 객체 제거
            return;
        }
        else
        {
            transform.parent = null;
            Instance = this;
            DontDestroyOnLoad(gameObject);          //씬 이동시 파괴 되지 않게 하기 위해서
        }
    }

}