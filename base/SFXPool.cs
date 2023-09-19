using Godot;
using System;
using System.Collections.Generic;

public partial class SFXPool : Node2D
{
	private List<AudioStreamPlayer2D> players;
	private int queueIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		players = new List<AudioStreamPlayer2D>(10);
		for (int i = 0; i < 10; i++)
		{
			players.Add(new AudioStreamPlayer2D());
			AddChild(players[i]);
		}
	}

	public void PlaySound(AudioStream stream) {
		for (int i = 0; i < players.Count; i++) {
			if (!players[i].Playing) {
				players[i].Stream = stream;
				players[i].Play();
				return;
			}
		}
		queueIndex = players.Count;
		AudioStreamPlayer2D newPlayer = new AudioStreamPlayer2D();
		AddChild(newPlayer);
		newPlayer.Stream = stream;
		newPlayer.Play();
	}
}
