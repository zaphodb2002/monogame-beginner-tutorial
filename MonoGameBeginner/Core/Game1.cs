using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class Game1 : Game
{
    // Design Levers
    private readonly float _horizontalOffsetDivisor = 5f;
    private readonly Vector2 _shootChoiceSpriteScale = Vector2.One * 6;
    private Color _playerColor = new (77,155,230);
    private Color _aiColor = Color.OrangeRed;
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private readonly RpsGameManager _rps = new ();
    
    private Vector2 _playerShootOrigin;
    private Vector2 _aiShootOrigin;
    private Vector2 _outcomeSpriteOrigin;
    private Vector2 _playerChoiceInputOrigin;
    private Vector2 _frameOrigin;

    private Sprite _playerChoice;
    private Sprite _aiChoice;
    private ShootResult _outcome;
    private Sprite _outcomeSprite;
    
    private Sprite _winSprite;
    private Sprite _loseSprite;
    private Sprite _tieSprite;
    

    private Sprite _oneSprite;
    private Sprite _twoSprite;
    private Sprite _threeSprite;
    private Sprite _shootSprite;
    private Sprite _readySprite;
    private Sprite _currentCountdownSprite;
    
    private Sprite _rockSprite;
    private Sprite _paperSprite;
    private Sprite _scissorsSprite;
    private Sprite _frameSprite;
    private ShootChoice _playerShootChoice;
    private Vector2 _countDownSpriteOrigin;
    private ShootChoice _aiShootChoice = ShootChoice.Undecided;

    private bool _isWaiting = false;


    #region Game Loop
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rps.CreateSprites(Content);
        var frameTexture = Content.Load<Texture2D>("img/frame");
        _frameSprite = new Sprite(frameTexture);
        SetUpScene();
        DisplayInputArea();
        _rps.StartCountDown();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        HandleInput();
        DisplayInputArea();
        DisplayChoices(_playerShootChoice, _aiShootChoice);
        DisplayOutcome();
        _rps.Update(gameTime);

        if (!_isWaiting)
        {
            _outcomeSprite = null;
            if (_rps.GetCountDownRemaining() >= 3)
                _currentCountdownSprite = _readySprite;
            else if (_rps.GetCountDownRemaining() > 2)
                _currentCountdownSprite = _threeSprite;
            else if (_rps.GetCountDownRemaining() > 1)
                _currentCountdownSprite = _twoSprite;
            else if (_rps.GetCountDownRemaining() > 0)
                _currentCountdownSprite = _oneSprite;
            else if (_rps.GetCountDownRemaining() > -1)
            {
                _currentCountdownSprite = _shootSprite;
                _aiShootChoice = _rps.AiShoot();
                _outcome = Shoot(_playerShootChoice);
                _isWaiting = true;
            }
            else
            {
                _currentCountdownSprite = null;
            }
        }
        
    }

    

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _playerChoice?.Draw(_spriteBatch, _playerShootOrigin, _shootChoiceSpriteScale);
        if(_playerChoice != null)
            _aiChoice.Draw(_spriteBatch, _aiShootOrigin, _shootChoiceSpriteScale);
        _outcomeSprite?.Draw(_spriteBatch, _outcomeSpriteOrigin, _shootChoiceSpriteScale);


        _rockSprite.Draw(_spriteBatch, _playerChoiceInputOrigin - new Vector2(_graphics.PreferredBackBufferHeight / 4, 0), _shootChoiceSpriteScale / 2f);
        _paperSprite.Draw(_spriteBatch, _playerChoiceInputOrigin, _shootChoiceSpriteScale / 2f);
        _scissorsSprite.Draw(_spriteBatch, _playerChoiceInputOrigin + new Vector2(_graphics.PreferredBackBufferHeight / 4, 0), _shootChoiceSpriteScale / 2f);
        _frameSprite.Draw(_spriteBatch, _frameOrigin, _shootChoiceSpriteScale / 2f);

        _currentCountdownSprite?.Draw(_spriteBatch, _countDownSpriteOrigin, _shootChoiceSpriteScale);

        _spriteBatch.End();
    }
    #endregion
    
    private void HandleInput()
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (!_isWaiting)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                _outcome = Shoot(_playerShootChoice);
            if (Mouse.GetState().X < _paperSprite.Position.X - 50)
            {
                _frameOrigin = _rockSprite.Position;
                _playerShootChoice = ShootChoice.Rock;
            }
            else if (Mouse.GetState().X > _paperSprite.Position.X + 50)
            {
                _frameOrigin = _scissorsSprite.Position;
                _playerShootChoice = ShootChoice.Scissors;
            }
            else
            {
                _frameOrigin = _paperSprite.Position;
                _playerShootChoice = ShootChoice.Paper;
            }
        }
        else
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Reset();
                
            }
        }
    }

    private void Reset()
    {
        _outcomeSprite = null;
        _outcome = ShootResult.Pending;
        _aiShootChoice = ShootChoice.Undecided;
        _playerShootChoice = ShootChoice.Undecided;
        _rps.StartCountDown();
        _isWaiting = false;
    }

    private void SetUpScene()
    {
        var screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        var offsetFromCenter = new Vector2(_graphics.PreferredBackBufferWidth / _horizontalOffsetDivisor, 0f);
        _playerShootOrigin = screenCenter - offsetFromCenter;
        _aiShootOrigin = screenCenter + offsetFromCenter;
        _outcomeSpriteOrigin = screenCenter - new Vector2(0, _graphics.PreferredBackBufferHeight / 6);
        _countDownSpriteOrigin = _outcomeSpriteOrigin - new Vector2(0, _graphics.PreferredBackBufferHeight / 8);
        _playerChoiceInputOrigin = screenCenter + new Vector2(0, _graphics.PreferredBackBufferWidth / 6);

        var winTexture = Content.Load<Texture2D>("img/win");
        var loseTexture = Content.Load<Texture2D>("img/lose");
        var tieTexture = Content.Load<Texture2D>("img/tie");

        _winSprite = new Sprite(winTexture);
        _winSprite.SetTint(Color.Green);
        _loseSprite = new Sprite(loseTexture);
        _loseSprite.SetTint(Color.Red);
        _tieSprite = new Sprite(tieTexture);
        _tieSprite.SetTint(Color.Yellow);

        var oneTexture = Content.Load<Texture2D>("img/1");
        var twoTexture = Content.Load<Texture2D>("img/2");
        var threeTexture = Content.Load<Texture2D>("img/3");
        var shootTexture = Content.Load<Texture2D>("img/shoot");
        var readyTexture = Content.Load<Texture2D>("img/ready");

        _oneSprite = new Sprite(oneTexture);
        _twoSprite = new Sprite(twoTexture);
        _threeSprite = new Sprite(threeTexture);
        _shootSprite = new Sprite(shootTexture);
        _readySprite = new Sprite(readyTexture);

        
    }
    private ShootResult Shoot(ShootChoice playerChoice)
    {
        _playerShootChoice = _rps.PlayerShoot(playerChoice);
        
        return _rps.Play(_playerShootChoice, _aiShootChoice);
    }

    private void DisplayChoices(ShootChoice playerShoot, ShootChoice aiShoot)
    {
        _playerChoice = _rps.ChoiceToSprite(playerShoot);
        _playerChoice.SetTint(_playerColor);
        _aiChoice = _rps.ChoiceToSprite(aiShoot);
        _aiChoice.SetTint(_aiColor);
    }

    private void DisplayInputArea()
    {
        _rockSprite = _rps.GetSprite(ShootChoice.Rock);
        _paperSprite = _rps.GetSprite(ShootChoice.Paper);
        _scissorsSprite = _rps.GetSprite(ShootChoice.Scissors);
        
    }
    
    private void DisplayOutcome()
    {
        if (_outcome != ShootResult.Pending)
        {
            switch (_outcome)
            {
                case ShootResult.Pending:
                    throw new Exception("How'd you get here?");
                case ShootResult.Lose:
                    _outcomeSprite = _loseSprite;
                    break;
                case ShootResult.Win:
                    _outcomeSprite = _winSprite;
                    break;
                case ShootResult.Tie:
                    _outcomeSprite = _tieSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}