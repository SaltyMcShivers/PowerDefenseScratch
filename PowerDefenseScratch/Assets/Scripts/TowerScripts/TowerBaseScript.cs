using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBaseScript : MonoBehaviour {
    public TowerManagerScript manager;

    TowerPowerScript currentTower;

    public GameObject towerSelectionUI;

    public Transform menuContainer;
    public GameObject towerReadyIcon;

    public ElectricPathNode electricSource;

    public Renderer boxModel;
    Material boxMaterial;
    public Color lightColor;

    bool preBuiltTower;
    bool disabled;
    bool disableBuild;
    bool canBuild;

    void Awake()
    {
        boxMaterial = boxModel.material;
    }

    void Start()
    {
        StartCoroutine("SetUpPreBuiltTower");
        Messenger.AddListener("Switch Menu Added", DisableInteraction);
        Messenger.AddListener("Switch Menu Removed", EnableInteraction);
    }

    public bool IsGettingPower()
    {
        return electricSource.IsGettingEnergy();
    }

    IEnumerator SetUpPreBuiltTower()
    {
        yield return new WaitForFixedUpdate();
        TowerPowerScript pow = GetComponentInChildren<TowerPowerScript>();
        if (pow != null)
        {
            preBuiltTower = true;
            boxModel.gameObject.SetActive(false);
            currentTower = pow;
            currentTower.electricSource = electricSource;
            Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
        }
    }

    public void SetBuildDisable(bool b)
    {
        disableBuild = b;
        if (canBuild)
        {
            if (disableBuild)
            {
                boxMaterial.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
            }
            else
            {
                boxMaterial.SetColor("_EmissionColor", lightColor);
            }
        }
    }

    void EnableInteraction()
    {
        disabled = false;
    }

    void DisableInteraction()
    {
        disabled = true;
    }

    void Update()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer) return;
        foreach(Touch t in Input.touches)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(t.position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (gameObject.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
            {
                if (currentTower == null && menuContainer.childCount == 0)
                {
                    if (!manager.CanBuildTower()) return;
                    GameObject newTS = Instantiate(towerSelectionUI, transform.position + Vector3.back * menuContainer.position.z, Quaternion.identity) as GameObject;
                    newTS.transform.SetParent(menuContainer);
                    newTS.GetComponent<TowerSelectionScript>().SetUpTowerSelection(this, manager.legalTowers);
                }
                else if (menuContainer.childCount == 1)
                {
                    menuContainer.GetComponentInChildren<TowerSelectionScript>().RemoveMenu();
                }
                else if (manager.IsTowerDeleting())
                {
                    RemoveTower();
                }
                else
                {
                    currentTower.TogglePower();
                }
                return;
            }
        }
    }

    void OnMouseOver()
    {
        if (disabled) return;
        if (Time.timeScale == 0) return;
        if (Input.GetMouseButtonUp(0))
        {
            if (currentTower == null && menuContainer.childCount == 0)
            {
                if (disableBuild) return;
                if (!manager.CanBuildTower()) return;
                GameObject newTS = Instantiate(towerSelectionUI, transform.position, Quaternion.identity) as GameObject;
                newTS.transform.SetParent(menuContainer);
                newTS.GetComponent<TowerSelectionScript>().SetUpTowerSelection(this, manager.legalTowers, manager.TutorialTower);
                /*
                GameObject newTower = Instantiate(towerToBuild, transform.position, Quaternion.identity) as GameObject;
                currentTower = newTower.GetComponent<TowerPowerScript>();
                Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
                */
            }
            else if (menuContainer.childCount == 1)
            {
                menuContainer.GetComponentInChildren<TowerSelectionScript>().RemoveMenu();
            }
            else if (manager.IsTowerDeleting())
            {
                RemoveTower();
            }
            else
            {
                currentTower.TogglePower();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RemoveTower();
        }
    }

    public void RemoveTower()
    {
        if (currentTower == null) return;
        if (preBuiltTower) return;
        Messenger<TowerPowerScript>.Invoke("Tower Destroyed", currentTower.GetComponentInChildren<TowerPowerScript>());
        StartCoroutine("KillTowerCoroutine");
    }

    IEnumerator KillTowerCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject oldTower = currentTower.gameObject;
        currentTower = null;
        Destroy(oldTower);
        boxModel.gameObject.SetActive(true);
    }

    public void BuildTower(GameObject tower)
    {
        GameObject newTower = Instantiate(tower, transform.position, Quaternion.identity) as GameObject;
        currentTower = newTower.GetComponent<TowerPowerScript>();
        currentTower.electricSource = electricSource;
        newTower.transform.SetParent(transform);
        if (!manager.CanBuildTower()) SetBuildReadyIcon();
        Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
        boxModel.gameObject.SetActive(false);
    }

    public void SetBuildReadyIcon(bool makeActive = false)
    {
        canBuild = makeActive && currentTower == null;
        if (makeActive && !disableBuild)
        {
            boxMaterial.SetColor("_EmissionColor", lightColor);
        }
        else
        {
            boxMaterial.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
        }
        //towerReadyIcon.SetActive(makeActive && currentTower == null);
    }

    public void RemoveTowerReset()
    {
        GameObject oldTower = currentTower.gameObject;
        currentTower = null;
        DestroyImmediate(oldTower);
        boxModel.gameObject.SetActive(true);
    }
}
