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

		//The last action we took and the player's and own health at the time
		private int lastAction;
		private int lastOwnHealth;
		private int lastPlayerHealth;
		private int lastAngleDiv;
		private int lastDistance;

		//Our qTable for learning
		private Dictionary<int, Dictionary<int, double[][][]>> qTable;


		//returns int signifying which move/attack it should do
		private int getAttack(Player target){
			//Otherwise, find the best option
			Vector2 playerPos = target.position;
			Vector2 selfPos = npc.position;

			Vector2 betweenVec = playerPos - selfPos;

			//Calculate the directon of the between vector
			Double angleToPlayer = Math.Atan2(betweenVec.Y, betweenVec.X);
			
			//Which of our "cardinal directions" we're using
			int angleDiv = -1;

			//find the correct angleDiv for the true angle
			if(-Math.PI <= angleToPlayer && angleToPlayer < (-11 * Math.PI)/12){
				angleDiv = 180;
			}else if((-11 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-10 * Math.PI)/12){
				angleDiv = 195;
			}else if((-10 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-9 * Math.PI)/12){
				angleDiv = 210;
			}else if((-9 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-8 * Math.PI)/12){
				angleDiv = 225;
			}else if((-8 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-7 * Math.PI)/12){
				angleDiv = 240;
			}else if((-7 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-6 * Math.PI)/12){
				angleDiv = 255;
			}else if((-6 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-5 * Math.PI)/12){
				angleDiv = 270;
			}else if((-5 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-4 * Math.PI)/12){
				angleDiv = 285;
			}else if((-4 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-3 * Math.PI)/12){
				angleDiv = 300;
			}else if((-3 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-2 * Math.PI)/12){
				angleDiv = 315;
			}else if((-2 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (-1 * Math.PI)/12){
				angleDiv = 330;
			}else if((-1 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (0 * Math.PI)/12){
				angleDiv = 345;
			}else if((0 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (1 * Math.PI)/12){
				angleDiv = 0;
			}else if((1* Math.PI)/12 <= angleToPlayer && angleToPlayer < (2 * Math.PI)/12){
				angleDiv = 15;
			}else if((2 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (3 * Math.PI)/12){
				angleDiv = 30;
			}else if((3 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (4 * Math.PI)/12){
				angleDiv = 45;
			}else if((4 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (5 * Math.PI)/12){
				angleDiv = 60;
			}else if((5 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (6 * Math.PI)/12){
				angleDiv = 75;
			}else if((6 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (7 * Math.PI)/12){
				angleDiv = 90;
			}else if((7 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (8 * Math.PI)/12){
				angleDiv = 105;
			}else if((8 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (9 * Math.PI)/12){
				angleDiv = 120;
			}else if((9 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (10 * Math.PI)/12){
				angleDiv = 135;
			}else if((10 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (11 * Math.PI)/12){
				angleDiv = 150;
			}else if((1 * Math.PI)/12 <= angleToPlayer && angleToPlayer < (12 * Math.PI)/12){
				angleDiv = 165;
			}

			lastAngleDiv = angleDiv;

			double exactDistance = Math.Sqrt((betweenVec.X * betweenVec.X) * (betweenVec.Y * betweenVec.Y));
			int roundedDistance = (int) Math.Round(exactDistance / 5) * 5;

			lastDistance = roundedDistance;

			double ownHealthPercent = npc.life / npc.lifeMax;
			//statLifeMax2 accounts for item bonuses
			double playerHealthPercent = target.statLife / target.statLifeMax2;
			int ownHealth = -1;
			int playerHealth = -1;

			//calculate indices for own and player healths
			if(0 < ownHealthPercent && ownHealthPercent<= .25){
				ownHealth = 0;
			}else if(.25 < ownHealthPercent && ownHealthPercent<= .5){
				ownHealth = 1;
			}else if(.5 < ownHealthPercent && ownHealthPercent<= .75){
				ownHealth = 2;
			}else if(.75 < ownHealthPercent && ownHealthPercent<= 1){
				ownHealth = 3;
			}

			if(0 < playerHealthPercent && playerHealthPercent<= .25){
				playerHealth = 0;
			}else if(.25 < playerHealthPercent && playerHealthPercent<= .5){
				playerHealth = 1;
			}else if(.5 < playerHealthPercent && playerHealthPercent<= .75){
				playerHealth = 2;
			}else if(.75 < playerHealthPercent && playerHealthPercent<= 1){
				playerHealth = 3;
			}
			
			lastOwnHealth = ownHealth;
			lastPlayerHealth = playerHealth;

			//Make empty qTable values if this is a new state
			if (! qTable.ContainsKey(angleDiv)){
				qTable.Add(angleDiv, new Dictionary<int, double[][][]>());
			}
			if (! qTable[angleDiv].ContainsKey(roundedDistance)){
				//C# jagged arrays are dumb
				qTable[angleDiv].Add(roundedDistance, new double[4][][]);
				for (int i = 0; i < 4; i++){
					qTable[angleDiv][roundedDistance][i] = new double[4][];
					for (int j = 0; j < 4; j++){
						qTable[angleDiv][roundedDistance][i][j] = new double[10];
					}
				}
			}

			//The array of expected rewards for a specific state
			double[] expectedRewards = qTable[angleDiv][roundedDistance][ownHealth][playerHealth];

			//Check for epsilon greedy random action
			double epsilon = 0.1;
			int action = -1;
			if (Main.rand.NextDouble() < epsilon){
				action = Main.rand.Next(0, 10);
			}else{
				//find the max value's index, or what action the qTable says is best
				double? maxVal = null;
				action = -1;
				for (int i = 0; i < expectedRewards.Length; i++){
  					double thisNum = expectedRewards[i];
  					if (!maxVal.HasValue || thisNum > maxVal.Value){
    					maxVal = thisNum;
    					action = i;
  					}
				}
			}

			lastAction = action;
			return action;

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
			attackProgress = 0;
			npc.aiStyle = 3;
			npc.lifeMax = 40000;
			npc.damage = 10;
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
			attackProgress = 10;
			//int damage = Main.expertMode ? 60 : 80;
			int damage = 10;
			Projectile.NewProjectile(npc.Center.X + 70, npc.Center.Y - 20, npc.velocity.X, npc.velocity.Y, ModContent.ProjectileType<Projectiles.HookRight>(), damage, 0f, Main.myPlayer, npc.whoAmI, 0f);
			Projectile.NewProjectile(npc.Center.X - 80, npc.Center.Y - 20, npc.velocity.X, npc.velocity.Y, ModContent.ProjectileType<Projectiles.HookLeft>(), damage, 0f, Main.myPlayer, npc.whoAmI, 0f);
			attackProgress = 120;
		}

		private void WormWall() {
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

		private void WormSpit(){
			attackProgress = 50;
			Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, -10, ProjectileID.DD2BetsyFireball, 20, 20);
			 
		}

		private void WormSpear()
		{
			attackProgress = 100;
			Player player = Main.player[npc.target];
			Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

			float speed = 40f;
			var move = target - npc.Center;
			float length = move.Length();
			if (length > speed)
			{
				move *= speed / length;
			}
			length = move.Length();
			if (length > speed)
			{
				move *= speed / length;
			}

			Projectile.NewProjectile(npc.Center.X, npc.Center.Y, move.X, move.Y, ModContent.ProjectileType<Projectiles.WormSpear>(), 20, 2f, Main.myPlayer, npc.whoAmI, 0f);

		}

		private void SummonCompanion(){
			//check attack progress has reset and there is no companion already here
			if (NPC.CountNPCS(ModContent.NPCType<WormCompanion>()) == 0){
				attackProgress = 150;
				//spawn the NPC using SpawnOnPlayer so it's not consistent where it comes from
				NPC.SpawnOnPlayer(Main.player[npc.target].whoAmI, ModContent.NPCType<WormCompanion>());
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
					//set the summon timer that will summon when done
					summoningTimer = 200;
					break;
				case 3:
					WormSpit();
					break;
				case 4:
					WormSpear();
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
			//Attack after reset
			if (attackProgress <= 0){
				DoAttack(5);
			}else{
				attackProgress--;
				if (attackProgress < 0){
					attackProgress = 0;
				}
			}
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