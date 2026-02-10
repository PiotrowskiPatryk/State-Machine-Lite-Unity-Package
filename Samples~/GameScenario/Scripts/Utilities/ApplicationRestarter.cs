using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.GameScenario.Scripts.Utilities
{
    public class ApplicationRestarter : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}