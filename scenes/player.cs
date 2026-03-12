using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public int Speed { get; set; } = 14;
	[Export]
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public float MouseSensitivity { get; set; } = 0.003f;

	private Vector3 _targetVelocity = Vector3.Zero;
	private Node3D _pivot;

	public override void _Ready()
	{
		_pivot = GetNode<Node3D>("Pivot");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			_pivot.RotateY(-mouseMotion.Relative.X * MouseSensitivity);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}
		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1.0f;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1.0f;
		}
		
		if (Input.IsActionJustPressed("dash"))
		{
			Speed += 100;
		}
		
		if (Input.IsActionJustReleased("dash"))
		{
			Speed -= 100;
		}

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			direction = _pivot.GlobalTransform.Basis * direction;
		}

		// Ground velocity
		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();
	}
}
