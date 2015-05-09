using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace AlumnoEjemplos.MeantToLiveGeo.User
{
    class UserInput
    {
        private Vehicle.Vehicle vehicle;

        private TgcD3dInput input;

        public UserInput(Vehicle.Vehicle vehicle)
        {
            this.vehicle = vehicle;
            input = GuiController.Instance.D3dInput;
        }

        public void updateUserMovements(float elapsedTime) 
        {
            if (input.keyDown(Key.UpArrow))
            {
                this.vehicle.accelerate();
                if (GuiController.Instance.ThirdPersonCamera.OffsetForward > -23) GuiController.Instance.ThirdPersonCamera.OffsetForward = GuiController.Instance.ThirdPersonCamera.OffsetForward -(1*elapsedTime);
            }
            else
            {
                this.vehicle.desaccelerate();
                if (GuiController.Instance.ThirdPersonCamera.OffsetForward < -20) GuiController.Instance.ThirdPersonCamera.OffsetForward = GuiController.Instance.ThirdPersonCamera.OffsetForward +((float)0.5*elapsedTime);
            }

            if (input.keyDown(Key.DownArrow))
            {
                this.vehicle.brake();
                if (GuiController.Instance.ThirdPersonCamera.OffsetForward < -20) GuiController.Instance.ThirdPersonCamera.OffsetForward = GuiController.Instance.ThirdPersonCamera.OffsetForward +(1*elapsedTime);
            }
            else
            {
                this.vehicle.desaccelerate();
            }

            if (input.keyDown(Key.RightArrow))
            {
                this.vehicle.turnRight();
            }

            if (input.keyDown(Key.LeftArrow))
            {
                this.vehicle.turnLeft();
            }
            
            if(this.vehicle.isRotated() && !input.keyDown(Key.RightArrow) && !input.keyDown(Key.LeftArrow))
            {
                this.vehicle.straighten();
            }
            

            this.vehicle.move(elapsedTime);
        }

    }
}
 