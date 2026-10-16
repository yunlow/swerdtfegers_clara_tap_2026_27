using activity_00_tap_26_27.Core;

namespace GameLibrary.Components
{
    public class HeroComponent : Component
    {
        private string _name;
        private string _id;

        public HeroComponent(GameObject game_object, string name, string id) : base(game_object)
        {
            _id = id;
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
    }
}