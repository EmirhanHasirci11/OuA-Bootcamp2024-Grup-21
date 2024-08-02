using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Emirhan_s_Folder.Scripts.LobbyV2.Core
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] objs = FindObjectsOfType<T>();
                    if (objs.Length > 0)
                    {
                        _instance = objs[0];
                        _instance = Instance;
                    }
                    else
                    {
                        GameObject go = new GameObject();
                        go.name = typeof(T).Name;
                        _instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
    }
}
