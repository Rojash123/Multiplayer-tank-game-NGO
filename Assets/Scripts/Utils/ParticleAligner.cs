using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule psMain;
    void Start()
    {
        psMain = GetComponent<ParticleSystem>().main;
    }

    // Update is called once per frame
    void Update()
    {
        psMain.startRotation = -transform.rotation.eulerAngles.z*Mathf.Deg2Rad;
    }
}
