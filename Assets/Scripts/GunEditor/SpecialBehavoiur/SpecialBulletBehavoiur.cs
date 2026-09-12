using UnityEngine;

public abstract class SpecialBulletBehavoiur : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void OnFire(){}
    virtual public void OnUpdate(){}
    virtual public void OnHit(Collision2D collision){}
    virtual public void OnDestory(){}
       
}
