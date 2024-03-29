using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private GameObject selectFrame, rulePage, controlPage, stuffHider;
    private float[] selectFrameYPos = { .55f, -.85f, -2.25f, -3.65f };
    private int selectedAction = 0, activePage = 0, curRulePage = 0;
    private GameObject[] rulePages = new GameObject[7];
    private float[] rulePageXPos = { -3.45f, 3.45f };
    private int controlSchemeTemp;
    private string[] controlSchemeTexts = {"Mouse Only",
                                           "Keyboard Only",
                                           "Mouse & Keyboard"};
    private TMPro.TextMeshProUGUI controlSchemeText, controlsHeader;
    private SpriteRenderer applyFrame;
    private bool onArcade = false;
    private GameObject arcadeControls, nonArcadeControls,
                       nonArcadeControlObject, nonArcadeControlTris;
    private int controlScheme; //0 == mouse only, 1 == keyboard only, 2 == both

    //Controls
    KeyCode back = KeyCode.Backspace;//Arcade is Z

    KeyCode lClick = KeyCode.Mouse0;

    KeyCode moveU = KeyCode.W;
    KeyCode moveL = KeyCode.A;
    KeyCode moveD = KeyCode.S;
    KeyCode moveR = KeyCode.D;

    KeyCode selectL = KeyCode.J;//Arcade is Space
    KeyCode selectR = KeyCode.K;//Arcade is V
    KeyCode useSelect = KeyCode.Return;//Arcade is B

    // Start is called before the first frame update
    void Start()
    {
        if (onArcade)
        {
            Cursor.visible = false;
            controlScheme = 1;
            controlSchemeTemp = 1;
            PlayerPrefs.SetInt("Control Scheme", 1);
        }
        else if (PlayerPrefs.HasKey("Control Scheme"))
        {
            controlScheme = PlayerPrefs.GetInt("Control Scheme");
            controlSchemeTemp = PlayerPrefs.GetInt("Control Scheme");
        }
        else
        {
            controlScheme = 2;
            controlSchemeTemp = 2;
            PlayerPrefs.SetInt("Control Scheme", 2);
        }

        for (int i = 0; i < 10; i++)
        {
            if (!PlayerPrefs.HasKey("High Score " + i))
            {
                PlayerPrefs.SetString("High Score " + i, "--- - 0");
                PlayerPrefs.Save();
            }
        }

        selectFrame = GameObject.Find("Select Frame");

        rulePage = GameObject.Find("Rule Page");
        controlPage = GameObject.Find("Control Screen");
        controlsHeader = GameObject.Find("Controls Header").GetComponent<TMPro.TextMeshProUGUI>();
        arcadeControls = GameObject.Find("Arcade Controls");
        nonArcadeControls = GameObject.Find("NonArcade Controls");
        nonArcadeControlObject = GameObject.Find("NonArcade Control Screen Objects");
        nonArcadeControlTris = GameObject.Find("NonArcade Control Screen Tris");
        controlSchemeText = GameObject.Find("Control Scheme").GetComponent<TMPro.TextMeshProUGUI>();
        applyFrame = GameObject.Find("Apply Frame").GetComponent<SpriteRenderer>();

        for (int i = 0; i < rulePages.Length; i++)
        {
            rulePages[i] = GameObject.Find("Page " + i);
        }

        stuffHider = GameObject.Find("Stuff Hider");

        StartCoroutine(HideStuff());
    }

    IEnumerator HideStuff()
    {
        yield return new WaitForSeconds(.02f);

        rulePage.SetActive(false);

        if (onArcade)
        {
            nonArcadeControls.SetActive(false);
            nonArcadeControlObject.SetActive(false);
            nonArcadeControlTris.SetActive(false);
            controlsHeader.text = "Controls";
        }
        else
        {
            arcadeControls.SetActive(false);
        }
        controlPage.SetActive(false);

        if (controlScheme == 0)
        {
            selectFrame.SetActive(false);
        }

        stuffHider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (onArcade && Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (activePage == 0)
        {
            if (controlScheme != 1)
            {
                if (Input.GetKeyDown(lClick))
                {
                    Vector3 mousePos = Input.mousePosition;

                    if (mousePos.x > 701 && mousePos.x < 1218 && mousePos.y > 546 && mousePos.y < 651)
                    {
                        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
                    }

                    if (mousePos.x > 701 && mousePos.x < 1218 && mousePos.y > 395 && mousePos.y < 501)
                    {
                        activePage = 1;
                        rulePage.SetActive(true);
                    }

                    if (mousePos.x > 701 && mousePos.x < 1218 && mousePos.y > 243 && mousePos.y < 350)
                    {
                        activePage = 2;
                        controlPage.SetActive(true);
                        controlSchemeText.text = controlSchemeTexts[controlScheme];
                        controlSchemeTemp = controlScheme;
                        if (controlScheme == 0)
                        {
                            applyFrame.color = new Color(1, 1, 1, 1);
                        }
                        else
                        {
                            applyFrame.color = new Color(57 / 255f, 209 / 255f, 1 / 255f, 1);
                        }
                    }

                    if (mousePos.x > 701 && mousePos.x < 1218 && mousePos.y > 92 && mousePos.y < 199)
                    {
                        Application.Quit();
                    }
                }
            }

            if (controlScheme != 0)
            {
                if (Input.GetKeyDown(selectL))
                {
                    if (selectedAction > 0)
                    {
                        selectedAction--;
                        selectFrame.transform.position = new Vector3(selectFrame.transform.position.x,
                                                                     selectFrameYPos[selectedAction],
                                                                     selectFrame.transform.position.z);
                    }
                }

                if (Input.GetKeyDown(selectR))
                {
                    if (selectedAction < 3)
                    {
                        selectedAction++;
                        selectFrame.transform.position = new Vector3(selectFrame.transform.position.x,
                                                                     selectFrameYPos[selectedAction],
                                                                     selectFrame.transform.position.z);
                    }
                }

                if (Input.GetKeyDown(back))
                {
                    Application.Quit();
                }

                if (Input.GetKeyDown(useSelect))
                {
                    switch (selectedAction)
                    {
                        case 0:
                            SceneManager.LoadScene("InGame", LoadSceneMode.Single);
                            break;
                        case 1:
                            activePage = 1;
                            rulePage.SetActive(true);
                            break;
                        case 2:
                            activePage = 2;
                            controlPage.SetActive(true);
                            controlSchemeText.text = controlSchemeTexts[controlScheme];
                            controlSchemeTemp = controlScheme;
                            if (controlScheme == 0)
                            {
                                applyFrame.color = new Color(1, 1, 1, 1);
                            }
                            else
                            {
                                applyFrame.color = new Color(57 / 255f, 209 / 255f, 1 / 255f, 1);
                            }
                            break;
                        case 3:
                            Application.Quit();
                            break;
                    }
                }
            }
        }
        else if (activePage == 1)
        {
            if (controlScheme != 0)
            {
                if (Input.GetKeyDown(selectL))
                {
                    if (curRulePage > 0)
                    {
                        rulePages[curRulePage + 1].transform.position = new Vector3(20,
                                                      rulePages[curRulePage + 1].transform.position.y,
                                                      rulePages[curRulePage + 1].transform.position.z);
                        curRulePage--;
                        rulePages[curRulePage].transform.position = new Vector3(
                                                          rulePageXPos[0],
                                                          rulePages[curRulePage].transform.position.y,
                                                          rulePages[curRulePage].transform.position.z);
                        rulePages[curRulePage + 1].transform.position = new Vector3(
                                                      rulePageXPos[1],
                                                      rulePages[curRulePage + 1].transform.position.y,
                                                      rulePages[curRulePage + 1].transform.position.z);
                    }
                }

                if (Input.GetKeyDown(selectR))
                {
                    if (curRulePage < 5)
                    {
                        rulePages[curRulePage].transform.position = new Vector3(-20,
                                                          rulePages[curRulePage].transform.position.y,
                                                          rulePages[curRulePage].transform.position.z);
                        curRulePage++;
                        rulePages[curRulePage].transform.position = new Vector3(
                                                          rulePageXPos[0],
                                                          rulePages[curRulePage].transform.position.y,
                                                          rulePages[curRulePage].transform.position.z);
                        rulePages[curRulePage + 1].transform.position = new Vector3(
                                                      rulePageXPos[1],
                                                      rulePages[curRulePage + 1].transform.position.y,
                                                      rulePages[curRulePage + 1].transform.position.z);
                    }
                }

                if (Input.GetKeyDown(back))
                {
                    activePage = 0;
                    rulePage.SetActive(false);
                }
            }

            if (controlScheme != 1)
            {
                if (Input.GetKeyDown(lClick))
                {
                    Vector3 mousePos = Input.mousePosition;

                    if (mousePos.x > 25 && mousePos.x < 206 && mousePos.y > 450 && mousePos.y < 630)
                    {
                        if (curRulePage > 0)
                        {
                            rulePages[curRulePage + 1].transform.position = new Vector3(20,
                                                          rulePages[curRulePage + 1].transform.position.y,
                                                          rulePages[curRulePage + 1].transform.position.z);
                            curRulePage--;
                            rulePages[curRulePage].transform.position = new Vector3(
                                                              rulePageXPos[0],
                                                              rulePages[curRulePage].transform.position.y,
                                                              rulePages[curRulePage].transform.position.z);
                            rulePages[curRulePage + 1].transform.position = new Vector3(
                                                          rulePageXPos[1],
                                                          rulePages[curRulePage + 1].transform.position.y,
                                                          rulePages[curRulePage + 1].transform.position.z);
                        }
                    }

                    if (mousePos.x > 1708 && mousePos.x < 1889 && mousePos.y > 450 && mousePos.y < 630)
                    {
                        if (curRulePage < 5)
                        {
                            rulePages[curRulePage].transform.position = new Vector3(-20,
                                                              rulePages[curRulePage].transform.position.y,
                                                              rulePages[curRulePage].transform.position.z);
                            curRulePage++;
                            rulePages[curRulePage].transform.position = new Vector3(
                                                              rulePageXPos[0],
                                                              rulePages[curRulePage].transform.position.y,
                                                              rulePages[curRulePage].transform.position.z);
                            rulePages[curRulePage + 1].transform.position = new Vector3(
                                                          rulePageXPos[1],
                                                          rulePages[curRulePage + 1].transform.position.y,
                                                          rulePages[curRulePage + 1].transform.position.z);
                        }
                    }

                    if (mousePos.x > 25 && mousePos.x < 206 && mousePos.y > 881 && mousePos.y < 1062)
                    {
                        activePage = 0;
                        rulePage.SetActive(false);
                    }
                }
            }
        }
        else if (activePage == 2)
        {
            if (controlScheme != 0)
            {
                if (Input.GetKeyDown(selectL) && !onArcade)
                {
                    controlSchemeTemp--;
                    if (controlSchemeTemp < 0)
                    {
                        controlSchemeTemp = 2;
                    }
                    controlSchemeText.text = controlSchemeTexts[controlSchemeTemp];
                }

                if (Input.GetKeyDown(selectR) && !onArcade)
                {
                    controlSchemeTemp++;
                    if (controlSchemeTemp > 2)
                    {
                        controlSchemeTemp = 0;
                    }
                    controlSchemeText.text = controlSchemeTexts[controlSchemeTemp];
                }

                if (Input.GetKeyDown(useSelect) && !onArcade)
                {
                    controlScheme = controlSchemeTemp;
                    PlayerPrefs.SetInt("Control Scheme", controlScheme);
                    PlayerPrefs.Save();
                    if (controlScheme == 0)
                    {
                        applyFrame.color = new Color(1, 1, 1, 1);
                        selectFrame.SetActive(false);
                    }
                    else
                    {
                        applyFrame.color = new Color(57 / 255f, 209 / 255f, 1 / 255f, 1);
                        selectFrame.SetActive(true);
                    }
                }

                if (Input.GetKeyDown(back))
                {
                    activePage = 0;
                    controlPage.SetActive(false);
                }
            }

            if (controlScheme != 1)
            {
                if (Input.GetKeyDown(lClick))
                {
                    Vector3 mousePos = Input.mousePosition;

                    if (mousePos.x > 593 && mousePos.x < 659 && mousePos.y > 241 && mousePos.y < 350)
                    {
                        controlSchemeTemp--;
                        if (controlSchemeTemp < 0)
                        {
                            controlSchemeTemp = 2;
                        }
                        controlSchemeText.text = controlSchemeTexts[controlSchemeTemp];
                    }

                    if (mousePos.x > 1260 && mousePos.x < 1328 && mousePos.y > 241 && mousePos.y < 350)
                    {
                        controlSchemeTemp++;
                        if (controlSchemeTemp > 2)
                        {
                            controlSchemeTemp = 0;
                        }
                        controlSchemeText.text = controlSchemeTexts[controlSchemeTemp];
                    }

                    if (mousePos.x > 701 && mousePos.x < 1218 && mousePos.y > 92 && mousePos.y < 199)
                    {
                        controlScheme = controlSchemeTemp;
                        PlayerPrefs.SetInt("Control Scheme", controlScheme);
                        PlayerPrefs.Save();
                        if (controlScheme == 0)
                        {
                            applyFrame.color = new Color(1, 1, 1, 1);
                            selectFrame.SetActive(false);
                        }
                        else
                        {
                            applyFrame.color = new Color(57 / 255f, 209 / 255f, 1 / 255f, 1);
                            selectFrame.SetActive(true);
                        }
                    }

                    if (mousePos.x > 25 && mousePos.x < 206 && mousePos.y > 881 && mousePos.y < 1062)
                    {
                        activePage = 0;
                        controlPage.SetActive(false);
                    }
                }
            }
        }
    }
}
