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

            AddModel(new PointLightModel("wolf", new Vector3(0, 2f, -1000)));

            SetupEffectAndRenderTargets();

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

            DrawDepthAndNormalMaps(mainCamera);
            DrawLightMap(mainCamera);
            PrepareMainPass(mainCamera);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var gameObject in gameObjects)
            {
                if (FrustumContains((gameObject as SimpleModel).AABB))
                {
                    gameObject.Draw(mainCamera);
                    objectsDrawn++;
                }
            }

            debug.Draw(mainCamera);

            spriteBatch.Begin();
            spriteBatch.Draw(normalTarget, new Rectangle(10, 10, 400, 200), Color.White);
            spriteBatch.Draw(depthTarget, new Rectangle(435, 10, 400, 200), Color.White);
            spriteBatch.Draw(lightTarget, new Rectangle(860, 10, 400, 200), Color.White);

            spriteBatch.End();

            GameUtilities.SetGraphicsDeviceFor3D();

            base.Draw(gameTime);
        }

        int objectsDrawn = 0;

        public bool FrustumContains(BoundingBox aabb)
        {
            if (mainCamera.Frustum.Contains(aabb) != ContainmentType.Disjoint)
                return true;
            else return false;
        }

        Effect depthAndNormalEffect;
        RenderTarget2D depthTarget;
        RenderTarget2D normalTarget;
        RenderTarget2D lightTarget;

        Effect lightMapEffect;
        Model PointLightMesh;
        List<Material> Lights = new List<Material>();

        private void SetupEffectAndRenderTargets()
        {
            lightMapEffect = Content.Load<Effect>("effects\\LightMapEffect");
            PointLightMesh = Content.Load<Model>("models\\LightMesh");
            PointLightMesh.Meshes[0].MeshParts[0].Effect = lightMapEffect;

            lightTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            for (int i = 0; i < 10; i++)
            {
                Lights.Add(new PointLightModel.PointLightMaterial()
                {
                    AmbientColour = new Color(.15f, .15f, .15f),
                    Position = PickRandomPosition(-100, 100),
                    LightColour = PickRandomColor(),
                    Attenuation = 50,
                    FallOff = 100,
                });
            }

            depthAndNormalEffect = Content.Load<Effect>
                ("effects\\CaptureDepthAndNormal");

            depthTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24);

            normalTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        private void DrawDepthAndNormalMaps(Camera camera)
        {
            GraphicsDevice.SetRenderTargets(normalTarget, depthTarget);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.Clear(Color.White);

            foreach (var obj in gameObjects)
            {
                obj.CacheEffect();

                obj.SetModelEffect(depthAndNormalEffect, false);
                obj.Draw(camera);

                obj.RestoreEffect();
            }

            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawLightMap(Camera camera)
        {
            lightMapEffect.Parameters["NormalTexture"].SetValue(normalTarget);
            lightMapEffect.Parameters["DepthTexture"].SetValue(depthTarget);

            var viewProjection = camera.View * camera.Projection;
            var inverseVP = Matrix.Invert(viewProjection);
            lightMapEffect.Parameters["InverseViewProjection"].SetValue(inverseVP);

            GraphicsDevice.SetRenderTarget(lightTarget);

            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            foreach (var light in Lights.OfType<PointLightModel.PointLightMaterial>())
            {
                light.SetEffectParameters(lightMapEffect);

                var wvp = (Matrix.CreateScale(light.Attenuation) *
                    Matrix.CreateTranslation(light.Position)) *
                    viewProjection;

                lightMapEffect.Parameters["WorldViewProjection"].SetValue(wvp);

                float distance = Vector3.Distance(camera.World.Translation, light.Position);

                if (distance < light.Attenuation)
                    GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                PointLightMesh.Meshes[0].Draw();

                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SetRenderTarget(null);
        }

        private void PrepareMainPass(Camera _camera)
        {
            foreach (var obj in gameObjects)
            {
                if (obj.Model != null)
                {
                    foreach (var mesh in obj.Model.Meshes)
                    {
                        foreach (var part in mesh.MeshParts)
                        {
                            obj.SetEffectParameter(part.Effect, "LightTexture", lightTarget);
                            obj.SetEffectParameter(part.Effect, "ViewportWidth", (float)GraphicsDevice.Viewport.Width);
                            obj.SetEffectParameter(part.Effect, "ViewportHeight", (float)GraphicsDevice.Viewport.Height);
                        }
                    }
                }
            }
        }

    }
}
