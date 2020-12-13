using UnityEngine;
using UnityEngine.SceneManagement;
public class OnClick : MonoBehaviour
{
    // add callbacks in the inspector like for buttons

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.name == "Play"){
                    SceneManager.LoadScene("Home");
                }else if(hit.transform.name == "Back"){
                    SceneManager.LoadScene("titre");
                }
                else if(hit.transform.name == "Credits"){
                    SceneManager.LoadScene("Credits");
                }
                else if(hit.transform.name == "Exit"){
                    Application.Quit();
                }
            }
        }
    }
}