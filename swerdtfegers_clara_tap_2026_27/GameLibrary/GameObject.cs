using GameLibrary.Components;
using System.Collections.Generic;

namespace activity_00_tap_26_27.Core
{
    public class GameObject
    {
        private readonly List<Component> _componentTable = new List<Component>();
        private readonly string _name;
        private bool _isActive = false;

        public GameObject(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public bool GetIsActive()
        {
            return _isActive;
        }

        public virtual void SetIsActive(bool is_active)
        {
            if (_isActive != is_active)
            {
                for (int component_index = 0; component_index < _componentTable.Count; component_index++)
                {
                    Component current_component = _componentTable[component_index];

                    if (current_component.GetIsActive())
                    {
                        if (is_active)
                        {
                            current_component.OnEnable();
                        }
                        else
                        {
                            current_component.OnDisable();
                        }
                    }
                }

                _isActive = is_active;
            }
        }

        public void AddComponent(Component component)
        {
            _componentTable.Add(component);
        }

        public TYPE GetComponent<TYPE>() where TYPE : Component
        {
            for (int component_index = 0; component_index < _componentTable.Count; component_index++)
            {
                if (_componentTable[component_index] is TYPE selected_component)
                {
                    return selected_component;
                }
            }

            return null;
        }

        public void Update(float elapsed_time)
        {
            foreach (Component component in _componentTable)
            {
                if (component.GetIsActive())
                {
                    component.Update(elapsed_time);
                }
            }
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            foreach (Component component in _componentTable)
            {
                if (component.GetIsActive())
                {
                    component.FixedUpdate(fixed_elapsed_time);
                }
            }
        }


    }
}