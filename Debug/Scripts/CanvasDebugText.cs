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
    public class CanvasDebugText : BBehaviour
    {
        public string DebugID { get { return debugID; } }

        [Header("Debug Text")]
        [SerializeField] private string debugID = "DebugID";

        [SerializeField] private bool clearTextContentOnLateUpdate = false;
        [SerializeField] private bool clearTextDisplayOnLateUpdate = false;

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

            if (clearTextDisplayOnLateUpdate)
            {
                myBText.SetText(myText);
            }
            
            if (clearTextContentOnLateUpdate)
            {
                myText = "";
            }
        }

        public void Log(string newText, bool incrementText = false)
        {
            if (incrementText)
            {
                if (myText == "")
                {
                    myText = newText;
                }
                else
                {
                    myText += "\n" + newText;
                }
            }
            else
            {
                myText = newText;
            }
            
            myBText.SetText(myText);
        }
    }
}