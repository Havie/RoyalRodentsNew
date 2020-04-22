using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bOutpost : MonoBehaviour
{
	private static Sprite _builtSpriteLevel1;
    private static Sprite _builtSpriteLevel1_available;
    private static Sprite _builtSpriteLevel1_selected;
    private static Sprite _builtSpriteLevel2;
    private static Sprite _builtSpriteLevel2_available;
    private static Sprite _builtSpriteLevel2_selected;
    private static Sprite _builtSpriteLevel3;
    private static Sprite _builtSpriteLevel3_available;
    private static Sprite _builtSpriteLevel3_selected;
    private static int maxLevel = 3;

	private float _hitpoints = 50;
	private float _hitPointGrowth = 10;

	private static bool _isSet;

    private bool _selected;

	//create structure costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel1 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel2 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel3 = new Dictionary<ResourceManagerScript.ResourceType, int>();

    // Start is called before the first frame update
    void Start()
    {
		SetUpComponent();
	}

    // Update is called once per frame
    void Update()
    {

    }

	private static void SetUpComponent()
	{
		if (!_isSet)
		{
			//Set Upgrade/Build Costs (1-3 levels)
			_costLevel1.Add(ResourceManagerScript.ResourceType.Trash, 5);

			_costLevel2.Add(ResourceManagerScript.ResourceType.Trash, 8);
			_costLevel2.Add(ResourceManagerScript.ResourceType.Wood, 5);

			_costLevel3.Add(ResourceManagerScript.ResourceType.Trash, 13);
			_costLevel3.Add(ResourceManagerScript.ResourceType.Wood, 8);
			_costLevel3.Add(ResourceManagerScript.ResourceType.Stone, 5);

			_builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/Outpost/trash_outpost_blue_final");
            _builtSpriteLevel1_available = Resources.Load<Sprite>("Buildings/Outpost/outline_trash_outpost_final");
            _builtSpriteLevel1_selected = Resources.Load<Sprite>("Buildings/Outpost/outline_select_trash_outpost_final");
            _builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/Outpost/wood_outpost_blue_final");
            _builtSpriteLevel2_available = Resources.Load<Sprite>("Buildings/Outpost/outline_wood_outpost_final");
            _builtSpriteLevel2_selected = Resources.Load<Sprite>("Buildings/Outpost/outline_select_wood_outpost_final");
            _builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/Outpost/stone_outpost_final");
            _builtSpriteLevel3_available = Resources.Load<Sprite>("Buildings/Outpost/outline_stone_outpost_final");
            _builtSpriteLevel3_selected = Resources.Load<Sprite>("Buildings/Outpost/outline_select_stone_outpost_final");

            _isSet = true;
		}
	}

	public float BuildingComplete(int level)
	{
		if (!_isSet)
			SetUpComponent();

		if (level == 1)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
            //SetUp New Employees
            GameObject _Worker6Prefab = Resources.Load<GameObject>("UI/Workers6");
            _Worker6Prefab = Instantiate(_Worker6Prefab);
            _Worker6Prefab.transform.SetParent(this.transform);
            _Worker6Prefab.transform.localPosition = new Vector3(0, 0, 0);
            _Worker6Prefab.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

            //Hack Lazy
            Employee[] workers = _Worker6Prefab.GetComponent<eWorkers>().getWorkers();
            this.transform.GetComponent<BuildableObject>().ChangeWorkers(workers);
            // *MIGHT* have to change them to locked here incase inspector is wrong

        }
        else if (level == 2)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel2;
        else if (level == 3)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel3;


        BuildableObject bo = this.transform.GetComponent<BuildableObject>();
        bo.UnlockWorkers(2);

        return (_hitpoints + (_hitPointGrowth * level));
	}

    public void DemolishAction()
    {
        //To Do: Employment slots set back to normal
    }


    public static Dictionary<ResourceManagerScript.ResourceType, int> getCost(int level)
	{

		if (_costLevel1.Count == 0)
		{
			SetUpComponent();
		}

		if (level == 1)
		{
			return _costLevel1;
		}
		else if (level == 2)
		{
			return _costLevel2;
		}
		else if (level == 3)
		{
			return _costLevel3;
		}
		else
			return null;
	}

	public static int getMaxLevel()
	{
		return maxLevel;
	}

    public  Sprite getAvailable(int level)
    {
        if (level == 1)
            return _builtSpriteLevel1_available;
        if (level == 2)
            return _builtSpriteLevel2_available;
        if (level == 3)
            return _builtSpriteLevel3_available;

        else return null;
    }

    public  Sprite getSelected(int level)
    {
        if (level == 1)
            return _builtSpriteLevel1_selected;
        if (level == 2)
            return _builtSpriteLevel2_selected;
        if (level == 3)
            return _builtSpriteLevel3_selected;

        return null;
    }

    public void setSelected(bool cond)
    {
        _selected = cond;
    }
    public bool getSelected()
    {
        return _selected;
    }

    public void resetSprite(int level)
    {
        if (level == 1)
           this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
        else if (level == 2)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel2;
        else if (level == 3)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel3;
    }
}
