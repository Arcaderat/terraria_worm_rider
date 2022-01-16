using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;

namespace WormRiderBoss.Projectiles
{
	public class Hook : ModProjectile
	{
		public override void SetDefaults()
		{
			//mod.Logger.Info("worm hook spawned");
			//DisplayName.SetDefault("WORM HOOK BOIS");
			Main.projFrames[projectile.type] = 10;
			projectile.width = 40; 
			projectile.height = 40; 
			projectile.timeLeft = 60; 
			projectile.penetrate = 1; 
			projectile.friendly = false; 
			projectile.hostile = true; 
			projectile.tileCollide = true; 
			projectile.damage = 20;
			projectile.ranged = false;
			projectile.aiStyle = 18;
		}
        public override void AI()
        {
			mod.Logger.Info("worm hook spawned");
			base.AI();
        }

    }
}
