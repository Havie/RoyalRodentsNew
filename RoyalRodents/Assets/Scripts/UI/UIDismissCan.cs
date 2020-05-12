using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDismissCan : MonoBehaviour
{
    public Sprite _sp1;
    public Sprite _sp2;

    private bool _mode;

    // Start is called before the first frame update
    void Start()
    {
        //Get IMGs
        _sp1 = Resources.Load<Sprite>("UI/trash");
        _sp2 = Resources.Load<Sprite>("UI/Trash Fill");

        this.GetComponent<Image>().sprite = _sp1;
    }

    public void ImClicked()
    {
        _mode = !_mode;
        MVCController.Instance.setDismissMode(_mode);
        ToggleSprites();
        ShowOnRodents();

    }

    private void ToggleSprites()
    {
        if (_mode)
            this.GetComponent<Image>().sprite = _sp2;
        else
            this.GetComponent<Image>().sprite = _sp1;
    }

    private void ShowOnRodents()
    {
        foreach(Rodent r in GameManager.Instance.getPlayerRodents())
        {
            r.ShowDismissMenu(_mode);
        }

        if (!_mode)
            MVCController.Instance.showRecruitMenu(false, Vector3.zero, "", 1, 1);
    }

    public void setOff()
    {
        _mode = false;
        ToggleSprites();
        ShowOnRodents();
    }

}
