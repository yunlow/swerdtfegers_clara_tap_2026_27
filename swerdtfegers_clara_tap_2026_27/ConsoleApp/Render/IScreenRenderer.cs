using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Render
{
    public interface IScreenRenderer
    {
        public void Render(IState state, ConsoleRenderManager render_manager);
    }
}
