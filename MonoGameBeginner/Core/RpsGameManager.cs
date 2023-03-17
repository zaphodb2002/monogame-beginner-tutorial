using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class RpsGameManager
{
    private readonly Random _random = new Random(69420);
    
    private Sprite _rockSprite;
    private Sprite _paperSprite;
    private Sprite _scissorsSprite;
    public void CreateSprites(ContentManager content)
    {
        var rockTexture = content.Load<Texture2D>("img/rock");
        var paperTexture = content.Load<Texture2D>("img/paper");
        var scissorsTexture = content.Load<Texture2D>("img/scissors");

        _rockSprite = new Sprite(rockTexture);
        _paperSprite = new Sprite(paperTexture);
        _scissorsSprite = new Sprite(scissorsTexture);

        
    }

    public ShootChoice PlayerShoot()
    {
        return AiShoot();
    }

    public ShootChoice AiShoot()
    {
        var choice = _random.Next(Enum.GetNames<ShootChoice>().Length);
        return (ShootChoice)choice; 
    }
    public ShootResult Play(ShootChoice player, ShootChoice ai)
    {
        if (player == ai)
        {
            return ShootResult.Tie;
        }

        if (player == ShootChoice.Scissors)
        {
            if (ai == ShootChoice.Rock)
            {
                return ShootResult.Lose;
            }
        }

        if (player == ShootChoice.Rock)
        {
            if (ai == ShootChoice.Scissors)
            {
                return ShootResult.Win;
            }
        }

        if (player > ai)
        {
            return ShootResult.Win;
        }
        
        return ShootResult.Lose;
    }

    public Sprite ChoiceToSprite(ShootChoice choice)
    {
        switch (choice)
        {
            case ShootChoice.Rock:
                return new Sprite(_rockSprite.Texture);
                break;
            case ShootChoice.Paper:
                return new Sprite(_paperSprite.Texture);
                break;
            case ShootChoice.Scissors:
                return new Sprite(_scissorsSprite.Texture);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}