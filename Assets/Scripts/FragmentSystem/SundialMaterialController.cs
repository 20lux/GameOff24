using UnityEngine;

public class SundialMaterialController : MonoBehaviour
{
    [SerializeField] private GameObject sundial;
    [SerializeField] private float power = 0.0f;
    private Material[] materials = new Material[2];
    private string str_Power = "_Power";

    void Start()
    {
        materials = sundial.GetComponent<Renderer>().materials;
        Debug.Log("Materials");
        materials[1].SetFloat(str_Power, power);
    }

    public void IncreaseSundialPower()
    {
        if (power >= 0.0f)
        {
            power += 1.0f;
            Debug.Log("Material power: " + power);
            materials[1].SetFloat(str_Power, power);
        }
        else
        {
            power = 0.0f;
            materials[1].SetFloat(str_Power, power);
        }
    }

    public void DecreaseSundialPower()
    {
        if (power > 0)
        {
            power -= 1.0f;
            materials[1].SetFloat(str_Power, power);
        }
        else
        {
            power = 0.0f;
            materials[1].SetFloat(str_Power, power);
        }
    }

    public void ResetSundialPower()
    {
        power = 0.0f;
        materials[1].SetFloat(str_Power, power);
    }

}
