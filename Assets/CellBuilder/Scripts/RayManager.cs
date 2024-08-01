using UnityEngine;

public class RayManager : Manager
{
    private IRayCastable lastHit;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.TryGetComponent<IRayCastable>(out IRayCastable currentHit))
            {
                if (currentHit != lastHit)
                {
                    lastHit?.OnRaycastExit();
                    currentHit.OnRaycastEnter();
                    lastHit = currentHit;
                }

                currentHit.OnRaycastHover();
            }
        }
        else if (lastHit != null)
        {
            lastHit.OnRaycastExit();
            lastHit = null;
        }
    }
}
