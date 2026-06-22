using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

public class DialogueBox {
    public string Text { get; set; }
    private TimeSpan _timer = TimeSpan.Zero;

    public void Draw(GameTime gameTime, SpriteFont _font) {
        _timer += gameTime.ElapsedGameTime;
        string tmp="";
        int i=0;
        while(i<Text.Length){
            tmp+=Text[i];
            //draw each character with a delay
            Core.SpriteBatch.DrawString(_font, tmp, new Vector2(50, 400), Color.Black);
        }
        // Use Core.SpriteBatch to draw the text, portioning it based off of the timer
        // You can get a view into a portion of the string with Text.AsSpan(start, length)
    }
}
/*
DialogueBox dialogue=new DialogueBox();
dialogue.set("hello there");
dialogue.draw();
//Supposed to output with delay.
*/