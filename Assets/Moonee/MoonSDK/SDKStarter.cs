using System.Collections;
using System.Collections.Generic;
using Events;
using GameAnalyticsSDK.Setup;
using Level_selection.Bestiary_system;
using Level_selection.Map_system;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moonee.MoonSDK
{
    public class SDKStarter : MonoBehaviour
    {
        [SerializeField] private GameObject moonSDK;
        [SerializeField] private GameObject intro;
        [SerializeField]
        private Bestiary _bestiary;

        [SerializeField]
        private List<GameObject> _deactivateObject = new List<GameObject>();

        private void Start()
        {
            intro.gameObject.SetActive(false);
            StartCoroutine(Starter());
        }
        private void InitializeMoonSDK()
        {
            moonSDK.SetActive(true);
            DontDestroyOnLoad(moonSDK);

            if (_bestiary.GetCountOpenModelByWorldType(WorldType.Ice) > 0)
            {
                EventStreams.UserInterface.Publish(new EventOpenMap(true));
            }
            else
            {
                EventStreams.UserInterface.Publish(new EventPlay());
            }

            foreach (var o in _deactivateObject)
            {
                o.SetActive(false);
            }
        }
        private IEnumerator Starter()
        {
            intro.SetActive(true);
            yield return new WaitForSeconds(4f);
            intro.SetActive(false);

            InitializeMoonSDK();
        }
    }
}
