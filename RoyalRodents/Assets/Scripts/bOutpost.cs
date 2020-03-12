using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bOutpost : MonoBehaviour
{
	private Sprite _builtSpriteLevel1;
	private Sprite _builtSpriteLevel2;
	private Sprite _builtSpriteLevel3;
	private static int maxLevel = 3;

	private float _hitpoints = 50;
	private float _hitPointGrowth = 10;

	private static bool _isSet;

	//create structure costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel2 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel3 = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        _builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/Outpost/trash_outpost");
		_builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/Outpost/wood_outpost");
		_builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/Outpost/stone_outpost");
	}

    // Update is called once per frame
    void Update()
    {

    }

	private static void setupCosts()
	{
		if (!_isSet)
		{
			//Set Upgrade/Build Costs (1-3 levels)
			_costLevel1.Add("Trash", 2);

			_costLevel2.Add("Trash", 4);
			_costLevel2.Add("Wood", 2);

			_costLevel3.Add("Trash", 6);
			_costLevel3.Add("Wood", 4);
			_costLevel3.Add("Metal", 2);

			_isSet = true;
		}
	}

	public float BuildingComplete(int level)
	{
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

	public static Dictionary<string, int> getCost(int level)
	{

		if (_costLevel1.Count == 0)
		{
			setupCosts();
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
}
