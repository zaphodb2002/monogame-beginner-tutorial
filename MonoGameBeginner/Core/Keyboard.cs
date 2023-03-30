using System;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class Keyboard
{
    private static KeyboardState currentState;
    private static KeyboardState previousState;

    public static KeyboardState Refresh()
    {
        previousState = currentState;
        currentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        return currentState;
    }

    public static bool IsHeld(Keys key)
    {
        return currentState.IsKeyDown(key);
    }

    public static bool IsPressed(Keys key)
    {
        var result = currentState.IsKeyDown(key) && !previousState.IsKeyDown(key);
        return result;
    }
}