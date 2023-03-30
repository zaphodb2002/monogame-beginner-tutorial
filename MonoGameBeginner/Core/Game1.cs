using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class Game1 : Game
{
    #region Properties
    // Design Levers
    private readonly float _horizontalOffsetDivisor = 5f;
    private readonly Vector2 _shootChoiceSpriteScale = Vector2.One * 6;
    private readonly Color _playerColor = new (77,155,230);
    private readonly Color _aiColor = Color.OrangeRed;
    private SpriteFont _defaultFont;
    
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
    
    private Texture2D _blankTexture;
    
    private ShootChoice _playerShootChoice;
    private Vector2 _countDownSpriteOrigin;
    private ShootChoice _aiShootChoice = ShootChoice.Undecided;

    private bool _isWaiting = true;
    
    private string _matchResultText = "";
    private string _playerScoreString = "";
    private string _aiScoreString = "";
    private Vector2 _playerMatchScoreOrigin;
    private Vector2 _aiMatchScoreOrigin;

    

    #endregion
    
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
        _defaultFont = Content.Load<SpriteFont>("default");
        _rps.CreateSprites(Content);
        SetUpScene();
        Reset();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        HandleInput();
        DisplayInputArea();
        DisplayChoices(_playerShootChoice, _aiShootChoice);
        DisplayMatchScore(_rps.GetPlayerScore(), _rps.GetAiScore());
        DisplayOutcome();
        DisplayGameOver(playerWon: _rps.PlayerWon);
        _rps.Update(gameTime);

        if (!_isWaiting)
        {
            _currentCountdownSprite.SetVisible(true);
            _outcomeSprite?.SetVisible(false);
            _matchResultText = String.Empty;
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
                _currentCountdownSprite.SetVisible(false);
            }
        }
        else
        {
            _outcomeSprite?.SetVisible(true);
        }
        
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _playerChoice?.Draw(_spriteBatch, _shootChoiceSpriteScale);
        if(_playerChoice != null)
            _aiChoice.Draw(_spriteBatch, _shootChoiceSpriteScale);
        _outcomeSprite?.Draw(_spriteBatch, _shootChoiceSpriteScale);


        _rockSprite.Draw(_spriteBatch, _shootChoiceSpriteScale / 2f);
        _paperSprite.Draw(_spriteBatch, _shootChoiceSpriteScale / 2f);
        _scissorsSprite.Draw(_spriteBatch, _shootChoiceSpriteScale / 2f);
        _frameSprite.Draw(_spriteBatch, _shootChoiceSpriteScale / 2f);

        _currentCountdownSprite?.Draw(_spriteBatch, _shootChoiceSpriteScale);

        _spriteBatch.DrawString(_defaultFont, _playerScoreString, _playerMatchScoreOrigin, _playerColor);
        _spriteBatch.DrawString(_defaultFont, _aiScoreString, _aiMatchScoreOrigin, _aiColor);
        _spriteBatch.DrawString(_defaultFont, _matchResultText, _countDownSpriteOrigin, Color.White);

        _spriteBatch.End();
    }
    #endregion

    private void HandleInput()
    {
        Keyboard.Refresh();
        Mouse.Refresh();
        if (Keyboard.IsPressed(Keys.Escape))
            Exit();

        if (!_isWaiting)
        {
            if (Mouse.LeftButtonPressed())
                _outcome = Shoot(_playerShootChoice);
            if (Mouse.X < _paperSprite.Position.X - 50)
            {
                _frameOrigin = _rockSprite.Position;
                _playerShootChoice = ShootChoice.Rock;
            }
            else if (Mouse.X > _paperSprite.Position.X + 50)
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
            if (Mouse.LeftButtonPressed())
            {
                Reset();
            }
        }
    }

    private void Reset()
    {
        _outcome = ShootResult.Pending;
        _aiShootChoice = ShootChoice.Undecided;
        _playerShootChoice = ShootChoice.Undecided;
        if(_rps.PlayerWon != null)
            _rps.NewGame();
        _rps.Reset();
        _rps.StartCountDown();
        _isWaiting = false;
        
    
    }

    private void SetUpScene()
    {
        var screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        var offsetFromCenter = new Vector2(_graphics.PreferredBackBufferWidth / _horizontalOffsetDivisor, 0f);
        _playerShootOrigin = screenCenter - offsetFromCenter;
        _aiShootOrigin = screenCenter + offsetFromCenter;
        _outcomeSpriteOrigin = screenCenter - new Vector2(0, _graphics.PreferredBackBufferHeight / 6f);
        _countDownSpriteOrigin = _outcomeSpriteOrigin - new Vector2(0, _graphics.PreferredBackBufferHeight / 8);
        _playerChoiceInputOrigin = screenCenter + new Vector2(0, _graphics.PreferredBackBufferWidth / 6);
        
        var frameTexture = Content.Load<Texture2D>("img/frame");
        _frameSprite = new Sprite(frameTexture);

        var winTexture = Content.Load<Texture2D>("img/win");
        _winSprite = new Sprite(winTexture);
        _winSprite.SetTint(Color.Green);
        _winSprite.SetPosition(_outcomeSpriteOrigin);
        
        var loseTexture = Content.Load<Texture2D>("img/lose");
        _loseSprite = new Sprite(loseTexture);
        _loseSprite.SetTint(_aiColor);
        _loseSprite.SetPosition(_outcomeSpriteOrigin);
        
        var tieTexture = Content.Load<Texture2D>("img/tie");
        _tieSprite = new Sprite(tieTexture);
        _tieSprite.SetTint(Color.Yellow);
        _tieSprite.SetPosition(_outcomeSpriteOrigin);

        var oneTexture = Content.Load<Texture2D>("img/1");
        _oneSprite = new Sprite(oneTexture);
        _oneSprite.SetPosition(_countDownSpriteOrigin);

        var twoTexture = Content.Load<Texture2D>("img/2");
        _twoSprite = new Sprite(twoTexture);
        _twoSprite.SetPosition(_countDownSpriteOrigin);
        
        var threeTexture = Content.Load<Texture2D>("img/3");
        _threeSprite = new Sprite(threeTexture);
        _threeSprite.SetPosition(_countDownSpriteOrigin);
        
        var shootTexture = Content.Load<Texture2D>("img/shoot");
        _shootSprite = new Sprite(shootTexture);
        _shootSprite.SetPosition(_countDownSpriteOrigin);
        
        var readyTexture = Content.Load<Texture2D>("img/ready");
        _readySprite = new Sprite(readyTexture);
        
        _currentCountdownSprite = _readySprite;
        _currentCountdownSprite.SetPosition(_countDownSpriteOrigin);

        _blankTexture = new Texture2D(GraphicsDevice,1,1); // TODO: This is ugly
        _outcomeSprite = new Sprite(_blankTexture); 
        _outcomeSprite.SetPosition(_outcomeSpriteOrigin);
        
        _rockSprite = _rps.GetSprite(ShootChoice.Rock);
        _rockSprite.SetPosition(_playerChoiceInputOrigin - new Vector2(_graphics.PreferredBackBufferHeight / 4, 0));
        
        _paperSprite = _rps.GetSprite(ShootChoice.Paper);
        _paperSprite.SetPosition(_playerChoiceInputOrigin);
        
        _scissorsSprite = _rps.GetSprite(ShootChoice.Scissors);
        _scissorsSprite.SetPosition(_playerChoiceInputOrigin + new Vector2(_graphics.PreferredBackBufferHeight / 4, 0));

        _playerChoice = _rps.ChoiceToSprite(ShootChoice.Undecided);
        _aiChoice = _rps.ChoiceToSprite(ShootChoice.Undecided);

        _playerMatchScoreOrigin = new Vector2(16, 16);
        _aiMatchScoreOrigin = new Vector2(_graphics.PreferredBackBufferWidth - (16 * 12), 16);

    }
    private ShootResult Shoot(ShootChoice playerChoice)
    {
        _playerShootChoice = _rps.PlayerShoot(playerChoice);
        
        var result = _rps.Play(_playerShootChoice, _aiShootChoice);
        return result;
    }

    private void DisplayChoices(ShootChoice playerShoot, ShootChoice aiShoot)
    {
        _playerChoice = _rps.ChoiceToSprite(playerShoot);
        _playerChoice.SetTint(_playerColor);
        _playerChoice.SetPosition( _playerShootOrigin);
        _aiChoice = _rps.ChoiceToSprite(aiShoot);
        _aiChoice.SetTint(_aiColor);
        _aiChoice.SetPosition(_aiShootOrigin);
    }

    private void DisplayInputArea()
    {
        _frameSprite.SetPosition(_frameOrigin);
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
            
             _currentCountdownSprite.SetVisible(false);
        }

    }
    
    private void DisplayMatchScore(int playerScore, int aiScore)
    {
        _playerScoreString = $"Player:{playerScore}";
        _aiScoreString = $"Opponent: {aiScore}";
    }

    private void DisplayGameOver(bool? playerWon)
    {
        if (playerWon == null)
        {
            _matchResultText = "";
        }
        else
        {
            _outcomeSprite = null;
            _currentCountdownSprite.SetVisible(false);

            _matchResultText = playerWon == true ?
                "You won the match!" + "\n" +
                $"You Won {_rps.GetPlayerScore()} / {RpsGameManager.BestOfRounds} Rounds." 
                : "You lost the match. Bummer." + "\n" +
                  $"You Won {_rps.GetPlayerScore()} / {RpsGameManager.BestOfRounds} Rounds.";
        }
        


    }
}