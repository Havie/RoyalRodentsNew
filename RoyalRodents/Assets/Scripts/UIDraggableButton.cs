using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDraggableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool _AssignmentButton = true;

    private bool _selected;
    private bool _hovering;
    private Vector3 _startLoc;
    private Quaternion _startRot;

    private float _Wiggle0= 0.1f;
    private float _Wiggle1= 3;
    private float _Wiggle2=-3;
    private float _Wiggle3 = -0.1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_selected)
        {
            Wiggle(this.transform.rotation.z);

            Vector2 _MousePos = (Input.mousePosition);
            this.transform.position = new Vector3(_MousePos.x, _MousePos.y, 0);

            if (Input.GetMouseButtonUp(0))
            {
                _hovering = false;
                _selected = false;

                if (_AssignmentButton)
                {
                    GameObject go = MVCController.Instance.checkClick(Input.mousePosition);
                    if (go != null)
                    {
                        if (go.GetComponent<BuildableObject>())
                        {
                            //Debug.Log("Successful Raycast2 =" + go.gameObject);
                            this.transform.GetComponent<UIRodentHolder>().ImSelected();
                            this.transform.position = _startLoc;
                        }
                        else
                        {
                           // Debug.Log("Failed Raycast =" + go.gameObject);
                            this.transform.position = _startLoc;
                            this.transform.rotation = _startRot;
                        }
                    }
                    else
                    {
                        this.transform.position = _startLoc;
                        this.transform.rotation = _startRot;
                    }
                }

                

            }
        }
        else if (_hovering)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log("Selected");
                _selected = true;
                _startLoc = this.transform.position;
                _startRot = this.transform.rotation;
            }
        }


    }

    private void Wiggle(float _z)
    {
        //Idk what kind fucking crazy ass values are coming in here ...
       // Debug.Log("Passed in " + _z);

        if (_z == _Wiggle0)
            this.transform.eulerAngles = new Vector3(0, 0, _Wiggle1);
        else if (_z == _Wiggle1)
            this.transform.eulerAngles = new Vector3(0, 0, _Wiggle3);
        else if (_z == _Wiggle2)
            this.transform.eulerAngles = new Vector3(0, 0, _Wiggle0);
        else if (_z == _Wiggle3)
            this.transform.eulerAngles = new Vector3(0, 0, _Wiggle2);
        else
        {
            int random = Random.Range(0, 3);
            if(random==0)
                this.transform.eulerAngles = new Vector3(0, 0, _Wiggle0);
            else if (random == 1)
                this.transform.eulerAngles = new Vector3(0, 0, _Wiggle1);
            else if (random == 2)
                this.transform.eulerAngles = new Vector3(0, 0, _Wiggle2);
            else if (random == 3)
                this.transform.eulerAngles = new Vector3(0, 0, _Wiggle3);
        }
            
    }

    //No fucking Idea why onMouseOver doesn't work for UI, leaving this here out of spite
    // completely unused 
    public void onMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
          //  Debug.Log("Selected");
            _selected = true;
        }
    }

    /************************************************************************/
    /* Docs for these events
     *https://docs.unity3d.com/Manual/SupportedEvents.html?_ga=2.25451573.923581064.1582999543-1338267523.1573929337                                                                  */
    /************************************************************************/

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Enter");
        _hovering = true;
        MVCController.Instance.CheckClicks(false);

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("EXIT!!");
        _hovering = false;
        MVCController.Instance.CheckClicks(true);
    }
}
