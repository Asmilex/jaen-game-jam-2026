
// Recomendado: chequea todos los Renderers del objeto y usa frustum planes.
using UnityEngine;

public static class CameraUtils {
    public static bool IsVisibleByCamera(GameObject obj, Camera cam, bool requireUnoccluded = false, LayerMask occlusionMask = default)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        foreach (var r in renderers)
        {
            if (GeometryUtility.TestPlanesAABB(planes, r.bounds))
            {
                if (!requireUnoccluded) return true;

                Vector3 dir = r.bounds.center - cam.transform.position;
                if (Physics.Raycast(cam.transform.position, dir.normalized, out RaycastHit hit, dir.magnitude, occlusionMask))
                {
                    if (hit.collider != null && (hit.collider.gameObject == obj || hit.collider.transform.IsChildOf(obj.transform)))
                        return true;
                }
            }
        }
        return false;
    }
}