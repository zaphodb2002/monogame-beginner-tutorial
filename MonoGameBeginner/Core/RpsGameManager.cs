using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class RpsGameManager
{
    private const int BestOfWinsNeeded = 2;
    public const int BestOfRounds = 3;

    private int _playerWins = 0;
    private int _aiWins = 0;
    
    private readonly Random _random = new Random(69420);
    
    private const double TimerLength = 4.0;
    private double _countDownTimer = TimerLength;
    
    private Sprite _rockSprite;
    private Sprite _paperSprite;
    private Sprite _scissorsSprite;
    private Sprite _undecidedSprite;
    private bool _timerRunning;
    public bool? PlayerWon = null;

    public void Update(GameTime gameTime)
    {
        if (_playerWins >= BestOfWinsNeeded)
        {
            ResolveGame(playerWon: true);
        }
        else if(_aiWins >= BestOfWinsNeeded)
        {
            ResolveGame(playerWon: false);
        }
        
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

    public void NewGame()
    {
        _playerWins = 0;
        _aiWins = 0;
        PlayerWon = null;
    }

    private void ResolveGame(bool playerWon)
    {
        PlayerWon = playerWon;
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
        var result = ShootResult.Lose;
        if (player == ai)
        {
            result = ShootResult.Tie;
        }

        if (player == ShootChoice.Scissors)
        {
            if (ai == ShootChoice.Rock)
            {
                result = ShootResult.Lose;
            }
        }

        if (player == ShootChoice.Rock)
        {
            if (ai == ShootChoice.Scissors)
            {
                result =  ShootResult.Win;
            }
        }

        if (player > ai)
        {
            result = ShootResult.Win;
        }

        if (result == ShootResult.Win)
            _playerWins++;
        else if (result == ShootResult.Lose)
            _aiWins++;
        return result;
    }

    public Sprite ChoiceToSprite(ShootChoice choice)
    {
        switch (choice)
        {
            case ShootChoice.Rock:
                return new Sprite(_rockSprite.Texture);
            case ShootChoice.Paper:
                return new Sprite(_paperSprite.Texture);
            case ShootChoice.Scissors:
                return new Sprite(_scissorsSprite.Texture);
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

    public int GetPlayerScore()
    {
        return _playerWins;
    }

    public int GetAiScore()
    {
        return _aiWins;
    }

    public void Reset()
    {
        PlayerWon = null;
       
    }
}