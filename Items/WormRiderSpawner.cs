using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using WormRiderBoss.NPCs;
using WormRiderBoss.Projectiles;

namespace WormRiderBoss.Items
{
	public class WormRiderSpawner : ModItem
	{
        public override bool UseItem(Player player) {
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.WormRider>());
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

        public override void AddRecipes(){
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(9);
            recipe.SetResult(this);
            recipe.AddRecipe();

        }
        
        public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.rare = ItemRarityID.Cyan;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item44;
			item.consumable = true;
		}
        
        // We use the CanUseItem hook to prevent a player from using this item while the boss is present in the world.
		public override bool CanUseItem(Player player) {
            //TODO only allow spawning in desert?
			//prevent the player from spawning more than one at a time
			if (NPC.AnyNPCs(ModContent.NPCType<WormRider>())) {
				return false;
			}
			return true;
		}
	}
}