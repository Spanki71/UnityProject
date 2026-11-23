// Opens the showroom in the Game view. Unity keeps a maximized Scene view maximized when entering Play mode, which would hide the showroom and take the keyboard.
// The Game view gets the whole window for the session, the usual window layout returns after Play.
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Codewart
{
    [InitializeOnLoad]
    static class ShowroomView
    {
        const string Shown = "Codewart.ShowroomView";

        static ShowroomView() => EditorApplication.playModeStateChanged += Changed;

        static void Changed(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && Object.FindAnyObjectByType<PackShowroom>()) Show();
            if (state == PlayModeStateChange.EnteredEditMode && SessionState.GetBool(Shown, false)) Restore();
        }

        static void Unmaximize()
        {
            foreach (var w in Resources.FindObjectsOfTypeAll<EditorWindow>().Where(w => w.maximized).ToArray()) w.maximized = false;
        }

        static void Show()
        {
            Unmaximize();     // a hidden Game view comes back only with the full layout
            if (!EditorApplication.ExecuteMenuItem("Window/General/Game")) return;
            var game = EditorWindow.focusedWindow;
            if (!game || game.GetType().Name != "GameView") return;
            game.maximized = true;
            game.Focus();
            SessionState.SetBool(Shown, true);
        }

        static void Restore()
        {
            SessionState.EraseBool(Shown);
            Unmaximize();
        }
    }
}
