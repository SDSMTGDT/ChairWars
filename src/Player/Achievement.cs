using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ChairWars.Players
{
    class Achievement //: IXmlIO
    {
        private string Name;
        public int Task { get; private set; }
        public int Progress { get; private set; }


        public Achievement(string name, int task, int progress)
        {
            Name = name;
            Task = task;
            Progress = progress;
        }

    }
}
