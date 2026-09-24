namespace GameLibrary.Components
{
    public abstract class Component
    {
        private bool _isActive = true;

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