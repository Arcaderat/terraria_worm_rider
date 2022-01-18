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
    public class WormCompanion : ModNPC{
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
			aiType = NPCID.Worm;
			animationType = NPCID.DuneSplicerHead;

            //now we can override/double check some aspects are what we want instead of just a regular splicer
			npc.friendly = false;
            npc.lifeMax = 10000;
            npc.scale = 2f;
            npc.boss = true;

		}

        //Seems to allow the dune splicer to spawn at least
        public override bool CheckActive()
        {
            return false;
        }

        public override void AI(){
            //To be completely honest, I'm not certain how most of this works, it's all copied from the source
			npc.ai[3] = (float)npc.whoAmI;
			npc.realLife = npc.whoAmI;
			int num18 = npc.whoAmI;
            //This, however, is the number that dictates the number of body segments
			int num19 = 50;
			for (int num20 = 0; num20 < num19; num20++)
			{
				int num21 = 511;
				if (num20 == num19 - 1)
				{
					num21 = 512;
				}
				int num22 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num21, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[num22].ai[3] = (float)npc.whoAmI;
				Main.npc[num22].realLife = npc.whoAmI;
				Main.npc[num22].ai[1] = (float)num18;
                //We can add more Main.npc[num22] items here to edit the body parts' stats
                Main.npc[num22].scale = 2;
                Main.npc[num22].damage = 70;
				CopyInteractions(Main.npc[num22], npc);
				Main.npc[num18].ai[0] = (float)num22;
				NetMessage.SendData(23, -1, -1, null, num22, 0f, 0f, 0f, 0, 0, 0);
				num18 = num22;
			}


            //TODO override with whatever AI we want to give it
            base.AI();
        }
        
        //Stolen from source needed to get body parts working with head
        public void CopyInteractions(NPC npc1, NPC npc2)
		{
			for (int i = 0; i < npc1.playerInteraction.Length; i++)
			{
				npc1.playerInteraction[i] = npc2.playerInteraction[i];
			}
			npc1.lastInteraction = npc2.lastInteraction;
		}

        //Sets the random natural spawn rate to 0
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return 0.0F;
		}
    }
}