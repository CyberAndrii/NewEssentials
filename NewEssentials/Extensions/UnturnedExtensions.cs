﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NewEssentials.Models;
using SDG.Unturned;
using UnityEngine;

namespace NewEssentials.Extensions
{
    public static class UnturnedExtensions
    {
        private static readonly byte[] PlaceholderArray = Array.Empty<byte>();

        public static SerializableItem[] ToSerializableItems(this PlayerClothing clothing)
        {
            var serializableItems = new List<SerializableItem>();
            
            if (clothing.hat != 0)
                serializableItems.Add(new SerializableItem(clothing.hat.ToString(), clothing.hatState, 1, 100, clothing.hatQuality));

            if (clothing.glasses != 0)
                serializableItems.Add(new SerializableItem(clothing.glasses.ToString(), clothing.glassesState, 1, 100, clothing.glassesQuality));

            if (clothing.mask != 0)
                serializableItems.Add(new SerializableItem(clothing.mask.ToString(), clothing.maskState, 1, 100, clothing.maskQuality));

            if (clothing.shirt != 0)
                serializableItems.Add(new SerializableItem(clothing.shirt.ToString(), clothing.shirtState, 1, 100, clothing.shirtQuality));

            if (clothing.vest != 0)
                serializableItems.Add(new SerializableItem(clothing.vest.ToString(), clothing.vestState, 1, 100, clothing.vestQuality));

            if (clothing.backpack != 0)
                serializableItems.Add(new SerializableItem(clothing.backpack.ToString(), clothing.backpackState, 1, 100, clothing.backpackQuality));

            if (clothing.pants != 0)
                serializableItems.Add(new SerializableItem(clothing.pants.ToString(), clothing.pantsState, 1, 100, clothing.pantsQuality));

            return serializableItems.ToArray();
        }
        
        public static async UniTask MaxAllSkillsAsync(this PlayerSkills playerSkills, bool kunii = false)
        {
            await UniTask.SwitchToMainThread();
            for (byte speciality = 0; speciality < playerSkills.skills.Length; speciality++)
            {
                Skill[] skills = playerSkills.skills[speciality];
                byte[] newLevels = new byte[skills.Length];

                for (byte index = 0; index < skills.Length; index++)
                {
                    Skill skill = skills[index];

                    if (kunii)
                        skill.level = byte.MaxValue;
                    else
                        skill.setLevelToMax();

                    newLevels[index] = skill.level;
                }

                // No achievements lol
                playerSkills.channel.send("tellSkills", playerSkills.channel.owner.playerID.steamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, speciality, newLevels);
            }
        }

        public static async UniTask KickAsync(this Player player, string reason)
        {
            await UniTask.SwitchToMainThread();
            Provider.kick(player.channel.owner.playerID.steamID, reason);
        }

        public static void ClearInventory(this Player player)
        {
            Items[] items = player.inventory.items;
            for (byte b = 0; b < PlayerInventory.PAGES; b++)
            {
                if (b == PlayerInventory.AREA)
                    continue;

                items[b].Clear();

                if (b < PlayerInventory.SLOTS)
                    player.equipment.sendSlot(b);
            }

            PlayerClothing clothing = player.clothing;
            
            // I like sendSwap better but it can only be receieved from owner I think
            // I also really hate everything past this point
            if (clothing.backpack != 0)
            {
                clothing.askWearBackpack(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }

            if (clothing.glasses != 0)
            {
                clothing.askWearGlasses(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }

            if (clothing.hat != 0)
            {
                clothing.askWearHat(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }

            if (clothing.mask != 0)
            {
                clothing.askWearMask(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }

            if (clothing.pants != 0)
            {
                clothing.askWearPants(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }

            if (clothing.shirt != 0)
            {
                clothing.askWearShirt(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }

            if (clothing.vest != 0)
            {
                clothing.askWearVest(0, 0, PlaceholderArray, true);
                items[PlayerInventory.SLOTS].Clear();
            }
        }

        public static void Clear(this Items item)
        {
            for (byte b = 0; b < item.getItemCount(); b++)
            {
                item.removeItem(b);
            }
        }

        public static async UniTask<bool> TeleportToLocationAsync(this Player player, Vector3 position) =>
            await TeleportToLocationAsync(player, position, player.transform.eulerAngles.y);

        public static async UniTask<bool> TeleportToLocationAsync(this Player player, Vector3 position, float yaw)
        {
            await UniTask.SwitchToMainThread();
            return player.teleportToLocation(position, yaw);
        }

        public static async UniTask TeleportToLocationUnsafeAsync(this Player player, Vector3 position)
        {
            await UniTask.SwitchToMainThread();
            player.teleportToLocationUnsafe(position, player.transform.eulerAngles.y);
        }
    }
}