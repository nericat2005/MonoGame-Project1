using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;



namespace DungeonSlime.Scenes;
public class Intro : Scene
{
    

    List<string> dialogueLines = new List<string>
    {
        "W-What are you doing here?!",
        "GET OUT!!"
    };

    int currentLineIndex = 0;
    string fullText => dialogueLines[currentLineIndex];
    StringBuilder visibleText = new StringBuilder();
    int charIndex = 0;
    float elapsedTime = 0f;
    bool lineFullyDisplayed => charIndex >= fullText.Length;



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
        
        _slimePosition = new Vector2(1.5f*_tilemap.TileWidth, screenBounds.Height/2);
        _princessPosition = new Vector2(screenBounds.Width/2, screenBounds.Height/2);//centerColumn * _tilemap.TileWidth, centerRow + _tilemap.TileHeight);

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

  
        
        // Load the font.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");


    }
    public override void Update(GameTime gameTime)
    {    
        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!lineFullyDisplayed && elapsedTime > 0.05f)
        {
            visibleText.Append(fullText[charIndex]);
            charIndex++;
            elapsedTime = 0f;
        }

        // Input to advance dialogue
        if (lineFullyDisplayed && Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            AdvanceDialogue();
        }

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
        
     
        Circle leftDoorBounds = new Circle(
            (int)(_leftDoorPos.X + (_leftDoor.Width * 0.5f)),
            (int)(_leftDoorPos.Y + (_leftDoor.Height * 0.5f)),
            (int)(_leftDoor.Width * 0.5f)
        );

        if (leftDoorBounds.Intersects(slimeBounds))
        {
            //Change scene to PrincessScene. Save count for the amount of times this is triggered..?
            Core.ChangeScene(new GameScene());
            // Play the door opening sound effect.
            //Core.Audio.PlaySoundEffect(_collectSoundEffect);
            // Increase the player's score.
            //_score += 100;
        }
    }
    private void AdvanceDialogue()
    {
        if (currentLineIndex < dialogueLines.Count - 1)
        {
            currentLineIndex++;
            visibleText.Clear();
            charIndex = 0;
            elapsedTime = 0f;
        }
        else
        {
            Core.ChangeScene(new GameScene());
            // End of dialogue — trigger next scene or close panel
        }
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
        _princess.Draw(Core.SpriteBatch, _princessPosition);

        Core.SpriteBatch.Draw(_leftDoor, _leftDoorPos, Color.White);   
        
        Core.SpriteBatch.DrawString(_font, visibleText, new Vector2(screenBounds.Width/2, screenBounds.Height/2), Color.Black);
        
        Core.SpriteBatch.End();
    }

}
