using Dalamud.Plugin;
using ImGuiNET;
using Dalamud.Configuration;
using Num = System.Numerics;
using Lumina.Excel.GeneratedSheets;
using System;

namespace WOLdo
{
    public class WOLdo : IDalamudPlugin
    {
        public string Name => "WOLdo";
        private DalamudPluginInterface pi;
        public Config Configuration;
        public string location = "";
        public bool enabled = true;
        public Lumina.Excel.ExcelSheet<TerritoryType> terr;
        public float scale = 1f;
        public ImColor col = new ImColor { Value = new Num.Vector4(1f, 1f, 1f, 1f) };
        public bool no_move = false;
        public bool config = false;

        public void Initialize(DalamudPluginInterface pi)
        {

            this.pi = pi;
            Configuration = pi.GetPluginConfig() as Config ?? new Config();
            col = Configuration.Col;
            no_move = Configuration.NoMove;
            scale = Configuration.Scale;
            enabled = Configuration.Enabled;
            terr = pi.Data.GetExcelSheet<TerritoryType>();

            this.pi.UiBuilder.OnBuildUi += DrawWindow;

            this.pi.CommandManager.AddHandler("/woldo", new Dalamud.Game.Command.CommandInfo(Command)
            {
                HelpMessage = "Where's WOLdo config."
            });
        }
        public void Dispose()
        {
            this.pi.UiBuilder.OnBuildUi -= DrawWindow;
            this.pi.CommandManager.RemoveHandler("/woldo");
            terr = null;
        }

        public void Command(string command, string arguments)
        {
            config = true;
        }

        private void DrawWindow()
        {

            ImGuiWindowFlags window_flags = 0;
            window_flags |= ImGuiWindowFlags.NoTitleBar;
            window_flags |= ImGuiWindowFlags.NoScrollbar;
            window_flags |= ImGuiWindowFlags.NoScrollbar;
            if (no_move)
            {
                window_flags |= ImGuiWindowFlags.NoMove;
                window_flags |= ImGuiWindowFlags.NoMouseInputs;
                window_flags |= ImGuiWindowFlags.NoNav;
            }
            window_flags |= ImGuiWindowFlags.AlwaysAutoResize;
            window_flags |= ImGuiWindowFlags.NoBackground;


            if (config)
            {
                
                ImGui.SetNextWindowSize(new Num.Vector2(200, 160), ImGuiCond.FirstUseEver);
                ImGui.Begin("Where's WOLdo Config", ref config, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.Checkbox("Enabled", ref enabled);
                ImGui.ColorEdit4("Colour", ref col.Value, ImGuiColorEditFlags.NoInputs);
                ImGui.InputFloat("Size", ref scale);
                ImGui.Checkbox("Locked", ref no_move);

                if (ImGui.Button("Save and Close Config"))
                {
                    SaveConfig();
                    config = false;
                }
                ImGui.End();
            }

            location = "";
            if (pi.ClientState.LocalPlayer != null)
            {
                location = "Uhoh";
                try
                {
                    location = terr.GetRow(pi.ClientState.TerritoryType).PlaceName.Value.Name;
                }

                catch (Exception e)
                {
                    location = "Change zone to load";
                }
                
                            
            }


            if (enabled)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
                ImGui.Begin("WOLdo", ref enabled, window_flags);
                ImGui.SetWindowFontScale(scale);
                ImGui.Text(location);
                ImGui.End();
                ImGui.PopStyleColor();
            }

        }

        public void SaveConfig()
        {
            Configuration.Enabled = enabled;
            Configuration.Col = col;
            Configuration.Scale = scale;
            Configuration.NoMove = no_move;
            this.pi.SavePluginConfig(Configuration);
        }
    }

    public class Config : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool Enabled { get; set; } = true;
        public ImColor Col { get; set; } = new ImColor { Value = new Num.Vector4(1f,1f,1f,1f) };
        public float Scale { get; set; } = 1f;
        public bool NoMove { get; set; } = false;

    }
}