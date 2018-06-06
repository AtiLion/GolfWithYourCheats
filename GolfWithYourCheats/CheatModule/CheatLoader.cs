using CheatModule.API;
using UnityEngine;

namespace CheatModule
{
    public class CheatLoader : MonoBehaviour
    {
        #region Properties

        public static GameObject MainGameObject { get; private set; }
        public static CheatMod MainInstance { get; private set; }

        #endregion Properties

        public static void Hook()
        {
            Logging.LogImportant("Loading " + DllInfo.Name + "...");
            if (MainGameObject == null)
            {
                MainGameObject = new GameObject();
                DontDestroyOnLoad(MainGameObject);
            }
            if (MainInstance == null)
                MainInstance = MainGameObject.AddComponent<CheatMod>();
            Logging.LogImportant(DllInfo.Name + " loaded!");
        }
    }
}