using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioNPC : MonoBehaviour
{
    public float displaytime = 4.0f;
    public GameObject dialogBox;
    float timerDisplay;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        dialogBox.SetActive(false);
        timerDisplay = -1.0f;

        animator = GetComponent<Animator>();
        animator.SetTrigger("Fix");
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        timerDisplay = displaytime;
        dialogBox.SetActive(true);        

    }
}
