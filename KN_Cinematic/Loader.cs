using BepInEx;
using KN_Core;

namespace KN_Cinematic {
  [BepInPlugin("trbflxr.kn_cinematic", "KN_Cinematic", StringVersion)]
  public class Loader : BaseUnityPlugin {
    private const int Version = 202;
    private const int Patch = 0;
    private const int ClientVersion = 273;
    private const string StringVersion = "2.0.2";

    public Loader() {
      Core.CoreInstance.AddMod(new Cinematic(Core.CoreInstance, Version, Patch, ClientVersion));
    }
  }
}