﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChensGradiusMod.Items.Accessories.Options.Rotate
{
  public class RotateOptionFour : RotateOptionBase
  {
    public override void SetStaticDefaults()
    {
      base.SetStaticDefaults();

      DisplayName.SetDefault("Option type Rotate (Fourth)");
    }

    public override void SetDefaults()
    {
      base.SetDefaults();

      item.rare = 8;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
      ModPlayer(player).optionFour = true;
      ModPlayer(player).rotateOption = true;

      base.UpdateAccessory(player, hideVisual);
    }

    public override bool CanEquipAccessory(Player player, int slot)
    {
      return base.CanEquipAccessory(player, slot) &&
             player.GetModPlayer<GradiusModPlayer>().rotateOption &&
             player.GetModPlayer<GradiusModPlayer>().optionThree &&
             player.GetModPlayer<GradiusModPlayer>().optionTwo &&
             player.GetModPlayer<GradiusModPlayer>().optionOne;
    }

    protected override string ProjectileName => "OptionFourObject";

    protected override int OptionPosition => 4;

    public override void AddRecipes()
    {
      ModRecipe recipe = new ModRecipe(mod);
      recipe.AddIngredient(mod, "OptionFour");
      recipe.AddIngredient(ItemID.TheEyeOfCthulhu);
      recipe.AddIngredient(ItemID.Ectoplasm, 15);
      recipe.AddIngredient(ItemID.LunarTabletFragment, 15);
      recipe.AddIngredient(ItemID.ChlorophyteBar, 20);
      recipe.AddIngredient(ItemID.Wire, 300);
      recipe.AddTile(TileID.TinkerersWorkbench);
      recipe.AddTile(TileID.Loom);
      recipe.SetResult(this);
      recipe.AddRecipe();
    }
  }
}
