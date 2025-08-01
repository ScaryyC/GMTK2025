using UnityEngine;

public class Tower : MonoBehaviour
{
    public bool visisted = false;
    public Collider areaCollider;
    public ParticleSystem pfx;

    public void VisitTower()
    {
        if (areaCollider == null)
        {
            Debug.Log("This tower's collder is not hooked up");
            return;
        }
        areaCollider.enabled = false;
        visisted = true;

        if (pfx != null)
        {
            pfx.Play();
        }
    }

}
