using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingTxt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loading;
    [SerializeField] float delay = 1f;
    private void Awake() {
        loading = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        StartCoroutine(AnimLoading());
    }
    IEnumerator AnimLoading()
    {
        while (true)
        {
            loading.SetText("loading");
            yield return new WaitForSeconds(delay);
            loading.SetText("loading.");
            yield return new WaitForSeconds(delay);
            loading.SetText("loading..");
            yield return new WaitForSeconds(delay);
            loading.SetText("loading...");
            yield return new WaitForSeconds(delay);
        }
    }
}
