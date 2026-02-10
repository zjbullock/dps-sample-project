using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    protected GameObject mesh;

    public void ToggleDebugObject(bool active)
    {
        if (this.mesh == null)
        {
            return;
        }

        this.mesh.SetActive(active);
    }
}
