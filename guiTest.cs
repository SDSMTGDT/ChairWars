using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;


namespace ChairWars
{
    public class guiTest : WindowControl
    {
        public guiTest()
        {
            InitializeComponent();
        }

        private LabelControl helloWorldLabel;
        public ButtonControl okButton;
        private void InitializeComponent()
        {
            /*this.okButton = new ButtonControl();
            this.okButton.Bounds = new UniRectangle(
              new UniScalar(1.0f, -180.0f), new UniScalar(1.0f, -40.0f), 80, 24
            );

            this.okButton.Pressed += OkButtonPressed; 
            
            */
            this.helloWorldLabel = new LabelControl();

            this.helloWorldLabel.Text = "Game Paused. Press Start to resume.";

            this.helloWorldLabel.Bounds = new Nuclex.UserInterface.UniRectangle(5.0f, 5.0f, 10.0f, 30.0f);

            //DemoDialog
            this.Bounds = new Nuclex.UserInterface.UniRectangle(270.0f, 215.0f, 255.0f, 50.0f);
            Children.Add(helloWorldLabel);
            //Children.Add(okButton);
        }

        private void OkButtonPressed(object sender, EventArgs args)
        {
            
        }
    }



}
