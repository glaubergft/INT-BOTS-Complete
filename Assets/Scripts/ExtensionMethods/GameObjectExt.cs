using UnityEngine;

public static class GameObjectExt
{
    public static T FindParentComponent<T>(this GameObject gameObject)
    {
        T result = default(T);
        var currentObj = gameObject.transform;
        while (currentObj != null)
        {
            result = currentObj.GetComponent<T>();
            if (result == null)
            {
                currentObj = currentObj.parent;
            }
            else
            {
                currentObj = null;
            }
        }
        return result;
    }
}
