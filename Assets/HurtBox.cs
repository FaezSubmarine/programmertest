using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public float damage { get; private set; }
    IEnumerator living;

    bool dieOnImpact;
    public void setUpBox(float damage, float liveTime, Vector3 scaling, bool dieOnImpact = false)
    {
        this.damage = damage;
        this.dieOnImpact = dieOnImpact;
        transform.localScale = scaling;
        living = ForHowLong(liveTime);
        StartCoroutine(living);
        
    }
    IEnumerator ForHowLong(float liveTime)
    {
        yield return new WaitForSeconds(liveTime);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dieOnImpact && collision.gameObject.layer != 8 && collision.gameObject.layer != 9 && collision.gameObject.layer != 10)
        {
            Destroy(gameObject);
        }
    }
}
