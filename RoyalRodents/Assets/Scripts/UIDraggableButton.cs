using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool _AssignmentButton = true;

    private bool _selected;
    private bool _hovering;
    private Vector3 _startLoc;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_selected)
        {
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
                        if (go.tag.Equals("DummyObjMVC"))
                        {
                           // Debug.Log("Successful Raycast =" + go.gameObject);
                            this.transform.GetComponent<UIRodentHolder>().ImSelected();
                            this.transform.position = _startLoc;
                        }
                        else
                        {
                           // Debug.Log("Failed Raycast =" + go.gameObject);
                            this.transform.position = _startLoc;
                        }
                    }
                    else
                        this.transform.position = _startLoc;
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
            }
        }


    }

    //No fucking Idea why onMouseOver doesn't work for UI, leaving this here out of spite
    // completely unused 
    public void onMouseOver()
    {
        Debug.Log("Enter");
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Selected");
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
