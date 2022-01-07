using Terraria.ModLoader;
using Terraria;

namespace WormRiderBoss.NPCs
{
	public class WormRider : ModNPC
	{
		int frameNum;
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("The Worm Rider");
			//the number of frames of our sprite
			Main.npcFrameCount[npc.type] = 4;
		}
		public override void SetDefaults() {
			//TODO adjust default settings to balance the boss and also so they make sense for our specific boss
			npc.aiStyle = 2;
			npc.lifeMax = 40000;
			npc.damage = 1;
			npc.defense = 0;
			npc.knockBackResist = 0f;
			npc.width = 57;
			npc.height = 57;
			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			
			frameNum = 0;
		}
		//Sets the random natural spawn rate to 0
        public override float SpawnChance(NPCSpawnInfo spawnInfo){
			return 0.0F;
		}

		//Frame animation
		public override void FindFrame(int frameHeight) {
			//animation runs in 1 second
			if (frameNum < 15){
				npc.frame.Y = 0 * frameHeight;
			}else if(frameNum < 30){
				npc.frame.Y = 1 * frameHeight;
			}else if(frameNum < 45){
				npc.frame.Y = 2 * frameHeight;
			}else{
				npc.frame.Y = 3 * frameHeight;
			}
			frameNum = (frameNum + 1) % 60;
		}
	}
}