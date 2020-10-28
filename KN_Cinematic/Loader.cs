using BepInEx;
using KN_Core;

namespace KN_Cinematic {
  [BepInPlugin("trbflxr.kn_cinematic", "KN_Cinematic", StringVersion)]
  public class Loader : BaseUnityPlugin {
    private const int Version = 201;
    private const int Patch = 1;
    private const int ClientVersion = 273;
    private const string StringVersion = "2.0.1";

    public Loader() {
      Core.CoreInstance.AddMod(new Cinematic(Core.CoreInstance, Version, Patch, ClientVersion));
    }
  }
}