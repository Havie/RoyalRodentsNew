using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVolume : MonoBehaviour
{
    public bool _EnemySpawn = false;

    public bool _rightSide;
    public Transform _EnemySpawnDummy;

    private bool _timeToSpawn;
    private ArrayList _AvailableRodents = new ArrayList();

    private float _baseLineTime = 0f;
    [SerializeField]
    private bool _occupied; 

    GameObject Rat;
    GameObject Beaver;

    [SerializeField]
    private int _EnemyCount=2;

    [SerializeField]
    private bool _inPlayerZone;


    // Start is called before the first frame update
    void Start()
    {
        AddType(Rodent.eRodentType.Rat);
        AddType(Rodent.eRodentType.Beaver);

        Rat = Resources.Load<GameObject>("Rodent/FatRat/RatPreFab");
        Beaver = Resources.Load<GameObject>("Rodent/Beaver/BeaverPreFab");

        _timeToSpawn = true;

        if (_EnemySpawnDummy == null)
            _EnemySpawnDummy = GameObject.FindGameObjectWithTag("EnemyRodents").transform;

        //subscribe to the event system
        if (_EnemySpawn)
            EventSystem.Instance.WaveTrigger += SpawnSomething;
        else
            EventSystem.Instance.SpawnTrigger += SpawnSomething;


    }

    void LateUpdate()
    {
        if(_timeToSpawn && !_occupied)
        {
            //Spawn random rodent based on prefab
            int index = Random.Range(0, _AvailableRodents.Count);
           // Debug.LogWarning( " Must not be inclusive, chose index=" + index);
            Rodent.eRodentType selected =(Rodent.eRodentType)_AvailableRodents[index];
              SpawnRodent(selected);

            StartCoroutine(SpawnCountDown());
        }
        
    }
    void onDisable()
    {
        //unsubscribe
        EventSystem.Instance.WaveTrigger -= SpawnSomething;
    }

    IEnumerator SpawnCountDown()
    {
        //Need some other constraints on an overall limit to how many spawn
        _timeToSpawn = false;
        float _startTime = Time.time;
        if (_EnemySpawn)
            _baseLineTime = _startTime;
        yield return new WaitForSeconds(0.5f);
        while(_startTime > _baseLineTime)
        {
            _startTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _timeToSpawn = true;
    }

    public void AddType(Rodent.eRodentType type)
    {
            _AvailableRodents.Add(type);
    }

    private void SpawnRodent(Rodent.eRodentType type)
    {
        if (!_occupied)
        {
            if (!_EnemySpawn)
            {
                //Spawn Recruitable Rodents
                if (type == Rodent.eRodentType.Rat)
                {
                    spawnThis(Rat, false);
                }
                else if (type == Rodent.eRodentType.Beaver)
                {
                    spawnThis(Beaver, false);
                }
            }
            else
            {
                //Spawn Enemy Rodents
                if (type == Rodent.eRodentType.Rat)
                {
                    spawnThis(Rat, true);
                }
                else if (type == Rodent.eRodentType.Beaver)
                {
                    spawnThis(Beaver, true);
                }
            }
        }
    }
    private void spawnThis(GameObject toSpawn, bool Enemy)
    {
        if(Enemy)
        {

            GameObject _spawnedRodent = GameObject.Instantiate(toSpawn, this.transform.position, this.transform.rotation);

            //parent this to keep hierarchy clean
            if (_EnemySpawnDummy)
                _spawnedRodent.transform.SetParent(_EnemySpawnDummy);
            else
                Debug.LogWarning("No Enemy Dummy for Hierarchy");

            // Tag becoming obsolete
            _spawnedRodent.tag = "EnemyRodent";
            // Ensure Sprite is enemy
            Rodent r = _spawnedRodent.GetComponent<Rodent>();
            if (r)
            {
                r.setTeam(2);
                // Force them to be aggressive and head toward player   //hack
                if (_inPlayerZone)
                    r.setTargetEnemyVersion(GameManager.Instance.getTownCenter().gameObject);
            }
            // Increase some kind of count
            --_EnemyCount;
            if (_EnemyCount == 0)
                _occupied = true;
        }
        else
        {
            _occupied = true;
            GameObject _spawnedRat = GameObject.Instantiate(toSpawn, this.transform.position, this.transform.rotation);
            //parent this thing to this obj keep hierarchy cleaner? Might end up negatively affecting the subject Script?
            _spawnedRat.transform.SetParent(this.transform);

            // Tag becoming obsolete
            _spawnedRat.tag = "NeutralRodent";
            // Ensure Sprite is Neutral
            _spawnedRat.GetComponent<Rodent>().setTeam(0);
            // Increase some kind of count
        }
    }
    public void SpawnSomething()
    {
        //print("Spawning something.." + this.gameObject.name  + " __ " + this.transform.parent.name);
        //Do a random roll to see if we spawn (50/50)
        int roll = Random.Range(0, 10);
        if (roll % 2 == 0)
        {
            _occupied = false;
            if (_EnemySpawn)
            {
                if (_inPlayerZone)
                {
                    int maxEnemy = (int)System.Math.Ceiling(Cycle2DDN.Instance.getDayCount() / 2.0);
                   // print(maxEnemy);
                    _EnemyCount = Random.Range((maxEnemy / 2) + 1, maxEnemy);
                    //TO:DO base this on something (mischief meter RIP)
                    SpawnaKing();
                }
                else
                    _EnemyCount = 1;

                //print("EnemyCount= " + _EnemyCount);
            }
            //Pop up text wave has spawned
            if (_EnemyCount > 0 && _inPlayerZone)
            {
                if (_rightSide)
                    UISpeechBubble.Instance.ShowRightSide(true);
                else
                    UISpeechBubble.Instance.ShowLeftSide(true);
            }
            else
                print("Failed spawn:" + this.gameObject.name + " __ " + this.transform.parent.name);
        }
    }

    public void SpawnaKing()
    {
        GameObject king = Resources.Load<GameObject>("Rodent/King_Enemy/EnemyKingPreFab");
        GameObject _spawnedRodent = GameObject.Instantiate(king, this.transform.position, this.transform.rotation);
        if (_EnemySpawnDummy)
            _spawnedRodent.transform.SetParent(_EnemySpawnDummy);

        Rodent r = _spawnedRodent.GetComponent<Rodent>();
        if (r)
        {
            r.setTeam(2);
            // Force them to be aggressive and head toward player   //hack
            if (_inPlayerZone)
                r.setTargetEnemyVersion(GameManager.Instance.getTownCenter().gameObject);
        }

    }
}
