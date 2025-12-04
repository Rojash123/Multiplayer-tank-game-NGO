using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CheckBrainCamera : MonoBehaviour
{
    private void Awake()
    {
        if (GetComponent<CinemachineBrain>()== null)
        {
            this.AddComponent<CinemachineBrain>();
        }
    }
}
