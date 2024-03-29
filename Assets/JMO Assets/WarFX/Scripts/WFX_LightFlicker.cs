using UnityEngine;
using System.Collections;

/**
 *	Rapidly sets a light on/off.
 *	
 *	(c) 2015, Jean Moreno
**/

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
	public float time = 0.05f;
	
	private float timer;

	
	void Start ()
	{
		timer = time;
		StartCoroutine("Flicker");
	}
	
	IEnumerator Flicker()
	{
        //		while(true)
        //	{
        Light light = GetComponent<Light>();

            light.enabled = !light.enabled;
                
			do
			{
				timer -= Time.deltaTime;
				yield return null;
			}
			while(timer > 0);
        //GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
        light.enabled = light.enabled;
        //}
    }
}
