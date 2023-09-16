using Godot;
using System;

public partial class GameOverInstructions : RichTextLabel
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void UpdateText(int score, int hiScore)
	{
		Text = $@"[center]Press Space to Continue
Score: {score}
High Score: {hiScore}[/center]";
	}
}
