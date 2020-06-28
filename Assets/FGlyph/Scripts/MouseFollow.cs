using UnityEngine;
using System.Collections.Generic;

public class MouseFollow : MonoBehaviour
{

    [SerializeField]
    GameObject prefab;

    public float speed = 8.0f;
    public float distanceFromCamera = 5.0f;

    private int i = 0;
    private List<Vector2> mouseCoords;



    private void AnimateCoords()
    {

        Vector3 mousePosition = new Vector3(mouseCoords[i].x * Screen.width, mouseCoords[i].y *Screen.height, distanceFromCamera);
        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld, 1.0f - Mathf.Exp(-speed * Time.deltaTime));

        Instantiate(prefab, position, prefab.transform.rotation);
        i++;
        if (i == mouseCoords.Count)
        {
            CancelInvoke("AnimateCoords");
            mouseCoords.Clear();
            i = 0;
        }
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
       
        mousePosition.z = distanceFromCamera;

        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld, 1.0f - Mathf.Exp(-speed * Time.deltaTime));

       // Debug.Log(mousePosition + " " + mouseScreenToWorld + " " + position);
        transform.position = position;
    }



}

