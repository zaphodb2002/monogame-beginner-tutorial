using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core;

public static class Mouse
{
    public static float X => GetMousePosition().X;
    public static float Y => GetMousePosition().Y;
    
    private static MouseState _currentState;
    private static MouseState _previousState;

    public static MouseState Refresh()
    {
        _previousState = _currentState;
        _currentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        return _currentState;
    }

    private static bool LeftButtonHeld()
    {
        return _currentState.LeftButton == ButtonState.Pressed;
    }

    public static bool LeftButtonPressed()
    {
        return _currentState.LeftButton == ButtonState.Pressed && _previousState.LeftButton != ButtonState.Pressed;
    }
    
    private static Vector2 GetMousePosition()
    {
        var x = Microsoft.Xna.Framework.Input.Mouse.GetState().X;
        var y = Microsoft.Xna.Framework.Input.Mouse.GetState().Y;
        return new Vector2(x, y);
    }
}