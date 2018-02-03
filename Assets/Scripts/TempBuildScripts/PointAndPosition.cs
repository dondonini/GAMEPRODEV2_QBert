using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndPosition : MonoBehaviour {

    public GameObject target;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) 
                if (hit.transform.CompareTag("Ground"))
                {
                    target.transform.position = hit.transform.position + new Vector3(0f, 1f, 0f);
                }
        }
    }
}
