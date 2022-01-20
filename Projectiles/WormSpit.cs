using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace WormRiderBoss.Projectiles
{
	public class WormSpit : ModProjectile
	{
        public override string Texture => "Terraria/Projectile_" + ProjectileID.DD2OgreSpit;

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.DD2OgreSpit);
			projectile.damage = 40;
		}

		

    }
}