using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sample;
using System.Collections.Generic;
using System.Diagnostics;

namespace _3DGraphicsProjectVersion2
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        InputEngine input;
        DebugEngine debug;
        ImmediateShapeDrawer shapeDrawer;

        List<SimpleModel> gameObjects = new List<SimpleModel>();
        Camera mainCamera;

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

        public void AddModel(CustomEffectModel Model)
        {
            Model.Initialize();
            Model.LoadContent();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AddModel(new PointLightModel("wolf", new Vector3(0, 2f, -32)));
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
