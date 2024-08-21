using UnityEngine;
using Oculus.Interaction.Surfaces;
using Oculus.Interaction;

public class SetupRayInteractable : MonoBehaviour
{
    private void Start()
    {
        // Get the MeshCollider component from the same GameObject
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            Debug.LogError("No MeshCollider found on the GameObject.");
            return;
        }

        // Get the ColliderSurface component from the same GameObject
        ColliderSurface colliderSurface = GetComponent<ColliderSurface>();
        if (colliderSurface == null)
        {
            Debug.LogError("No ColliderSurface component found on the GameObject.");
            return;
        }

        // Set the MeshCollider as the Collider in the ColliderSurface using InjectCollider
        colliderSurface.InjectCollider(meshCollider);

        // Get the RayInteractable component from the same GameObject
        RayInteractable rayInteractable = GetComponent<RayInteractable>();
        if (rayInteractable == null)
        {
            Debug.LogError("No RayInteractable component found on the GameObject.");
            return;
        }

        // Assign the ColliderSurface to the RayInteractable's Surface property
        rayInteractable.InjectSurface(colliderSurface);

        Debug.Log("Successfully set up the RayInteractable surface.");
    }
}
