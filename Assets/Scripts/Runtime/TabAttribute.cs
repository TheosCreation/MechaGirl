using UnityEngine;

namespace Runtime
{
    public class TabAttribute : PropertyAttribute
    {
        public string tabName;

        public TabAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}