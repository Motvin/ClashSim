using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashSim;

public enum CardType
{
	Troop,
	Building,
	Spell
}

public enum CardRarity
{
	Common,
	Epic,
	Rare,
	Legendary
}

[Flags]
public enum TargetTypes
{
	Air = 0b0000_0001,
	Ground = 0b0000_0010,
	Buildings = 0b0000_0100,
}

public struct FieldLocation
{
	public int X { get; set; }
	public int Y { get; set; }
}

public struct MoveProgress
{
	public double X { get; set; }
	public double Y { get; set; }
}

public class Deck
{
	public string DeckName { get; set; }

	public Card[] Cards { get; set; } = new Card[8];
}

public class Card
{
	//??? values are not correct
	const double Speed_VeryFast = 5.2;
	const double Speed_Fast = 5.2;
	const double Speed_Medium = 5.2;
	const double Speed_Slow = 5.2;

	const double Range_Long = 5.2;
	const double Range_Short = 5.2;

	public string Name { get; set; }
	public int Level { get; set; }
	public int ElixirCost { get; set; }
	public int MobCount { get; set; }
	public CardType CardType { get; set; }
	public CardRarity CardRarity { get; set; }
	public TargetTypes TargetTypes { get; set; }

	public int Damage { get; set; }
	public int CrownTowerDamage { get; set; }
	public int AreaDamage { get; set; }
	public int SpawnDamage { get; set; }
	public int DeathDamage { get; set; }
	public int ChargeDamage { get; set; }

	public int SpawnCount { get; set; } // for witch and tombstone and others???

	public int Hitpoints { get; set; }
	public int ShieldHitpoints { get; set; } // 0 means no shield

	public double Size { get; set; }
	public double Mass { get; set; }

	public double HitSpeed { get; set; }
	public double MoveSpeed { get; set; }
	public double ProjectileRange { get; set; } // projectile range, 0 means no ranged attack

	public double DeployTime { get; set; }
	public double LifeTime { get; set; } // the time until they are destroyed/changed into something else (Royal Delivery)

	//??? some of the below might be the same
	public double FreezeDuration { get; set; }
	public double SnareDuration { get; set; }
	public double StunDuration { get; set; }
	public double SlowdownDuration { get; set; }

	public double RagePercent { get; set; }

	public bool MovesInAir { get; set; }

	public bool PlaceAnywhere { get; set; }//??? all spells, miner, etc. can placed almost anywhere except for on buildings - probably need a different setting for miner, goblin barrel, etc.

	public bool PlaceAnywhereOnSide { get; set; } // log, barrels, etc., can be placed in the odd corners where other mobs can't

	public static string GetCardImageFileName(string imagePath, string cardName)
	{
		string path = System.IO.Path.Combine(imagePath, cardName + ".png");
		return path;
	}
}

public interface ITarget
{
	FieldLocation Location { get; set; }

	int HitpointsRemaining { get; set; }
}

public class Mob : ITarget
{
	public FieldLocation Location { get; set; }

	public int HitpointsRemaining { get; set; }

	public Card Card { get; set; }

	public double FaceDirection { get; set; }

	public int ShieldHitpointsRemaining { get; set; } // 0 means no shield or shield is broken

	public double DeployTimeRemaining { get; set; }
	public double LifeTimeRemaining { get; set; }

	//??? I think these can be different, combine if not
	public ITarget AttackTarget { get; set; } // can be null if not targeting anything currently
	public ITarget MoveTarget { get; set; }

	public MoveProgress MoveProgress { get; set; }
	public double HitProgress { get; set; }
}

public class Tower : ITarget
{
	public FieldLocation Location { get; set; }

	public int HitpointsRemaining { get; set; }

	public int Level { get; set; }

	public bool IsKingTower { get; set; }

	public int MaxHitpoints { get; set; }

	public double Range { get; set; } // projectile range
	public int Damage { get; set; }

	public ITarget AttackTarget { get; set; } // can be null if not targeting anything currently
}

public class Projectile
{
	public FieldLocation Location { get; set; }

	public MoveProgress MoveProgress { get; set; }

	public ITarget FromTarget { get; set; }
}

public class Player
{
	public string PlayerName { get; set; }
	public bool IsTop { get; set; }
	public int PlayerLevel { get; set; } // this determines the hitpoints of towers

	public Deck Deck { get; set; }

	public Card[] CardsInOrder { get; set; }

	public int NextCardIdx { get; set; }

	public int Elixir {get; set; }

	public int KingTowerHitpoints {get; set; }
	public int LeftTowerHitpoints {get; set; }
	public int RightTowerHitpoints {get; set; }

	public int KingTowerDamage {get; set; }
	public int PrincessTowerDamage {get; set; }

	public List<Mob> MobList {get; set; }
}

public class PlayerLevel
{
	public int Level { get; set; }
	public int KingTowerHitpoints { get; set; }
	public int PrincessTowerHitpoints { get; set; }
	public int KingTowerDamage { get; set; }
	public int PrincessTowerDamage { get; set; }
}
