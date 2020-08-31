using KN_Core;

namespace KN_Maps {
  public class Maps : BaseMod {

    private SafeFlyMod fly_;

    public Maps(Core core) : base(core, "MAPS", 4) {
      fly_ = new SafeFlyMod(core);
    }

    public override void OnStart() {
      fly_.OnStart();
    }

    public override void OnStop() {
      fly_.OnStop();
    }

    protected override void OnCarLoaded() {
      fly_.OnCarLoaded();
    }

    public override void ResetState() { }

    public override void ResetPickers() { }

    public override void GuiPickers(int id, Gui gui, ref float x, ref float y) { }

    public override void OnGUI(int id, Gui gui, ref float x, ref float y) {
      fly_.OnGui(gui, ref x, ref y);
    }

    public override void Update(int id) {
      fly_.Update();
    }
  }
}