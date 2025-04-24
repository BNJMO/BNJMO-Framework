using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNJMO
{
    /// <summary> 
    /// Check DebugManager for DebugIDs
    /// </summary>
    [RequireComponent(typeof(BText))]
    public class DebugText : BBehaviour
    {
        public string DebugID { get { return debugID; } }

        [Header("Debug Text")]
        [SerializeField] private string debugID = "DebugID";

        private BText myBText;
        private string myText = "";

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();
            
            // Update Text
            myBText = GetComponent<BText>();
            if (myBText)
            {
                myBText.SetText(DebugID);
                myBText.Revalidate();
            }
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            myBText = GetComponentWithCheck<BText>();
        }

        protected override void Start()
        {
            base.Start();

            myText = "";
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            myBText.SetText(myText);
            myText = "";
        }

        public void Log(string newText)
        {
            myText += newText + "\n";
        }
    }
}