using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class Game1 : Game
{
    // Design Levers
    private readonly float _horizontalOffsetDivisor = 5f;
    private readonly Vector2 _shootChoiceSpriteScale = Vector2.One * 4;
    private Color _playerColor = new Color(77,155,230);
    private Color _aiColor = Color.OrangeRed;
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private readonly RpsGameManager _rps = new RpsGameManager();
    
    private Vector2 _playerShootOrigin;
    private Vector2 _aiShootOrigin;
    private Vector2 _outcomeSpriteOrigin;

    private Sprite _playerChoice;
    private Sprite _aiChoice;
    private ShootResult _outcome;
    private Sprite _outcomeSprite;
    
    private Sprite _winSprite;
    private Sprite _loseSprite;
    private Sprite _tieSprite;
    private Sprite _undecidedSprite;


    #region Game Loop
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rps.CreateSprites(Content);
        SetUpScene();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        HandleInput();
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

            _outcome = ShootResult.Pending;
        }
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        if(_playerChoice != null)
            _playerChoice.Draw(_spriteBatch, _playerShootOrigin, _shootChoiceSpriteScale);
        if(_playerChoice != null)
            _aiChoice.Draw(_spriteBatch, _aiShootOrigin, _shootChoiceSpriteScale);
        if(_outcomeSprite != null)
            _outcomeSprite.Draw(_spriteBatch, _outcomeSpriteOrigin, _shootChoiceSpriteScale);
        _spriteBatch.End();
    }
    #endregion
    
    private void HandleInput()
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.Space))
            _outcome = Shoot();
    }
    
    private void SetUpScene()
    {
        var screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        var offsetFromCenter = new Vector2(_graphics.PreferredBackBufferWidth / _horizontalOffsetDivisor, 0f);
        _playerShootOrigin = screenCenter - offsetFromCenter;
        _aiShootOrigin = screenCenter + offsetFromCenter;
        _outcomeSpriteOrigin = screenCenter + new Vector2(0, _graphics.PreferredBackBufferHeight / 5);
        
        var winTexture = Content.Load<Texture2D>("img/win");
        var loseTexture = Content.Load<Texture2D>("img/lose");
        var tieTexture = Content.Load<Texture2D>("img/tie");

        _winSprite = new Sprite(winTexture);
        _winSprite.SetTint(Color.Green);
        _loseSprite = new Sprite(loseTexture);
        _loseSprite.SetTint(Color.Red);
        _tieSprite = new Sprite(tieTexture);
        _tieSprite.SetTint(Color.Yellow);

        var questionTexture = Content.Load<Texture2D>("img/question");
        _undecidedSprite = new Sprite(questionTexture);
    }
    private ShootResult Shoot()
    {
        var playerShoot = _rps.PlayerShoot();
        var aiShoot = _rps.AiShoot();
        DisplayChoices(playerShoot, aiShoot);
        
        return _rps.Play(playerShoot, aiShoot);
    }

    private void DisplayChoices(ShootChoice playerShoot, ShootChoice aiShoot)
    {
        _playerChoice = _rps.ChoiceToSprite(playerShoot);
        _playerChoice.SetTint(_playerColor);
        _aiChoice = _rps.ChoiceToSprite(aiShoot);
        _aiChoice.SetTint(_aiColor);
    }
}