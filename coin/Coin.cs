using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Coin : Node2D
{
	[Export]
	public Player PlayerNode;
	[Export]
	public float MinDistanceOnAppear;
	[Export]
	public RigidBody2D Rb;
	private RandomNumberGenerator rng;
	public RichTextLabel ScoreLabel;
	[Export]
	private SFXPool sfxPool;
	[Export]
	private AudioStream sfx;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Rb.BodyEntered += (other) => {
			if (other.GetParent() == PlayerNode) {
				Randomize();
				PlayerNode.Score++;
				UpdateScore();
				sfxPool.PlaySound(sfx);
			}
		};
		End();
		rng = new();
		ScoreLabel = GetTree().Root.GetNode<RichTextLabel>("Base/UI/Control/Score");
		Randomize();
	}

	public void Begin() {
		Randomize();
		Show();
		Rb.ContactMonitor = true;
		UpdateScore();
	}

	public void End() {
		Hide();
		Rb.ContactMonitor = false;
	}

	public void Randomize() {
		Vector2 playerPos = PlayerNode.Position;
		float distToPlayer() => playerPos.DistanceTo(Position);
		do {
			Position = new Vector2(rng.RandfRange(-538, 538),
				rng.RandfRange(-284, 284));
		} while (distToPlayer() < MinDistanceOnAppear);
	}

	public void UpdateScore() {
		if (ScoreLabel != null)
			ScoreLabel.Text = $"Score: {PlayerNode.Score}";
	}
}
