using UnityEngine;

namespace Game.Networks
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T staticInstance;

        public static T Instance
        {
            get
            {
                if (staticInstance != null)
                {
                    return staticInstance;
                }
                staticInstance = FindObjectOfType(typeof(T)) as T;

#if UNITY_EDITOR
                if (staticInstance is null)
                {
                    Debug.Log("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                }
#endif
                return staticInstance;
            }
        }
    }
}