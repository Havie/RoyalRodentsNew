using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bTownCenter : MonoBehaviour
{
    [SerializeField]
    private static Sprite _builtSpriteLevel1;
    private static Sprite _builtSpriteLevel2;
    private static Sprite _builtSpriteLevel3;
    private static Sprite _builtSpriteLevel4;
    private static Sprite _builtSpriteLevel5;
    private static int maxLevel = 5;

    private static float _hitpoints = 250;
    private float _hitPointGrowth = 10;

    private static bool _isSet;

    //create structure costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel1 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel2 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel3 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel4 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel5 = new Dictionary<ResourceManagerScript.ResourceType, int>();

    // Start is called before the first frame update
    void Start()
    {
        SetUpComponent();
        GameManager.Instance.setTownCenter(this);
    }


    private static void SetUpComponent()
    {
        if (!_isSet)
        {
            //Set Upgrade/Build Costs (1-5 levels)
            _costLevel1.Add(ResourceManagerScript.ResourceType.Trash, 6);

            _costLevel2.Add(ResourceManagerScript.ResourceType.Trash, 10);
            _costLevel2.Add(ResourceManagerScript.ResourceType.Wood, 6);

            _costLevel3.Add(ResourceManagerScript.ResourceType.Trash, 18);
            _costLevel3.Add(ResourceManagerScript.ResourceType.Wood, 10);
            _costLevel3.Add(ResourceManagerScript.ResourceType.Stone, 6);

            _costLevel4.Add(ResourceManagerScript.ResourceType.Trash, 32);
            _costLevel4.Add(ResourceManagerScript.ResourceType.Wood, 18);
            _costLevel4.Add(ResourceManagerScript.ResourceType.Stone, 10);
            _costLevel4.Add(ResourceManagerScript.ResourceType.Shiny, 2);

            _costLevel5.Add(ResourceManagerScript.ResourceType.Trash, 57);
            _costLevel5.Add(ResourceManagerScript.ResourceType.Wood, 32);
            _costLevel5.Add(ResourceManagerScript.ResourceType.Stone, 18);
            _costLevel5.Add(ResourceManagerScript.ResourceType.Shiny, 5);

            _builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/TownCenter/tc_1_final");
            _builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/TownCenter/tc_2_blue_final");
            _builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/TownCenter/tc_3_blue_final");
            _builtSpriteLevel4 = Resources.Load<Sprite>("Buildings/TownCenter/tc_4_blue_final");
            _builtSpriteLevel5 = Resources.Load<Sprite>("Buildings/TownCenter/tc_5_blue_final");
            _isSet = true;
        }
    }

    public float BuildingComplete(int level)
    {
        if (!_isSet)
            SetUpComponent();

        //Set new structure sprite
        if (level == 1)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
        else if (level == 2)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel2;
        else if (level == 3)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel3;
        else if (level == 4)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel4;
        else if (level == 5)
            this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel5;

        return (_hitpoints + (_hitPointGrowth * level));
    }

    public void StartingBuildComplete()
    {
        if (!_builtSpriteLevel1)
        {
            _builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/TownCenter/tc_1_final");
        }
        this.transform.GetComponent<BuildableObject>().SetType("TownCenter");
        this.transform.GetComponent<BuildableObject>().SetLevel(1);
        this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
        //MEGA hack of all Hacks
       // this.transform.GetComponentInChildren<Employee>().gameObject.GetComponentInChildren<eWorkerOBJ>().gameObject.GetComponent<SpriteRenderer>().sprite = null;
        BuildableObject bo = this.GetComponent<BuildableObject>();
        foreach( Employee e in bo._Workers)
        {
            GameObject go = e.gameObject;
            if (go)
            {
                eWorkerOBJ worker = go.GetComponentInChildren<eWorkerOBJ>();
                if (worker)
                {
                    go = worker.gameObject;
                    if (go)
                    {
                        SpriteRenderer sp = go.GetComponent<SpriteRenderer>();
                        if (sp)
                            sp.sprite = null;
                    }
                }
            }
        }


        // Debug.Log("Created Initial TownCenter with Sprite:::" + _built);
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
		else if (level == 4)
		{
			return _costLevel4;
		}
		else if (level == 4)
		{
			return _costLevel4;
		}
		else if (level == 5)
		{
			return _costLevel5;
		}
		else
			return null;
	}

	public static int getMaxLevel()
	{
		return maxLevel;
	}

    public static float getHPStats()
    {
        return _hitpoints;
    }
}
