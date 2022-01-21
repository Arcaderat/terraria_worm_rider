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
		private int attackProgress;
		private int summoningTimer;

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
			attackProgress = 0;
			npc.aiStyle = 3;
			npc.lifeMax = 40000;
			npc.damage = 1;
			npc.defense = 0;
			npc.knockBackResist = 0f;
			npc.width = 70;
			npc.height = 104;
			npc.boss = true;
			npc.noGravity = false;
			npc.friendly = false;
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

//Worm Rider Attacks
		private void Hookem()
		{
			if (attackProgress == 0)
			{
				attackProgress = 10;
				//int damage = Main.expertMode ? 60 : 80;
				int damage = 10;
				Projectile.NewProjectile(npc.Center.X + 70, npc.Center.Y - 20, npc.velocity.X, npc.velocity.Y, ModContent.ProjectileType<Projectiles.HookRight>(), damage, 0f, Main.myPlayer, npc.whoAmI, 0f);
				Projectile.NewProjectile(npc.Center.X - 80, npc.Center.Y - 20, npc.velocity.X, npc.velocity.Y, ModContent.ProjectileType<Projectiles.HookLeft>(), damage, 0f, Main.myPlayer, npc.whoAmI, 0f);
				attackProgress = 120;
			}
			attackProgress--;
			if (attackProgress < 0)
			{
				attackProgress = 0;
			}
		}

		private void WormWall() {
			if (attackProgress == 0)
			{
				attackProgress = 16;
				if (NPC.CountNPCS(510) < 12)
				{

					NPC.NewNPC((int)npc.Center.X + 300, (int)Main.player[npc.target].Center.Y - 500, ModContent.NPCType<SkyWorm>());
					NPC.NewNPC((int)npc.Center.X - 300, (int)Main.player[npc.target].Center.Y - 500, ModContent.NPCType<SkyWorm>());
					NPC.NewNPC((int)npc.Center.X + 600, (int)Main.player[npc.target].Center.Y - 500, ModContent.NPCType<SkyWorm>());
					NPC.NewNPC((int)npc.Center.X - 600, (int)Main.player[npc.target].Center.Y - 500, ModContent.NPCType<SkyWorm>());
					NPC.NewNPC((int)npc.Center.X + 900, (int)Main.player[npc.target].Center.Y - 500, ModContent.NPCType<SkyWorm>());
					NPC.NewNPC((int)npc.Center.X - 900, (int)Main.player[npc.target].Center.Y - 500, ModContent.NPCType<SkyWorm>());

				}
			}
			attackProgress--;
			if (attackProgress < 0)
			{
				attackProgress = 0;
			}
		}

		private void WormSpit(){
			if (attackProgress == 0)
			{
				attackProgress = 50;
				Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, -10, ProjectileID.DD2BetsyFireball, 20, 20);
			} 
			attackProgress--;
			if (attackProgress < 0){
				attackProgress = 0;
			}
		}
		private void SummonCompanion(){
			//TODO make stand still for a bit when it calls
			//check attack progress has reset and there is no companion already here
			if (NPC.CountNPCS(ModContent.NPCType<WormCompanion>()) == 0){
				attackProgress = 150;
				//spawn the NPC using SpawnOnPlayer so it's not consistent where it comes from
				NPC.SpawnOnPlayer(Main.player[npc.target].whoAmI, ModContent.NPCType<WormCompanion>());
			}

			attackProgress--;
			if (attackProgress < 0)
			{
				attackProgress = 0;
			}
		}
		
		private void DoAttack(int numAttacks)
		{
			int choice = Main.rand.Next(numAttacks);
			//if companion already summoned, choose a different attack
			if (NPC.CountNPCS(ModContent.NPCType<WormCompanion>()) != 0){
				while (choice == 2){
					choice = Main.rand.Next(numAttacks);
				}
			}
			switch (choice)
            {
				case 0:
					Hookem();
					break;
				case 1:
					WormWall();
					break;
				case 2:
					if(attackProgress == 0){
						//set the summon timer that will summon when done
						summoningTimer = 200;
					}
					break;
				case 3:
					WormSpit();
					break;
			}
			if (attackProgress == 0)
			{
				attackTimer += 160f * timeMultiplier;
			}
		}



//Worm Rider AI
		public override void AI()
		{
			//if we're summoning, do nothing
			if (summoningTimer > 0){
				if (summoningTimer == 1){
					SummonCompanion();
				}
				npc.velocity = new Vector2(0f, 0f);
				summoningTimer--;
				return;
			}
			//Attack
			DoAttack(4);
			//Targeting
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
			//NPC Rotation
			npc.rotation = 0.0f;
			npc.netAlways = true;
			npc.TargetClosest(true);
			//Ensures NPC Life is not greater than its max
			if (npc.life >= npc.lifeMax)
				npc.life = npc.lifeMax;
			//Handles Despawning ~ doesn't appear to work
			if(npc.target < 0 || npc.target == 255 || player.dead || !player.active){
				npc.TargetClosest(false);
				if(npc.timeLeft > 20){
					npc.timeLeft = 20;
					return;
				}
			}
			//Movement
			int distance = (int)Vector2.Distance(target, npc.Center);
			MoveTowards(npc, target, 5f, 30f);
			npc.netUpdate = true;
			Jump(npc);
			//Idk what this does
			npc.timeLeft = NPC.activeTime;
		}

		public override Boolean PreNPCLoot()
		{
			mod.Logger.Info("it died");
			return true;
		}

//Worm Rider Movement Functions

		private void Jump(NPC npc){
			if(npc.velocity.Y == 0)
			{
			  npc.ai[0] = 1;
			}
			if(npc.ai[0] > 0)
			{
			  npc.velocity.Y -= 8f;
			  npc.ai[0]--;
			}
		}
		private void MoveTowards(NPC npc, Vector2 playerTarget, float speed, float turnResistance){
			var move = playerTarget - npc.Center;
			float length = move.Length();
			if(length > speed){
				move *= speed / length;
			}
			move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
			length = move.Length();
			if(length > speed)
			{
				move *= speed / length;
			}
			npc.velocity.X = move.X;
		}
		private void MoveAway(NPC npc, Vector2 playerTarget, float speed, float turnResistance){
			var move = -(playerTarget - npc.Center);
			float length = move.Length();
			if(length > speed){
				move *= speed / length;
			}
			move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
			length = move.Length();
			if(length > speed)
			{
				move *= speed / length;
			}
			npc.velocity.X = move.X;
		}
	}
}