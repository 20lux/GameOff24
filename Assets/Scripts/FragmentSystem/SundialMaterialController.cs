using UnityEngine;

public class SundialMaterialController : MonoBehaviour
{
    [SerializeField] private GameObject sundial;
    [SerializeField] private float power = 0;
    private Material[] materials = new Material[2];

    void Start()
    {
        materials = sundial.GetComponent<Renderer>().materials;
        materials[1].SetFloat("_power", power);
    }

    public void IncreaseSundialPower()
    {
        if (power > 0)
        {
            power++;
            materials[1].SetFloat("_power", power);
        }
        else
        {
            power = 0;
        }
    }

    public void DecreaseSundialPower()
    {
        if (power > 0)
        {
            power--;
            materials[1].SetFloat("_power", power);
        }
        else
        {
            power = 0;
        }
    }

    public void ResetSundialPower()
    {
        power = 0;
        materials[1].SetFloat("_power", power);
    }

}
