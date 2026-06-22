using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes;

public class RatScene : Scene
{
        // Defines the slime animated sprite.
    private AnimatedSprite _slime;
    private AnimatedSprite _rat;
    private Texture2D _rightDoor;
    private Vector2 _rightDoorPos;
    
    // Tracks the position of the slime.
    private Vector2 _slimePosition;
    private Vector2 _ratPosition;
    

    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 5.0f;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;

    // The sound effect to play when the bat bounces off the edge of the screen.
    private SoundEffect _bounceSoundEffect;


    // The sound effect to play when the slime eats a bat.
    //private SoundEffect _collectSoundEffect; //add dialogue sound effect?

    // The SpriteFont Description used to draw text
    private SpriteFont _font;

    // Defines the position to draw the score text at.
    //private Vector2 _scoreTextPosition;

    // Defines the origin used when drawing the score text.
    //private Vector2 _scoreTextOrigin;

    Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TileHeight * 2
        );

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        
        _slimePosition = new Vector2(screenBounds.Width/2 /*-1.5f*_tilemap.TileWidth*/, screenBounds.Height/2);
        _ratPosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow + _tilemap.TileHeight);

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        //_scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        //float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        //_scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        // Assign the initial random velocity to the bat.
        AssignRandomBatVelocity();
    }
    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        _rightDoor = Core.Content.Load<Texture2D>("images/door");
        
        _rightDoorPos = new Vector2(
            screenBounds.Width-_rightDoor.Width,
            screenBounds.Height/2
        );

        // Create the slime animated sprite from the atlas.
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);
        _rat = atlas.CreateAnimatedSprite("slime-animation");
        _rat.Scale = new Vector2(4.0f, 4.0f);

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        // Load the bounce sound effect.
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the font.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");
        /*
        DialogueManager dialogue;

        
        dialogue = new DialogueManager();
        dialogue.LoadFromFile("Content/dialogue.txt"); // adjust path as needed
        */

    }
    public override void Update(GameTime gameTime)
    {
        // Update the slime animated sprite.
        _slime.Update(gameTime);
        // Check for keyboard input and handle it.
        CheckKeyboardInput();

        // Creating a bounding circle for the slime.
        Circle slimeBounds = new Circle(
            (int)(_slimePosition.X + (_slime.Width * 0.5f)),
            (int)(_slimePosition.Y + (_slime.Height * 0.5f)),
            (int)(_slime.Width * 0.5f)
        );

        // Use distance based checks to determine if the slime is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // move it back inside.
        if (slimeBounds.Left < _roomBounds.Left)
        {
            _slimePosition.X = _roomBounds.Left;
        }
        else if (slimeBounds.Right > _roomBounds.Right)
        {
            _slimePosition.X = _roomBounds.Right - _slime.Width;
        }

        if (slimeBounds.Top < _roomBounds.Top)
        {
            _slimePosition.Y = _roomBounds.Top;
        }
        else if (slimeBounds.Bottom > _roomBounds.Bottom)
        {
            _slimePosition.Y = _roomBounds.Bottom - _slime.Height;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        
     
        Circle rightDoorBounds = new Circle(
            (int)(_rightDoorPos.X + (_rightDoor.Width * 0.5f)),
            (int)(_rightDoorPos.Y + (_rightDoor.Height * 0.5f)),
            (int)(_rightDoor.Width * 0.5f)
        );

        if (rightDoorBounds.Intersects(slimeBounds))
        {
            //Change scene to PrincessScene. Save count for the amount of times this is triggered..?
            Core.ChangeScene(new GameScene());
            // Play the door opening sound effect.
            //Core.Audio.PlaySoundEffect(_collectSoundEffect);
            // Increase the player's score.
            //_score += 100;
        }
        /*
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            dialogue.Advance();
        }*/
    }
    private void AssignRandomBatVelocity()
    {
        // Generate a random angle.
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

    }

    private void CheckKeyboardInput()
    {
        // Get a reference to the keyboard inof
        KeyboardInfo keyboard = Core.Input.Keyboard;

        // If the escape key is pressed, return to the title screen.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
        }

        // If the space key is held down, the movement speed increases by 1.5
        float speed = MOVEMENT_SPEED;
        if (keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }

        // If the M key is pressed, toggle mute state for audio.
        if (keyboard.WasKeyJustPressed(Keys.M))
        {
            Core.Audio.ToggleMute();
        }

        // If the + button is pressed, increase the volume.
        if (keyboard.WasKeyJustPressed(Keys.OemPlus))
        {
            Core.Audio.SongVolume += 0.1f;
            Core.Audio.SoundEffectVolume += 0.1f;
        }

        // If the - button was pressed, decrease the volume.
        if (keyboard.WasKeyJustPressed(Keys.OemMinus))
        {
            Core.Audio.SongVolume -= 0.1f;
            Core.Audio.SoundEffectVolume -= 0.1f;
        }
    }

    
    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        //Core.GraphicsDevice.Clear(Color.CornflowerBlue);
        //Core.SpriteBatch.End();
        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);

        // Draw the slime sprite.
        _slime.Draw(Core.SpriteBatch, _slimePosition);
        _rat.Draw(Core.SpriteBatch, _ratPosition);

        Core.SpriteBatch.Draw(_rightDoor, _rightDoorPos, Color.White);   
        
        Core.SpriteBatch.DrawString(_font, "Hello World!", new Vector2(50, 400), Color.Black);

        //Core.SpriteBatch.DrawString(dialogFont, dialogue.GetCurrentLine(), new Vector2(50, 400), Color.White);
        
        Core.SpriteBatch.End();
    }

}
