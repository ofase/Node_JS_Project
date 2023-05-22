using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;


public class ResourceManager
{
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();       //리소스 리스트 목록 관리자 
    public Manager Manager => Manager.Instance;                                                             //매니저를 통해서 Object 관리를 하기 위해 선언

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab : {key}");
            return null;
        }

        if (pooling)
            return Manager.Pool.Pop(prefab);

        GameObject go = Object.Instantiate(prefab, parent);

        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        if (Manager.Pool.Push(go))
            return;

        Object.Destroy(go);
    }

    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))                                                    //키 값을 검사해서 
        {
            if (typeof(T) == typeof(Sprite))                                                                     //스프라이트일 경우 2D 예외 처리 
            {
                Texture2D tex = resource as Texture2D;
                Sprite spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                return spr as T;
            }
            return resource as T;
        }
        return Resources.Load<T>(key);                                                                           // 키값 오브젝트를 돌려준다. 
    }

    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        if (_resources.TryGetValue(key, out Object resource))                                                    //키 값을 검사해서 
        {
            callback?.Invoke(resource as T);                                                                        //콜백 검사후 반환
            return;
        }

        var asyncOperation = Addressables.LoadAssetAsync<T>(key);                                               //리소스 로딩
        asyncOperation.Completed += (op) =>                                                                     //완료되면 콜백 검사해서 반환
        {
            _resources.Add(key, op.Result);
            callback?.Invoke(op.Result);
        };

    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var OpHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        OpHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;
            foreach (var result in op.Result)
            {
                LoadAsync<T>(result.PrimaryKey, (obj) =>
                {
                    loadCount++;                                                                //로딩을 카운트 올려서 완료 할때 까지 진행 
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };
    }
}