using Terraria.ModLoader;

namespace WormRiderBoss.NPCs
{
	public class WormRider : ModNPC
	{
		
		public override void SetDefaults() {
			npc.aiStyle = 2;
			npc.lifeMax = 40000;
			npc.damage = 100;
			npc.defense = 55;
			npc.knockBackResist = 0f;
			npc.width = 100;
			npc.height = 100;
			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
		}
		//Sets the random natural spawn rate to 0
        public override float SpawnChance(NPCSpawnInfo spawnInfo){
			return 0.0F;
		}
	}
}