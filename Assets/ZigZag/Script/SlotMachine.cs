using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SlotManager : MonoBehaviour
{
    public GameObject itemPre;
    public GameObject machineParent;
    public GameObject[] spawnPos;
    public Color32 correctColor;
    public Color32 normalColor;

    public List<GameObject> positionItems = new List<GameObject>();
    public GameObject moveBox;
    public Image resultImage;


    public float moveDuration = 0.1f;  
    public float delayBetweenMoves = 0.1f;  
    public Button moveButton;
   
    private bool isMoving = false;
    private float total_balence = 50f;
    private float bet_balence = 0.1f;
    //float total_bet_belence = 0f;
    private float win_balence = 0f;

    [Header("Text")]
    [SerializeField]
    private Text total_balence_text;
    [SerializeField] private Text bet_balence_text;
    [SerializeField] private Text win_balence_text;
    [SerializeField] private Text messege_text;

    [SerializeField] List<Sprite> itemSprite = new List<Sprite>();

    async Task Start()
    {
        await Loading();

        for (int i = 0; i < positionItems.Count; i++)
        {
            positionItems[i].GetComponent<Image>().sprite = itemSprite[i];
        }  
        
        UpdateTextUI();
        messege_text.text = "click on spin button to spin!";
        moveButton.onClick.AddListener(() =>{
            StartCoroutine(StartMoving());
        });
    }


    IEnumerator StartMoving()
    {
       
        if (total_balence > 0)
        {
            AudioController.Instance.PlaySFX("click");
            moveButton.interactable = false;
            if (total_balence >= bet_balence)
            {
                total_balence -= bet_balence;
                UpdateTextUI();
                for (int i = 0; i < 8; i++)
                {


                    yield return MoveToNextPosition(moveBox,resultImage,i);
                    
                }
            }
            else
            {
                messege_text.text = "bet ballece > total bet please change bet!";
            }
        }
        else
        {
            messege_text.text = "you don't have ballece!";
        }






        //yield return new WaitForSeconds(1F + delayBetweenMoves * 32);


        yield return WinCheck();

        yield return new WaitForSeconds(2f);


        messege_text.text = "click on spin button to spin!";
        //ResetGame();
        //ResetGame();

    }



    
    IEnumerator MoveToNextPosition(GameObject moveBox,Image resultImage,int s)
    {

        
        
        messege_text.text = "spining ...!";

        for (int i = 0; i < 2; i++)
        {
            



            for (int j = 0; j < positionItems.Count; j++)
            {

                Vector3 targetPosition = positionItems[j].transform.position;
               
               

                LeanTween.scale(moveBox, Vector3.one * 1.2f, moveDuration / 2).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                {
                    LeanTween.move(moveBox, targetPosition, moveDuration).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
                    {

                        LeanTween.rotateZ(moveBox, 10f, moveDuration / 2).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);
                        
                                    AudioController.Instance.PlaySFX("spin");
                       

                        LeanTween.scale(moveBox, Vector3.one, moveDuration / 2).setEase(LeanTweenType.easeOutElastic);
                    });

                    try
                    {
                        AnimateItemAtPosition(positionItems[j]);
                    }
                    catch { }


                });

                
                yield return new WaitForSeconds(delayBetweenMoves);

            }

            

            if (i >= 1)
            {
               
                int randomStopIndex = Random.Range(1, positionItems.Count+1);

                //print($"[]{randomStopIndex-1}");
               
                resultImage.sprite = positionItems[randomStopIndex-1].GetComponent<Image>().sprite;
               

                for (int j = 0; j < randomStopIndex; j++)
                {

                    Vector3 targetPosition = positionItems[j].transform.position;



                    LeanTween.scale(moveBox, Vector3.one * 1.2f, moveDuration / 2).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                    {
                        LeanTween.move(moveBox, targetPosition, moveDuration).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
                        {

                            LeanTween.rotateZ(moveBox, 10f, moveDuration / 2).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);


                           
                                        AudioController.Instance.PlaySFX("spin");
                          

                            LeanTween.scale(moveBox, Vector3.one, moveDuration / 2).setEase(LeanTweenType.easeOutElastic);
                        });
                        try
                        {
                            AnimateItemAtPosition(positionItems[j]);
                        }
                        catch { }
                    });


                    yield return new WaitForSeconds(delayBetweenMoves);

                }
               
            }

        }

        yield return new WaitForSeconds(0.2f);

        ShowResult(resultImage,s);

        yield return ResetGame();
       
    }


    IEnumerator ResetGame()
    {

        yield return new WaitForSeconds(1f);

            resultImage.transform.parent.GetComponent<Image>().color = normalColor;
            LeanTween.scale(resultImage.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutElastic);
            LeanTween.alpha(resultImage.rectTransform,0f, 0.5f).setEase(LeanTweenType.easeInQuad);
        
    }


    void AnimateItemAtPosition(GameObject currentItem)
    {
       
      

       
        LeanTween.scale(currentItem, Vector3.one * 1.1f, moveDuration / 1).setEase(LeanTweenType.easeOutBounce).setOnComplete(() =>
        {
            
            LeanTween.scale(currentItem, Vector3.one, moveDuration / 1).setEase(LeanTweenType.easeInBounce);
        });

      
        LeanTween.rotateZ(currentItem, 15f, moveDuration).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);

       
       

       
        LeanTween.alpha(currentItem, 0.5f, moveDuration / 2).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong(1);
    }

    void ShowResult(Image image,int i)
    {

        AudioController.Instance.PlaySFX("stop");
        image.transform.localScale = Vector3.zero;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        image.transform.parent.GetComponent<Image>().color = correctColor;

      
        LeanTween.scale(image.gameObject, Vector3.one*1.5f, 0.5f).setEase(LeanTweenType.easeOutElastic);
        LeanTween.alpha(image.rectTransform, 1f*1.3f, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(()=> {
            
            
        });

        //Vector3 pos = spawnPos[0].transform.localPosition;

        //if (i < 2) pos = spawnPos[0].transform.localPosition;
        //else if (i < 4) pos = spawnPos[1].transform.localPosition;
        //else if (i < 6) pos = spawnPos[2].transform.localPosition;
        //else pos = spawnPos[3].transform.localPosition;

        GameObject pre = Instantiate(itemPre, machineParent.transform.localPosition, Quaternion.identity);
        pre.GetComponent<Image>().sprite = image.sprite;
        pre.transform.SetParent(machineParent.transform, false);

       
       
    }





    IEnumerator WinCheck()
    {
        //0246
        //1357
        
        yield return new WaitForSeconds(1f);

        List<Image> resultImage = new List<Image>();


        for(int i = 0; i < machineParent.transform.childCount; i++)
        {
            resultImage.Add(machineParent.transform.GetChild(i).GetComponent<Image>());
        }

        if (resultImage[0].sprite.name == resultImage[5].sprite.name
             && resultImage[5].sprite.name == resultImage[2].sprite.name
             && resultImage[2].sprite.name == resultImage[7].sprite.name
             )
        {
            messege_text.text = $"win at row1[1,0,1,0]row2[0,1,0,1] =[{bet_balence * 4}]";

            LeanTween.scale(resultImage[0].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[5].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[2].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[7].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 5;
        }else if (resultImage[0].sprite.name == resultImage[5].sprite.name
            && resultImage[5].sprite.name == resultImage[2].sprite.name
            )
        {
            messege_text.text = $"win at row1[1,0,1,0]row2[0,1,0,0] =[{bet_balence * 3}]";

            LeanTween.scale(resultImage[0].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[5].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[2].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 4;
        }else if (resultImage[5].sprite.name == resultImage[2].sprite.name
             && resultImage[2].sprite.name == resultImage[7].sprite.name
           )
        {
            messege_text.text = $"win at row1[0,0,1,0]row2[0,1,0,1] =[{bet_balence * 3}]";

            LeanTween.scale(resultImage[5].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[2].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[7].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 4;
        }


        else
        {
            messege_text.text = $"fail!";
        }

        //0123
        //4567
        if (resultImage[4].sprite.name == resultImage[1].sprite.name
             && resultImage[1].sprite.name == resultImage[6].sprite.name
             && resultImage[6].sprite.name == resultImage[3].sprite.name
             )
        {
            messege_text.text = $"win at row1[0,1,0,1]row2[1,0,1,0] =[{bet_balence * 4}]";

            LeanTween.scale(resultImage[4].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[1].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[6].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[3].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 5;
        }else if (resultImage[4].sprite.name == resultImage[1].sprite.name
            && resultImage[1].sprite.name == resultImage[6].sprite.name
            )
        {
            messege_text.text = $"win at row1[0,1,0,0]row2[1,0,1,0] =[{bet_balence * 3}]";

            LeanTween.scale(resultImage[4].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[1].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[6].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 4;
        }else if (resultImage[1].sprite.name == resultImage[6].sprite.name
             && resultImage[6].sprite.name == resultImage[3].sprite.name
           )
        {
            messege_text.text = $"win at row1[0,1,0,1]row2[0,0,1,0] =[{bet_balence * 3}]";

            LeanTween.scale(resultImage[1].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[6].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[3].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 4;
        }


        //if (resultImage[0].sprite.name == resultImage[2].sprite.name
        //    && resultImage[2].sprite.name == resultImage[4].sprite.name
        //    && resultImage[4].sprite.name == resultImage[6].sprite.name
        //    )
        //{
        //    messege_text.text = $"win at row 1 [1,1,1,1] =[{bet_balence * 4}]";

        //    LeanTween.scale(resultImage[0].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[2].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[4].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[6].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    total_balence += bet_balence * 5;
        //}
        //else if (resultImage[0].sprite.name == resultImage[2].sprite.name
        //    && resultImage[2].sprite.name == resultImage[4].sprite.name
        // )
        //{
        //    messege_text.text = $"win at row 1 [1,1,1,0] =[{bet_balence * 3}]";

        //    LeanTween.scale(resultImage[0].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[2].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[4].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();

        //    total_balence += bet_balence * 4;
        //}
        //else if (resultImage[2].sprite.name == resultImage[4].sprite.name
        //    && resultImage[4].sprite.name == resultImage[6].sprite.name
        // )
        //{
        //    messege_text.text = $"win at row 1 [0,1,1,1] =[{bet_balence * 3}]";

        //    LeanTween.scale(resultImage[2].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[4].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[6].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    total_balence += bet_balence * 4;
        //}


        if (resultImage[0].sprite.name == resultImage[1].sprite.name
            && resultImage[1].sprite.name == resultImage[2].sprite.name
            && resultImage[2].sprite.name == resultImage[3].sprite.name
            )
        {
            messege_text.text = $"win at row 1 [1,1,1,1] =[{bet_balence * 4}]";

            LeanTween.scale(resultImage[0].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[1].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[2].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[3].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 5;
        }
        else if (resultImage[0].sprite.name == resultImage[1].sprite.name
         && resultImage[1].sprite.name == resultImage[2].sprite.name
         )
        {
            messege_text.text = $"win at row 1 [1,1,1,0] =[{bet_balence * 3}]";

            LeanTween.scale(resultImage[0].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[1].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[2].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 4;
        }
        else if (resultImage[2].sprite.name == resultImage[3].sprite.name
         && resultImage[1].sprite.name == resultImage[2].sprite.name
         )
        {
            messege_text.text = $"win at row 1 [0,1,1,1] =[{bet_balence * 3}]";

            LeanTween.scale(resultImage[1].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[2].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[3].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            total_balence += bet_balence * 4;
        }




        if (resultImage[4].sprite.name == resultImage[5].sprite.name
             && resultImage[5].sprite.name == resultImage[6].sprite.name
             && resultImage[6].sprite.name == resultImage[7].sprite.name
           )
        {
            messege_text.text = $"win at row 2 [1,1,1,1] =[{bet_balence * 4}]";
            total_balence += bet_balence * 5;

            LeanTween.scale(resultImage[7].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[4].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[5].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[6].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
        }
        else if (resultImage[4].sprite.name == resultImage[5].sprite.name
           && resultImage[5].sprite.name == resultImage[6].sprite.name
         )
        {
            messege_text.text = $"win at row 1 [1,1,1,0] =[{bet_balence * 3}]";
            total_balence += bet_balence * 4;

            LeanTween.scale(resultImage[4].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[5].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[6].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();

        }
        else if (resultImage[5].sprite.name == resultImage[6].sprite.name
           && resultImage[6].sprite.name == resultImage[7].sprite.name
         )
        {
            messege_text.text = $"win at row 1 [0,1,1,1] =[{bet_balence * 3}]";
            total_balence += bet_balence * 4;

            LeanTween.scale(resultImage[7].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[5].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
            LeanTween.scale(resultImage[6].gameObject, Vector3.one * 1.2f, 0.3f).setEaseSpring().setLoopPingPong();
        }



        //1357

        //if (resultImage[1].sprite.name == resultImage[3].sprite.name
        // && resultImage[3].sprite.name == resultImage[5].sprite.name
        // && resultImage[5].sprite.name == resultImage[7].sprite.name
        //)
        //{
        //    messege_text.text = $"win at row 2 [1,1,1,1] =[{bet_balence * 4}]";
        //    total_balence += bet_balence * 5;

        //    LeanTween.scale(resultImage[1].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[3].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[5].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[7].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //}
        //else if (resultImage[1].sprite.name == resultImage[3].sprite.name
        // && resultImage[3].sprite.name == resultImage[5].sprite.name
        // )
        //{
        //    messege_text.text = $"win at row 1 [1,1,1,0] =[{bet_balence * 3}]";
        //    total_balence += bet_balence * 4;

        //    LeanTween.scale(resultImage[1].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[3].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[5].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();

        //}
        //else if (resultImage[3].sprite.name == resultImage[5].sprite.name
        // && resultImage[5].sprite.name == resultImage[7].sprite.name
        // )
        //{
        //    messege_text.text = $"win at row 1 [0,1,1,1] =[{bet_balence * 3}]";
        //    total_balence += bet_balence * 4;

        //    LeanTween.scale(resultImage[3].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[5].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[7].gameObject, Vector3.one * 0.2f, 1f).setEaseOutSine();
        //}





        //if (resultImage[6].sprite.name == resultImage[7].sprite.name &&
        //    resultImage[7].sprite.name == resultImage[8].sprite.name)
        //{
        //    messege_text.text = $"win at column 3 [{bet_balence * 3}]";
        //    total_balence += bet_balence * 4;

        //    LeanTween.scale(resultImage[6].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[7].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[8].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //}

        //if (resultImage[0].sprite.name == resultImage[3].sprite.name &&
        //    resultImage[3].sprite.name == resultImage[6].sprite.name)
        //{
        //    messege_text.text = $"win at row 1 [{bet_balence * 3}]";
        //    total_balence += bet_balence * 4;

        //    LeanTween.scale(resultImage[0].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[3].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[6].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //}

        //if (resultImage[1].sprite.name == resultImage[4].sprite.name &&
        //    resultImage[4].sprite.name == resultImage[7].sprite.name)
        //{
        //    messege_text.text = $"win at row 2 [{bet_balence * 3}]";
        //    total_balence += bet_balence * 4;

        //    LeanTween.scale(resultImage[1].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[4].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[7].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //}
        //if (resultImage[2].sprite.name == resultImage[5].sprite.name &&
        //    resultImage[5].sprite.name == resultImage[8].sprite.name)
        //{
        //    messege_text.text = $"win at row 3 [{bet_balence * 3}]";
        //    total_balence += bet_balence * 4;

        //    LeanTween.scale(resultImage[2].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[5].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //    LeanTween.scale(resultImage[8].gameObject, Vector3.one * 3, 1f).setEaseOutSine();
        //}

        
        AudioController.Instance.PlaySFX("show");

        UpdateTextUI();

        yield return new WaitForSeconds(3f);

       

        for (int i = 0; i < resultImage.Count; i++)
        {
            LeanTween.scale(resultImage[i].gameObject, Vector3.zero, 0.2f).setEaseInBounce();
            AudioController.Instance.PlaySFX("d");
            yield return new WaitForSeconds(0.2f);
            Destroy(resultImage[i].gameObject);

            //Destroy(machineParent.transform.GetChild(i).gameObject);
           
          
        }

        resultImage.Clear();

        moveButton.interactable = true;
    }

    void UpdateTextUI()
    {
        total_balence_text.text = total_balence.ToString("0.0");
        bet_balence_text.text = bet_balence.ToString("0.0");
    }


    public void ChangeBetBalence(int i)
    {
        AudioController.Instance.PlaySFX("click");
        print("g");
        switch (i)
        {
            case 1:
                if (bet_balence <= 1) bet_balence += 0.1f;
                else bet_balence = 0.1f;
                break;
            default:
                if (bet_balence > 0.1f) bet_balence -= 0.1f;
                else bet_balence = 1f;
                break;
        }
        bet_balence_text.text = bet_balence.ToString("0.0");
        // UpdateTextUI();
    }


    // ##################################################################################
    [Header("GameObject")]
    [SerializeField]
    private GameObject homePanel;
    [SerializeField]  private GameObject settingPanel,loadingPanel;

    public void OnClickStart(int i)
    {
        AudioController.Instance.PlaySFX("click");
        if (i == 0) LeanTween.scale(homePanel, Vector3.zero, 0.5f).setEaseInExpo();
        else LeanTween.scale(homePanel, Vector3.one, 0.5f).setEaseInExpo();
    }

    public void OnClickSetting(int i)
    {
        AudioController.Instance.PlaySFX("click");
        if (i == 0) LeanTween.scale(settingPanel, Vector3.zero, 0.5f).setEaseInExpo();
        else LeanTween.scale(settingPanel, Vector3.one, 0.5f).setEaseInExpo();
    }

    async Task Loading()
    {
        LeanTween.scale(loadingPanel.transform.GetChild(0).gameObject, Vector3.one * 0.8f, 0.4f).setEaseInExpo().setLoopPingPong();
        await Task.Delay(3000);
        LeanTween.scale(loadingPanel, Vector3.zero, 0.5f).setEaseInExpo();
    }

    public void OnclickExit()
    {
        AudioController.Instance.PlaySFX("click");
        Application.Quit();
    }


}// end of class

[System.Serializable]
public class PositionItem
{
    public List<GameObject> positionList = new List<GameObject>();
}



//void MoveToNextPosition()
//{


//    if (totalLoops >= maxLoops && currentTargetIndex == 0 && remainingSteps == -1)
//    {
//        // Determine a random stopping point within the range
//        int randomStopIndex = Random.Range(0, itemPositions1.Length);
//        print(randomStopIndex);
//        r = randomStopIndex;
//        remainingSteps = randomStopIndex + itemPositions1.Length + 1;
//    }

//    // Calculate the next target index
//    currentTargetIndex = (currentTargetIndex + 1) % itemPositions1.Length;

//    // If the current target index is 0, it means a full loop is completed
//    if (currentTargetIndex == 0)
//    {
//        totalLoops++;
//    }

//    // Decrement remaining steps if set
//    if (remainingSteps > 0)
//    {
//        remainingSteps--;
//    }

//    // Check if it's time to stop
//    if (remainingSteps == 0)
//    {
//        isMoving = false; // Stop the loop
//        ShowResult(itemPositions1[r].GetComponent<Image>().sprite); // Show the result image
//        return;
//    }

//    // Get the position of the target item
//    Vector3 targetPosition = itemPositions1[currentTargetIndex].transform.position;

//    // Animate the red box with additional effects

//    LeanTween.scale(redBox, Vector3.one * 1.2f, moveDuration / 2).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
//    {
//        LeanTween.move(redBox, targetPosition, moveDuration).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
//        {

//            LeanTween.rotateZ(redBox, 10f, moveDuration / 2).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);


//            AnimateItemAtPosition(currentTargetIndex);

//            LeanTween.scale(redBox, Vector3.one, moveDuration / 2).setEase(LeanTweenType.easeOutElastic).setOnComplete(() =>
//            {

//                Invoke("MoveToNextPosition", delayBetweenMoves);
//            });
//        });
//    });
//}
