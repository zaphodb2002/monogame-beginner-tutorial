using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class RpsGameManager
{
    private readonly Random _random = new Random(69420);
    
    private const double TimerLength = 4.0;
    private double _countDownTimer = TimerLength;
    
    private Sprite _rockSprite;
    private Sprite _paperSprite;
    private Sprite _scissorsSprite;
    private Sprite _undecidedSprite;
    private bool _timerRunning;

    public void Update(GameTime gameTime)
    {
        if (_timerRunning)
        {
            if (_countDownTimer <= -1)
            {
                _timerRunning = false;
                return;
            }
            Console.WriteLine(_countDownTimer);
            _countDownTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        }
        

    }

    public double GetCountDownRemaining()
    {
        return _countDownTimer;
    }
    
    public void CreateSprites(ContentManager content)
    {
        var rockTexture = content.Load<Texture2D>("img/rock");
        var paperTexture = content.Load<Texture2D>("img/paper");
        var scissorsTexture = content.Load<Texture2D>("img/scissors");

        _rockSprite = new Sprite(rockTexture);
        _paperSprite = new Sprite(paperTexture);
        _scissorsSprite = new Sprite(scissorsTexture);
        
        var questionTexture = content.Load<Texture2D>("img/question");
        _undecidedSprite = new Sprite(questionTexture);
    }

    public ShootChoice PlayerShoot(ShootChoice playerChoice)
    {
        return playerChoice;
    }

    public ShootChoice AiShoot()
    {
        var choice = _random.Next(0, 2);
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
            case ShootChoice.Undecided:
                return new Sprite(_undecidedSprite.Texture);
            default:
                return new Sprite(_undecidedSprite.Texture);
        }
    }

    public Sprite GetSprite(ShootChoice choice)
    {
        switch (choice)
        {
            case ShootChoice.Rock:
                return _rockSprite;
            case ShootChoice.Paper:
                return _paperSprite;
            case ShootChoice.Scissors:
                return _scissorsSprite;
            default:
                throw new ArgumentOutOfRangeException(nameof(choice), choice, null);
        }
    }

    public void StartCountDown()
    {
        _countDownTimer = TimerLength;
        _timerRunning = true;
    }
}