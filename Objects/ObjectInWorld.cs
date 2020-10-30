using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInWorld : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
           GameObject.Find("-UI-").GetComponent<UI>().ClickToObjectInWorld(gameObject);
        }
    }
    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GameObject.Find("-UI-").GetComponent<UI>().EnterToObjectInWorld(gameObject);
        }
    }
}
