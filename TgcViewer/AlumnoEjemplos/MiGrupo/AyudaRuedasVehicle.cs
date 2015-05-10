/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.MeantToLiveGeo.Terrain;

 // Esta clase es la encargada de todo el moviemiento y actualizacion del vehiculo.
 
 
 
namespace AlumnoEjemplos.MeantToLiveGeo.Vehicle
{
    class Vehicle
    {
        const float ACELERATION = 3f;
        const float BRAKE_ACELERATION = 5f;
        const float DESACELERATION = 0.5f;
        const float MAX_SPEED = 100f;
        const float MIN_SPEED = -30f;

        private float speed = 0;
        
        private TgcMesh mesh;

        private Wheel frontLeft;

        private Wheel frontRight;

        private Wheel backLeft;

        private Wheel backRight;

        private float frontLeftCurrentHeight;

        private float frontRightCurrentHeight;

        private float backLeftCurrentHeight;

        private float backRightCurrentHeight;

        private Vector3 currentRectBetweenBackWheels;

        private Vector3 currentRectBetweenBackAndFrontWheels;
        
        private Vector3 currentPivotPoint = new Vector3();

        private Vector3 newPivotPoint = new Vector3();

        private Vector3 backPivotPoint = new Vector3();

        private HeightMap heightMap;

        private float distanceBetweenFrontAxisToCenter;

        private float distanceBetweenBackAxisToCenter;

        private float distanceBetweenAxis;

        private float chassisElevation;

        private float heightVehicleFromFloor;

        private float distanceFromWheelToCenter;

        private float angleY;
        
        public const float SCALE = 3f;
        
        public Vector3 VECTOR_SCALE = new Vector3(SCALE, SCALE, SCALE);

        public Vehicle(HeightMap heightMap)
        {
            this.heightMap = heightMap;
            this.load();
        }

        private void load()
        {
            string urlMesh = GuiController.Instance.AlumnoEjemplosMediaDir + "MeantToLive\\Hummer_MTL\\Hummer_chasis-TgcScene.xml";
            
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(urlMesh);
            this.mesh = scene.Meshes[0];
            this.mesh.Scale = VECTOR_SCALE;

            this.distanceBetweenFrontAxisToCenter = 3.08f * SCALE; // 1.52f;
            this.distanceBetweenBackAxisToCenter = 0f * SCALE; // 1.56f;
            this.distanceBetweenAxis = this.distanceBetweenBackAxisToCenter + this.distanceBetweenFrontAxisToCenter;
            this.chassisElevation = 0f * SCALE; // 0.35f;
            this.distanceFromWheelToCenter = 0.75f * SCALE;
            this.heightVehicleFromFloor = this.chassisElevation + Wheel.RADIO_WHEEL;

            string urlWheel = GuiController.Instance.AlumnoEjemplosMediaDir + "MeantToLive\\Hummer_MTL\\Hummer_rueda-TgcScene.xml";

            TgcSceneLoader loaderWheel = new TgcSceneLoader();
            TgcScene sceneWheel = loaderWheel.loadSceneFromFile(urlWheel);

            TgcMesh meshFrontLeft = sceneWheel.Meshes[0].clone("FL-Wheel");
            meshFrontLeft.Scale = VECTOR_SCALE;
            this.frontLeft = new Wheel(meshFrontLeft, 1, new Vector3(-this.distanceFromWheelToCenter, -this.chassisElevation, this.distanceBetweenFrontAxisToCenter), this.heightMap);
            TgcMesh meshFrontRight = sceneWheel.Meshes[0].clone("FR-Wheel");
            meshFrontRight.Scale = VECTOR_SCALE;
            this.frontRight = new Wheel(meshFrontRight, -1, new Vector3(this.distanceFromWheelToCenter, -this.chassisElevation, this.distanceBetweenFrontAxisToCenter), this.heightMap);
            TgcMesh meshBackLeft = sceneWheel.Meshes[0].clone("BL-Wheel");
            meshBackLeft.Scale = VECTOR_SCALE;
            this.backLeft = new Wheel(meshBackLeft, 1, new Vector3(-this.distanceFromWheelToCenter, -this.chassisElevation, -this.distanceBetweenBackAxisToCenter), this.heightMap);
            TgcMesh meshBackRight = sceneWheel.Meshes[0].clone("BR-Wheel");
            meshBackRight.Scale = VECTOR_SCALE;
            this.backRight = new Wheel(meshBackRight, -1, new Vector3(this.distanceFromWheelToCenter, -this.chassisElevation, -this.distanceBetweenBackAxisToCenter), this.heightMap);

            this.mesh.Position = new Vector3(600, this.heightMap.get(600 / LandscapeRenderer.kGridSpacing, 300 / LandscapeRenderer.kGridSpacing) + this.heightVehicleFromFloor, 300);

            this.updateCurrentPivotPoints();

            this.updateCurrentWheelHeight();
        }

        public void render()
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("ShowBoundingBox"))
                this.mesh.BoundingBox.render();

