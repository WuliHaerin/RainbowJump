using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RainbowJump.Scripts
{
    public class Player : MonoBehaviour
    {
        public float jumpForce = 10f;
        public Manager manager;

        public Rigidbody2D rb;
        public int dieMode = 0;

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && !EventSystem.current.IsPointerOverGameObject())
            {
                rb.simulated = true;
                rb.velocity = Vector2.up * jumpForce;
                manager.PlayTapSound();
            }


            if (transform.position.y < -5f)
            {
                if (!isCancelAd && !isInvincible)
                {
                    dieMode = 2;
                    PreOver();
                    
                }
                else if (isInvincible)
                {
                    Vector3 newPos = transform.position;
                    newPos.y += 3f;
                    transform.position = newPos;
                    rb.simulated = false;
                }

                //manager.gameOver = true;
                //manager.PlayDeathSound();
                //transform.position = new Vector3(transform.position.x, -4.9f, transform.position.z);
            }


        }

        public GameObject AdPanel;
        public bool isCancelAd;
        public bool isInvincible;


        public void SetAdPanel(bool a)
        {
            AdPanel.SetActive(a);
            Time.timeScale = a ? 0 : 1;
        }

        public void CancelAd()
        {
            isCancelAd = true;
            SetAdPanel(false);
        }

        public void ResetAll()
        {
            StopAllCoroutines();
            isCancelAd = false;
            isInvincible = false;
            dieMode = 0;
        }

        public void Revive()
        {
            AdManager.ShowVideoAd("192if3b93qo6991ed0",
           (bol) =>
           {
               if (bol)
               {
                   if (dieMode == 2 || dieMode == 3)
                   {
                       Vector3 newPos = transform.position;
                       newPos.y += 3f;
                       transform.position = newPos;
                       rb.simulated = false;
                   }
                   SetAdPanel(false);
                   StopCoroutine("GameOver");
                   StartCoroutine("Invincible");

                   AdManager.clickid = "";
                   AdManager.getClickid();
                   AdManager.apiSend("game_addiction", AdManager.clickid);
                   AdManager.apiSend("lt_roi", AdManager.clickid);


               }
               else
               {
                   StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
               }
           },
           (it, str) =>
           {
               Debug.LogError("Error->" + str);
               //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
           });

        }

        public void PreOver()
        {
            SetAdPanel(true);
            StartCoroutine("GameOver");
        }

        public IEnumerator GameOver()
        {
            yield return new WaitForSeconds(0.2f);
            manager.gameOver = true;
            manager.PlayDeathSound();
            ResetAll();
        }

        public IEnumerator Invincible()
        {
            isInvincible = true;
            GetComponent<SpriteRenderer>().color = Color.blue;
            yield return new WaitForSeconds(3f);
            isInvincible = false;
            GetComponent<SpriteRenderer>().color = Color.white;
        }



        void OnTriggerEnter2D(Collider2D col)
        {
            if (!isCancelAd && !isInvincible)
            {
                dieMode = 1;
                PreOver();
            }
        }

        void OnBecameInvisible()
        {
            if (!isCancelAd && !isInvincible)
            {
                dieMode = 3;
                PreOver();
            }
            else if (isInvincible)
            {
                Vector3 newPos = transform.position;
                newPos.y += 3f;
                transform.position = newPos;
                rb.simulated = false;
            }

        }
    }
}
