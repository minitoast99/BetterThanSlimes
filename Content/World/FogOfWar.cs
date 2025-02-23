﻿using Terraria;
using Terraria.ModLoader;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using System.Reflection;
using Terraria.Graphics.Light;

namespace YourModNamespace
{
    public class LightingEnginePatch : ModSystem
    {
        private static ILHook _updateLightDecayHook;

        public override void Load()
        {
            // Find the UpdateLightDecay method in the LightingEngine class
            MethodInfo updateLightDecayMethod = typeof(LightingEngine)
                .GetMethod("UpdateLightDecay", BindingFlags.NonPublic | BindingFlags.Instance);

            if (updateLightDecayMethod != null)
            {
                // Create an IL hook for the UpdateLightDecay method
                _updateLightDecayHook = new ILHook(updateLightDecayMethod, ModifyLightDecayValues);
                Mod.Logger.Info("Successfully hooked into UpdateLightDecay!");
            }
            else
            {
                Mod.Logger.Error("Failed to find method: UpdateLightDecay");
            }
        }

        public override void Unload()
        {
            // Dispose of the hook when the mod is unloaded
            _updateLightDecayHook?.Dispose();
            Mod.Logger.Info("Unloaded LightingEnginePatch functionality.");
        }

        private void ModifyLightDecayValues(ILContext il)
        {
            var c = new ILCursor(il);

            // Find the instruction where LightDecayThroughAir is set to 0.91f
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.91f)))
            {
                // Replace 0.91f with 0.88f
                c.Prev.Operand = 0.60f;
                Mod.Logger.Info("Modified LightDecayThroughAir to 0.88f!");
            }
            else
            {
                Mod.Logger.Error("Failed to find LightDecayThroughAir assignment!");
            }

            // Find the instruction where LightDecayThroughSolid is set to 0.56f
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.56f)))
            {
                // Replace 0.56f with 0.00f
                c.Prev.Operand = 0.00f;
                Mod.Logger.Info("Modified LightDecayThroughSolid to 0.00f!");
            }
            else
            {
                Mod.Logger.Error("Failed to find LightDecayThroughSolid assignment!");
            }
        }
    }
}