using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Sprite
{
    private Texture2D _texture;
    public Texture2D Texture => _texture;
    private Vector2 _origin;
    public Vector2 Origin => _origin;
    private Color _tint;
    private Vector2 _position;
    private bool _visible = true;
    public Vector2 Position => _position;
    public Color Tint => _tint;
    

    public Sprite(Texture2D texture)
    {
        _texture = texture;
        _tint = Color.White;
        _origin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 scale)
    {
        if (_visible)
        {
            spriteBatch.Draw(
                _texture,
                _position,
                null,
                _tint,
                0f,
                _origin,
                scale, 
                SpriteEffects.None,
                0f);    
        }
        
    }

    public void SetPosition(Vector2 pos)
    {
        _position = pos;
    }


    public void SetTint(Color color)
    {
        _tint = color;
    }

    public void SetVisible(bool value)
    {
        _visible = value;
    }
}