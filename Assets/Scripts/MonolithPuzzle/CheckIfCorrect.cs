using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CheckIfCorrect : MonoBehaviour
{
    [Tooltip("Correct dial positions add to this list. When list is up to 3, trigger animation")]
    public List<string> list = new List<string>();
    [Tooltip("Player camera animation for ending sequence")]
    public Animator animator;

    public void InitiateEnd()
    {
        if (list.Count == 3)
        {
            // Disable player movement
            Debug.Log("Starting the end bit!");
            animator.SetBool("Activate", true);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(13);

        Initiate.Fade("06_End", Color.black, 1);
    }
}
