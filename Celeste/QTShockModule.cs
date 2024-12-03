using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Celeste.Mod.QTShock;

public class QTShockModule : EverestModule {
    public static QTShockModule Instance { get; private set; }
    public override Type SettingsType => typeof(QTShockModuleSettings);
    public static QTShockModuleSettings Settings => (QTShockModuleSettings) Instance._Settings;
    private static HttpClient client = new();

    public QTShockModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(QTShockModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(QTShockModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        Everest.Events.Player.OnDie += OnDie;
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }

    private void OnDie(Player p)
    {
        if (!Settings.Toggle) return;

        // Prepare the OpenShock API payload
        var payload = new
        {
            shocks = new[] {
                new {
                    id = Settings.Shocker.ToString(),
                    type = Settings.Type switch
                    {
                        QTShockModuleSettings.EnumType.Beep => "Beep",
                        QTShockModuleSettings.EnumType.Vibrate => "Vibrate",
                        QTShockModuleSettings.EnumType.Shock => "Shock",
                        _ => "Shock"
                    },
                    intensity = Settings.Strength,
                    duration = 1000, // Default duration, adjust as needed
                    exclusive = true
                }
            },
            customName = "CelesteMod"
        };

        _ = Task.Run(async () =>
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.shocklink.net/2/shockers/control")
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(payload),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("OpenShockToken", Settings.ApiKey);
                request.Headers.Add("Content-Type", "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Optionally log success
                Logger.Log(nameof(LogLevel.Info), $"OpenShock API request successful. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                // Log the error
                Logger.Log(nameof(LogLevel.Error), $"OpenShock API Error: {ex.Message}");
            }
        });
    }
}