            this.mesh.render();
            this.backLeft.render();
            this.backRight.render();
            this.frontLeft.render();
            this.frontRight.render();

            this.updateCurrentPivotPoints();
        }

        public Vector3 getPosition()
        {
            return mesh.Position;
        }

        
         // Acelera el vehiculo haciendolo avanzar
          
        
        public void accelerate()
        {
            float newSpeed = this.speed + ACELERATION;
            this.speed = newSpeed > MAX_SPEED ? MAX_SPEED : newSpeed;
        }

        // Si el usuario dejo de acelerar, entonces el vehiculo desacelera lentamente simulando una inercia.
         
        public void desaccelerate()
        {
            if(this.speed > 0) {
                float newSpeed = this.speed - DESACELERATION;
                this.speed = newSpeed < 0 ? 0 : newSpeed;
            } else {
                float newSpeed = this.speed + DESACELERATION;
                this.speed = newSpeed > 0 ? 0 : newSpeed;
            }
        }

        // Aplica una desaceleracion para frenar el vehiculo, si el vehiculo frena completamente este empieza a retroceder.
          
         
        public void brake()
        {
            float newSpeed = this.speed - (this.speed > 0 ? BRAKE_ACELERATION : ACELERATION);
            this.speed = newSpeed < MIN_SPEED ? MIN_SPEED : newSpeed;
        }

        
         // Gira las ruedas a la derecha
          
        
        public void turnRight()
        {
            this.frontLeft.turnRight();
            this.frontRight.turnRight();
        }
        // Gira las ruedas a la izquierda
         
        public void turnLeft()
        {
            this.frontLeft.turnLeft();
            this.frontRight.turnLeft();
        }

        // Si el usuario no presiono ninguna tecla de direccion, las ruedas vuelven a su posicion.
         
       
        public void straighten()
        {
            this.frontLeft.straighten();
            this.frontRight.straighten();
        }

        // Mueve el vehiculo a la nueva pocision de acuerdo a las teclas que presiono el usuario. 
         
        public void move(float elapsedTime)
        {
            float distance;
            float heightInPosition = this.heightMap.get(this.getPosition().X / LandscapeRenderer.kGridSpacing, this.getPosition().Z / LandscapeRenderer.kGridSpacing);
            if(heightInPosition > -1)
            {
               distance = this.speed * elapsedTime;
            }
            else
            {
                distance = this.speed * elapsedTime * 0.5f;
            }
            

            this.frontLeft.turn(elapsedTime);
            this.frontRight.turn(elapsedTime);

            this.updateNewPivotPoint(distance);

            this.angleY = this.calculateYRotationAngle();

            this.updateVehiclePosition(this.angleY);

            if(distance > 0)
            {
                this.updateHeightPosition(elapsedTime);
            }
        }

        // Calcula el angulo de rotacion en el eje Y 
         
        private float calculateYRotationAngle()
        {
            float angle = 0f;
            Vector3 displacement = Vector3.Subtract(this.newPivotPoint, this.currentPivotPoint);
            if (displacement.Length() > 0f)
            {
                Vector2 backPivotToNewPivot = new Vector2(this.newPivotPoint.X - this.backPivotPoint.X,
                                        this.newPivotPoint.Z - this.backPivotPoint.Z);
                Vector2 pivotToNewPivot = new Vector2(this.currentPivotPoint.X - this.backPivotPoint.X,
                                        this.currentPivotPoint.Z - this.backPivotPoint.Z);

                angle = this.angleBetweenRect(backPivotToNewPivot, pivotToNewPivot);
            }
            return angle * 0.3f;
        }

        // Actualiza la posicion del vehiculo de acuerdo al angulo de giro calculado 
         
        private void updateVehiclePosition(float angleY)
        {
            Vector3 newFrontDisplacement = Vector3.Subtract(this.newPivotPoint, this.backPivotPoint);
            Vector3 direction = Vector3.Normalize(newFrontDisplacement);
            Vector3 finalPosition = direction * this.distanceBetweenAxis;
            Vector3 displacement = Vector3.Subtract(newFrontDisplacement, finalPosition);
            this.mesh.rotateY(angleY);
            float distance = this.speed > 0 ? displacement.Length() : -displacement.Length();
            this.mesh.moveOrientedY(distance);

            this.frontLeft.rotateWheel(distance);
            this.frontRight.rotateWheel(distance);
            this.backLeft.rotateWheel(distance);
            this.backRight.rotateWheel(distance);

            this.frontLeft.setPositionInVehicle(this.mesh, this.currentPivotPoint, angleY);
            this.frontRight.setPositionInVehicle(this.mesh, this.currentPivotPoint, angleY);
            this.backLeft.setPositionInVehicle(this.mesh, this.backPivotPoint, angleY);
            this.backRight.setPositionInVehicle(this.mesh, this.backPivotPoint, angleY);
        }

        private void updateHeightPosition(float elapsedTime)
        {
            //float height = this.heightMap.get(2 / LandscapeRenderer.kGridSpacing, 2 / LandscapeRenderer.kGridSpacing);
            //float height = (this.frontLeft.getHeightInYourPosition() + this.frontRight.getHeightInYourPosition() + this.backLeft.getHeightInYourPosition() + this.backRight.getHeightInYourPosition()) / 4;
            float height = (this.backLeft.getHeightInYourPosition() + this.backRight.getHeightInYourPosition()) / 2;
            this.mesh.Position = new Vector3(this.mesh.Position.X, height + this.heightVehicleFromFloor, this.mesh.Position.Z);

           // Vector3 newRectBetweenBackWheels = Vector3.Subtract(this.backRight.getPosition(), this.backLeft.getPosition());
            //float angleZ = this.angleBetweenRect(new Vector2(this.currentRectBetweenBackWheels.Z, this.currentRectBetweenBackWheels.Y), new Vector2(newRectBetweenBackWheels.Z, newRectBetweenBackWheels.Y));
            //this.mesh.rotateZ(angleZ);
            //this.frontLeft.rotateZ(angleZ);
           // this.frontRight.rotateZ(angleZ);
           // this.backLeft.rotateZ(angleZ);
           // this.backRight.rotateZ(angleZ);

            //float xZ = (float)Math.Sin((float)angleZ) * this.distanceFromWheelToCenter;
            //float yZ = (float)Math.Cos((float)angleZ) * this.distanceFromWheelToCenter;
            //float zZ = 0f;
            //this.frontLeft.move(xZ, yZ, zZ);
            //this.frontRight.move(xZ, yZ, zZ);
            //this.backLeft.move(xZ, yZ, zZ);
            //this.backRight.move(xZ, yZ, zZ);

            //Vector3 newRectBetweenBackAndFrontWheels = Vector3.Subtract(this.backRight.getPosition(), this.frontRight.getPosition());
            //float angleX = this.angleBetweenRect(new Vector2(this.currentRectBetweenBackAndFrontWheels.X, this.currentRectBetweenBackAndFrontWheels.Y), new Vector2(newRectBetweenBackAndFrontWheels.X, newRectBetweenBackAndFrontWheels.Y));
            //this.mesh.rotateX(angleX);

            //float xX = 0f;
            //float yX = (float)Math.Cos((float)angleZ) * this.distanceFromWheelToCenter;
            //float zX = (float)Math.Sin((float)angleZ) * this.distanceFromWheelToCenter;
            //this.frontLeft.move(xX, yX, zX);
            //this.frontRight.move(xX, yX, zX);
            
            this.updateCurrentWheelHeight();
        }

        private void updateCurrentWheelHeight() 
        { 
            this.frontLeftCurrentHeight = this.frontLeft.getHeightInYourPosition();
            this.frontRightCurrentHeight = this.frontRight.getHeightInYourPosition();
            this.backLeftCurrentHeight = this.backLeft.getHeightInYourPosition();
            this.backRightCurrentHeight = this.backRight.getHeightInYourPosition();

            this.currentRectBetweenBackWheels = Vector3.Subtract(this.backRight.getPosition(), this.backLeft.getPosition());
            this.currentRectBetweenBackAndFrontWheels = Vector3.Subtract(this.backRight.getPosition(), this.frontRight.getPosition());
        }

        private void updateCurrentPivotPoints()
        {
            this.currentPivotPoint.X = 0f;
            this.currentPivotPoint.Y = 0f;
            this.currentPivotPoint.Z = this.distanceBetweenAxis;

            this.newPivotPoint.X = this.currentPivotPoint.X;
            this.newPivotPoint.Y = this.currentPivotPoint.Y;
            this.newPivotPoint.Z = this.currentPivotPoint.Z;

            this.backPivotPoint.X = 0f;
            this.backPivotPoint.Y = 0f;
            this.backPivotPoint.Z = 0f;
        }

        private void updateNewPivotPoint(float distance)
        {
            this.newPivotPoint.X += (float)Math.Sin((float)this.frontLeft.getGradesRotated()) * distance;
            this.newPivotPoint.Y += 0;
            this.newPivotPoint.Z += (float)Math.Cos((float)this.frontLeft.getGradesRotated()) * distance;
        }

        private float angleBetweenRect(Vector2 a, Vector2 b)
        {
            Vector2 ab = Vector2.Subtract(Vector2.Normalize(a), Vector2.Normalize(b));
            return ab.X != 0 ? Geometry.DegreeToRadian((float)Math.Atan2(ab.X, -ab.Y)) : 0;
        }

        public TgcMesh getMesh()
        {
            return this.mesh;
        }

        public bool isRotated()
        {
            return this.frontLeft.isRotated();
        }

        public void dispose()
        {
            this.mesh.dispose();
            this.frontLeft.dispose();
            this.frontRight.dispose();
            this.backLeft.dispose();
            this.backRight.dispose();
        }

        public float getRotation()
        {
            return this.angleY;
        }
    }
}
*/