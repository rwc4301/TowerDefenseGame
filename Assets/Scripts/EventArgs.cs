public struct AttackedEventArgs
{
	public IVincible Attacker { get; set; }
	public IVincible Attacked { get; set; }
	public int Damage { get; set; }
	public SpecialProperty[] OnAttackProperties { get; set; }
}

public struct MissionCompleteEventArgs
{

}

public struct OrganInfectedEventArgs
{

}