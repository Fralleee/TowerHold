using System;

public static class NameGeneration
{
	static readonly string[] Adjectives = {
		"Ancient", "Mystic", "Dark", "Golden", "Hidden", "Lost",
		"Forbidden", "Enchanted", "Serene", "Whispering"
	};

	static readonly string[] Biomes = {
		"Forest", "Lands", "Mountains", "Fields", "Valleys"
	};

	static readonly string[] FantasyNames = {
		"Valor", "Eternia", "Arcadia", "Mystara", "Avalon",
		"Eldoria", "Narnia", "Midgard", "Asgard", "Olympus",
		"Atlantis", "Camelot", "Zion", "Avalonia", "Thule",
		"Elveron", "Hyrule", "Celestia", "Drakonia", "Eden",
		"Lumina", "Solaris"
	};

	public static string GenerateLevelName(int seed)
	{
		var random = new Random(seed);

		var adjective = Adjectives[random.Next(Adjectives.Length)];
		var biome = Biomes[random.Next(Biomes.Length)];
		var fantasyName = FantasyNames[random.Next(FantasyNames.Length)];

		return $"{adjective} {biome} of {fantasyName}";
	}
}
