using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVolume : MonoBehaviour
{
    public bool _EnemySpawn = false;
    public Transform _EnemySpawnDummy;

    private bool _timeToSpawn;
    private ArrayList _AvailableRodents = new ArrayList();

    private float _baseLineTime = 0f;
    [SerializeField]
    private bool _occupied; 

    GameObject Rat;

    [SerializeField]
    private int _EnemyCount=2;

    [SerializeField]
    private bool _inPlayerZone;


    // Start is called before the first frame update
    void Start()
    {
        AddType(Rodent.eRodentType.Rat);

        Rat = Resources.Load<GameObject>("Rodent/FatRat/RatPreFab");

        _timeToSpawn = true;

        if (_EnemySpawnDummy == null)
            _EnemySpawnDummy = GameObject.FindGameObjectWithTag("EnemyRodents").transform;

        //subscribe to the event system
        EventSystem.Instance.spawnTrigger += SpawnSomething;

    }

    void LateUpdate()
    {
        if(_timeToSpawn && !_occupied)
        {
            //Spawn random rodent based on prefab
            int index = Random.Range(0, _AvailableRodents.Count - 1);
            Rodent.eRodentType selected =(Rodent.eRodentType)_AvailableRodents[index];

            SpawnRodent(selected);

            StartCoroutine(SpawnCountDown());
        }
        
    }
    void onDisable()
    {
        //unsubscribe
        EventSystem.Instance.spawnTrigger -= SpawnSomething;
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
                    _occupied = true;
                    GameObject _spawnedRat = GameObject.Instantiate(Rat, this.transform.position, this.transform.rotation);
                    //parent this thing to this obj keep hierarchy cleaner? Might end up negatively affecting the subject Script?
                    _spawnedRat.transform.SetParent(this.transform);

                    // Tag becoming obsolete
                    _spawnedRat.tag = "NeutralRodent";
                    // Ensure Sprite is Neutral
                    _spawnedRat.GetComponent<Rodent>().setTeam(0);
                    // Increase some kind of count
                }
            }
            else
            {
                //Spawn Enemy Rodents
                if (type == Rodent.eRodentType.Rat)
                {
                    GameObject _spawnedRat = GameObject.Instantiate(Rat, this.transform.position, this.transform.rotation);

                    //parent this to keep hierarchy clean
                    if (_EnemySpawnDummy)
                        _spawnedRat.transform.SetParent(_EnemySpawnDummy);
                    else
                        Debug.LogWarning("No Enemy Dummy for Hierarchy");

                    // Tag becoming obsolete
                    _spawnedRat.tag = "EnemyRodent";
                    // Ensure Sprite is Neutral
                    Rodent r = _spawnedRat.GetComponent<Rodent>();
                    if (r)
                    {
                        r.setTeam(2);      
                        // Force them to be aggressive and head toward player   //hack
                        if(_inPlayerZone)
                            r.setTargetEnemyVersion(GameManager.Instance.getTownCenter().gameObject);
                    }
                    // Increase some kind of count
                   --_EnemyCount;
                    if (_EnemyCount == 0)
                        _occupied = true;
                }
            }
        }
    }

    public void SpawnSomething()
    {
        _occupied = false;
        _EnemyCount = 3;
    }

    public void SpawnaKing()
    {
        //To-Do: spawn erm
    }
}
