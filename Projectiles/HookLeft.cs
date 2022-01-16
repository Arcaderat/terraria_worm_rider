using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;

namespace WormRiderBoss.Projectiles
{
	public class HookLeft : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 10; //The number of frames the sprite sheet has
		}

		public override void SetDefaults()
		{
			//mod.Logger.Info("worm hook spawned");
			//DisplayName.SetDefault("WORM HOOK BOIS");
			projectile.width = 150;
			projectile.height = 135;
			projectile.timeLeft = 40; 
			projectile.penetrate = 1; 
			projectile.friendly = false; 
			projectile.hostile = true; 
			projectile.tileCollide = false; 
			projectile.damage = 20;
			projectile.ranged = false;
			projectile.aiStyle = 0;
		}

		public override void AI()
		{
			projectile.spriteDirection = 1;
			//This will cycle through all of the frames in the sprite sheet
			int frameSpeed = 4; //How fast you want it to animate
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
			base.AI();
		}

    }
}
