using MySqlConnector;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static DevExpress.Pdf.Native.BouncyCastle.Asn1.X509.Target;

namespace ClashSim
{
	public static class Db
	{
		public static TargetTypes GetTargetTypesFromString(string str)
		{
			TargetTypes t;
			int idx = str.IndexOf('|');
			if (idx >= 0)
			{
				// there can be 2 values if both air and ground
				string str1 = str.Substring(0, idx);
				string str2 = str.Substring(idx + 1);
				t =  Enum.Parse<TargetTypes>(str1) | Enum.Parse<TargetTypes>(str2);
			}
			else
			{
				t =  Enum.Parse<TargetTypes>(str);
			}
			return t;
		}

		public static Dictionary<string, Card> GetCards(string dbConnStr)
		{
			Dictionary<string, Card> nameAndLevelToCardDict = new Dictionary<string, Card>();

			using MySqlConnection conn = new MySqlConnection(dbConnStr);
			conn.Open();

			using MySqlCommand cmd = conn.CreateCommand();
			cmd.CommandText =
@"
SELECT
c.Name,
c.ElixirCost,
c.CardType,
c.CardRarity,
c.TargetTypes,
c.Size,
c.Mass,
c.HitSpeed,
c.MoveSpeed,
c.DeployTime,
c.MovesInAir,
c.MobCount,
c.ProjectileRange,
c.SpawnCount,
c.PlaceAnywhere,
c.PlaceAnywhereOnSide,
v.Level,
v.Hitpoints,
v.Damage,
v.CrownTowerDamage,
v.AreaDamage,
v.SpawnDamage,
v.DeathDamage,
v.ChargeDamage,
v.ShieldHitpoints,
v.LifeTime,
v.FreezeDuration,
v.SnareDuration,
v.StunDuration,
v.SlowdownDuration,
v.RagePercent
FROM card c
INNER JOIN cardlevel v ON v.Name = c.Name
ORDER BY c.Name, v.Level
";

			using MySqlDataReader rdr = cmd.ExecuteReader();

			while (rdr.Read())
			{
				Card c = new Card();

				int i = 0;

				c.Name = rdr.GetString(i++);
				c.ElixirCost = rdr.GetInt32(i++);
				c.CardType = Enum.Parse<CardType>(rdr.GetString(i++));
				c.CardRarity = Enum.Parse<CardRarity>(rdr.GetString(i++));
				c.TargetTypes = GetTargetTypesFromString(rdr.GetString(i++));
				c.Size = rdr.GetDouble(i++);
				c.Mass = rdr.GetDouble(i++);
				c.HitSpeed = rdr.GetDouble(i++);
				c.MoveSpeed = rdr.GetDouble(i++);
				c.DeployTime = rdr.GetDouble(i++);
				c.MovesInAir = rdr.GetBoolean(i++);
				c.MobCount = rdr.GetInt32(i++);
				c.ProjectileRange = rdr.GetDouble(i++);
				c.SpawnCount = rdr.GetInt32(i++);
				c.PlaceAnywhere = rdr.GetBoolean(i++);
				c.PlaceAnywhereOnSide = rdr.GetBoolean(i++);
				c.Level = rdr.GetInt32(i++);
				c.Hitpoints = rdr.GetInt32(i++);
				c.Damage = rdr.GetInt32(i++);
				c.CrownTowerDamage = rdr.GetInt32(i++);
				c.AreaDamage = rdr.GetInt32(i++);
				c.SpawnDamage = rdr.GetInt32(i++);
				c.DeathDamage = rdr.GetInt32(i++);
				c.ChargeDamage = rdr.GetInt32(i++);
				c.ShieldHitpoints = rdr.GetInt32(i++);
				c.LifeTime = rdr.GetDouble(i++);
				c.FreezeDuration = rdr.GetDouble(i++);
				c.SnareDuration = rdr.GetDouble(i++);
				c.StunDuration = rdr.GetDouble(i++);
				c.SlowdownDuration = rdr.GetDouble(i++);
				c.RagePercent = rdr.GetDouble(i++);

				string key = c.Name + '|' + c.Level.ToString("0");
				nameAndLevelToCardDict[key] = c;
			}

			return nameAndLevelToCardDict;
		}

		public static Dictionary<int, PlayerLevel> GetPlayerLevelInfo(string dbConnStr)
		{
			Dictionary<int, PlayerLevel> playerLevelToInfoDict = new Dictionary<int, PlayerLevel>();

			using MySqlConnection conn = new MySqlConnection(dbConnStr);
			conn.Open();

			using MySqlCommand cmd = conn.CreateCommand();
			cmd.CommandText =
@"
SELECT
p.Level,
p.KingTowerHitpoints,
p.PrincessTowerHitpoints,
p.KingTowerDamage,
p.PrincessTowerDamage
FROM PlayerLevel p
order by p.Level
";
			using MySqlDataReader rdr = cmd.ExecuteReader();

			if (rdr.Read())
			{
				int i = 0;

				string key;

				PlayerLevel p = new PlayerLevel();

				p.Level = rdr.GetInt32(i++);

				p.KingTowerHitpoints = rdr.GetInt32(i++);
				p.PrincessTowerHitpoints = rdr.GetInt32(i++);
				p.KingTowerDamage = rdr.GetInt32(i++);
				p.PrincessTowerDamage = rdr.GetInt32(i++);

				playerLevelToInfoDict.Add(p.Level, p);
			}

			return playerLevelToInfoDict;
		}

		public static Deck GetDeck(string dbConnStr, string deckName, Dictionary<string, Card> nameAndLevelToCardDict)
		{
			Deck deck = null;

			using MySqlConnection conn = new MySqlConnection(dbConnStr);
			conn.Open();

			using MySqlCommand cmd = conn.CreateCommand();
			cmd.CommandText =
@"
SELECT
d.CardName1,
d.CardLevel1,
d.CardName2,
d.CardLevel2,
d.CardName3,
d.CardLevel3,
d.CardName4,
d.CardLevel4,
d.CardName5,
d.CardLevel5,
d.CardName6,
d.CardLevel6,
d.CardName7,
d.CardLevel7,
d.CardName8,
d.CardLevel8
FROM deck d
where d.DeckName = '" + deckName + "'";
;

			using MySqlDataReader rdr = cmd.ExecuteReader();

			if (rdr.Read())
			{
				int i = 0;
				int n = 0;

				string key;

				deck = new Deck();

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];

				key = rdr.GetString(i++) + '|';
				key += rdr.GetInt32(i++).ToString("0");

				deck.Cards[n++] = nameAndLevelToCardDict[key];
			}

			return deck;
		}
	}
}
