using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDKClient
{
    public class LeBianTest : MonoBehaviour
    {
        public Button b;
        public Text t;

        // Start is called before the first frame update
        void Start()
        {
            b.onClick.AddListener(() =>
            {
                t.text = "IsAfterUpdate" + SDKInterface.Instance.LBIsAfterUpdate().ToString() + SDKInterface.Instance.LBGetFullResPath(); ;
                SDKInterface.Instance.LBRequestUpdate();
                SDKInterface.Instance.LBDownloadFullRes();
                 
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
