using Godot;
using System;
using System.Diagnostics;

public partial class Player : Node2D
{
	private Vector2 vel;
	[Export]
	public float Gravity, JumpStrength, RotationSpeed, 
		VisualRotation, AmbientRotation;
	private float rotation;
	private float lastFrameDeltaTime;
	private bool lefting, righting;

	public Control GameOverText, StartText;
	public GameOverInstructions GameOverInstructions;
	public RichTextLabel HealthLabel;

	public int Score, HighScore;
	public int Health;

	public enum State {
		HANGING,
		PLAYING,
		CIRCLING,
		GAME_OVER
	}

	public State MyState;

	[Export]
	public Coin CoinNode;
	[Export]
	public Evil EvilNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Window root = GetTree().Root;
		GameOverText = root
			.GetNode<Control>("Base/UI/Control/Game Over");
		StartText = root
			.GetNode<Control>("Base/UI/Control/Start");
		GameOverInstructions = GameOverText
			.GetNode<GameOverInstructions>("MarginContainer2/Instructions");
		HealthLabel = root
			.GetNode<RichTextLabel>("Base/UI/Control/Health");
		StartText.Show();
		HighScore = 0;
		Reset();
	}

	public void Reset() {
		Position = Vector2.Zero;
		Rotation = 0;
		vel = Vector2.Zero;
		rotation = 0f;
		lefting = false;
		righting = false;
		MyState = State.HANGING;
		GameOverText.Hide();
		Score = 0;
		CoinNode.UpdateScore();
		EvilNode.Hide();
		Health = 3;
		UpdateHealthText();
		EvilNode.Speed = EvilNode.StartSpeed;
	}

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("up")) {
			if (MyState == State.PLAYING)
				vel = Vector2.Up * JumpStrength 
					+ Vector2.Right * rotation;
			else if (MyState == State.HANGING)
			{
				MyState = State.PLAYING;
				StartText.Hide();
				vel = Vector2.Up * JumpStrength;
				CoinNode.Begin();
				EvilNode.Show();
				EvilNode.RandomizePosition();
			}
			else if (MyState == State.GAME_OVER)
				Reset();
		}

		lefting |= @event.IsActionPressed("left");
		lefting &= !@event.IsActionReleased("left");
		righting |= @event.IsActionPressed("right");
		righting &= !@event.IsActionReleased("right");
    }

	private void RotateMe(float amount) {
		rotation += amount * lastFrameDeltaTime;
		rotation = Mathf.Clamp(rotation, 
			-90f / VisualRotation, 
			90f / VisualRotation);
		RotationDegrees = rotation * VisualRotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch (MyState) {
			case State.HANGING: break;

			case State.PLAYING:
				vel += Vector2.Down * Gravity * (float)delta;
				Position += (vel + Vector2.Right * (rotation * AmbientRotation))
					* (float)delta;
				lastFrameDeltaTime = (float)delta;

				if (lefting)
					RotateMe(-RotationSpeed);
				if (righting)
					RotateMe(RotationSpeed);

				WrapAround();
				CheckDeath();
			break;

			case State.CIRCLING:
				vel += Vector2.Down * Gravity * (float)delta;
				Position += vel * (float)delta;
				Rotate(Mathf.DegToRad(RotationSpeed) * (float)delta);
				CheckDeath();
			break;

			case State.GAME_OVER: break;
		}
	}

    private void WrapAround()
    {
		if (Position.X < -638)
			Position += Vector2.Right * 1276;
		if (Position.X > 638)
			Position += Vector2.Left * 1276;
		if (Position.Y < -384)
			Position += Vector2.Down * 768;
    }

    private void CheckDeath()
    {
		if (Position.Y > 384) {
			MyState = State.GAME_OVER;
			GameOverText.Show();
			if (Score > HighScore)
				HighScore = Score;
			GameOverInstructions.UpdateText(Score, HighScore);
			CoinNode.End();
		}
    }

	private void BeginCircling() {
		vel = Vector2.Up * JumpStrength * 1.2f;
		MyState = State.CIRCLING;
	}

	public void GetHit() {
		Health--;
		if (Health < 1) {
			BeginCircling();
		}
		UpdateHealthText();
	}

	public void UpdateHealthText() {
		HealthLabel.Text = "[right]";
		for (int i = 0; i < Health; i++) {
			HealthLabel.Text += "♥";
		}
		HealthLabel.Text += "[/right]";
	}
}
