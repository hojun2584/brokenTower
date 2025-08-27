using Hojun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{

    GameObject warrior;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 마우스 왼쪽 버튼 클릭 감지
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.TryGetComponent<Node>(out Node spawn))
                {
                    Vector3 spawnPosition = spawn.GetPositionSetY(5f);
                    GameObject spawnCharater = Instantiate(warrior , spawnPosition, Quaternion.identity );
                    

                    if(spawnCharater.TryGetComponent<Summoned>(out Summoned warriorObject))
                    {

                    }


                }
            }

        }

    }
}
