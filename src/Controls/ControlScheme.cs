using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Diagnostics;

// ToDo:
// Add alternative key back in.
// Implement for keyboard and mosue stuff.
// Add enum actions in... oops 

namespace ChairWars.Controls
{
    public enum ControllerType : byte { notset = 0, controller = 1, kbm = 2 }


    public abstract class KeyMap<T>
    {
        public KeyMap()
        {
            keys = new List<T>();
            Subscriptions = new List<IDisposable>();
        }

        ~KeyMap()
        {
            Subscriptions.ForEach(f => f.Dispose());
        }

        protected List<T> keys;
        public IList<T> Keys
        {
            get { return keys; }
        }

        public bool AddKey(T key)
        {
            if( keys.Contains(key) )
                return false;

            keys.Add(key);
            return true;
        }

        public bool RemoveKey(T key)
        {
            var count = keys.Count;
            for( int i = count - 1; i != 0; --i)
            {
                if( keys[i].Equals(key) )
                {
                    keys.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public List<IDisposable> Subscriptions
        {
            get;
            set;
        }
    }

    public class KeyMapController : KeyMap<Buttons>
    {

        public KeyMapController() : base() { }

    }

    // For later...
    public class KeyMapKeyboard : KeyMap<Keys>
    {

    }

    public abstract class ControlScheme<T>
    {
        protected int arraySize = 0;

        //protected T[] altKeyList;
        //public T[] AltKeyList
        //{
        //    get { return altKeyList; }
        //    protected set { altKeyList = value; }
        //}

        protected T[] keyList;

        //protected T[] menuKeyList;
        //public T[] MenuKeyList
        //{
        //    get { return menuKeyList; }
        //    protected set { menuKeyList = value; }
        //}

        public PlayerIndex playerID;
        public ControllerType controllerType;

        public ControlScheme()
        {
            //AltKey = default(T);
            arraySize = -1;
            controllerType = default(ControllerType);
            playerID = PlayerIndex.One;
        }

        public ControlScheme(string fileName, Enum actions)
            : base()
        {
            Debug.Assert(actions == null, "Actions cannot be null!");
            arraySize = Enum.GetValues(actions.GetType()).Length;

            keyList = new T[arraySize];

        }


    }
    public class ControlSchemeC : ControlScheme<KeyMapController>
    {
        public ControlSchemeC(string fileName, Enum actions) : base(fileName, actions)
        { }

        public ControlSchemeC() : base() {  }

    }

    // Finish me: ...
    // Add a keymap esp. for the mouse.
    public class ControlSchemeKBM : ControlScheme<KeyMapKeyboard>
    {
       
    }

    public static class ControlSchemeHelper
    {
        public static T[] GetAllValues<T>() where T : struct
        {
            var values = Enum.GetValues(typeof(T));
            T[] asTypedArray = new T[values.Length];
            Array.Copy(values, asTypedArray, values.Length);

            return asTypedArray;
        }
    }
}
