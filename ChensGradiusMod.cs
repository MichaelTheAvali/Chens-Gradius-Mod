using ChensGradiusMod.Items.Placeables.MusicBoxes;
using ChensGradiusMod.Projectiles.Enemies;
using ChensGradiusMod.Tiles.MusicBoxes;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ChensGradiusMod
{
  public class ChensGradiusMod : Mod
  {
    public static ModHotKey forceActionKey;
    public static ModHotKey optionActionKey;

    public ChensGradiusMod() { }

    public override void Load()
    {
      forceActionKey = RegisterHotKey("Force Action Toggle", "Mouse3");
      optionActionKey = RegisterHotKey("Option Action Key", "Mouse2");

      if (!Main.dedServ)
      {
        AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/PositionLight"),
                    ModContent.ItemType<PositionLightMusicBox>(),
                    ModContent.TileType<PositionLightMusicBoxTile>());
        AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/DepartureForSpace"),
                    ModContent.ItemType<DepartureForSpaceMusicBox>(),
                    ModContent.TileType<DepartureForSpaceMusicBoxTile>());
        AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Tabidachi"),
                    ModContent.ItemType<TabidachiMusicBox>(),
                    ModContent.TileType<TabidachiMusicBoxTile>());
      }

      //Mod achievementLib = ModLoader.GetMod("AchievementLib");
      //if (achievementLib != null)
      //{
      //  achievementLib.Call("AddAchievement", this, "Bydo Technology",
      //                      "Create an indestructible weapon made of alien flesh.",
      //                      GradiusAchievement.TextureString("PlaceholderAchievement", locked: true),
      //                      GradiusAchievement.TextureString("PlaceholderAchievement", locked: false));
      //  achievementLib.Call("AddAchievement", this, "Wreek Weapon",
      //                      "Create an Option, an invulnerable drone which copies the host's attacks.",
      //                      GradiusAchievement.TextureString("PlaceholderAchievement", locked: true),
      //                      GradiusAchievement.TextureString("PlaceholderAchievement", locked: false));
      //  achievementLib.Call("AddAchievement", this, "From Myth To Legend",
      //                      "Create your own legend by destroying a Big Core.",
      //                      GradiusAchievement.TextureString("PlaceholderAchievement", locked: true),
      //                      GradiusAchievement.TextureString("PlaceholderAchievement", locked: false));
      //}
    }

    public override void Unload()
    {
      base.Unload();

      forceActionKey = null;
      optionActionKey = null;
    }

    public override object Call(params object[] args)
    {
      try
      {
        string functionName = args[0] as string;
        switch (functionName)
        {
          case "AddOptionRule":
            {
              // Vanilla Projectile Rule
              // args[1]: Vanilla Projectile Type. e.g. ProjectileID.Bee
              // ... or Mod Projectile Rule
              // args[1]: Your Mod's Internal Name. e.g. "ChensGradiusMod"
              // args[2]: Your Projectile's Internal name. e.g. "NewArrowsProjectile"

              if (args.Length > 3 && args.Length < 2)
              {
                Logger.Error($"ChensGradiusMod {functionName} Error:" +
                             "Wrong number of arguments.");
                throw new Exception($"ChensGradiusMod {functionName} Error:" +
                                    "Wrong number of arguments.");
              }

              bool? result;
              if (args.Length == 2)
              {
                if (args[1] == null) result = null;
                else result = OptionRules.ImportOptionRule(Convert.ToInt32(args[1]));
              }
              else result = OptionRules.ImportOptionRule(args[1] as string, args[2] as string);

              if (result == null)
              {
                Logger.Warn($"ChensGradiusMod {functionName} Warning:" +
                            "Given projectile type is null. This projectile type is not added.");
                result = false;
              }
              return result;
            }

          case "AddCustomDamage":
            {
              // args[1]: Your Mod's Internal Name. e.g. "ChensGradiusMod"
              // args[2]: Internal class name of the GlobalProjectile containing the custom damage type. e.g. "MyGlobalProjectile"
              // args[3]: Internal boolean variable name of your custom damage.

              if (args.Length > 4)
              {
                Logger.Error($"ChensGradiusMod {functionName} Error:" +
                             "Wrong number of arguments.");
                throw new Exception($"ChensGradiusMod {functionName} Error:" +
                                    "Wrong number of arguments.");
              }

              bool result = OptionRules.ImportDamageType(args[1] as string, args[2] as string, args[3] as string);
              return result;
            }

          case "ProjectileBanCheck":
            {
              // args[1]: integer Type of the projectile to check. (projectile.type)

              if (args.Length > 2)
              {
                Logger.Error($"ChensGradiusMod {functionName} Error:" +
                             "Wrong number of arguments.");
                throw new Exception($"ChensGradiusMod {functionName} Error:" +
                                    "Wrong number of arguments.");
              }

              bool? result;
              if (args[1] == null)
              {
                result = null;
                Logger.Warn($"ChensGradiusMod {functionName} Warning:" +
                            "Given projectile type is null.");
              }
              else result = OptionRules.IsBanned(Convert.ToInt32(args[1]));
              return result;
            }
        }
      }
      catch (Exception e)
      {
        Logger.Error("ChensGradiusMod Call Error: " + e.StackTrace + e.Message);
      }

      return null;
    }

    public override void AddRecipeGroups()
    {
      RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("Ebonstone Block or Crimstone Block"), new int[]
      {
        ItemID.CrimstoneBlock,
        ItemID.EbonstoneBlock
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:EvilStones", group);

      group = new RecipeGroup(() => Language.GetTextValue("Tissue Sample or Shadow Scale"), new int[]
      {
        ItemID.ShadowScale,
        ItemID.TissueSample
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:EvilDrops", group);

      group = new RecipeGroup(() => Language.GetTextValue("Gold Bar or Platinum Bar"), new int[]
      {
        ItemID.PlatinumBar,
        ItemID.GoldBar
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:GoldTierBar", group);

      group = new RecipeGroup(() => Language.GetTextValue("Cobalt Bar or Palladium Bar"), new int[]
      {
        ItemID.PalladiumBar,
        ItemID.CobaltBar
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:CobaltTierBar", group);

      group = new RecipeGroup(() => Language.GetTextValue("Copper Bar or Tin Bar"), new int[]
      {
        ItemID.TinBar,
        ItemID.CopperBar
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:TinTierBar", group);

      group = new RecipeGroup(() => Language.GetTextValue("Any Mechanical Boss Soul"), new int[]
      {
        ItemID.SoulofMight,
        ItemID.SoulofSight,
        ItemID.SoulofFright,
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:MechSoul", group);

      group = new RecipeGroup(() => Language.GetTextValue("Silver Bar or Tungsten Bar"), new int[]
      {
        ItemID.TungstenBar,
        ItemID.SilverBar
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:SilverTierBar", group);

      group = new RecipeGroup(() => Language.GetTextValue("Any Evil Mushroom"), new int[]
      {
        ItemID.VileMushroom,
        ItemID.ViciousMushroom
      });
      RecipeGroup.RegisterGroup("ChensGradiusMod:EvilMushroom", group);
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
      byte playerNumber;
      GradiusModPlayer modPlayer;
      PacketMessageType msgType = (PacketMessageType)reader.ReadByte();

      switch (msgType)
      {
        case PacketMessageType.GradiusModSyncPlayer:
          playerNumber = reader.ReadByte();
          modPlayer = Main.player[playerNumber].GetModPlayer<GradiusModPlayer>();
          modPlayer.isFreezing = reader.ReadBoolean();
          modPlayer.rotateMode = reader.ReadInt32();
          modPlayer.revolveDirection = reader.ReadInt32();
          modPlayer.wasHolding = reader.ReadBoolean();
          modPlayer.forceBase = reader.ReadBoolean();
          modPlayer.needleForce = reader.ReadBoolean();
          modPlayer.optionOne = reader.ReadBoolean();
          modPlayer.optionTwo = reader.ReadBoolean();
          modPlayer.optionThree = reader.ReadBoolean();
          modPlayer.optionFour = reader.ReadBoolean();
          modPlayer.normalOption = reader.ReadBoolean();
          modPlayer.freezeOption = reader.ReadBoolean();
          modPlayer.rotateOption = reader.ReadBoolean();
          int listCount = reader.ReadInt32();
          for (int i = 0; i < listCount; i++) modPlayer.optionFlightPath.Add(reader.ReadVector2());
          modPlayer.optionSeed = reader.ReadBoolean();
          modPlayer.seedRotateDirection = reader.ReadInt32();
          modPlayer.chargeMultiple = reader.ReadBoolean();
          modPlayer.chargeMode = reader.ReadInt32();
          modPlayer.aimOption = reader.ReadBoolean();
          modPlayer.searchOption = reader.ReadBoolean();
          modPlayer.isSearching = reader.ReadBoolean();
          break;

        case PacketMessageType.ClientChangesFreezeOption:
          playerNumber = reader.ReadByte();
          modPlayer = Main.player[playerNumber].GetModPlayer<GradiusModPlayer>();

          modPlayer.isFreezing = reader.ReadBoolean();
          modPlayer.wasHolding = reader.ReadBoolean();

          if (GradiusHelper.IsServer())
          {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketMessageType.ClientChangesFreezeOption);
            packet.Write(playerNumber);
            packet.Write(modPlayer.isFreezing);
            packet.Write(modPlayer.wasHolding);
            packet.Send(-1, playerNumber);
          }
          break;

        case PacketMessageType.ClientChangesRotateOption:
          playerNumber = reader.ReadByte();
          modPlayer = Main.player[playerNumber].GetModPlayer<GradiusModPlayer>();

          modPlayer.rotateMode = reader.ReadInt32();
          modPlayer.revolveDirection = reader.ReadInt32();
          modPlayer.wasHolding = reader.ReadBoolean();

          if (GradiusHelper.IsServer())
          {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketMessageType.ClientChangesRotateOption);
            packet.Write(playerNumber);
            packet.Write(modPlayer.rotateMode);
            packet.Write(modPlayer.revolveDirection);
            packet.Write(modPlayer.wasHolding);
            packet.Send(-1, playerNumber);
          }
          break;

        case PacketMessageType.ClientChangesSeedDirection:
          playerNumber = reader.ReadByte();
          modPlayer = Main.player[playerNumber].GetModPlayer<GradiusModPlayer>();

          modPlayer.seedRotateDirection = reader.ReadInt32();

          if (GradiusHelper.IsServer())
          {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketMessageType.ClientChangesSeedDirection);
            packet.Write(playerNumber);
            packet.Write(modPlayer.seedRotateDirection);
            packet.Send(-1, playerNumber);
          }
          break;

        case PacketMessageType.ClientChangesChargeMultiple:
          playerNumber = reader.ReadByte();
          modPlayer = Main.player[playerNumber].GetModPlayer<GradiusModPlayer>();

          modPlayer.chargeMode = reader.ReadInt32();
          modPlayer.wasHolding = reader.ReadBoolean();

          if (GradiusHelper.IsServer())
          {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketMessageType.ClientChangesChargeMultiple);
            packet.Write(playerNumber);
            packet.Write(modPlayer.chargeMode);
            packet.Write(modPlayer.wasHolding);
            packet.Send(-1, playerNumber);
          }
          break;

        case PacketMessageType.SpawnRetaliationBullet:
          if (GradiusHelper.IsServer())
          {
            Vector2 spawnPoint = reader.ReadVector2();
            Vector2 spawnVelocity = reader.ReadVector2();
            int dmg = reader.ReadInt32();
            float kb = reader.ReadSingle();

            Projectile.NewProjectile(spawnPoint, spawnVelocity,
                                     ModContent.ProjectileType<GradiusEnemyBullet>(),
                                     dmg, kb, Main.myPlayer);
          }

          break;

        case PacketMessageType.ClientChangesSearchOption:
          playerNumber = reader.ReadByte();
          modPlayer = Main.player[playerNumber].GetModPlayer<GradiusModPlayer>();

          modPlayer.isSearching = reader.ReadBoolean();
          modPlayer.wasHolding = reader.ReadBoolean();

          if (GradiusHelper.IsServer())
          {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketMessageType.ClientChangesSearchOption);
            packet.Write(playerNumber);
            packet.Write(modPlayer.isSearching);
            packet.Write(modPlayer.wasHolding);
            packet.Send(-1, playerNumber);
          }
          break;
      }
    }

    internal enum PacketMessageType : byte
    {
      GradiusModSyncPlayer,
      ClientChangesFreezeOption,
      ClientChangesRotateOption,
      ClientChangesSeedDirection,
      ClientChangesChargeMultiple,
      SpawnRetaliationBullet,
      ClientChangesSearchOption
    }
  }
}