﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using Microsoft.Extensions.Localization;
using OpenMod.Unturned.Commands;
using SDG.Unturned;
using UnityEngine;

namespace NewEssentials.Commands.Clear
{
    [Command("vehicles")]
    [CommandAlias("vehicle")]
    [CommandAlias("v")]
    [CommandParent(typeof(CClearRoot))]
    [CommandDescription("Clears all vehicles or only empty vehicles")]
    [CommandSyntax("[empty]")]
    public class CClearVehicles : UnturnedCommand
    {
        private readonly IStringLocalizer m_StringLocalizer;

        public CClearVehicles(IStringLocalizer stringLocalizer, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
        }

        protected override async UniTask OnExecuteAsync()
        {
            if (Context.Parameters.Length > 1)
                throw new CommandWrongUsageException(Context);

            if (Context.Parameters.Length == 0)
            {
                await UniTask.SwitchToMainThread();

                byte counter = BombVehicles(VehicleManager.vehicles);

                await Context.Actor.PrintMessageAsync(m_StringLocalizer["clear:vehicles"]);
            }
            else
            {
                string notEmpty = Context.Parameters[0];
                if(!notEmpty.Equals("empty", StringComparison.InvariantCultureIgnoreCase))
                    throw new CommandWrongUsageException(Context);
                
                await UniTask.SwitchToMainThread();

                byte counter = BombVehicles(VehicleManager.vehicles.Where(v => v.isEmpty));

                await Context.Actor.PrintMessageAsync(m_StringLocalizer["clear:vehicles_empty", new {Count = counter}]);
            }
        }

        private byte BombVehicles(IEnumerable<InteractableVehicle> vehicles)
        {
            byte counter = 0;
            foreach (var vehicle in vehicles)
            {
                vehicle.transform.position = new Vector3(0, 0, 0);
                VehicleManager.sendVehicleExploded(vehicle);
                counter++;
            }

            return counter;
        }
    }
}