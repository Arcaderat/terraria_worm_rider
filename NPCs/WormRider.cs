using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria.Localization;
using WormRiderBoss.Projectiles;

namespace WormRiderBoss.NPCs
{
	//loads the head icon for the minimap
	[AutoloadBossHead]
	public class WormRider : ModNPC
	{
		internal int attackProgress
		{
			get => (int)npc.ai[3];
			private set => npc.ai[3] = value;
		}

		private float attackTimer
		{
			get => npc.ai[1];
			set => npc.ai[1] = value;
		}

		internal int attack
		{
			get => (int)npc.ai[2];
			private set => npc.ai[2] = value;
		}

		private int difficulty
		{
			get
			{
				double strength = (double)npc.life / (double)npc.lifeMax;
				int difficulty = (int)(4.0 * (1.0 - strength));
				if (Main.expertMode)
				{
					difficulty++;
				}
				return difficulty;
			}
		}

		private float difficultyGradient
		{
			get
			{
				double strength = (double)npc.life / (double)npc.lifeMax;
				double difficulty = 4.0 * (1.0 - strength);
				return (float)(difficulty % 1.0);
			}
		}

		private float timeMultiplier => 1f - (difficulty + difficultyGradient) * 0.2f;

		public int[] attackWeights = new[] { 2000, 2000, 2000, 2000, 3000 };
		public const int minAttackWeight = 1000;
		public const int maxAttackWeight = 4000;


		private int stage
		{
			get => (int)npc.ai[0];
			set => npc.ai[0] = value;
		}


		int frameNum;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("The Worm Rider");
			//the number of frames of our sprite
			Main.npcFrameCount[npc.type] = 4;
		}
		public override void SetDefaults() {
			//TODO adjust default settings to balance the boss and also so they make sense for our specific boss
			npc.aiStyle = 0;
			npc.lifeMax = 40000;
			npc.damage = 1;
			npc.defense = 0;
			npc.knockBackResist = 0f;
			npc.width = 70;
			npc.height = 104;
			npc.boss = true;
			npc.noGravity = false;
			npc.noTileCollide = false;
			//prevents the game from despawning the WormRider if you get too far away from it
			npc.boss = true;

			frameNum = 0;
		}
		//Sets the random natural spawn rate to 0
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return 0.0F;
		}

		//Frame animation
		public override void FindFrame(int frameHeight) {
			//animation runs in 1 second
			if (frameNum < 15) {
				npc.frame.Y = 0 * frameHeight;
			} else if (frameNum < 30) {
				npc.frame.Y = 1 * frameHeight;
			} else if (frameNum < 45) {
				npc.frame.Y = 2 * frameHeight;
			} else {
				npc.frame.Y = 3 * frameHeight;
			}
			frameNum = (frameNum + 1) % 60;
		}

		private void Hookem()
		{
			if (attackProgress == 0)
			{
				//int damage = Main.expertMode ? 60 : 80;
				int damage = 10;
				Projectile.NewProjectile(npc.Center.X + 70, npc.Center.Y - 20, 0f, 0f, ModContent.ProjectileType<Projectiles.HookRight>(), damage, 0f, Main.myPlayer, npc.whoAmI, 0f);
				Projectile.NewProjectile(npc.Center.X - 80, npc.Center.Y - 20, 0f, 0f, ModContent.ProjectileType<Projectiles.HookLeft>(), damage, 0f, Main.myPlayer, npc.whoAmI, 0f);
				attackProgress = 120;
			}
			attackProgress--;
			if (attackProgress < 0)
			{
				attackProgress = 0;	
			}
		}
		/*
		private void DoAttack(int numAttacks)
		{
			if (attackTimer > 0f)
			{
				attackTimer -= 1f;
				return;
			}
			if (attack < 0)
			{
				int totalWeight = 0;
				for (int k = 0; k < numAttacks; k++)
				{
					if (attackWeights[k] < minAttackWeight)
					{
						attackWeights[k] = minAttackWeight;
					}
					totalWeight += attackWeights[k];
				}
				int choice = Main.rand.Next(totalWeight);
				for (attack = 0; attack < numAttacks; attack++)
				{
					if (choice < attackWeights[attack])
					{
						break;
					}
					choice -= attackWeights[attack];
				}
				attackWeights[attack] -= 80;
				npc.netUpdate = true;
			}
			switch (attack)
			{
				case 0:
					SnakeAttack();
					break;
				case 1:
					SnakeAttack();
					break;
				case 2:
					SnakeAttack();
					break;
				case 3:
					SnakeAttack();
					break;
				case 4:
					SnakeAttack();
					break;
			}
			if (attackProgress == 0)
			{
				attackTimer += 160f * timeMultiplier;
				attack = -1;
			}
		}
		*/
		private void DoAttack(int numAttacks)
		{
			Hookem();
			if (attackProgress == 0)
			{
				attackTimer += 160f * timeMultiplier;
			}
		}




		public override void AI()
		{
			
			npc.timeLeft = NPC.activeTime;
			DoAttack(5);
		}

		public override Boolean PreNPCLoot()
		{
			mod.Logger.Info("it died");
			return true;
		}




	}
}