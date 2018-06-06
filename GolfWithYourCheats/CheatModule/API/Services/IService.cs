using CheatModule.API.Interfaces;

namespace CheatModule.API.Services
{
    public interface IService : ILoadable
    {
        #region Properties

        bool Running { get; set; }

        string Name { get; }
        string ID { get; }

        #endregion Properties

        #region Functions

        void Update();

        void OnThreadedUpdate();

        void OnGUI();

        #endregion Functions
    }
}