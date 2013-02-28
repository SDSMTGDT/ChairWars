using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChairWars.GameStates
{

    public abstract class AbstractGameState : Nuclex.Game.States.GameState
    {
        protected AbstractGameState()
            : base()
        {
            controlSubscriptions = new LinkedList<IDisposable>();
        }

        protected LinkedList<IDisposable> controlSubscriptions;
        public abstract void LoadControls();

        protected virtual void UnloadControls()
        {
            foreach (var subscription in controlSubscriptions)
                subscription.Dispose();
        }

        protected override void OnLeaving()
        {
            UnloadControls();
            base.OnLeaving();
        }

        protected override void OnEntered()
        {
            LoadControls();
            base.OnEntered();
        }

        protected override void OnPause()
        {
            UnloadControls();
            base.OnPause();
        }

        protected override void OnResume()
        {
            LoadControls();
            base.OnResume();
        }
    }

    public abstract class AbstractDrawableGameState : Nuclex.Game.States.DrawableGameState
    {
        protected AbstractDrawableGameState()
            : base()
        {
            controlSubscriptions = new LinkedList<IDisposable>();
        }

        protected LinkedList<IDisposable> controlSubscriptions;

        protected abstract void LoadControls();

        protected virtual void UnloadControls()
        {
            foreach (var subscription in controlSubscriptions)
                subscription.Dispose();
        }

        protected override void OnLeaving()
        {
            UnloadControls();
            base.OnLeaving();
        }

        protected override void OnEntered()
        {
            LoadControls();
            base.OnEntered();
        }

        protected override void OnPause()
        {
            //UnloadControls();
            base.OnPause();
        }

        protected override void OnResume()
        {
            //LoadControls();
            base.OnResume();
        }
    }

}
