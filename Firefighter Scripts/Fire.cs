using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class Fire : MonoBehaviour, IPointerDownHandler
{
    private bool fireBurning = false;
    private bool blockClicks;

    Image fireSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (blockClicks)
            return;

        blockClicks = true;
        fireSprite.DOKill();
        CancelInvoke("BecomeBomb");
        CancelInvoke("SelfDestructBomb");
        GetComponent<Animator>().enabled = false;
        fireSprite.raycastTarget = false;

        if (transform.tag == "Bomb")
        {
            fireSprite.sprite = GameManager.instance.bombExplosionSprite;
            GameManager.instance.IncreaseBombsHit();
        }

        if (transform.tag == "Fire")
        {
            fireSprite.sprite = GameManager.instance.smokeSprite;
            GameManager.instance.IncreaseFlamesHit();
        }

        Vector3 randomRot = Random.rotation.eulerAngles;
        randomRot.x = 0;
        randomRot.y = 0;
        transform.rotation = Quaternion.Euler(randomRot);
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f);
        fireSprite.DOFade(0f, 1f);
        PutFireOut();
    }

    void Start()
    {
        fireSprite = GetComponent<Image>();
    }

    private void Update()
    {

    }

    public void ShowFire()
    {
        if (GameManager.instance.CheckFireBurningLimitReached())
            return;

        fireBurning = true;
        blockClicks = false;
        fireSprite.DOKill();
        CancelInvoke("BecomeBomb");
        CancelInvoke("SelfDestructBomb");
        GameManager.instance.IncreaseFiresBurning();
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        GetComponent<Animator>().enabled = true;
        fireSprite.DOFade(1f, 0f);
        fireSprite.raycastTarget = true;
        transform.tag = "Fire";

        Invoke("BecomeBomb", GameManager.instance.bombTimer);
    }

    public void HideFlameInstantly()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.DOScale(Vector3.one,0f);
        CancelInvoke("BecomeBomb");
        CancelInvoke("SelfDestructBomb");
        GetComponent<Animator>().enabled = false;
        fireSprite.raycastTarget = false;
        fireSprite.DOFade(0f, 0f);
        PutFireOut();
    }

    public void PutFireOut()
    {
        GameManager.instance.DecreaseActiveFlames();
        transform.tag = "Untagged";
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Invoke("ResetFire", GameManager.instance.fireSpawnDelay);
    }

    public void BecomeBomb()
    {
        fireBurning = true;
        blockClicks = false;
        GetComponent<Animator>().enabled = false;
        fireSprite.sprite = GameManager.instance.bombSprite;
        transform.tag = "Bomb";
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Invoke("SelfDestructBomb", GameManager.instance.bombSelfDestructTimer);
    }

    public void SelfDestructBomb()
    {
        GameManager.instance.IncreaseBombSelfDestructs();

        blockClicks = true;
        GetComponent<Animator>().enabled = false;
        fireSprite.raycastTarget = false;
        fireSprite.sprite = GameManager.instance.bombExplosionSprite;
        Vector3 randomRot = Random.rotation.eulerAngles;
        randomRot.x = 0;
        randomRot.y = 0;
        transform.rotation = Quaternion.Euler(randomRot);
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f);
        fireSprite.DOFade(0f, 1f);
        PutFireOut();
    }

    public bool CheckFireBurning()
    {
        return fireBurning;
    }

    public void ResetFire()
    {
        fireBurning = false;
    }
}
