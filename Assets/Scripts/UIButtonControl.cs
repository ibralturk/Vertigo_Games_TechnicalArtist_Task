using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonControl : MonoBehaviour
{
    public TMP_Text Menu;
    public GameObject Maincamera;

    public GameObject leftinventoryui;
    public GameObject leftattachmentui;
    public GameObject tryui;
    public GameObject sightui;

    public GameObject defaultsight;
    public GameObject thermalimaging;
    public GameObject nightstalker;

    public Vector3 defaultposition = new Vector3(0, 0, 0);   
    public Vector3 sightposition = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        sightui.gameObject.SetActive(false);
        defaultsight.gameObject.SetActive(true);
        thermalimaging.gameObject.SetActive(false);
        nightstalker.gameObject.SetActive(false);
        leftattachmentui.gameObject.SetActive(false);
        leftinventoryui.gameObject.SetActive(true);
        tryui.gameObject.SetActive(true);

        Maincamera.transform.position = defaultposition;

        Menu.text = "INVENTORY";
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backbutton()
    {
        Menu.text = "INVENTORY";
        Maincamera.transform.position = defaultposition;
        leftattachmentui.gameObject.SetActive(false);
        leftinventoryui.gameObject.SetActive(true);
        tryui.gameObject.SetActive(true);
        sightui.gameObject.SetActive(false);
    }
    public void sightinventorybutton()
    {
        sightui.gameObject.SetActive(true);
        Maincamera.transform.position = sightposition;
        Menu.text = "ATTACHMENT";
        leftattachmentui.gameObject.SetActive(true);
        leftinventoryui.gameObject.SetActive(false);
        tryui.gameObject.SetActive(false);
    }

    public void defaultsightbutton()
    {
            nightstalker.gameObject.SetActive(false);
            thermalimaging.gameObject.SetActive(false);

            defaultsight.gameObject.SetActive(true);    
 
    }

    public void thermalimagingbutton()
    {
        defaultsight.gameObject.SetActive(false);
        nightstalker.gameObject.SetActive(false);

        thermalimaging.gameObject.SetActive(true);        
    }

    public void nightstalkerbutton()
    {
        thermalimaging.gameObject.SetActive(false);
        defaultsight.gameObject.SetActive(false);

        nightstalker.gameObject.SetActive(true);
    }
}
