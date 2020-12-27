public class InterruptedEventArgs
{
	public VincibleObject Interrupter { get; set; }
	public VincibleObject Interruptee { get; set; }
}

public class AttackedEventArgs : InterruptedEventArgs
{
	public int Damage { get; set; }
	public SpecialProperty[] OnAttackProperties { get; set; }
}