using activity_00_tap_26_27.Core;

namespace GameLibrary.Components
{
    public abstract class Component
    {
        private bool _isActive = true;
        private GameObject _gameObject;

        protected Component(GameObject game_object)
        {
            _gameObject = game_object;
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public virtual void Update(float elapsed_time)
        {

        }

        public virtual void FixedUpdate(float elapsed_fixed_time)
        {

        }

        public bool GetIsActive()
        {
            return _isActive;
        }

        public void SetActive(bool is_active)
        {
            if (_isActive != is_active)
            {
                if (is_active)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }

                _isActive = is_active;
            }
        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }
    }
}