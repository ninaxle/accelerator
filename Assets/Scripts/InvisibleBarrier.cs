using UnityEngine;

public class InvisibleBarrier : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
        {
            _meshRenderer.enabled = false;
        }
    }
}