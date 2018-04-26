using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sample;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _3DGraphicsProjectVersion2
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        InputEngine input;
        DebugEngine debug;
        ImmediateShapeDrawer shapeDrawer;

        List<CustomEffectModel> gameObjects = new List<CustomEffectModel>();
        Camera mainCamera;

        Effect pointLightEffect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            input = new InputEngine(this);
            debug = new DebugEngine();
            shapeDrawer = new ImmediateShapeDrawer();

            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            GameUtilities.Content = Content;
            GameUtilities.GraphicsDevice = GraphicsDevice;
            GameUtilities.Random = new System.Random();

            debug.Initialize();
            shapeDrawer.Initialize();

            mainCamera = new Camera("cam", new Vector3(0, 20, 10), new Vector3(0, 0, -1));
            mainCamera.Initialize();

            base.Initialize();
        }

        public Vector3 PickRandomPosition(int min, int max)
        {
            return new Vector3(
                GameUtilities.Random.Next(min, max),
                GameUtilities.Random.Next(0, 100),
                GameUtilities.Random.Next(min, max));
        }

        public Color PickRandomColor()
        {
            return new Color(
                GameUtilities.Random.Next(1, 255),
                GameUtilities.Random.Next(1, 255),
                GameUtilities.Random.Next(1, 255));
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AddModel(new PointLightModel("rat", new Vector3(0, 1f, -100)));
            CreateLights();
            GameUtilities.Random = new System.Random();
        }

        public void AddModel(CustomEffectModel Model)
        {
            Model.Initialize();
            Model.LoadContent();

            gameObjects.Add(Model);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            GameUtilities.Time = gameTime;
            if(InputEngine.IsKeyHeld(Keys.Escape))
            {
                Exit();
            }

            mainCamera.Update();

            gameObjects.ForEach(go => go.Update());

            foreach (var l in Lights.OfType<PointLightModel.PointLightMaterial>())
            {
                DebugEngine.AddBoundingSphere(new BoundingSphere(l.Position, l.Attenuation), l.DiffuseColour);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var gameObject in gameObjects)
            {
                if (FrustumContains((gameObject as SimpleModel).AABB))
                {
                    gameObject.Draw(mainCamera);
                }
            }

            debug.Draw(mainCamera);

            spriteBatch.Begin();
            spriteBatch.End();

            GameUtilities.SetGraphicsDeviceFor3D();

            base.Draw(gameTime);
        }

        public bool FrustumContains(BoundingBox aabb)
        {
            if (mainCamera.Frustum.Contains(aabb) != ContainmentType.Disjoint)
                return true;
            else return false;
        }

        List<Material> Lights = new List<Material>();

        private void CreateLights()
        {
            pointLightEffect = Content.Load<Effect>("PointLight");
            for (int i = 0; i < 3; i++)
            {
                Lights.Add(new PointLightModel.PointLightMaterial()
                {
                    AmbientColour = new Color(.15f, .15f, .15f),
                    Position = PickRandomPosition(-70, 120),
                    LightColour = PickRandomColor(),
                    Attenuation = 50,
                    FallOff = 50,
                });
            }
        }
    }
}
