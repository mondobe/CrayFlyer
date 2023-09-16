using Godot;
using System;

public partial class Evil : Node2D
{
	private RandomNumberGenerator rng;
	[Export]
	public float StartSpeed;
	public float Speed;
	[Export]
	public RigidBody2D Rb;
	[Export]
	public Player PlayerNode;

    public override void _Ready()
    {
        rng = new RandomNumberGenerator();
		Rb.BodyEntered += (other) => {
			if (other.GetParent() == PlayerNode
				&& PlayerNode.MyState == Player.State.PLAYING)
			{
				PlayerNode.GetHit();
			}
		};
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Translate(Vector2.Down * Speed * (float) delta);
		if (Position.Y > 404)
			RandomizePosition();
		Speed += rng.RandfRange(-0.1f, 0.2f);
	}

	public void RandomizePosition() =>
		Position = new Vector2(rng.RandfRange(-558, 558), -404);
}
