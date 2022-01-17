using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria.Localization;

namespace WormRiderBoss.NPCs{
    public class SkyWorm : ModNPC{
        //Grab the dune splicer sprites
        public override string Texture => "Terraria/NPC_" + NPCID.DuneSplicerHead;

        //Framecount needed for animation still
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.DuneSplicerHead];
		}

		public override void SetDefaults()
		{
			//Get defaults form dune splicer
            npc.CloneDefaults(NPCID.DuneSplicerHead);
			aiType = NPCID.DuneSplicerHead;
			animationType = NPCID.DuneSplicerHead;

            //now we can override/double check some aspects are what we want instead of just a regular splicer
            npc.noGravity = false;
			npc.friendly = false;
            //TODO adjust
            npc.damage = 29;
		}

        public override void AI(){
            //we can just kill them when they get significantly lower than the player
            //because they have gravity on unlike normal worms
            if(npc.position.Y > Main.player[npc.target].Center.Y + 100){
                npc.life = 0;
            }
            base.AI();
        }

        //Sets the random natural spawn rate to 0
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return 0.0F;
		}
    }
}