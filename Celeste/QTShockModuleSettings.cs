namespace Celeste.Mod.QTShock;
public class QTShockModuleSettings : EverestModuleSettings {

    [SettingName("Toggle Mod")]
    [SettingSubText("Toggles the mod on and off.")]
    public bool Toggle { get; set; } = true;

    [SettingName("Shocker ID")]
    [SettingSubText("ID of the shocker device from OpenShock.")]
    public string Shocker { get; set; } = "";

    [SettingRange(1, 100)]
    [SettingName("Strength")]
    [SettingSubText("Intensity of the action (1-100).")]
    public int Strength { get; set; } = 40;

    [SettingName("Type")]
    public EnumType Type { get; set; } = EnumType.Shock;
    public enum EnumType {
        Beep,
        Vibrate,
        Shock
    }

    [SettingName("API Key")]
    [SettingSubText("Your OpenShock API token.")]
    public string ApiKey { get; set; } = "";
}
